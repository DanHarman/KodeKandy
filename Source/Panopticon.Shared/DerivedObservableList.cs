// <copyright file="DerivedObservableList.cs" company="million miles per hour ltd">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace KodeKandy.Panopticon
{
    public class DerivedObservableList<T> : ReadOnlyCollection<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public DerivedObservableList()
            : this(new ObservableList<T>())
        {
        }

// ReSharper disable once ParameterTypeCanBeEnumerable.Local
        public DerivedObservableList(ObservableList<T> observableList)
            : base(new List<T>(observableList))
        {
            // Bubble up the inner ObservableList's events.
            Inner.PropertyChanged += OnPropertyChanged;
            Inner.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        ///     Provides access to the internal <see cref="ObservableList{T}" />.
        /// </summary>
        internal ObservableList<T> Inner
        {
            get { return (ObservableList<T>) Items; }
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var handlerSnapshot = PropertyChanged;

            if (handlerSnapshot != null)
            {
                handlerSnapshot(this, propertyChangedEventArgs);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var handlerSnapshot = CollectionChanged;

            if (handlerSnapshot != null)
            {
                handlerSnapshot(this, notifyCollectionChangedEventArgs);
            }
        }
    }
}