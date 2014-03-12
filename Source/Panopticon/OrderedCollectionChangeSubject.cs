// <copyright file="OrderedCollectionChangeSubject.cs" company="million miles per hour ltd">
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
// 
// </copyright>

namespace KodeKandy.Panopticon
{
    //public class OrderedCollectionChangeSubject<T> : PropertyChangeSubject, IObservable<CollectionChange<T>>
    //{
    //    private readonly Func<NotifyCollectionChangedEventHandler> getNotifyCollectionChangedEventHandler;
    //    private ImmutableList<IObserver<CollectionChange<T>>> observers = new ImmutableList<IObserver<CollectionChange<T>>>();

    //    public OrderedCollectionChangeSubject(object source, object gate,
    //        Func<NotifyCollectionChangedEventHandler> getNotifyCollectionChangedEventHandler = null,
    //        Func<PropertyChangedEventHandler> getPropertyChangedEventHandler = null)
    //        : base(source, gate, getPropertyChangedEventHandler)
    //    {
    //        this.getNotifyCollectionChangedEventHandler = getNotifyCollectionChangedEventHandler ?? (() => null);
    //    }

    //    #region Implementation of IObservable<out CollectionChange<T>>

    //    public IDisposable Subscribe(IObserver<CollectionChange<T>> observer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected new LockedCollectionChangeOperationScope GetScope()
    //    {
    //        return new LockedCollectionChangeOperationScope(base.GetScope(), getNotifyCollectionChangedEventHandler(), observers.Data);
    //    }

    //    public class LockedCollectionChangeOperationScope
    //    {
    //        public LockedPropertyChangeOperationScope PropertyChangeScope { get; private set; }
    //        public NotifyCollectionChangedEventHandler Handler { get; private set; }
    //        public IEnumerable<IObserver<CollectionChange<T>>> Observers { get; private set; }

    //        public LockedCollectionChangeOperationScope(LockedPropertyChangeOperationScope propertyChangeScope,
    //            NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler, IEnumerable<IObserver<CollectionChange<T>>> observers)
    //        {
    //            PropertyChangeScope = propertyChangeScope;
    //            Handler = notifyCollectionChangedEventHandler;
    //            Observers = observers;
    //        }
    //    }

    //    public LockedCollectionChangeOperationScope PerformLockedOperationAndGetNotificationContext(Action lockedOperation)
    //    {
    //        LockedCollectionChangeOperationScope scope = null;

    //        lock (Gate)
    //        {
    //            CheckDisposed();

    //            if (!IsStopped)
    //            {
    //                lockedOperation();

    //                scope = GetScope();
    //            }
    //        }

    //        return scope;
    //    }

    //    public void NotifyCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs, LockedCollectionChangeOperationScope scope)
    //    {
    //        if (scope.Handler != null)
    //            scope.Handler(Source, notifyCollectionChangedEventArgs);
    //    }

    //    public void NotifyCollectionChanges(CollectionChange<T> collectionChange, LockedCollectionChangeOperationScope scope)
    //    {
    //        if (scope.Observers != null)
    //        {
    //            foreach (var observer in scope.Observers)
    //                observer.OnNext(collectionChange);
    //        }
    //    }

    //    #endregion
    //}
}