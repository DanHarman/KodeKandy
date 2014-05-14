// <copyright file="PropertyChangeSubject.cs" company="million miles per hour ltd">
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
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

namespace KodeKandy.Panopticon
{
    public class PropertyChangeSubject : IObservable<IPropertyChange>, IDisposable
    {
        private readonly Func<PropertyChangedEventHandler> getPropertyChangedEventHandler;
        private readonly Subject<IPropertyChange> propertyChangeSubject = new Subject<IPropertyChange>();
        private int suppressNotificationCount;

        public PropertyChangeSubject(object source, Func<PropertyChangedEventHandler> getPropertyChangedEventHandler = null)
        {
            Require.NotNull(source, "source");

            Source = source;
            this.getPropertyChangedEventHandler = getPropertyChangedEventHandler ?? (() => null);
        }

        public object Source { get; private set; }

        public void SetPropertyValue<T>(ref T property, T value, string propertyName, object userData = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
                return;

            property = value;
            NotifyPropertyValueChanged(value, propertyName, userData);
        }

        public void NotifyPropertyValueChanged<T>(T value, string propertyName, object userData = null)
        {
            if (IsNotificationSuppressed)
                return;

            var handlerSnapshot = getPropertyChangedEventHandler();

            if (handlerSnapshot != null)
                handlerSnapshot(Source, new PropertyChangedEventArgs(propertyName));

            propertyChangeSubject.OnNext(PropertyChange.Create(Source, value, propertyName, userData));
        }

        /// <summary>
        ///     Suppress all change notifications for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <param name="completionAction">An action to optionally perform when all suppression scopes are exited (as it supports reentrance a count is maintained).</param>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>        
        public IDisposable BeginNotificationSuppression(Action completionAction = null)
        {
            Interlocked.Increment(ref suppressNotificationCount);
            return Disposable.Create(() =>
            {
                var count = Interlocked.Decrement(ref suppressNotificationCount);
                if (completionAction != null && count == 0)
                {
                    completionAction();
                }
            });
        }

        protected bool IsNotificationSuppressed
        {            
            get {  return Interlocked.CompareExchange(ref suppressNotificationCount, 0, 0) != 0; }
        }

        #region Implementation of IObservable<out IPropertyChange>

        public IDisposable Subscribe(IObserver<IPropertyChange> observer)
        {
            return propertyChangeSubject.Subscribe(observer);
        }

        #endregion

        #region Implementation of IDisposable

        public virtual void Dispose()
        {
            propertyChangeSubject.OnCompleted();
            propertyChangeSubject.Dispose();
        }

        #endregion
    }
}