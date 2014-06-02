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
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon
{
    public class ObservableList<T> : Collection<T>, IObservableObject, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private const string CountName = "Count";
        private const string IndexerName = "Item[]";
        private readonly CollectionChangeSubject<T> collectionChangeSubject;

        public ObservableList()
        {
            collectionChangeSubject = new CollectionChangeSubject<T>(this);
        }

        public ObservableList(IEnumerable<T> collection)
            : base(new List<T>(collection))
        {
            collectionChangeSubject = new CollectionChangeSubject<T>(this);
        }

        public IObservable<CollectionChange<T>> CollectionChanges
        {
            get { return collectionChangeSubject; }
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { collectionChangeSubject.CollectionChanged += value; }
            remove { collectionChangeSubject.CollectionChanged -= value; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IObservableObject Members

        /// <summary>
        ///     An observable providing notification of property changes. It will complete when the object is
        ///     disposed.
        /// </summary>
        public IObservable<IPropertyChange> PropertyChanges
        {
            get { return collectionChangeSubject; }
        }

        /// <summary>
        ///     Suppress all change notifications for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        public IDisposable BeginNotificationSuppression()
        {
            return collectionChangeSubject.BeginNotificationSuppression(() =>
            {
                // Once a collection falls out of notification suppression, we must fire a reset/image to all subscribers so they can catch up.
                // If you think this should only happy on more than 'n' changes, then count your changes first before calling suppress!
                collectionChangeSubject.NotifyPropertyValueChanged(0, CountName);
                collectionChangeSubject.NotifyPropertyValueChanged(default(T), IndexerName);
                collectionChangeSubject.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                collectionChangeSubject.NotifyCollectionChange(CollectionChange.CreateImage(this, new ReadOnlyCollection<T>(this)));
            });
        }

        public void Dispose()
        {
            collectionChangeSubject.Dispose();
        }

        #endregion

        #region Overrides of Collection<T>

        protected override void ClearItems()
        {
            base.ClearItems();

            collectionChangeSubject.NotifyPropertyValueChanged(0, CountName);
            collectionChangeSubject.NotifyPropertyValueChanged(default(T), IndexerName);
            collectionChangeSubject.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            collectionChangeSubject.NotifyCollectionChange(CollectionChange.CreateImage(this, new ReadOnlyCollection<T>(new T[] {})));
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            collectionChangeSubject.NotifyPropertyValueChanged(index, CountName);
            collectionChangeSubject.NotifyPropertyValueChanged(default(T), IndexerName);
            collectionChangeSubject.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            collectionChangeSubject.NotifyCollectionChange(CollectionChange.CreateAdd(this, new ReadOnlyCollection<T>(new[] {item})));
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = Items[index];
            base.RemoveItem(index);

            collectionChangeSubject.NotifyPropertyValueChanged(index, CountName);
            collectionChangeSubject.NotifyPropertyValueChanged(default(T), IndexerName);
            collectionChangeSubject.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem,
                index));
            collectionChangeSubject.NotifyCollectionChange(CollectionChange.CreateRemove(this, new ReadOnlyCollection<T>(new[] {removedItem})));
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = Items[index];
            base.SetItem(index, item);

            collectionChangeSubject.NotifyPropertyValueChanged(index, CountName);
            collectionChangeSubject.NotifyPropertyValueChanged(default(T), IndexerName);
            collectionChangeSubject.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
            collectionChangeSubject.NotifyCollectionChange(
                CollectionChange.CreateUpdate(this, new ReadOnlyCollection<T>(new[] {item}), new ReadOnlyCollection<T>(new[] {oldItem})));
        }

        #endregion

        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<TVal>(ref TVal property, TVal value, [CallerMemberName] string propertyName = null)
        {
            collectionChangeSubject.SetPropertyValue(ref property, value, propertyName);
        }

        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<TVal>(ref TVal property, TVal value, object userData, [CallerMemberName] string propertyName = null)
        {
            collectionChangeSubject.SetPropertyValue(ref property, value, propertyName, userData);
        }
    }
}