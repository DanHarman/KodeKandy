using System.ComponentModel;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    public class PropertyChange2
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
    }
}