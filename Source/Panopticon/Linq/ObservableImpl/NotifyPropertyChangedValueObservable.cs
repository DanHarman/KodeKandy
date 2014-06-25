// <copyright file="NotifyPropertyChangedValueObservable.cs" company="million miles per hour ltd">
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
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading;
using KodeKandy.Panopticon.Internal;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     Forms a link in an observable chain of INotifyPropertyChanged watchers. It is used to subscribe to property changes
    ///     on, potentially nested objects, as long as they support INotifyPropertyChanged.
    /// 
    ///     It has been heavily optimised for performance, and is several orders of magnitude faster than implementations
    ///     relying on Observable.FromEventPattern() or building observable objects containing their own Subject.
    /// 
    ///     It is thread safe, but if the observered class is having properties modified on more than one thread then race
    ///     conditinos may occur in terms of notification order etc. It is recommended that observed classes are only modified
    ///     on one thread. This is consistent with most other uses of INotifyPropertyChanged.
    /// </summary>
    /// <remarks>
    ///     We only implement IObserver{TClass} here for efficiency/perf reasons, it is not intended that consumers push into
    ///     it - the result of that would be messy.
    /// </remarks>
    /// <typeparam name="TClass">The class type being obsevered.</typeparam>
    /// <typeparam name="TProperty">The property type being observered on the observed class.</typeparam>
    internal class NotifyPropertyChangedValueObservable<TClass, TProperty> : IObserver<TClass>, IObservable<TProperty>
        where TClass : class, INotifyPropertyChanged
    {
        private readonly object _gate = new object();
        private readonly string _propertyName;
        private readonly Func<TClass, TProperty> _propertyValueGetter;
        private readonly IObservable<TClass> _sourceObservable;
        private IObserver<TProperty> _observer = NopObserver<TProperty>.Instance;
        private TProperty _propertyValue;
        private TClass _source;
        private IDisposable _sourceSubscriptionDisposable;

        public NotifyPropertyChangedValueObservable(IObservable<TClass> sourceObservable, string propertyName,
            Func<TClass, TProperty> propertyValueGetter)
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (propertyValueGetter == null)
                throw new ArgumentNullException("propertyValueGetter");

            _sourceObservable = sourceObservable;
            _propertyName = propertyName;
            _propertyValueGetter = propertyValueGetter;
        }

        #region IObservable<TProperty> Members

        public IDisposable Subscribe(IObserver<TProperty> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            lock (_gate)
            {
                if (!(_observer is CompletedObserver<TProperty>))
                {
                    if (_observer == NopObserver<TProperty>.Instance)
                    {
                        // If we have no _observer then make it our _observer and subscribe to _sourceObservable. Because there is no
                        // current value and the sourceObservable should send us its initial value immediately, we don't need to
                        // call observer.OnNext().
                        _observer = observer;
                        _sourceSubscriptionDisposable = _sourceObservable.Subscribe(this);
                    }
                    else
                    {
                        var multiObserver = _observer as ImmutableMultiObserver<TProperty>;
                        if (multiObserver != null)
                        {
                            // If we already have a ImmutableMultiObserver then add the new _observer to it.
                            _observer = multiObserver.Add(observer);
                        }
                        else
                        {
                            // We didn't have a multiobserver, so we must have just had a single observer, so replace it with a multiobserver containing
                            // both the old and new observer.
                            var oldObserver = _observer;
                            _observer =
                                new ImmutableMultiObserver<TProperty>(new ImmutableList<IObserver<TProperty>>(new[] {oldObserver, observer}));
                        }

                        // Send the new observer the current property value. This is done inside the lock to prevent race conditions around the initial
                        // value. Behaviour subject does similar, although I'm not sure we need to make this guarantee as it is stronger than the
                        // guarantee for the property changing once already subscribed (whereby ordering is not guaranteed).
                        observer.OnNext(_propertyValue);
                    }

                    return new Subscription(this, observer);
                }
            }

            // If we got here we have a completed observer and that means _observer won't be modified so we can work outside the lock safely.
            var completedObserver = _observer as CompletedObserver<TProperty>;

            if (completedObserver == CompletedObserver<TProperty>.Instance)
                observer.OnCompleted();
            else
                observer.OnError(completedObserver.Error);

            return Disposable.Empty;
        }

        #endregion

        #region IObserver<TClass> Members

        void IObserver<TClass>.OnNext(TClass newSource)
        {
            IObserver<TProperty> oldObserver;
            TClass oldSource;
            TProperty initialPropertyValue;

            lock (_gate)
            {
                if (_observer is CompletedObserver<TProperty>)
                    return;

                oldObserver = _observer;
                oldSource = _source;
                _source = newSource;

                // We need to get the current property value from this sourceObservable value.
                initialPropertyValue = _propertyValueGetter(_source);
                _propertyValue = initialPropertyValue;
            }

            // Connect to INotifyPropertyChanged on the new _source.
            if (newSource != null)
                newSource.PropertyChanged += OnPropertyChanged;

            // Disconnect from INotifyPropertyChanged on previous _source. Because _source has been changed it doesn't matter
            // if we end up with an extra event from this old sourceObservable sneaking through as it will be filtered out.
            if (oldSource != null)
                oldSource.PropertyChanged -= OnPropertyChanged;

            oldObserver.OnNext(initialPropertyValue);
        }

        void IObserver<TClass>.OnError(Exception error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            IObserver<TProperty> oldObserver;

            lock (_gate)
            {
                if (_observer is CompletedObserver<TProperty>)
                    return;

                oldObserver = _observer;
                CleanUp(new CompletedObserver<TProperty>(error));
            }

            oldObserver.OnError(error);
        }

        void IObserver<TClass>.OnCompleted()
        {
            IObserver<TProperty> oldObserver;

            lock (_gate)
            {
                if (_observer is CompletedObserver<TProperty>)
                    return;

                oldObserver = _observer;
                CleanUp(CompletedObserver<TProperty>.Instance);
            }

            oldObserver.OnCompleted();
        }

        #endregion

        /// <summary>
        ///     Internal <see cref="PropertyChangedEventHandler" /> to bind to the targets PropertyChanged event.
        ///     Gets the current property value from the current sourceObservable and pushes it to the observers.
        /// </summary>
        /// <remarks>
        ///     We keep a local copy of the sourceObservable value so that we don't end up with concurrency issues which
        ///     would occur if we pulled it form the sourceObservable whenever it was needed e.g. when adding a new observer, as
        ///     we do not have a lock around the sourceObservable, but we can control when it pushes new values into us with our
        ///     lock.
        /// </remarks
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_source == sender && _propertyName == propertyChangedEventArgs.PropertyName)
            {
                IObserver<TProperty> currObserver;

                lock (_gate)
                {
                    currObserver = _observer;
                    _propertyValue = _propertyValueGetter(_source);
                }

                if (currObserver != NopObserver<TProperty>.Instance)
                    currObserver.OnNext(_propertyValue);
            }
        }

        /// <summary>
        ///     Removes a subscription, disposing of the upstream subscription if no longer required.
        /// </summary>
        /// <remarks>
        ///     This function is intentionally tolerant of observers being supplied that are not subscribed, or
        ///     thid observable having already completed.
        /// </remarks>
        /// <param name="observer">The observer to unsubscribe.</param>
        private void Unsubscribe(IObserver<TProperty> observer)
        {
            lock (_gate)
            {
                if (_observer is CompletedObserver<TProperty>)
                    return;

                var multiObserver = _observer as ImmutableMultiObserver<TProperty>;
                if (multiObserver != null)
                {
                    _observer = multiObserver.Remove(observer);
                    return;
                }

                if (_observer != observer)
                    return;

                // If we got here we only had one observer, so clear our upstream subscription.
                CleanUp(NopObserver<TProperty>.Instance);
            }
        }

        /// <summary>
        ///     Cleans up by unsubscribing to the sourceObservable, and releases references.
        /// </summary>
        /// <remarks>Must be called inside the lock.</remarks>
        private void CleanUp(IObserver<TProperty> observer)
        {
            _observer = observer;
            _source = default(TClass);
            _propertyValue = default(TProperty);
            if (_sourceSubscriptionDisposable != null)
            {
                _sourceSubscriptionDisposable.Dispose();
                _sourceSubscriptionDisposable = null;
            }
        }

        #region Nested type: Subscription

        /// <summary>
        ///     Lightweight subscription class that unsubscribes from the NotifyPropertyChangedLink when disposed.
        ///     This is much more performant than using a Disposable.Create().
        /// </summary>
        private class Subscription : IDisposable
        {
            private IObserver<TProperty> _observer;
            private NotifyPropertyChangedValueObservable<TClass, TProperty> _valueObservable;

            public Subscription(NotifyPropertyChangedValueObservable<TClass, TProperty> valueObservable, IObserver<TProperty> observer)
            {
                _valueObservable = valueObservable;
                _observer = observer;
            }

            #region IDisposable Members

            public void Dispose()
            {
                var currObserver = Interlocked.Exchange(ref _observer, null);
                if (currObserver == null)
                    return;

                _valueObservable.Unsubscribe(currObserver);
                _valueObservable = null;
            }

            #endregion
        }

        #endregion
    }
}