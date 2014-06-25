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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using KodeKandy.Panopticon.Linq.ObservableImpl;

namespace KodeKandy.Panopticon.Linq
{
    public static class Opticon
    {
        public static IObservable<TProperty> When<TClass, TProperty>(this TClass source, string propertyName, Func<TClass, TProperty> outValueGetter)
            where TClass : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            return new NotifyPropertyChangedValueObservable<TClass, TProperty>(Forever(source), propertyName, outValueGetter);
        }

        public static IObservable<TProperty> When<TClass, TProperty>(this IObservable<TClass> sourceObservable, string propertyName,
            Func<TClass, TProperty> outValueGetter)
            where TClass : class, INotifyPropertyChanged
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            return new NotifyPropertyChangedValueObservable<TClass, TProperty>(sourceObservable, propertyName, outValueGetter);
        }

        public static IObservable<TMember> When<TClass, TMember>(this TClass source, Expression<Func<TClass, TMember>> memberPath)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath == null)
                throw new ArgumentNullException("memberPath");

            return MemberObservableFactory.CreateValueObserver(source, memberPath);
        }

        public static IObservable<PropertyChanged> WhenPropertyChange<TClass>(this TClass source)
            where TClass : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return new NotifyPropertyChangedObservable<TClass>(Forever(source));
        }

        public static IObservable<PropertyChanged> WhenPropertyChange<TClass>(this IObservable<TClass> sourceObservable)
            where TClass : class, INotifyPropertyChanged
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");

            return new NotifyPropertyChangedObservable<TClass>(sourceObservable);
        }

        public static IObservable<PropertyChanged> WhenPropertyChange<TClass, TMember>(this TClass source,
            Expression<Func<TClass, TMember>> memberPath)
            where TClass : class
            where TMember : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath == null)
                throw new ArgumentNullException("memberPath");

            return MemberObservableFactory.CreatePropertyChangedObserver(source, memberPath);
        }

        /// <summary>
        ///     An Observable that always returns a specific value and never completes.
        /// </summary>
        /// <typeparam name="T">The type of the observable.</typeparam>
        /// <param name="value">The value to be returned.</param>
        /// <returns>The forever observable.</returns>
        public static IObservable<T> Forever<T>(this T value)
        {
            return new Forever<T>(value);
        }

        public static DerivedObservableList<TTargetCollectionItem> Map<TSourceCollection, TSourceCollectionItem, TTargetCollectionItem>(
            this TSourceCollection source,
            Func<TSourceCollectionItem, TTargetCollectionItem> mapFunc)
            where TSourceCollection : INotifyCollectionChanged, ICollection<TSourceCollectionItem>
        {
            var derivedCollection = new DerivedObservableList<TTargetCollectionItem>();

            return derivedCollection;
        }
    }
}