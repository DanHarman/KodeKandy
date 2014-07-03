// <copyright file="NotifyPropertyChangedObservable.cs" company="million miles per hour ltd">
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
    ///     An observable link, that returns a stream of INotifyPropertyChangedEventArgs as a PropertyChanged object.
    /// </summary>
    /// <typeparam name="TClass">The class whose PropertyChanged event is being observerd.</typeparam>
    internal class NotifyPropertyChangedObservable<TClass> : IObserver<IPropertyValueChanged<TClass>>, IObservable<IPropertyChanged<TClass>>
        where TClass : class, INotifyPropertyChanged
    {
        private readonly object _gate = new object();
        private readonly IObservable<IPropertyValueChanged<TClass>> _sourceObservable;
        private IObserver<IPropertyChanged<TClass>> _observer = NopObserver<IPropertyChanged<TClass>>.Instance;
        private TClass _source;
        private IDisposable _sourceSubscriptionDisposable = Disposable.Empty;

        public NotifyPropertyChangedObservable(IObservable<IPropertyValueChanged<TClass>> sourceObservable)
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");

            _sourceObservable = sourceObservable;
        }

        #region IObservable<IPropertyChanged<TClass>> Members

        public IDisposable Subscribe(IObserver<IPropertyChanged<TClass>> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            CompletedObserver<IPropertyChanged<TClass>> completedObserver;

            lock (_gate)
            {
                completedObserver = _observer as CompletedObserver<IPropertyChanged<TClass>>;

                if (completedObserver == null)
                {
                    if (_observer == NopObserver<IPropertyChanged<TClass>>.Instance)
                    {
                        // If we have no _observer then make it our _observer and later on subscribe to _sourceObservable.
                        _observer = observer;
                        _sourceSubscriptionDisposable = _sourceObservable.Subscribe(this);
                    }
                    else
                    {
                        var multiObserver = _observer as ImmutableMultiObserver<IPropertyChanged<TClass>>;
                        if (multiObserver != null)
                        {
                            // If we already have a ImmutableMultiObserver then add the new _observer to it.
                            _observer = multiObserver.Add(observer);
                        }
                        else
                        {
                            // We didn't have a multiobserver, so we must have just had a single observer, so replace it with a multiobserver containing
                            // both the old and new observer.
                            _observer = new ImmutableMultiObserver<IPropertyChanged<TClass>>(
                                new ImmutableList<IObserver<IPropertyChanged<TClass>>>(new[] {_observer, observer}));
                        }

                        observer.OnNext(PropertyChanged.Create(_source));
                    }

                    return new Subscription(this, observer);
                }
            }

            if (completedObserver == CompletedObserver<IPropertyChanged<TClass>>.Instance)
                observer.OnCompleted();
            else
                observer.OnError(completedObserver.Error);

            return Disposable.Empty;
        }

        #endregion

        #region IObserver<IPropertyValueChanged<object,TClass>> Members

        void IObserver<IPropertyValueChanged<TClass>>.OnNext(IPropertyValueChanged<TClass> newSource)
        {
            TClass oldSource;
            lock (_gate)
            {
                oldSource = _source;
                _source = newSource.Value;
                _observer.OnNext(PropertyChanged.Create(_source));
            }

            if (newSource.HasValue)
                newSource.Value.PropertyChanged += OnPropertyChanged;

            if (oldSource != null)
                oldSource.PropertyChanged -= OnPropertyChanged;
        }

        void IObserver<IPropertyValueChanged<TClass>>.OnError(Exception error)
        {
            IObserver<IPropertyChanged<TClass>> oldObserver;
            IObserver<IPropertyChanged<TClass>> newObserver = new CompletedObserver<IPropertyChanged<TClass>>(error);


            lock (_gate)
            {
                if (_observer is CompletedObserver<IPropertyChanged<TClass>>)
                    return;

                oldObserver = _observer;
                _observer = newObserver;
                CleanUp();
            }

            oldObserver.OnError(error);
        }

        void IObserver<IPropertyValueChanged<TClass>>.OnCompleted()
        {
            IObserver<IPropertyChanged<TClass>> oldObserver;
            IObserver<IPropertyChanged<TClass>> newObserver = CompletedObserver<IPropertyChanged<TClass>>.Instance;

            // Spin in the completed observer.
            lock (_gate)
            {
                if (_observer is CompletedObserver<IPropertyChanged<TClass>>)
                    return;

                oldObserver = _observer;
                _observer = newObserver;
                CleanUp();
            }

            // Fire OnCompleted on the old oberver. If it happens to be a completed one, its effectively a nop.
            oldObserver.OnCompleted();
        }

        #endregion

        /// <summary>
        ///     Cleans up by unsubscribing to the sourceObservable, and releases references.
        /// </summary>
        /// <remarks>Must be called inside the lock.</remarks>
        private void CleanUp()
        {
            if (_source != null)
            {
                _source.PropertyChanged -= OnPropertyChanged;
                _source = null;
            }
            if (_sourceSubscriptionDisposable != null)
            {
                _sourceSubscriptionDisposable.Dispose();
                _sourceSubscriptionDisposable = null;
            }
        }


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            IObserver<IPropertyChanged<TClass>> currObserver;

            lock (_gate)
            {
                if (_source != sender)
                    return;

                currObserver = _observer;
            }

            currObserver.OnNext(PropertyChanged.Create((TClass)sender, propertyChangedEventArgs));
        }

        /// <summary>
        ///     Removes a subscription, disposing of the upstream subscription if no longer required.
        /// </summary>
        /// <remarks>
        ///     This function is intentionally tolerant of observers being supplied that are not subscribed, or
        ///     thid observable having already completed.
        /// </remarks>
        /// <param name="observer">The observer to unsubscribe.</param>
        private void Unsubscribe(IObserver<IPropertyChanged<TClass>> observer)
        {
            IObserver<IPropertyChanged<TClass>> oldObserver;
            IObserver<IPropertyChanged<TClass>> newObserver;

            do
            {
                oldObserver = _observer;

                if (oldObserver is CompletedObserver<IPropertyChanged<TClass>>)
                    return;

                var multiObserver = oldObserver as ImmutableMultiObserver<IPropertyChanged<TClass>>;
                if (multiObserver != null)
                {
                    newObserver = multiObserver.Remove(observer);
                }
                else
                {
                    if (oldObserver != observer)
                        return;
                    newObserver = NopObserver<IPropertyChanged<TClass>>.Instance;
                }
            } while (Interlocked.CompareExchange(ref _observer, newObserver, oldObserver) != oldObserver);

            // If we got here we only had one observer, so clear our upstream subscription.
            if (oldObserver == observer)
                CleanUp();
        }

        #region Nested type: Subscription

        private class Subscription : IDisposable
        {
            private IObserver<IPropertyChanged<TClass>> _observer;
            private NotifyPropertyChangedObservable<TClass> _propertyChangedObservable;

            public Subscription(NotifyPropertyChangedObservable<TClass> propertyChangedObservable, IObserver<IPropertyChanged<TClass>> observer)
            {
                _propertyChangedObservable = propertyChangedObservable;
                _observer = observer;
            }

            #region IDisposable Members

            public void Dispose()
            {
                var currObserver = Interlocked.Exchange(ref _observer, null);
                if (currObserver == null)
                    return;

                _propertyChangedObservable.Unsubscribe(currObserver);
                _propertyChangedObservable = null;
            }

            #endregion
        }

        #endregion
    }
}