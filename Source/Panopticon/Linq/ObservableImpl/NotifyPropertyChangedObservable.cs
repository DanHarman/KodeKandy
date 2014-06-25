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
    internal class NotifyPropertyChangedObservable<TClass> : IObserver<TClass>, IObservable<PropertyChanged>
        where TClass : class, INotifyPropertyChanged
    {
        private readonly IObservable<TClass> _sourceObservable;
        private IObserver<PropertyChanged> _observer = NopObserver<PropertyChanged>.Instance;
        private TClass _source;
        private IDisposable _sourceSubscriptionDisposable = Disposable.Empty;

        public NotifyPropertyChangedObservable(IObservable<TClass> sourceObservable)
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");

            _sourceObservable = sourceObservable;
        }

        #region IObservable<PropertyChanged> Members

        public IDisposable Subscribe(IObserver<PropertyChanged> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            IObserver<PropertyChanged> oldObserver;
            IObserver<PropertyChanged> newObserver;

            do
            {
                oldObserver = _observer;

                if (oldObserver == CompletedObserver<PropertyChanged>.Instance)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                var completedObserver = oldObserver as CompletedObserver<PropertyChanged>;
                if (completedObserver != null)
                {
                    observer.OnError(completedObserver.Error);
                    return Disposable.Empty;
                }

                if (oldObserver == NopObserver<PropertyChanged>.Instance)
                {
                    // If we have no _observer then make it our _observer and later on subscribe to _sourceObservable.
                    newObserver = observer;
                }
                else
                {
                    var multiObserver = _observer as ImmutableMultiObserver<PropertyChanged>;
                    if (multiObserver != null)
                    {
                        // If we already have a ImmutableMultiObserver then add the new _observer to it.
                        newObserver = multiObserver.Add(observer);
                    }
                    else
                    {
                        // We didn't have a multiobserver, so we must have just had a single observer, so replace it with a multiobserver containing
                        // both the old and new observer.
                        newObserver = new ImmutableMultiObserver<PropertyChanged>(
                            new ImmutableList<IObserver<PropertyChanged>>(new[] {oldObserver, observer}));
                    }
                }
            } while (Interlocked.CompareExchange(ref _observer, newObserver, oldObserver) != oldObserver);

            // Need to kick off the source observable subscription if we are the first subscriber.
            if (newObserver == observer)
            {
                _sourceSubscriptionDisposable = _sourceObservable.Subscribe(this);
            }

            return new Subscription(this, observer);
        }

        #endregion

        #region IObserver<TClass> Members

        void IObserver<TClass>.OnNext(TClass newSource)
        {
            TClass oldSource;
            do
            {
                oldSource = _source;
            } while (Interlocked.CompareExchange(ref _source, newSource, oldSource) != oldSource);

            if (newSource != null)
                newSource.PropertyChanged += OnPropertyChanged;

            if (oldSource != null)
                oldSource.PropertyChanged -= OnPropertyChanged;
        }

        void IObserver<TClass>.OnError(Exception error)
        {
            IObserver<PropertyChanged> oldObserver;
            IObserver<PropertyChanged> newObserver = new CompletedObserver<PropertyChanged>(error);

            // Spin in the completed observer.
            do
            {
                oldObserver = _observer;

                if (oldObserver is CompletedObserver<PropertyChanged>)
                    break;
            } while (Interlocked.CompareExchange(ref _observer, newObserver, oldObserver) != oldObserver);

            CleanUp();

            // Fire OnCompleted on the old oberver. If it happens to be a completed one, its effectively a nop.
            oldObserver.OnError(error);
        }

        void IObserver<TClass>.OnCompleted()
        {
            IObserver<PropertyChanged> oldObserver;
            IObserver<PropertyChanged> newObserver = CompletedObserver<PropertyChanged>.Instance;

            // Spin in the completed observer.
            do
            {
                oldObserver = _observer;

                if (oldObserver is CompletedObserver<PropertyChanged>)
                    break;
            } while (Interlocked.CompareExchange(ref _observer, newObserver, oldObserver) != oldObserver);

            CleanUp();

            // Fire OnCompleted on the old oberver. If it happens to be a completed one, its effectively a nop.
            oldObserver.OnCompleted();
        }

        #endregion

        private void CleanUp()
        {
            // Clear the source observable subscription.
            IDisposable oldSourceSubscriptionDisposable;
            do
            {
                oldSourceSubscriptionDisposable = _sourceSubscriptionDisposable;
// ReSharper disable once PossibleUnintendedReferenceComparison
            } while (Interlocked.CompareExchange(ref _sourceSubscriptionDisposable, Disposable.Empty, oldSourceSubscriptionDisposable) !=
                     oldSourceSubscriptionDisposable);

            if (!ReferenceEquals(oldSourceSubscriptionDisposable, Disposable.Empty))
                oldSourceSubscriptionDisposable.Dispose();

            // Clear the current source event subscription.
            TClass oldSource;
            do
            {
                oldSource = _source;
            } while (Interlocked.CompareExchange(ref _source, null, oldSource) != oldSource);

            if (oldSource != null)
                oldSource.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_source == sender)
            {
                _observer.OnNext(new PropertyChanged(sender, propertyChangedEventArgs));
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
        private void Unsubscribe(IObserver<PropertyChanged> observer)
        {
            IObserver<PropertyChanged> oldObserver;
            IObserver<PropertyChanged> newObserver;

            do
            {
                oldObserver = _observer;

                if (oldObserver is CompletedObserver<PropertyChanged>)
                    return;

                var multiObserver = oldObserver as ImmutableMultiObserver<PropertyChanged>;
                if (multiObserver != null)
                {
                    newObserver = multiObserver.Remove(observer);
                }
                else
                {
                    if (oldObserver != observer)
                        return;
                    newObserver = NopObserver<PropertyChanged>.Instance;
                }
            } while (Interlocked.CompareExchange(ref _observer, newObserver, oldObserver) != oldObserver);

            // If we got here we only had one observer, so clear our upstream subscription.
            if (oldObserver == observer)
                CleanUp();
        }

        #region Nested type: Subscription

        private class Subscription : IDisposable
        {
            private IObserver<PropertyChanged> _observer;
            private NotifyPropertyChangedObservable<TClass> _propertyChangedObservable;

            public Subscription(NotifyPropertyChangedObservable<TClass> propertyChangedObservable, IObserver<PropertyChanged> observer)
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