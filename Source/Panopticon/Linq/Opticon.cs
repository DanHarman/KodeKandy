// <copyright file="Opticon.cs" company="million miles per hour ltd">
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
using System.Linq.Expressions;
using KodeKandy.Panopticon.Linq.ObservableImpl;

namespace KodeKandy.Panopticon.Linq
{
    public static class Opticon
    {
        public static IObservable<TClass> Observe<TClass>(TClass source)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return new Forever<TClass>(source);
        }

        public static IObservable<TProperty> Observe<TClass, TProperty>(TClass source, Expression<Func<TClass, TProperty>> memberPath)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return MemberObservableFactory.Create(source, memberPath);
        }

        /// <summary>
        ///     An Observable that always returns a specific value and never completes.
        /// </summary>
        /// <typeparam name="T">The type of the observable.</typeparam>
        /// <param name="value">The value to be returned.</param>
        /// <returns>The forever observable.</returns>
        public static IObservable<T> Forever<T>(T value)
        {
            return new Forever<T>(value);
        }
    }
}