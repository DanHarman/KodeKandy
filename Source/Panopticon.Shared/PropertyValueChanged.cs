using System;
using System.Collections.Generic;
using System.ComponentModel;
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon
{
    public static class PropertyValueChanged
    {
        public static PropertyValueChanged<TProperty> CreateWithValue<TProperty>(object source, string propertyName, TProperty value)
        {
            return new PropertyValueChanged<TProperty>(source, propertyName, value);
        }

        public static PropertyValueChanged<TProperty> CreateWithValue<TProperty>(object source, PropertyChangedEventArgs propertyChangedEventArgs, TProperty value)
        {
            return new PropertyValueChanged<TProperty>(source, propertyChangedEventArgs, value);
        }


        public static PropertyValueChanged<TProperty> CreateWithoutValue<TProperty>(object source, string propertyName)
        {
            return new PropertyValueChanged<TProperty>(source, propertyName);
        }
    }

    /// <summary>
    ///     Captures the info normally found on a PropetyChagnedEventArgs AND the current value of a property, if it is
    ///     obtainable, which may not be the case if we are observing a property chain with a null node.
    /// </summary>
    /// <typeparam name="TProperty">The observered properties type.</typeparam>
    public class PropertyValueChanged<TProperty> : PropertyChanged, IEquatable<PropertyValueChanged<TProperty>>
    {
        private readonly bool _hasValue;
        private readonly TProperty _value;

        public PropertyValueChanged(object source, string propertyName, TProperty value)
            : this(source, new PropertyChangedEventArgsEx(propertyName), value)
        {
        }

        public PropertyValueChanged(object source, [NotNull] PropertyChangedEventArgs propertyChangedEventArgs, TProperty value)
            : this(source, propertyChangedEventArgs)
        {
            _value = value;
            _hasValue = true;
        }

        public PropertyValueChanged(object source, string propertyName)
            : this(source, new PropertyChangedEventArgsEx(propertyName))
        {
        }

        public PropertyValueChanged(object source, [NotNull] PropertyChangedEventArgs propertyChangedEventArgs)
            : base(source, propertyChangedEventArgs)
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
}