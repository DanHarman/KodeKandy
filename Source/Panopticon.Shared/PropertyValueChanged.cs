// <copyright file="PropertyValueChanged.cs" company="million miles per hour ltd">
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
using System.Collections.Generic;
using System.ComponentModel;
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon
{
    public static class PropertyValueChanged
    {
        public static IPropertyValueChanged<TProperty> CreateWithValue<TProperty>(object source, string propertyName, TProperty value)
        {
            return new PropertyValueChanged<TProperty>(source, propertyName, value);
        }

        public static IPropertyValueChanged<TProperty> CreateWithValue<TProperty>(object source, PropertyChangedEventArgs propertyChangedEventArgs,
            TProperty value)
        {
            return new PropertyValueChanged<TProperty>(source, propertyChangedEventArgs, value);
        }


        public static IPropertyValueChanged<TProperty> CreateWithoutValue<TProperty>(object source, string propertyName)
        {
            return new PropertyValueChanged<TProperty>(source, propertyName);
        }
    }

    /// <summary>
    ///     Represents a property value change and ensures co-variance is supported.
    /// </summary>
    /// <remarks>This was introduced as otherwise subscribing to properties on base classes cauaesd covariance problems.</remarks>
    /// <typeparam name="TProperty">The type of the property being observed.</typeparam>
    public interface IPropertyValueChanged<out TProperty>
    {
        TProperty Value { get; }

        bool HasValue { get; }
    }

    /// <summary>
    ///     Captures the info normally found on a PropetyChagnedEventArgs AND the current value of a property, if it is
    ///     obtainable, which may not be the case if we are observing a property chain with a null node.
    /// </summary>
    /// <typeparam name="TProperty">The observered properties type.</typeparam>
    public class PropertyValueChanged<TProperty> : PropertyChanged, IPropertyValueChanged<TProperty>, IEquatable<PropertyValueChanged<TProperty>>
    {
        private readonly bool _hasValue;
        private readonly TProperty _value;

        public PropertyValueChanged(object source, string propertyName, TProperty value)
            : this(source, new PropertyChangedEventArgsEx(propertyName), value)
        {
        }

        public PropertyValueChanged(object source, [NotNull] PropertyChangedEventArgs propertyChangedEventArgs, TProperty value)
            : this(source, propertyChangedEventArgs)
        {
            _value = value;
            _hasValue = true;
        }

        public PropertyValueChanged(object source, string propertyName)
            : this(source, new PropertyChangedEventArgsEx(propertyName))
        {
        }

        public PropertyValueChanged(object source, [NotNull] PropertyChangedEventArgs propertyChangedEventArgs)
            : base(source, propertyChangedEventArgs)
        {
        }

        #region IEquatable<PropertyValueChanged<TProperty>> Members

        public bool Equals(PropertyValueChanged<TProperty> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && EqualityComparer<TProperty>.Default.Equals(_value, other._value);
        }

        #endregion

        #region IPropertyValueChanged<TProperty> Members

        public TProperty Value
        {
            get { return _value; }
        }

        public bool HasValue
        {
            get { return _hasValue; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PropertyValueChanged<TProperty>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ EqualityComparer<TProperty>.Default.GetHashCode(_value);
            }
        }
    }
}