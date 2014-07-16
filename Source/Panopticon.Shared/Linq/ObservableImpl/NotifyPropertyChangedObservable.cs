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

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     An observable that watches a class and provides an observable stream of <see cref="IPropertyChanged" />.
    ///     This encapsulates the OnPropertyChanged
    ///     callback exposed by <see cref="INotifyPropertyChanged" /> into an RX stream and is not bound to a specific property
    ///     unlike <see cref="NotifyPropertyValueChangedObservable{TClass,TProperty}" />
    /// </summary>
    /// <typeparam name="TClass">The class whose PropertyChanged event is being projected.</typeparam>
    internal class NotifyPropertyChangedObservable<TClass> : AdaptingObservableBase<IPropertyValueChanged<TClass>, IPropertyChanged<TClass>>
        where TClass : class, INotifyPropertyChanged
    {
        public NotifyPropertyChangedObservable(IObservable<IPropertyValueChanged<TClass>> sourceObservable)
            : base(sourceObservable)
        {
        }

        protected override IAdaptor<IPropertyValueChanged<TClass>, IPropertyChanged<TClass>> CreateAdaptor()
        {
            return new PropertyChangedAdaptor(this);
        }

        #region Nested type: PropertyChangedAdaptor

        private class PropertyChangedAdaptor : IAdaptor<IPropertyValueChanged<TClass>, IPropertyChanged<TClass>>
        {
            private readonly NotifyPropertyChangedObservable<TClass> _observable;
            /// <summary>
            ///     The current instance of the source.
            /// </summary>
            private TClass _source;

            public PropertyChangedAdaptor(NotifyPropertyChangedObservable<TClass> observable)
            {
                _observable = observable;
            }

            #region IAdaptor<IPropertyValueChanged<TClass>,IPropertyChanged<TClass>> Members

            void IObserver<IPropertyValueChanged<TClass>>.OnNext(IPropertyValueChanged<TClass> newSource)
            {
                TClass oldSource;
                lock (_observable.Gate)
                {
                    oldSource = _source;
                    _source = newSource.Value;
                    _observable.Observer.OnNext(PropertyChanged.Create(_source));
                }

                if (newSource.HasValue)
                    newSource.Value.PropertyChanged += OnPropertyChanged;

                if (oldSource != null)
                    oldSource.PropertyChanged -= OnPropertyChanged;
            }

            void IObserver<IPropertyValueChanged<TClass>>.OnError(Exception error)
            {
                _observable.OnError(error);
            }

            void IObserver<IPropertyValueChanged<TClass>>.OnCompleted()
            {
                _observable.OnCompleted();
            }

            public void NotifyInitialValue(IObserver<IPropertyChanged<TClass>> observer)
            {
                observer.OnNext(PropertyChanged.Create(_source));
            }

            public void Dispose()
            {
                var sourceAsNotifyPropertyChanged = _source as INotifyPropertyChanged;
                if (sourceAsNotifyPropertyChanged != null)
                {
                    sourceAsNotifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
                }

                _source = null;
            }

            #endregion

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
            {
                IObserver<IPropertyChanged<TClass>> currObserver;

                lock (_observable.Gate)
                {
                    if (_source != sender)
                        return;

                    currObserver = _observable.Observer;
                }

                currObserver.OnNext(PropertyChanged.Create((TClass) sender, propertyChangedEventArgs));
            }
        }

        #endregion
    }
}