// <copyright file="AdaptingObservableBase.cs" company="million miles per hour ltd">
// Copyright (c) 2013-2014 All Right Reserved
// 
// This source is subject to the MIT License.
// Please see the License.txt file for more information.
// All other rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>

using System;
using System.Reactive.Disposables;
using System.Threading;
using KodeKandy.Panopticon.Internal;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     Base class offering common functionality used by Observable types that observe a source observable and then
    ///     efficiently project it into another observable type. It is conceptually similar to switch, but rather than being
    ///     thrown away each time its antecedent pushes a value, it updates itself. This along with other optimisations creates
    ///     order of magnitute performance gains.
    /// </summary>
    /// <typeparam name="TFrom">From observable type.</typeparam>
    /// <typeparam name="TTo">To observable type.</typeparam>
    internal abstract class AdaptingObservableBase<TFrom, TTo> : IObservable<TTo>
    {
        private readonly object _gate = new object();
        private readonly IObservable<TFrom> _sourceObservable;
        private IAdaptor<TFrom, TTo> _adaptor;
        private IObserver<TTo> _observer = NopObserver<TTo>.Instance;
        private IDisposable _sourceSubscriptionDisposable;

        protected AdaptingObservableBase(IObservable<TFrom> sourceObservable)
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");

            _sourceObservable = sourceObservable;
        }

        /// <summary>
        ///     The current subscriber(s).
        /// </summary>
        protected IObserver<TTo> Observer
        {
            get { return _observer; }
            set { _observer = value; }
        }

        /// <summary>
        ///     Synronisation lock.
        /// </summary>
        protected object Gate
        {
            get { return _gate; }
        }

        public IDisposable Subscribe(IObserver<TTo> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            CompletedObserver<TTo> completedObserver;

            lock (Gate)
            {
                completedObserver = Observer as CompletedObserver<TTo>;

                if (completedObserver == null)
                {
                    if (Observer == NopObserver<TTo>.Instance)
                    {
                        // If we have no _observer then make it our _observer and subscribe to _sourceObservable.
                        Observer = observer;
                        _adaptor = CreateAdaptor();
                        _sourceSubscriptionDisposable = _sourceObservable.Subscribe(_adaptor);
                    }
                    else
                    {
                        var multiObserver = Observer as ImmutableMultiObserver<TTo>;
                        if (multiObserver != null)
                        {
                            // If we already have a ImmutableMultiObserver then add the new _observer to it.
                            Observer = multiObserver.Add(observer);
                        }
                        else
                        {
                            // We didn't have a multiobserver, so we must have just had a single observer, so replace it with a multiobserver containing
                            // both the old and new observer.
                            Observer = new ImmutableMultiObserver<TTo>(new ImmutableList<IObserver<TTo>>(new[] {Observer, observer}));
                        }

                        // Send the new observer the current property value. This is done inside the lock to prevent race conditions around the initial
                        // value. Behaviour subject does similar, although I'm not sure we need to make this guarantee as it is stronger than the
                        // guarantee for the property changing once already subscribed (whereby ordering is not guaranteed).
                        _adaptor.NotifyInitialValue(observer);
                    }

                    return new Subscription(this, observer);
                }
            }

            if (completedObserver == CompletedObserver<TTo>.Instance)
                observer.OnCompleted();
            else
                observer.OnError(completedObserver.Error);

            return Disposable.Empty;
        }

        /// <summary>
        ///     Removes a subscription, disposing of the upstream subscription if no longer required.
        /// </summary>
        /// <remarks>
        ///     This function is intentionally tolerant of observers being supplied that are not subscribed, or
        ///     thid observable having already completed.
        /// </remarks>
        /// <param name="observer">The observer to unsubscribe.</param>
        private void Unsubscribe(IObserver<TTo> observer)
        {
            lock (_gate)
            {
                if (_observer is CompletedObserver<TTo>)
                    return;

                var multiObserver = _observer as ImmutableMultiObserver<TTo>;
                if (multiObserver != null)
                {
                    _observer = multiObserver.Remove(observer);
                    return;
                }

                if (_observer != observer)
                    return;

                // If we got here we only had one observer, so clear our upstream subscription.
                _observer = NopObserver<TTo>.Instance;
                CleanUp();
            }
        }

        protected abstract IAdaptor<TFrom, TTo> CreateAdaptor();

        /// <summary>
        ///     Cleans up by unsubscribing to the sourceObservable, release references & etc.
        /// </summary>
        /// <remarks>Must be called inside the lock.</remarks>
        protected void CleanUp()
        {
            if (_sourceSubscriptionDisposable != null)
            {
                _sourceSubscriptionDisposable.Dispose();
                _sourceSubscriptionDisposable = null;
            }

            if (_adaptor != null)
            {
                _adaptor.Dispose();
                _adaptor = null;
            }
        }

        /// <summary>
        ///     Common handler for when SourceObservable errors.
        /// </summary>
        /// <param name="error">The error exception.</param>
        protected void OnError(Exception error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            IObserver<TTo> oldObserver;
            IObserver<TTo> newObserver = new CompletedObserver<TTo>(error);

            lock (_gate)
            {
                if (_observer is CompletedObserver<TTo>)
                    return;

                oldObserver = _observer;
                _observer = newObserver;
                CleanUp();
            }

            oldObserver.OnError(error);
        }

        /// <summary>
        ///     Common handler for SourceObservable completion.
        /// </summary>
        protected void OnCompleted()
        {
            IObserver<TTo> oldObserver;

            lock (_gate)
            {
                if (_observer is CompletedObserver<TTo>)
                    return;

                oldObserver = _observer;
                _observer = CompletedObserver<TTo>.Instance;
                CleanUp();
            }

            oldObserver.OnCompleted();
        }

        #region Nested type: Subscription

        /// <summary>
        ///     Lightweight subscription class that unsubscribes from the observable when disposed.
        ///     This is much more performant than using a Disposable.Create().
        /// </summary>
        protected class Subscription : IDisposable
        {
            private AdaptingObservableBase<TFrom, TTo> _observable;
            private IObserver<TTo> _observer;

            public Subscription(AdaptingObservableBase<TFrom, TTo> observable,
                IObserver<TTo> observer)
            {
                _observable = observable;
                _observer = observer;
            }

            #region IDisposable Members

            public void Dispose()
            {
                // Guard against multiple invoications of Dispose().
                var currObserver = Interlocked.Exchange(ref _observer, null);
                if (currObserver == null)
                    return;

                _observable.Unsubscribe(currObserver);
                _observable = null;
            }

            #endregion
        }

        #endregion
    }
}