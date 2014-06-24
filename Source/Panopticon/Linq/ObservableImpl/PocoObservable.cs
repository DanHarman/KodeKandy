// <copyright file="PocoObservable.cs" company="million miles per hour ltd">
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
using System.Reactive.Linq;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     Forms a one-shot link for classes that do not implement <see cref="INotifyPropertyChanged" />, this fires the
    ///     current value of the member on the source as soon as it is is fired in.
    /// </summary>
    /// <typeparam name="TClass">Source class type.</typeparam>
    /// <typeparam name="TMember">Member type on source class.</typeparam>
    internal class PocoObservable<TClass, TMember> : IObservable<TMember>
        where TClass : class
    {
        private readonly Func<TClass, TMember> _memberValueGetter;
        private readonly IObservable<TClass> _sourceObservable;

        public PocoObservable(IObservable<TClass> source, Func<TClass, TMember> memberValueGetter)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberValueGetter == null)
                throw new ArgumentNullException("memberValueGetter");

            _sourceObservable = source;
            _memberValueGetter = memberValueGetter;
        }

        #region IObservable<TMember> Members

        public IDisposable Subscribe(IObserver<TMember> observer)
        {
            return _sourceObservable.Select(_memberValueGetter).Subscribe(observer);
        }

        #endregion
    }
}