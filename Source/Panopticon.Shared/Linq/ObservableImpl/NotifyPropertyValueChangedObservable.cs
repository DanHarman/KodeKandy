// <copyright file="NotifyPropertyValueChangedObservable.cs" company="million miles per hour ltd">
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
using KodeKandy.Panopticon.Internal;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     An observable that watches a class and provides an observable for a property on that class, if the class implement
    ///     <see cref="INotifyPropertyChanged" />. If the class does not implement that interface, then only the initial value
    ///     is pushed onto the observable.
    /// 
    ///     It has been highly optimised for this purpose so offers much higher performance than solutions built upon composing
    ///     FromEventPattern etc.
    /// 
    ///     The returned <see cref="IPropertyValueChanged{TProperty}" /> captures whether or not the source is null, so when
    ///     these observables are composed together, you can subscribe to a property path from a root node, and detect if
    ///     this path is broken.
    /// </summary>
    /// <typeparam name="TClass">The type of the observered clas.</typeparam>
    /// <typeparam name="TProperty">The type of the observered property.</typeparam>
    internal class NotifyPropertyValueChangedObservable<TClass, TProperty> :
        AdaptingObservableBase<IPropertyValueChanged<TClass>, IPropertyValueChanged<TProperty>>
        where TClass : class
    {
        private readonly string _propertyName;
        private readonly Func<TClass, TProperty> _propertyValueGetter;

        public NotifyPropertyValueChangedObservable(IObservable<IPropertyValueChanged<TClass>> sourceObservable, string propertyName,
            Func<TClass, TProperty> propertyValueGetter) : base(sourceObservable)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (propertyValueGetter == null)
                throw new ArgumentNullException("propertyValueGetter");

            _propertyName = propertyName;
            _propertyValueGetter = propertyValueGetter;
        }

        protected override IAdaptor<IPropertyValueChanged<TClass>, IPropertyValueChanged<TProperty>> CreateAdaptor()
        {
            return new NotifyPropertyValueChangedAdaptor(this);
        }

        #region Nested type: NotifyPropertyValueChangedAdaptor

        /// <summary>
        /// Takes a stream of TClass instances and projects into a flat mapped stream of property changes for a defined property.
        /// </summary>
        private class NotifyPropertyValueChangedAdaptor : IAdaptor<IPropertyValueChanged<TClass>, IPropertyValueChanged<TProperty>>
        {
            private readonly NotifyPropertyValueChangedObservable<TClass, TProperty> _observable;
            private IPropertyValueChanged<TProperty> _currentPropertyValueChanged;
            /// <summary>
            ///     The current instance of the source.
            /// </summary>
            private TClass _source;

            public NotifyPropertyValueChangedAdaptor(NotifyPropertyValueChangedObservable<TClass, TProperty> observable)
            {
                _observable = observable;
            }

            #region IAdaptor<IPropertyValueChanged<TClass>,IPropertyValueChanged<TProperty>> Members

            public void OnNext(IPropertyValueChanged<TClass> newSourcePropertyValueChanged)
            {
                IObserver<IPropertyValueChanged<TProperty>> oldObserver;
                TClass oldSource;
                IPropertyValueChanged<TProperty> initialPropertyValueChanged;

                lock (_observable.Gate)
                {
                    if (_observable.Observer is CompletedObserver<IPropertyValueChanged<TProperty>>)
                        return;

                    oldObserver = _observable.Observer;
                    oldSource = _source;
                    _source = newSourcePropertyValueChanged.Value;

                    // We need to get the current property value from this new source if it is not null, but if it is propagate the nullness
                    // with a PropertyValueChanged with HasValue == false.
                    initialPropertyValueChanged = _source != null
                        ? PropertyValueChanged.CreateWithValue(_source, _observable._propertyName, _observable._propertyValueGetter(_source))
                        : PropertyValueChanged.CreateWithoutValue<TClass, TProperty>(_source, _observable._propertyName);

                    _currentPropertyValueChanged = initialPropertyValueChanged;
                }

                // Connect to INotifyPropertyChanged on the new _source.
                var newSourceAsNotifyPropertyChanged = newSourcePropertyValueChanged.Value as INotifyPropertyChanged;
                if (newSourceAsNotifyPropertyChanged != null)
                    newSourceAsNotifyPropertyChanged.PropertyChanged += OnPropertyChanged;

                // Disconnect from INotifyPropertyChanged on previous _source. Because _source has been changed it doesn't matter
                // if we end up with an extra event from this old sourceObservable sneaking through as it will be filtered out.
                var oldSourceAsNotifyPropertyChanged = oldSource as INotifyPropertyChanged;
                if (oldSourceAsNotifyPropertyChanged != null)
                    oldSourceAsNotifyPropertyChanged.PropertyChanged -= OnPropertyChanged;

                oldObserver.OnNext(initialPropertyValueChanged);
            }

            public void OnError(Exception error)
            {
                _observable.OnError(error);
            }

            public void OnCompleted()
            {
                _observable.OnCompleted();
            }

            public void NotifyInitialValue(IObserver<IPropertyValueChanged<TProperty>> observer)
            {
                observer.OnNext(_currentPropertyValueChanged);
            }

            public void Dispose()
            {
                var sourceAsNotifyPropertyChanged = _source as INotifyPropertyChanged;
                if (sourceAsNotifyPropertyChanged != null)
                {
                    sourceAsNotifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
                }

                _source = null;
                _currentPropertyValueChanged = null;
            }

            #endregion

            /// <summary>
            ///     Internal <see cref="PropertyChangedEventHandler" /> to bind to the targets PropertyChanged event.
            ///     Gets the current property value from the current sourceObservable and pushes it to the observers.
            /// </summary>
            /// <remarks>
            ///     We keep a local copy of the current property value so that we don't end up with concurrency issues which
            ///     would occur if we pulled it form the sourceObservable on demand e.g. when adding a new observer, as
            ///     we do not have a lock around the sourceObservable, but we can control when it pushes new values into us with our
            ///     lock.
            /// </remarks>
            private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
            {
                IObserver<IPropertyValueChanged<TProperty>> currObserver;
                IPropertyValueChanged<TProperty> propertyValueChanged;

                lock (_observable.Gate)
                {
                    if (_source != sender || _observable._propertyName != propertyChangedEventArgs.PropertyName)
                        return;

                    currObserver = _observable.Observer;
                    propertyValueChanged = PropertyValueChanged.CreateWithValue(_source, propertyChangedEventArgs,
                        _observable._propertyValueGetter(_source));
                    _currentPropertyValueChanged = propertyValueChanged;
                }

                currObserver.OnNext(propertyValueChanged);
            }
        }

        #endregion
    }
}