// <copyright file="DerivedCollectionAdaptor.cs" company="million miles per hour ltd">
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
using System.Collections.Specialized;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    internal class DerivedCollectionAdaptor<TSourceItem, TDerivedItem>
    {
        private readonly DerivedObservableList<TDerivedItem> _derivedCollection;
        private readonly Func<TSourceItem, TDerivedItem> _mapFunc;
        private readonly INotifyCollectionChanged _sourceCollection;

        public DerivedCollectionAdaptor(INotifyCollectionChanged sourceCollection, Func<TSourceItem, TDerivedItem> mapFunc)
        {
            Require.NotNull(sourceCollection, "sourceCollection");
            Require.NotNull(mapFunc, "mapFunc");

            _sourceCollection = sourceCollection;
            _mapFunc = mapFunc;
            sourceCollection.CollectionChanged += SourceCollectionOnCollectionChanged;
            // Can we key it to the size of the sourceObservable collection?
            _derivedCollection = new DerivedObservableList<TDerivedItem>();
            // toto addrange of all current items in sourceObservable.
        }

        private void SourceCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (sender != _sourceCollection)
                return;

            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItem = _mapFunc((TSourceItem) notifyCollectionChangedEventArgs.NewItems);
                    _derivedCollection.Inner.Insert(notifyCollectionChangedEventArgs.NewStartingIndex, newItem);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _derivedCollection.Inner.RemoveAt(notifyCollectionChangedEventArgs.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    var replacementItem = _mapFunc((TSourceItem) notifyCollectionChangedEventArgs.NewItems);
                    _derivedCollection.Inner[notifyCollectionChangedEventArgs.OldStartingIndex] = replacementItem;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnReset();
                    break;
            }
        }

        private void OnReset()
        {
            var newItem = default(TDerivedItem);
            using (_derivedCollection.Inner.SuppressCollectionChanged())
            {
                foreach (var item in (IEnumerable<TSourceItem>) _sourceCollection)
                {
                    newItem = _mapFunc(item);
                    _derivedCollection.Inner.Add(newItem);
                }
            }
        }
    }
}