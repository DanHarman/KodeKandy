// <copyright file="NotifyPropertyChangedObservableBase.cs" company="million miles per hour ltd">
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
using System.Threading;
using KodeKandy.Panopticon.Internal;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     Base class offering common functionality shared across property changed observables. Not all functionality has been
    ///     normalised into this base as it had a performance impact when hooked into Subscribe/OnNext etc as various
    ///     virtual functions were required to hook it all up. This seems a reasonable compromise.
    /// </summary>
    /// <typeparam name="TClass">The type of the observered clas.</typeparam>
    /// <typeparam name="TObservable">The type of the offered observable.</typeparam>
    internal abstract class NotifyPropertyChangedObservableBase<TClass, TObservable>
        where TClass : class
    {
        private readonly object _gate = new object();
        private readonly IObservable<IPropertyValueChanged<TClass>> _sourceObservable;
        private IObserver<TObservable> _observer = NopObserver<TObservable>.Instance;

        protected NotifyPropertyChangedObservableBase(IObservable<IPropertyValueChanged<TClass>> sourceObservable)
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");

            _sourceObservable = sourceObservable;
        }

        /// <summary>
        ///     The current subscriber(s).
        /// </summary>
        protected IObserver<TObservable> Observer
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

        /// <summary>
        ///     The source observable stream.
        /// </summary>
        protected IObservable<IPropertyValueChanged<TClass>> SourceObservable
        {
            get { return _sourceObservable; }
        }

        /// <summary>
        ///     The current instance of the source.
        /// </summary>
        protected TClass Source { get; set; }

        /// <summary>
        ///     Disposable to unsubscribe from the SourceObservable stream.
        /// </summary>
        protected IDisposable SourceSubscriptionDisposable { get; set; }

        /// <summary>
        ///     Removes a subscription, disposing of the upstream subscription if no longer required.
        /// </summary>
        /// <remarks>
        ///     This function is intentionally tolerant of observers being supplied that are not subscribed, or
        ///     thid observable having already completed.
        /// </remarks>
        /// <param name="observer">The observer to unsubscribe.</param>
        private void Unsubscribe(IObserver<TObservable> observer)
        {
            lock (_gate)
            {
                if (_observer is CompletedObserver<TObservable>)
                    return;

                var multiObserver = _observer as ImmutableMultiObserver<TObservable>;
                if (multiObserver != null)
                {
                    _observer = multiObserver.Remove(observer);
                    return;
                }

                if (_observer != observer)
                    return;

                // If we got here we only had one observer, so clear our upstream subscription.
                _observer = NopObserver<TObservable>.Instance;
                CleanUp();
            }
        }

        /// <summary>
        ///     Cleans up by unsubscribing to the sourceObservable, release references & etc.
        /// </summary>
        /// <remarks>Must be called inside the lock.</remarks>
        protected abstract void CleanUp();

        /// <summary>
        ///     Common handler for when SourceObservable errors.
        /// </summary>
        /// <param name="error">The error exception.</param>
        protected void OnError(Exception error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            IObserver<TObservable> oldObserver;
            IObserver<TObservable> newObserver = new CompletedObserver<TObservable>(error);

            lock (_gate)
            {
                if (_observer is CompletedObserver<TObservable>)
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
            IObserver<TObservable> oldObserver;

            lock (_gate)
            {
                if (_observer is CompletedObserver<TObservable>)
                    return;

                oldObserver = _observer;
                _observer = CompletedObserver<TObservable>.Instance;
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
            private NotifyPropertyChangedObservableBase<TClass, TObservable> _observable;
            private IObserver<TObservable> _observer;

            public Subscription(NotifyPropertyChangedObservableBase<TClass, TObservable> observable,
                IObserver<TObservable> observer)
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