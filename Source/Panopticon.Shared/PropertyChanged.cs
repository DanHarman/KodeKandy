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
        private readonly object _source;

        /// <summary>
        ///     Constructs a property change indicating that the source has been refreshed so all properties may be different.
        /// </summary>
        /// <param name="source">The source of the change.</param>
        public PropertyChanged(object source)
            : this(source, PropertyChangedEventArgsEx.Default)
        {
        }

        public PropertyChanged(object source, string propertyName)
            : this(source, new PropertyChangedEventArgsEx(propertyName))
        {
        }

        public PropertyChanged(object source, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _source = source;
            _propertyChangedEventArgs = propertyChangedEventArgs;
        }

        public PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get { return _propertyChangedEventArgs; }
        }

        public object Source
        {
            get { return _source; }
        }

        #region IEquatable<PropertyChanged> Members

        public bool Equals(PropertyChanged other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_source, other._source) && Equals(_propertyChangedEventArgs.PropertyName, other._propertyChangedEventArgs.PropertyName);
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
                return ((_source != null ? _source.GetHashCode() : 0) * 397) ^
                       (_propertyChangedEventArgs != null ? _propertyChangedEventArgs.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("PropertyChanged: SourceHash='{0}', PropertyChangedEventArgs='{1}'", Source == null ? 0 : Source.GetHashCode(),
                PropertyChangedEventArgs);
        }
    }
}