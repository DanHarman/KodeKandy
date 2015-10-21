// <copyright file="ObservableList.cs" company="million miles per hour ltd">
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KodeKandy.Properties;

namespace KodeKandy.Panopticon
{
    public class ObservableList<T> : Collection<T>, IObservableObject, INotifyCollectionChanged
    {
        private readonly CollectionChangedHelper<T> _collectionChangeHelper;

        public ObservableList()
        {
            _collectionChangeHelper = new CollectionChangedHelper<T>(this);
        }

        public ObservableList(IEnumerable<T> collection)
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

        #region IObservableObject Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _collectionChangeHelper.PropertyChanged += value; }
            remove { _collectionChangeHelper.PropertyChanged -= value; }
        }

        /// <summary>
        ///     Suppress all PropertyChanged events for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        public IDisposable SuppressPropertyChanged()
        {
            return _collectionChangeHelper.SuppressPropertyChanged();
        }

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

        /// <summary>
        ///     Suppress all CollectionChanged events for the lifetime of the returned disposable.
        ///     Once the scope is exited a collection Reset event is fired.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        public IDisposable SuppressCollectionChanged()
        {
            return _collectionChangeHelper.SuppressCollectionChanged();
        }
    }
}