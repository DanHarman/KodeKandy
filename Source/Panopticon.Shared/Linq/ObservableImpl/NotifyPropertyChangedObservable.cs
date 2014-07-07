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
using KodeKandy.Panopticon.Internal;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     An observable that watches a class and provides an observable stream of <see cref="IPropertyChanged" />.
    ///     This encapsulates the OnPropertyChanged
    ///     callback exposed by <see cref="INotifyPropertyChanged" /> into an RX stream and is not bound to a specific property
    ///     unlike <see cref="NotifyPropertyValueChangedObservable{TClass,TProperty}" />
    /// </summary>
    /// <typeparam name="TClass">The class whose PropertyChanged event is being projected.</typeparam>
    internal class NotifyPropertyChangedObservable<TClass> : NotifyPropertyChangedObservableBase<TClass, IPropertyChanged<TClass>>,
        IObserver<IPropertyValueChanged<TClass>>, IObservable<IPropertyChanged<TClass>>
        where TClass : class, INotifyPropertyChanged
    {
        public NotifyPropertyChangedObservable(IObservable<IPropertyValueChanged<TClass>> sourceObservable)
            : base(sourceObservable)
        {
        }

        #region IObservable<IPropertyChanged<TClass>> Members

        public IDisposable Subscribe(IObserver<IPropertyChanged<TClass>> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            CompletedObserver<IPropertyChanged<TClass>> completedObserver;

            lock (Gate)
            {
                completedObserver = Observer as CompletedObserver<IPropertyChanged<TClass>>;

                if (completedObserver == null)
                {
                    if (Observer == NopObserver<IPropertyChanged<TClass>>.Instance)
                    {
                        // If we have no _observer then make it our _observer and subscribe to _sourceObservable.
                        Observer = observer;
                        SourceSubscriptionDisposable = SourceObservable.Subscribe(this);
                    }
                    else
                    {
                        var multiObserver = Observer as ImmutableMultiObserver<IPropertyChanged<TClass>>;
                        if (multiObserver != null)
                        {
                            // If we already have a ImmutableMultiObserver then add the new _observer to it.
                            Observer = multiObserver.Add(observer);
                        }
                        else
                        {
                            // We didn't have a multiobserver, so we must have just had a single observer, so replace it with a multiobserver containing
                            // both the old and new observer.
                            Observer = new ImmutableMultiObserver<IPropertyChanged<TClass>>(
                                new ImmutableList<IObserver<IPropertyChanged<TClass>>>(new[] {Observer, observer}));
                        }

                        observer.OnNext(PropertyChanged.Create(Source));
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

        #region IObserver<IPropertyValueChanged<TClass>> Members

        void IObserver<IPropertyValueChanged<TClass>>.OnNext(IPropertyValueChanged<TClass> newSource)
        {
            TClass oldSource;
            lock (Gate)
            {
                oldSource = Source;
                Source = newSource.Value;
                Observer.OnNext(PropertyChanged.Create(Source));
            }

            if (newSource.HasValue)
                newSource.Value.PropertyChanged += OnPropertyChanged;

            if (oldSource != null)
                oldSource.PropertyChanged -= OnPropertyChanged;
        }

        void IObserver<IPropertyValueChanged<TClass>>.OnError(Exception error)
        {
            base.OnError(error);
        }

        void IObserver<IPropertyValueChanged<TClass>>.OnCompleted()
        {
            base.OnCompleted();
        }

        #endregion

        /// <summary>
        ///     Cleans up by unsubscribing to the sourceObservable, release references & etc.
        /// </summary>
        /// <remarks>Must be called inside the lock.</remarks>
        protected override void CleanUp()
        {
            if (Source != null)
            {
                Source.PropertyChanged -= OnPropertyChanged;
                Source = null;
            }

            if (SourceSubscriptionDisposable != null)
            {
                SourceSubscriptionDisposable.Dispose();
                SourceSubscriptionDisposable = null;
            }
        }


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            IObserver<IPropertyChanged<TClass>> currObserver;

            lock (Gate)
            {
                if (Source != sender)
                    return;

                currObserver = Observer;
            }

            currObserver.OnNext(PropertyChanged.Create((TClass) sender, propertyChangedEventArgs));
        }
    }
}