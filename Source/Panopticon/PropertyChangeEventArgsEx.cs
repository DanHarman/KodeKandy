// <copyright file="PropertyChangeEventArgsEx.cs" company="million miles per hour ltd">
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

using System.ComponentModel;

namespace KodeKandy.Panopticon
{
    /// <summary>
    ///     Extended version of <see cref="PropertyChangedEventArgs" /> which also captures the source and optionally
    ///     user data. This former is useful given we may be in deep rx chains or merging many object subscriptions into
    ///     one. The latter is very useful for certain re-entrancy problems.
    /// </summary>
    public class PropertyChangeEventArgsEx : PropertyChangedEventArgs
    {
        public PropertyChangeEventArgsEx(object source, string propertyNeme, object userData = null)
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
    }
}