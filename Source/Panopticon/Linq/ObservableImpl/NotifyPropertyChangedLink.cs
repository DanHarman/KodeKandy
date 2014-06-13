// <copyright file="NotifyPropertyChangedLink.cs" company="million miles per hour ltd">
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
    /// <typeparam name="TSource">The class type being obsevered.</typeparam>
    /// <typeparam name="TProperty">The property type being observered on the observed class.</typeparam>
    internal class NotifyPropertyChangedLink<TSource, TProperty> : IObserver<TSource>, IObservable<TProperty>
        where TSource : class, INotifyPropertyChanged
    {
        private readonly object _gate = new object();
        private readonly string _propertyName;
        private readonly Func<TSource, TProperty> _propertyValueGetter;
        private readonly IObservable<TSource> _sourceObservable;
        private Exception _error;
        private bool _isStopped;
        private IObserver<TProperty> _observer;
        private TProperty _propertyValue;
        private TSource _source;
        private IDisposable _sourceSubscriptionDisposable;

        public NotifyPropertyChangedLink(IObservable<TSource> source, string propertyName, Func<TSource, TProperty> propertyValueGetter)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (propertyValueGetter == null)
                throw new ArgumentNullException("propertyValueGetter");

            _sourceObservable = source;
            _propertyName = propertyName;
            _propertyValueGetter = propertyValueGetter;
        }

        #region IObservable<TProperty> Members

        public IDisposable Subscribe(IObserver<TProperty> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            if (_observer != null)
                throw new Exception("NotifyPropertyChangedLink can only have a single subscriber, and one has already been bound.");

            Exception error;

            lock (_gate)
            {
                if (!_isStopped)
                {
                    if (_observer == null)
                    {
                        // If we have no _observer then make it our _observer and subscribe to _sourceObservable. Because there is no
                        // current value and the source should send us its initial value immediately, we don't need to
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
                            // We didn't have a multiobserver, so we must have just had a single _observer, so replace it with a multiobserver containing
                            // both the old and new _observer.
                            var oldObserver = _observer;
                            _observer =
                                new ImmutableMultiObserver<TProperty>(new Internal.ImmutableList<IObserver<TProperty>>(new[] {oldObserver, observer}));
                        }

                        // Send the new observer the current property value.
                        observer.OnNext(_propertyValue);
                    }

                    return new Subscription(this, observer);
                }

                error = _error;
            }

            if (error == null)
                observer.OnCompleted();
            else
                observer.OnError(error);

            return Disposable.Empty;
        }

        #endregion

        #region IObserver<TSource> Members

        void IObserver<TSource>.OnNext(TSource value)
        {
            IObserver<TProperty> currObserver;
            TSource oldSource;

            lock (_gate)
            {
                if (_isStopped)
                    return;

                currObserver = _observer;
                oldSource = _source;
                _source = value;

                // We need to get the current property value from this source value.
                _propertyValue = _propertyValueGetter(_source);
            }

            // Connect to INotifyPropertyChanged on the new _source.
            if (_source != null)
                _source.PropertyChanged += OnPropertyChanged;

            // Disconnect from INotifyPropertyChanged on previous _source. Because _source has been changed it doesn't matter
            // if we end up with an event from this old source sneaking through as it will be filtered out.
            if (oldSource != null)
                oldSource.PropertyChanged -= OnPropertyChanged;

            if (currObserver != null)
                currObserver.OnNext(_propertyValue);
        }

        void IObserver<TSource>.OnError(Exception error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            IObserver<TProperty> currObserver;

            lock (_gate)
            {
                if (_isStopped)
                    return;

                currObserver = _observer;
                CleanUp();
                _error = error;
                _isStopped = true;
            }

            if (currObserver != null)
                currObserver.OnError(error);
        }

        void IObserver<TSource>.OnCompleted()
        {
            IObserver<TProperty> currObserver;

            lock (_gate)
            {
                if (_isStopped)
                    return;

                currObserver = _observer;
                CleanUp();
                _isStopped = true;
            }

            if (currObserver != null)
                currObserver.OnCompleted();
        }

        #endregion

        /// <summary>
        ///     Internal <see cref="PropertyChangedEventHandler" /> to bind to the targets PropertyChanged event.
        ///     Gets the current property value from the current source and pushes it to the observers.
        /// </summary>
        /// <remarks>
        ///     We keep a local copy of the source value so that we don't end up with concurrency issues which
        ///     would occur if we pulled it form the source whenever it was needed e.g. when adding a new observer, as
        ///     we do not have a lock around the source, but we can control when it pushes new values into us with our lock.
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

                if (currObserver != null)
                    currObserver.OnNext(_propertyValue);
            }
        }

        /// <summary>
        ///     Removes a subscription, disposing of the upstream subscription if no longer required.
        /// </summary>
        /// <remarks>
        ///     This function is intentionally tolerant of observers being supplied that are not subscribed, or
        ///     the link having already completed.
        /// </remarks>
        /// <param name="observer">The observer to unsubscribe.</param>
        private void Unsubscribe(IObserver<TProperty> observer)
        {
            lock (_gate)
            {
                if (_isStopped)
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
                CleanUp();
            }
        }

        /// <summary>
        ///     Cleans up by unsubscribing to the source, and releases references.
        /// </summary>
        /// <remarks>Must be called inside the lock.</remarks>
        private void CleanUp()
        {
            _observer = null;
            _source = default(TSource);
            _propertyValue = default(TProperty);
            _sourceSubscriptionDisposable.Dispose();
            _sourceSubscriptionDisposable = null;
        }

        #region Nested type: Subscription

        /// <summary>
        ///     Lightweight subscription class that unsubscribes from the NotifyPropertyChangedLink when disposed.
        ///     This is much more performant than using a Disposable.Create().
        /// </summary>
        private class Subscription : IDisposable
        {
            private NotifyPropertyChangedLink<TSource, TProperty> _link;
            private IObserver<TProperty> _observer;

            public Subscription(NotifyPropertyChangedLink<TSource, TProperty> link, IObserver<TProperty> observer)
            {
                _link = link;
                _observer = observer;
            }

            #region IDisposable Members

            public void Dispose()
            {
                var currObserver = Interlocked.Exchange(ref _observer, null);
                if (currObserver == null)
                    return;

                _link.Unsubscribe(currObserver);
                _link = null;
            }

            #endregion
        }

        #endregion
    }
}