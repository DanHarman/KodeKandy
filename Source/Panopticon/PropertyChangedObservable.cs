// <copyright file="PropertyChangedObservable.cs" company="million miles per hour ltd">
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
using KodeKandy.Panopticon.Internal;

namespace KodeKandy.Panopticon
{
    public class PropertyChangedObservable : IObservable<PropertyChangedEventArgs>, IDisposable
    {
        private readonly Action<PropertyChangedEventHandler> addHandler;
        private readonly object gate = new object();
        private readonly Action<PropertyChangedEventHandler> removeHandler;
        private readonly Subject<PropertyChangedEventArgs> subject = new Subject<PropertyChangedEventArgs>();
       // private WeakPropertyChangedEventHandler handler;
        private bool isDisposed;
        private int refCount;

        public PropertyChangedObservable(Action<PropertyChangedEventHandler> addHandler, Action<PropertyChangedEventHandler> removeHandler)
        {
            this.addHandler = addHandler;
            this.removeHandler = removeHandler;
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (gate)
            {
                if (isDisposed)
                    return;
                isDisposed = true;
                subject.OnCompleted();
                subject.Dispose();
             //   handler = null;
            }
        }

        #endregion

        #region IObservable<PropertyChangedEventArgs> Members

        public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer)
        {
            // Subsribe before creating the event binding as otherwise on an immediate scheduler we may miss a message.
            var disposable = subject.Subscribe(observer);

            AddRef();

            return new CompositeDisposable(disposable, Disposable.Create(Release));
        }

        #endregion

        /// <summary>
        ///     Increment the subscriber reference count creating the single event handler if needed.
        /// </summary>
        private void AddRef()
        {
            lock (gate)
            {
                if (++refCount == 1)
                {
//                    handler = new WeakPropertyChangedEventHandler(OnNext);
//                    addHandler(handler.Handler);
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
//                    removeHandler(handler.Handler);
//                    handler = null;
                    removeHandler(OnNext);
                }
            }
        }

        private void OnNext(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            subject.OnNext(propertyChangedEventArgs);
        }
    }
}