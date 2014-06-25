using System;
using System.ComponentModel;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    public class PropertyChange2 : IEquatable<PropertyChange2>
    {
        private readonly object _sender;
        private readonly PropertyChangedEventArgs _propertyChangedEventArgs;
        public object Sender
        {
            get { return _sender; }
        }
        public PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get { return _propertyChangedEventArgs; }
        }

        public PropertyChange2(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _sender = sender;
            _propertyChangedEventArgs = propertyChangedEventArgs;
        }

        public PropertyChange2(object sender, string propertyName) : this(sender, new PropertyChangedEventArgs(propertyName))
        {
        }

        public bool Equals(PropertyChange2 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_sender, other._sender) && Equals(_propertyChangedEventArgs.PropertyName, other._propertyChangedEventArgs.PropertyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PropertyChange2) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_sender != null ? _sender.GetHashCode() : 0) * 397) ^ (_propertyChangedEventArgs != null ? _propertyChangedEventArgs.GetHashCode() : 0);
            }
        }
    }
}