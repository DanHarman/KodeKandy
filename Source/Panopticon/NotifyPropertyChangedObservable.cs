// <copyright file="NotifyPropertyChangedObservable.cs" company="million miles per hour ltd">
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
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace KodeKandy.Panopticon
{
    /// <summary>
    ///     Creates an Observable of PropertyChangedEventArgs from an object implementing
    ///     <see cref="INotifyPropertyChanged" />. The subscriptions is implicilty ref counted so multiple calls to subscribe
    ///     only create one underlying subscription to the PropertyChanges event on the object.
    /// 
    ///     This means it offers similar functionaly to Observable.FromEventPattern().Publish().RefCount() but outperforms it,
    ///     and allows for explicit disposal of attached Rx subscriptions.
    /// 
    ///     The binding to the event is late bound and delayed until a Subscribe call is made.
    /// </summary>
    public class NotifyPropertyChangedObservable : IObservable<PropertyChangedEventArgs>, IDisposable
    {
        private readonly Action<PropertyChangedEventHandler> addHandler;
        private readonly object gate = new object();
        private readonly Action<PropertyChangedEventHandler> removeHandler;
        private readonly Subject<PropertyChangedEventArgs> subject = new Subject<PropertyChangedEventArgs>();
        private bool isDisposed;
        private int refCount;

        /// <summary>
        ///     Create an Observable for of <see cref="INotifyPropertyChanged" />.
        /// </summary>
        /// <param name="addHandler">Action invoked to subscribe to the PropertyChanged event.</param>
        /// <param name="removeHandler">Action invoked to unsubscribe from the PropertyChanged event.</param>
        public NotifyPropertyChangedObservable(Action<PropertyChangedEventHandler> addHandler, Action<PropertyChangedEventHandler> removeHandler)
        {
            this.addHandler = addHandler;
            this.removeHandler = removeHandler;
        }

        #region IDisposable Members

        /// <summary>
        ///     Disposes of the observable sending OnCompleted() to all subscribers.
        /// </summary>
        public void Dispose()
        {
            lock (gate)
            {
                if (isDisposed)
                    return;
                isDisposed = true;
                subject.OnCompleted();
                subject.Dispose();
            }
        }

        #endregion

        #region IObservable<PropertyChangedEventArgs> Members

        /// <summary>
        ///     Subscribe to the <see cref="PropertyChangedEventArgs" /> stream.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns>Disposable used to terminate the subscription.</returns>
        public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer)
        {
            // Subsribe before creating the event binding as otherwise on an immediate scheduler we may miss a message.
            var disposable = subject.Subscribe(observer);

            AddRef();

            return new CompositeDisposable(disposable, Disposable.Create(Release));
        }

        #endregion

        /// <summary>
        ///     Create an Observable for of <see cref="INotifyPropertyChanged" />.
        /// </summary>
        /// <param name="addHandler">Action invoked to subscribe to the PropertyChanged event.</param>
        /// <param name="removeHandler">Action invoked to unsubscribe from the PropertyChanged event.</param>
        public static NotifyPropertyChangedObservable Create(Action<PropertyChangedEventHandler> addHandler,
            Action<PropertyChangedEventHandler> removeHandler)
        {
            return new NotifyPropertyChangedObservable(addHandler, removeHandler);
        }

        /// <summary>
        ///     Increment the subscriber reference count creating the single event handler if needed.
        /// </summary>
        private void AddRef()
        {
            lock (gate)
            {
                if (++refCount == 1)
                {
                    addHandler(OnNext);
                }
            }
        }

        /// <summary>
        ///     Decrement the subscriber reference count deleting the single event handler if no longer needed.
        /// </summary>
        private void Release()
        {
            lock (gate)
            {
                if (--refCount == 0)
                {
                    removeHandler(OnNext);
                }
            }
        }

        /// <summary>
        ///     Internal <see cref="PropertyChangedEventHandler" /> to bind to the targets PropertyChanged event.
        /// </summary>
        private void OnNext(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            subject.OnNext(propertyChangedEventArgs);
        }
    }
}