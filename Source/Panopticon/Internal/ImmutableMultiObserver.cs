// <copyright file="ImmutableMultiObserver.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Panopticon.Internal
{
    internal class NopObserver<T> : IObserver<T>
    {
        public static readonly NopObserver<T> Instance = new NopObserver<T>();

        private NopObserver()
        {
        }

        #region IObserver<T> Members

        public void OnNext(T value)
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        #endregion
    }

    internal class CompletedObserver<T> : IObserver<T>
    {
        public static readonly CompletedObserver<T> Instance = new CompletedObserver<T>();
        private readonly Exception _error;

        private CompletedObserver()
        {
        }

        public CompletedObserver(Exception error)
        {
            _error = error;
        }

        public Exception Error
        {
            get { return _error; }
        }

        #region IObserver<T> Members

        public void OnNext(T value)
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        #endregion
    }

    /// <summary>
    ///     An immutable observer that 'multicasts' to the observers it is constructed with. Can be used to allow an
    ///     IObservable implementator to treat many observers as a single one which allows the implementor, when it only has
    ///     one observer, to store that rather than manage a group of them. This can improve performance in situatinos where
    ///     there is often a single observer on the observable.
    /// </summary>
    internal class ImmutableMultiObserver<T> : IObserver<T>
    {
        private readonly ImmutableList<IObserver<T>> _observers;

        public ImmutableMultiObserver(ImmutableList<IObserver<T>> observers)
        {
            _observers = observers;
        }

        #region IObserver<T> Members

        public void OnCompleted()
        {
            foreach (var observer in _observers.Data)
                observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            foreach (var observer in _observers.Data)
                observer.OnError(error);
        }

        public void OnNext(T value)
        {
            foreach (var observer in _observers.Data)
                observer.OnNext(value);
        }

        #endregion

        internal IObserver<T> Add(IObserver<T> observer)
        {
            return new ImmutableMultiObserver<T>(_observers.Add(observer));
        }

        internal IObserver<T> Remove(IObserver<T> observer)
        {
            var i = Array.IndexOf(_observers.Data, observer);
            if (i < 0)
                return this;

            if (_observers.Data.Length == 2)
            {
                return _observers.Data[1 - i];
            }
            else
            {
                return new ImmutableMultiObserver<T>(_observers.Remove(observer));
            }
        }
    }
}