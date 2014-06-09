// <copyright file="ReturnForever.cs" company="million miles per hour ltd">
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
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     An Observable that always returns a specific value and never completes.
    /// </summary>
    /// <remarks>
    ///     This is similar to <see cref="Observable.Return{TResult}(TResult)" /> but never completes.
    /// </remarks>
    /// <typeparam name="T">The type of the observable.</typeparam>
    internal class ReturnForever<T> : IObservable<T>
    {
        private readonly T value;

        public ReturnForever(T value)
        {
            this.value = value;
        }

        #region IObservable<T> Members

        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnNext(value);
            return Disposable.Empty;
        }

        #endregion

        public static ReturnForever<T> Value(T value)
        {
            return new ReturnForever<T>(value);
        }
    }
}