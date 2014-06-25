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
// </copyright>

using System;
using System.ComponentModel;

namespace KodeKandy.Panopticon.MarkRx
{
    public interface ILegacyPropertyChange
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

        /// <summary>
        /// Additional user data on the notification. Useful if dealing with re-entrancy issues.
        /// </summary>
        object UserData { get; }
    }

    public interface ILegacyPropertyChange<out T> : ILegacyPropertyChange
    {
        /// <summary>
        ///     The properties new value.
        /// </summary>
        new T Value { get; }
    }

    public class LegacyLegacyPropertyChange<T> : PropertyChangedEventArgs, ILegacyPropertyChange<T>, IEquatable<LegacyLegacyPropertyChange<T>>
    {
        public LegacyLegacyPropertyChange(object source, T value, string propertyNeme, object userData = null)
            : base(propertyNeme)
        {
            Source = source;
            Value = value;
            UserData = userData;
        }

        #region IEquatable<PropertyChange<T>> Members

        public bool Equals(LegacyLegacyPropertyChange<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Source, other.Source) && string.Equals(PropertyName, other.PropertyName) && Equals(Value, other.Value) && Equals(UserData, other.UserData);
        }

        #endregion

        #region ILegacyPropertyChange<T> Members

        /// <summary>
        ///     The originator of the change notification.
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        ///     The properties new value.
        /// </summary>
        object ILegacyPropertyChange.Value
        {
            get { return Value; }
        }

        /// <summary>
        ///     The properties new value.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Additional user data on the notification. Useful if dealing with re-entrancy issues.
        /// </summary>
        public object UserData { get; private set; }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LegacyLegacyPropertyChange<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PropertyName != null ? PropertyName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (UserData != null ? UserData.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(LegacyLegacyPropertyChange<T> left, LegacyLegacyPropertyChange<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LegacyLegacyPropertyChange<T> left, LegacyLegacyPropertyChange<T> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1} = {2}", Source.GetType().Name, PropertyName, Value);
        }
    }
}