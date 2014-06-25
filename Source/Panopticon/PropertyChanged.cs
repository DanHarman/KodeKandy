// <copyright file="PropertyChanged.cs" company="million miles per hour ltd">
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
    ///     Captures the info normally provided in a OnPropertyChanged handler.
    /// </summary>
    public class PropertyChanged : IEquatable<PropertyChanged>
    {
        private readonly PropertyChangedEventArgs _propertyChangedEventArgs;
        private readonly object _sender;

        public PropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _sender = sender;
            _propertyChangedEventArgs = propertyChangedEventArgs;
        }

        public PropertyChanged(object sender, string propertyName) : this(sender, new PropertyChangedEventArgs(propertyName))
        {
        }

        public object Sender
        {
            get { return _sender; }
        }
        public PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get { return _propertyChangedEventArgs; }
        }

        #region IEquatable<PropertyChanged> Members

        public bool Equals(PropertyChanged other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_sender, other._sender) && Equals(_propertyChangedEventArgs.PropertyName, other._propertyChangedEventArgs.PropertyName);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PropertyChanged) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_sender != null ? _sender.GetHashCode() : 0) * 397) ^
                       (_propertyChangedEventArgs != null ? _propertyChangedEventArgs.GetHashCode() : 0);
            }
        }
    }
}