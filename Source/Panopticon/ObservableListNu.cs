using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon
{
    public class ObservableListNu<T> : Collection<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly CollectionChangedHelper<T> _collectionChangeHelper;

        public ObservableListNu()
        {
            _collectionChangeHelper = new CollectionChangedHelper<T>(this);
        }

        public ObservableListNu(IEnumerable<T> collection)
            : base(new List<T>(collection))
        {
            _collectionChangeHelper = new CollectionChangedHelper<T>(this);
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collectionChangeHelper.CollectionChanged += value; }
            remove { _collectionChangeHelper.CollectionChanged -= value; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _collectionChangeHelper.PropertyChanged += value; }
            remove { _collectionChangeHelper.PropertyChanged -= value; }
        }

        #endregion

        #region IObservableObject Members

        /// <summary>
        ///     Suppress all change notifications for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
//        public IDisposable BeginNotificationSuppression()
//        {
//            // TODO need to handle collection and property change supression separately I think...
//            return _collectionChangeHelper.BeginNotificationSuppression(() =>
//            {
//                // Once a collection falls out of notification suppression, we must fire a reset/image to all subscribers so they can catch up.
//                // If you think this should only happy on more than 'n' changes, then count your changes first before calling suppress!
//                _collectionChangeHelper.NotifyReset();
//            });
//        }

        #endregion

        #region Overrides of Collection<T>

        protected override void ClearItems()
        {
            base.ClearItems();

            _collectionChangeHelper.NotifyReset();
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            _collectionChangeHelper.NotifyAdd(item, index);
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = Items[index];
            base.RemoveItem(index);

            _collectionChangeHelper.NotifyRemove(removedItem, index);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = Items[index];
            base.SetItem(index, item);

            _collectionChangeHelper.NotifyReplace(item, oldItem, index);
        }

        #endregion

        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<TVal>(ref TVal property, TVal value, [CallerMemberName] string propertyName = null)
        {
            _collectionChangeHelper.SetPropertyValue(ref property, value, propertyName);
        }

        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<TVal>(ref TVal property, TVal value, object userData, [CallerMemberName] string propertyName = null)
        {
            _collectionChangeHelper.SetPropertyValue(ref property, value, propertyName, userData);
        }

        public IDisposable SuppressPropertyChanged()
        {
            return _collectionChangeHelper.SuppressPropertyChanged();
        }

        public IDisposable SuppressCollectionChanged()
        {
            return _collectionChangeHelper.SuppressCollectionChanged();
        }
    }
}