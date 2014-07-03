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
    public interface IPropertyChanged
    {
        object Source { get; }

        PropertyChangedEventArgs PropertyChangedEventArgs { get; }
    }

    public interface IPropertyChanged<out TClass> : IPropertyChanged
        where TClass : class
    {
        new TClass Source { get; }
    }

    public static class PropertyChanged
    {
        public static IPropertyChanged<TClass> Create<TClass>(TClass source)
            where TClass : class
        {
            return new PropertyChanged<TClass>(source);
        }

        public static IPropertyChanged<TClass> Create<TClass>(TClass source, string propertyName)
            where TClass : class
        {
            return new PropertyChanged<TClass>(source, propertyName);
        }

        public static IPropertyChanged<TClass> Create<TClass>(TClass source, PropertyChangedEventArgs propertyChangedEventArgs)
            where TClass : class
        {
            return new PropertyChanged<TClass>(source, propertyChangedEventArgs);
        }
    }

    /// <summary>
    ///     Captures the info normally provided in a OnPropertyChanged handler.
    /// </summary>
    public class PropertyChanged<TClass> : IPropertyChanged<TClass>, IEquatable<PropertyChanged<TClass>>
        where TClass : class
    {
        private readonly PropertyChangedEventArgs _propertyChangedEventArgs;
        private readonly TClass _source;

        /// <summary>
        ///     Constructs a property change indicating that the source has been refreshed so all properties may be different.
        /// </summary>
        /// <param name="source">The source of the change.</param>
        internal PropertyChanged(TClass source)
            : this(source, PropertyChangedEventArgsEx.Default)
        {
        }

        internal PropertyChanged(TClass source, string propertyName)
            : this(source, new PropertyChangedEventArgsEx(propertyName))
        {
        }

        internal PropertyChanged(TClass source, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _source = source;
            _propertyChangedEventArgs = propertyChangedEventArgs;
        }

        #region IEquatable<PropertyChanged<TClass>> Members

        public bool Equals(PropertyChanged<TClass> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_source, other.Source) && Equals(_propertyChangedEventArgs.PropertyName, other.PropertyChangedEventArgs.PropertyName);
        }

        #endregion

        #region IPropertyChanged<TClass> Members

        public PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get { return _propertyChangedEventArgs; }
        }

        object IPropertyChanged.Source
        {
            get { return _source; }
        }

        public TClass Source
        {
            get { return _source; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PropertyChanged<TClass>) obj);
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