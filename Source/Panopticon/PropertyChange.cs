// <copyright file="PropertyChange.cs" company="million miles per hour ltd">
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
// 
// </copyright>

using System;

namespace KodeKandy.Panopticon
{
    public interface IPropertyChange
    {
        /// <summary>
        ///     The originator of the change notification.
        /// </summary>
        object Source { get; }

        /// <summary>
        ///     The name of the changed property.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        ///     The properties new value.
        /// </summary>
        object Value { get; }
    }

    public interface IPropertyChange<out T> : IPropertyChange
    {
        /// <summary>
        ///     The properties new value.
        /// </summary>
        new T Value { get; }
    }

    public static class PropertyChange
    {
        public static IPropertyChange Create<T>(object source, T value, string propertyNeme)
        {
            return new PropertyChange<T>(source, value, propertyNeme);
        }
    }

    public class PropertyChange<T> : IPropertyChange<T>, IEquatable<PropertyChange<T>>
    {
        /// <summary>
        ///     The originator of the change notification.
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        ///     The name of the changed property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        ///     The properties new value.
        /// </summary>
        object IPropertyChange.Value
        {
            get { return Value; }
        }

        /// <summary>
        ///     The properties new value.
        /// </summary>
        public T Value { get; private set; }

        public PropertyChange(object source, T value, string propertyNeme)
        {
            Source = source;
            PropertyName = propertyNeme;
            Value = value;
        }

        public bool Equals(PropertyChange<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Source, other.Source) && string.Equals(PropertyName, other.PropertyName) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PropertyChange<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PropertyName != null ? PropertyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PropertyChange<T> left, PropertyChange<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PropertyChange<T> left, PropertyChange<T> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1} = {2}", Source.GetType().Name, PropertyName, Value);
        }
    }
}