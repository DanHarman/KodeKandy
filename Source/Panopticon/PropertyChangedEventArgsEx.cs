// <copyright file="PropertyChangedEventArgsEx.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Panopticon
{
    /// <summary>
    ///     Extended version of <see cref="PropertyChangedEventArgs" /> which captures a user data field.
    ///     The can be very useful for certain re-entrancy problems where an observer may want to filter out
    ///     changes it triggered itself.
    /// </summary>
    public class PropertyChangedEventArgsEx : PropertyChangedEventArgs, IEquatable<PropertyChangedEventArgsEx>
    {
        public static readonly PropertyChangedEventArgsEx Default = new PropertyChangedEventArgsEx(string.Empty);

        public PropertyChangedEventArgsEx(string propertyNeme)
            : this(propertyNeme, null)
        {
        }

        public PropertyChangedEventArgsEx(string propertyNeme, object userData)
            : base(propertyNeme)
        {
            UserData = userData;
        }

        /// <summary>
        ///     Additional user data on the notification. Useful if dealing with re-entrancy issues.
        /// </summary>
        public object UserData { get; private set; }

        #region IEquatable<PropertyChangedEventArgsEx> Members

        public bool Equals(PropertyChangedEventArgsEx other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(UserData, other.UserData);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PropertyChangedEventArgsEx) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (UserData != null ? UserData.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("PropertyChangedEventArgsEx: PropertyName='{0}', UserData='{1}'", PropertyName, UserData);
        }
    }
}