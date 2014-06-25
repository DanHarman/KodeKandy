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
    ///     Extended version of <see cref="PropertyChangedEventArgs" /> which also captures the source and optionally
    ///     user data. This former is useful given we may be in deep rx chains or merging many object subscriptions into
    ///     one. The latter is very useful for certain re-entrancy problems.
    /// </summary>
    public class PropertyChangedEventArgsEx : PropertyChangedEventArgs, IEquatable<PropertyChangedEventArgsEx>
    {
        public PropertyChangedEventArgsEx(object source, string propertyNeme, object userData = null)
            : base(propertyNeme)
        {
            Source = source;
            UserData = userData;
        }

        /// <summary>
        ///     The originator of the change notification.
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        ///     Additional user data on the notification. Useful if dealing with re-entrancy issues.
        /// </summary>
        public object UserData { get; private set; }

        #region IEquatable<PropertyChangedEventArgsEx> Members

        public bool Equals(PropertyChangedEventArgsEx other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Source, other.Source) && Equals(UserData, other.UserData);
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
                return ((Source != null ? Source.GetHashCode() : 0) * 397) ^ (UserData != null ? UserData.GetHashCode() : 0);
            }
        }
    }
}