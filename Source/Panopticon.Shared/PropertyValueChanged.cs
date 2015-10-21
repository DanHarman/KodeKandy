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
using KodeKandy.Properties;

namespace KodeKandy.Panopticon
{
    public static class PropertyValueChanged
    {
        public static IPropertyValueChanged<TProperty> CreateWithValue<TClass, TProperty>(TClass source, string propertyName, TProperty value)
            where TClass : class
        {
            return new PropertyValueChanged<TClass, TProperty>(source, propertyName, value);
        }

        public static IPropertyValueChanged<TProperty> CreateWithValue<TClass, TProperty>(TClass source, PropertyChangedEventArgs propertyChangedEventArgs,
            TProperty value)
            where TClass : class 
        {
            return new PropertyValueChanged<TClass, TProperty>(source, propertyChangedEventArgs, value);
        }


        public static IPropertyValueChanged<TProperty> CreateWithoutValue<TClass, TProperty>(TClass source, string propertyName)
            where TClass : class
        {
            return new PropertyValueChanged<TClass, TProperty>(source, propertyName);
        }
    }

    /// <summary>
    ///     Represents a property value change and ensures co-variance is supported.
    /// </summary>
    /// <remarks>This was introduced as otherwise subscribing to properties on base classes cauaesd covariance problems.</remarks>
    /// <typeparam name="TProperty">The type of the property being observed.</typeparam>
    public interface IPropertyValueChanged<out TProperty> : IPropertyChanged
    {
        TProperty Value { get; }

        bool HasValue { get; }
    }

    /// <summary>
    ///     Captures the info normally found on a PropetyChangedEventArgs AND the current value of a property, if it is
    ///     obtainable, which may not be the case if we are observing a property chain with a null node.
    /// </summary>
    /// <typeparam name="TProperty">The observered properties type.</typeparam>
    /// <typeparam name="TClass">The property declaring type's class.</typeparam>
    public class PropertyValueChanged<TClass, TProperty> : PropertyChanged<TClass>, IPropertyValueChanged<TProperty>, IEquatable<IPropertyValueChanged<TProperty>>
        where TClass : class
    {
        private readonly bool _hasValue;
        private readonly TProperty _value;

        public PropertyValueChanged(TClass source, string propertyName, TProperty value)
            : this(source, new PropertyChangedEventArgsEx(propertyName), value)
        {
        }

        public PropertyValueChanged(TClass source, [NotNull] PropertyChangedEventArgs propertyChangedEventArgs, TProperty value)
            : this(source, propertyChangedEventArgs)
        {
            _value = value;
            _hasValue = true;
        }

        public PropertyValueChanged(TClass source, string propertyName)
            : this(source, new PropertyChangedEventArgsEx(propertyName))
        {
        }

        public PropertyValueChanged(TClass source, [NotNull] PropertyChangedEventArgs propertyChangedEventArgs)
            : base(source, propertyChangedEventArgs)
        {
        }

        #region IEquatable<PropertyValueChanged<TProperty>> Members

        public bool Equals(IPropertyValueChanged<TProperty> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && EqualityComparer<TProperty>.Default.Equals(_value, other.Value);
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
            return Equals((IPropertyValueChanged<TProperty>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ EqualityComparer<TProperty>.Default.GetHashCode(_value);
            }
        }

        public override string ToString()
        {
            return string.Format("PropertyValueChanged<{0},{1}>: HasValue='{2}', Value='{3}', PropertyChangedEventArgs='{4}'", typeof(TClass).Name, typeof(TProperty).Name,
                HasValue, Value, PropertyChangedEventArgs);
        }
    }
}