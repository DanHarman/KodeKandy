// <copyright file="CollectionChangedHelper.cs" company="million miles per hour ltd">
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
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Threading;

namespace KodeKandy.Panopticon
{
    public class CollectionChangedHelper<T> : PropertyChangeHelper
    {
        private const string CountName = "Count";
        private const string IndexerName = "Item[]";
        private int suppressCollectionChangedCount;

        public CollectionChangedHelper(object source)
            : base(source)
        {
        }

        protected bool IsCollectionChangedSuppressed
        {
            get { return Interlocked.CompareExchange(ref suppressCollectionChangedCount, 0, 0) != 0; }
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (IsCollectionChangedSuppressed)
                return;

            var handlerSnapshot = CollectionChanged;

            if (handlerSnapshot != null)
                handlerSnapshot(Source, notifyCollectionChangedEventArgs);
        }

        public void NotifyAdd(T addedItem, int index)
        {
            NotifyPropertyChanged(CountName);
            NotifyPropertyChanged(IndexerName);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItem, index));
        }

        public void NotifyRemove(T removedItem, int index)
        {
            NotifyPropertyChanged(CountName);
            NotifyPropertyChanged(IndexerName);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem,
                index));
        }

        public void NotifyReplace(T newItem, T oldItem, int index)
        {
            NotifyPropertyChanged(CountName);
            NotifyPropertyChanged(IndexerName);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        }

        public void NotifyReset()
        {
            NotifyPropertyChanged(CountName);
            NotifyPropertyChanged(IndexerName);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     Suppress all CollectionChanged AND PropertyChanged events for the lifetime of the returned disposable.
        ///     Once the scope is exited a collection Reset event is fired.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        public IDisposable SuppressCollectionChanged()
        {
            var propertyChangedDisposable = SuppressPropertyChanged();
            Interlocked.Increment(ref suppressCollectionChangedCount);
            return Disposable.Create(() =>
            {
                propertyChangedDisposable.Dispose();
                if (Interlocked.Decrement(ref suppressCollectionChangedCount) == 0)
                    NotifyReset();
            });
        }
    }
}