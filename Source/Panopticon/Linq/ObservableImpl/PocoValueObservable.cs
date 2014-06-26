// <copyright file="PocoValueObservable.cs" company="million miles per hour ltd">
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
using System.Reactive.Linq;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     Forms a one-shot link for classes that do not implement <see cref="INotifyPropertyChanged" />, this fires the
    ///     current value of the member on the source as soon as it is is fired in.
    /// </summary>
    /// <typeparam name="TClass">Source class type.</typeparam>
    /// <typeparam name="TMember">Member type on source class.</typeparam>
    internal class PocoValueObservable<TClass, TMember> : IObservable<PropertyValueChanged<TMember>>
        where TClass : class
    {
        private readonly Func<TClass, TMember> _memberValueGetter;
        private readonly string _propertyName;
        private readonly IObservable<PropertyValueChanged<TClass>> _sourceObservable;

        public PocoValueObservable(IObservable<PropertyValueChanged<TClass>> sourceObservable, string propertyName,
            Func<TClass, TMember> memberValueGetter)
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");
            if (memberValueGetter == null)
                throw new ArgumentNullException("memberValueGetter");

            _sourceObservable = sourceObservable;
            _propertyName = propertyName;
            _memberValueGetter = memberValueGetter;
        }

        #region IObservable<PropertyValueChanged<TMember>> Members

        public IDisposable Subscribe(IObserver<PropertyValueChanged<TMember>> observer)
        {
            // If the source has provided a value then propagate it, otherwise propagate the change without a value (this occurs
            // when a property chain has a null node for example, so it is not possible to provide a value from the leaf node).
            return _sourceObservable
                .Select(newSource => newSource.HasValue
                    ? PropertyValueChanged.CreateWithValue(newSource.Value, _propertyName, _memberValueGetter(newSource.Value))
                    : PropertyValueChanged.CreateWithoutValue<TMember>(null, _propertyName))
                .Subscribe(observer);
        }

        #endregion
    }
}