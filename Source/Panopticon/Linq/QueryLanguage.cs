// <copyright file="QueryLanguage.cs" company="million miles per hour ltd">
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using KodeKandy.Panopticon.Linq.ObservableImpl;

namespace KodeKandy.Panopticon.Linq
{
    public static class QueryLanguage
    {
        public static IObservable<TOut> When<TIn, TOut>(this IObservable<TIn> source, string propertyName, Func<TIn, TOut> outValueGetter)
            where TIn : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            return new NotifyPropertyChangedLink<TIn, TOut>(source, propertyName, outValueGetter);
        }

        public static IObservable<Unit> WhenAny<TIn>(this IObservable<TIn> source)
            where TIn : INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            // TODO need a specialised notifyProeprtychanged link it would seem if we want to expose the propertyname and type.
            throw new NotImplementedException();
            //  return new NotifyPropertyChangedLink<TIn, TOut>(source, propertyName, outValueGetter);
        }

        public static DerivedObservableList<TTargetCollectionItem> Map<TSourceCollection, TSourceCollectionItem, TTargetCollectionItem>(this TSourceCollection source,
            Func<TSourceCollectionItem, TTargetCollectionItem> mapFunc)
            where TSourceCollection : INotifyCollectionChanged, ICollection<TSourceCollectionItem>
        {
            var derivedCollection = new DerivedObservableList<TTargetCollectionItem>();

            return derivedCollection;
        }
    }

    internal class DerivedCollectionAdaptor<TSourceItem, TDerivedItem>
    {
        private readonly INotifyCollectionChanged _sourceCollection;
        private readonly Func<TSourceItem, TDerivedItem> _mapFunc;
        private readonly DerivedObservableList<TDerivedItem> _derivedCollection;

        public DerivedCollectionAdaptor(INotifyCollectionChanged sourceCollection, Func<TSourceItem, TDerivedItem> mapFunc)
        {
            Require.NotNull(sourceCollection, "sourceCollection");
            Require.NotNull(mapFunc, "mapFunc");

            _sourceCollection = sourceCollection;
            _mapFunc = mapFunc;
            sourceCollection.CollectionChanged += SourceCollectionOnCollectionChanged;
            // Can we key it to the size of the source collection?
            _derivedCollection = new DerivedObservableList<TDerivedItem>();
            // toto addrange of all current items in source.
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