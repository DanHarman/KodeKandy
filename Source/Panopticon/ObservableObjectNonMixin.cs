// <copyright file="ObservableObjectNonMixin.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Panopticon
{
    //public class ObservableObjectNonMixin : IObservableObject, IObservable<PropertyChange>
    //{
    //    private ImmutableList<IObserver<PropertyChange>> observers = new ImmutableList<IObserver<PropertyChange>>();
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    private bool isDisposed;
    //    private bool isStopped;
    //    protected Exception Error { get; private set; }
    //    private readonly object gate = new object();

    //    public IObservable<PropertyChange> PropertyChanges
    //    {
    //        get { return this; }
    //    }

    //    /// <summary>
    //    ///     Sets a value on a model if it does not equal the currently value, and notifies subscribers of the changes.
    //    ///     This is similar to OnNext() on an rx Subject but has non-monadic functionality that sets a variable.
    //    ///     This set is protected by a lock.
    //    /// </summary>
    //    /// <typeparam name="T">The type of the property being set.</typeparam>
    //    /// <param name="property">The property that may be changed.</param>
    //    /// <param name="value">The value to set the property to.</param>
    //    /// <param name="propertyName">The name of the property being changed.</param>
    //    [NotifyPropertyChangedInvocator("propertyName")]
    //    public void SetPropertyValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChangedEventHandler handler = null;
    //        IObserver<PropertyChange>[] os = null;

    //        lock (gate)
    //        {
    //            CheckDisposed();

    //            if (!isStopped)
    //            {
    //                if (EqualityComparer<T>.Default.Equals(property, value))
    //                    return;

    //                property = value;
    //                handler = PropertyChanged;
    //                os = observers.Data;
    //            }
    //        }

    //        if (handler != null)
    //            handler(this, new PropertyChangedEventArgs(propertyName));

    //        if (os != null)
    //        {
    //            var propertyChange = new PropertyChange(this, propertyName, value);
    //            foreach (var observer in os)
    //                observer.OnNext(propertyChange);
    //        }
    //    }

    //    /// <summary>
    //    ///     Notifies all subscribed observers about the exception.
    //    /// </summary>
    //    /// <param name="error">The exception to send to all observers.</param>
    //    /// <exception cref="ArgumentNullException"><paramref name="error" /> is null.</exception>
    //    /// <remarks>
    //    ///     This is not exposed on <see cref="ObservableObject" /> as it does not make semantic sense, but is provided as a
    //    ///     protected method
    //    ///     so that derived types that it would make sense on, can expose the functionality.
    //    /// </remarks>
    //    protected void OnError(Exception error)
    //    {
    //        Require.NotNull(error, "error");

    //        IObserver<PropertyChange>[] os = null;

    //        lock (gate)
    //        {
    //            CheckDisposed();
    //            if (!isStopped)
    //            {
    //                os = observers.Data;
    //                observers = new ImmutableList<IObserver<PropertyChange>>();
    //                isStopped = true;
    //                Error = error;
    //            }
    //        }

    //        if (os != null)
    //        {
    //            foreach (var observer in os)
    //                observer.OnError(error);
    //        }
    //    }

    //    /// <summary>
    //    ///     Notifies all subscribed observers about the end of the sequence.
    //    /// </summary>
    //    /// <remarks>
    //    ///     This is not exposed on <see cref="ObservableObject" /> as it does not make semantic sense, but is provided as a
    //    ///     protected method so that derived types that it would make sense on, can expose the functionality.
    //    /// </remarks>
    //    protected void OnCompleted()
    //    {
    //        IObserver<PropertyChange>[] os = null;

    //        lock (gate)
    //        {
    //            CheckDisposed();
    //            if (!isStopped)
    //            {
    //                os = observers.Data;
    //                observers = new ImmutableList<IObserver<PropertyChange>>();
    //                isStopped = true;
    //            }
    //        }

    //        if (os != null)
    //        {
    //            foreach (var observer in os)
    //                observer.OnCompleted();
    //        }
    //    }

    //    IDisposable IObservable<PropertyChange>.Subscribe(IObserver<PropertyChange> observer)
    //    {
    //        var error = default(Exception);

    //        lock (gate)
    //        {
    //            CheckDisposed();

    //            if (!isStopped)
    //            {
    //                observers = observers.Add(observer);
    //                return new Subscription(this, observer);
    //            }

    //            error = Error;
    //        }

    //        if (error != null)
    //            observer.OnError(Error);
    //        else
    //            observer.OnCompleted();

    //        return Disposable.Empty;
    //    }

    //    private class Subscription : IDisposable
    //    {
    //        private readonly ObservableObjectNonMixin observableObject;
    //        private IObserver<PropertyChange> observer;

    //        public Subscription(ObservableObjectNonMixin observableObject, IObserver<PropertyChange> observer)
    //        {
    //            this.observableObject = observableObject;
    //            this.observer = observer;
    //        }

    //        public void Dispose()
    //        {
    //            if (observer == null) return;
    //            lock (observableObject.gate)
    //            {
    //                if (!observableObject.isDisposed && observer != null)
    //                {
    //                    observableObject.observers = observableObject.observers.Remove(observer);
    //                    observer = null;
    //                }
    //            }
    //        }
    //    }

    //    private void CheckDisposed()
    //    {
    //        if (isDisposed)
    //            throw new ObjectDisposedException(String.Empty);
    //    }

    //    public void Dispose()
    //    {
    //        lock (gate)
    //        {
    //            OnCompleted();
    //            isDisposed = true;
    //            observers = null;
    //        }
    //    }
    //}
}