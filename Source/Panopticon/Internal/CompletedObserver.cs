// <copyright file="CompletedObserver.cs" company="million miles per hour ltd">
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
}