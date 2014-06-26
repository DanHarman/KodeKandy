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
using System.Collections.Generic;
using System.ComponentModel;

namespace KodeKandy.Panopticon
{
    public class PropertyValueChanged<TProperty> : PropertyChanged, IEquatable<PropertyValueChanged<TProperty>>
    {
        private readonly TProperty _value;
        private readonly bool _hasValue;

        public PropertyValueChanged(object sender, string propertyName, TProperty value)
            : this(sender, new PropertyChangedEventArgs(propertyName), value)
        {
        }

        public PropertyValueChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs, TProperty value)
            : this(sender, propertyChangedEventArgs)
        {
            _value = value;
            _hasValue = true;
        }

        public PropertyValueChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
            : base(sender, propertyChangedEventArgs)
        {
        }

        public TProperty Value
        {
            get { return _value; }
        }

        public bool HasValue
        {
            get { return _hasValue; }
        }

        #region IEquatable<PropertyValueChanged<TProperty>> Members

        public bool Equals(PropertyValueChanged<TProperty> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && EqualityComparer<TProperty>.Default.Equals(_value, other._value);
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