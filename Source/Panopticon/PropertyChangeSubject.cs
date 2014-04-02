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
using System.Reactive.Subjects;

namespace KodeKandy.Panopticon
{
    public class PropertyChangeSubject : IObservable<IPropertyChange>, IDisposable
    {
        private readonly Func<PropertyChangedEventHandler> getPropertyChangedEventHandler;
        private readonly Subject<IPropertyChange> propertyChangeSubject = new Subject<IPropertyChange>();

        public PropertyChangeSubject(object source, Func<PropertyChangedEventHandler> getPropertyChangedEventHandler = null)
        {
            Require.NotNull(source, "source");

            Source = source;
            this.getPropertyChangedEventHandler = getPropertyChangedEventHandler ?? (() => null);
        }

        public object Source { get; private set; }

        public void SetPropertyValue<T>(ref T property, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
                return;

            property = value;
            NotifyPropertyValueChanged(value, propertyName);
        }

        public void NotifyPropertyValueChanged<T>(T value, string propertyName)
        {
            var handlerSnapshot = getPropertyChangedEventHandler();

            if (handlerSnapshot != null)
                handlerSnapshot(Source, new PropertyChangedEventArgs(propertyName));

            propertyChangeSubject.OnNext(PropertyChange.Create(Source, value, propertyName));
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

//    public class PropertyChangeSubject : IObservable<IPropertyChange>
//    {
//        private readonly Func<PropertyChangedEventHandler> getPropertyChangedEventHandler;
//        private ImmutableList<IObserver<IPropertyChange>> observers = new ImmutableList<IObserver<IPropertyChange>>();
//        private bool isDisposed;
//        private bool isStopped;
//
//        protected Exception Error { get; private set; }
//        private readonly object gate;
//        public object Gate
//        {
//            get { return gate; }
//        }
//
//        public PropertyChangeSubject(object source, object gate, Func<PropertyChangedEventHandler> getPropertyChangedEventHandler = null)
//        {
//            Require.NotNull(source, "source");
//
//            Source = source;
//            this.gate = gate;
//            this.getPropertyChangedEventHandler = getPropertyChangedEventHandler ?? (() => null);
//        }
//
//        public object Source { get; private set; }
//
//        public void SetPropertyValue<T>(ref T property, T value, string propertyName)
//        {
//            LockedPropertyChangeOperationScope scope = null;
//
//            lock (gate)
//            {
//                CheckDisposed();
//
//                if (!isStopped)
//                {
//                    if (EqualityComparer<T>.Default.Equals(property, value))
//                        return;
//
//                    property = value;
//                    scope = GetScope();
//                }
//            }
//
//            NotifyPropertyValueChanged(value, propertyName, scope);
//        }
//
//        public void NotifyPropertyValueChanged<T>(T value, string propertyName)
//        {
//            LockedPropertyChangeOperationScope scope = null;
//
//            lock (gate)
//            {
//                CheckDisposed();
//
//                if (!isStopped)
//                {
//                    scope = GetScope();
//                }
//            }
//
//            NotifyPropertyValueChanged(value, propertyName, scope);
//        }
//
//        public void NotifyPropertyValueChanged<T>(T value, string propertyName, LockedPropertyChangeOperationScope scope)
//        {
//            RaisePropertyChangedEventArgs(scope.Handler, new PropertyChangedEventArgs(propertyName));
//            OnNextPropertyChange(scope.Observers, PropertyChange.Create(Source, value, propertyName));
//        }
//
//        protected LockedPropertyChangeOperationScope GetScope()
//        {
//            return new LockedPropertyChangeOperationScope(getPropertyChangedEventHandler(), observers.Data);
//        }
//
//        public class LockedPropertyChangeOperationScope
//        {
//            public PropertyChangedEventHandler Handler { get; private set; }
//            public IObserver<IPropertyChange>[] Observers { get; private set; }
//
//            public LockedPropertyChangeOperationScope(PropertyChangedEventHandler handler, IObserver<IPropertyChange>[] observers)
//            {
//                Handler = handler;
//                Observers = observers;
//            }
//        }
//
//        private void RaisePropertyChangedEventArgs(PropertyChangedEventHandler handlerSnapshot, PropertyChangedEventArgs e)
//        {
//            if (handlerSnapshot != null)
//                handlerSnapshot(Source, e);
//        }
//
//        private void OnNextPropertyChange(IEnumerable<IObserver<IPropertyChange>> observersSnapshot, IPropertyChange propertyChange)
//        {
//            if (observersSnapshot != null)
//            {
//                foreach (var observer in observersSnapshot)
//                    observer.OnNext(propertyChange);
//            }
//        }
//
//        /// <summary>
//        ///     Notifies all subscribed observers about the exception.
//        /// </summary>
//        /// <param name="error">The exception to send to all observers.</param>
//        /// <exception cref="ArgumentNullException"><paramref name="error" /> is null.</exception>
//        /// <remarks>
//        ///     This is not exposed on <see cref="ObservableObject" /> as it does not make semantic sense, but is provided as a
//        ///     protected method
//        ///     so that derived types that it would make sense on, can expose the functionality.
//        /// </remarks>
//        protected void OnError(Exception error)
//        {
//            Require.NotNull(error, "error");
//
//            IObserver<IPropertyChange>[] os = null;
//
//            lock (gate)
//            {
//                CheckDisposed();
//                if (!isStopped)
//                {
//                    os = observers.Data;
//                    observers = new ImmutableList<IObserver<IPropertyChange>>();
//                    isStopped = true;
//                    Error = error;
//                }
//            }
//
//            if (os != null)
//            {
//                foreach (var observer in os)
//                    observer.OnError(error);
//            }
//        }
//
//        /// <summary>
//        ///     Notifies all subscribed observers about the end of the sequence.
//        /// </summary>
//        /// <remarks>
//        ///     This is not exposed on <see cref="ObservableObject" /> as it does not make semantic sense, but is provided as a
//        ///     protected method so that derived types that it would make sense on, can expose the functionality.
//        /// </remarks>
//        protected void OnCompleted()
//        {
//            IObserver<IPropertyChange>[] os = null;
//
//            lock (gate)
//            {
//                CheckDisposed();
//                if (!isStopped)
//                {
//                    os = observers.Data;
//                    observers = new ImmutableList<IObserver<IPropertyChange>>();
//                    isStopped = true;
//                }
//            }
//
//            if (os != null)
//            {
//                foreach (var observer in os)
//                    observer.OnCompleted();
//            }
//        }
//
//        public IDisposable Subscribe(IObserver<IPropertyChange> observer)
//        {
//            var error = default(Exception);
//
//            lock (gate)
//            {
//                CheckDisposed();
//
//                if (!isStopped)
//                {
//                    observers = observers.Add(observer);
//                    return new PropertyChangeSubjectSubscription(this, observer);
//                }
//
//                error = Error;
//            }
//
//            if (error != null)
//                observer.OnError(Error);
//            else
//                observer.OnCompleted();
//
//            return Disposable.Empty;
//        }
//
//        private class PropertyChangeSubjectSubscription : IDisposable
//        {
//            private readonly PropertyChangeSubject observableObject;
//            private IObserver<IPropertyChange> observer;
//
//            public PropertyChangeSubjectSubscription(PropertyChangeSubject observableObject, IObserver<IPropertyChange> observer)
//            {
//                this.observableObject = observableObject;
//                this.observer = observer;
//            }
//
//            public void Dispose()
//            {
//                if (observer == null) return;
//                lock (observableObject.gate)
//                {
//                    if (!observableObject.isDisposed && observer != null)
//                    {
//                        observableObject.observers = observableObject.observers.Remove(observer);
//                        observer = null;
//                    }
//                }
//            }
//        }
//
//        protected void CheckDisposed()
//        {
//            if (isDisposed)
//                throw new ObjectDisposedException(String.Empty);
//        }
//
//        protected bool IsStopped
//        {
//            get { return isStopped; }
//        }
//
//        public void Dispose()
//        {
//            lock (gate)
//            {
//                OnCompleted();
//                isDisposed = true;
//                observers = null;
//            }
//        }
//    }
}