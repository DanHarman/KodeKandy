// <copyright file="CollectionChangeSubject.cs" company="million miles per hour ltd">
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
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

namespace KodeKandy.Panopticon
{
    public class CollectionChangeSubject<T> : PropertyChangeSubject, IObservable<CollectionChange<T>>
    {
        private readonly Subject<CollectionChange<T>> collectionChangeSubject = new Subject<CollectionChange<T>>();
        private readonly Func<NotifyCollectionChangedEventHandler> getNotifyCollectionChangedEventHandler;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public CollectionChangeSubject(object source)
            : base(source)
        {
            this.getNotifyCollectionChangedEventHandler = getNotifyCollectionChangedEventHandler ?? (() => null);
        }

        public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (IsNotificationSuppressed)
                return;

            var handlerSnapshot = CollectionChanged;

            if (handlerSnapshot != null)
                handlerSnapshot(Source, notifyCollectionChangedEventArgs);
        }

        public void NotifyCollectionChange(CollectionChange<T> collectionChange)
        {
            if (IsNotificationSuppressed)
                return;

            collectionChangeSubject.OnNext(collectionChange);
        }

        #region Implementation of IObservable<out CollectionChange<T>>

        public IDisposable Subscribe(IObserver<CollectionChange<T>> observer)
        {
            return collectionChangeSubject.Subscribe(observer);
        }

        #endregion

        #region Implementation of IDisposable

        public override void Dispose()
        {
            base.Dispose();
            collectionChangeSubject.OnCompleted();
            collectionChangeSubject.Dispose();
        }

        #endregion
    }
}