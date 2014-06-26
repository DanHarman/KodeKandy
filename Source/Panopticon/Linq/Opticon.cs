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
using System.Reactive.Linq;
using KodeKandy.Panopticon.Linq.ObservableImpl;

namespace KodeKandy.Panopticon.Linq
{
    public static class Opticon
    {
        #region When

        public static IObservable<PropertyValueChanged<TProperty>> When<TClass, TProperty>(this TClass source, string propertyName, Func<TClass, TProperty> outValueGetter)
            where TClass : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            return new NotifyPropertyValueChangedObservable<TClass, TProperty>(source.ToPropertyValueChanged().Forever(), propertyName, outValueGetter);
        }

        public static IObservable<PropertyValueChanged<TProperty>> When<TClass, TProperty>(this IObservable<PropertyValueChanged<TClass>> sourceObservable, string propertyName,
            Func<TClass, TProperty> outValueGetter)
            where TClass : class, INotifyPropertyChanged
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            return new NotifyPropertyValueChangedObservable<TClass, TProperty>(sourceObservable, propertyName, outValueGetter);
        }

        public static IObservable<PropertyValueChanged<TMember>> When<TClass, TMember>(this TClass source, Expression<Func<TClass, TMember>> memberPath)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath == null)
                throw new ArgumentNullException("memberPath");

            return MemberObservableFactory.CreateValueObserver(source, memberPath);
        }

        #endregion

        #region WhenAny

        public static IObservable<PropertyChanged> WhenAny<TClass>(this TClass source)
            where TClass : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return new NotifyPropertyChangedObservable<TClass>(Forever(source));
        }

        public static IObservable<PropertyChanged> WhenAny<TClass>(this IObservable<TClass> sourceObservable)
            where TClass : class, INotifyPropertyChanged
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");

            return new NotifyPropertyChangedObservable<TClass>(sourceObservable);
        }

        public static IObservable<PropertyChanged> WhenAny<TClass, TMember>(this TClass source,
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

        #endregion

        #region Forever

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

        #endregion
   
        public static PropertyValueChanged<TClass> ToPropertyValueChanged<TClass>(this TClass value, object source = null, string propertyName = null)
        {
            return new PropertyValueChanged<TClass>(source, propertyName, value);
        }

        public static IObservable<T> ToValues<T>(this IObservable<PropertyValueChanged<T>> source)
        {
            return source.Where(pvc => pvc.HasValue).Select( pvc => pvc.Value);
        }

        #region UserDataOrDefault

        /// <summary>
        ///     Retrieves the UserData from a PropertyChangedEventArgs if it is in fact a PropertyChangedEventArgsEx, otherwise it
        ///     will return null.
        /// </summary>
        /// <param name="change">The change that may have a UserData property.</param>
        /// <returns>The UserData if present otherwise null.</returns>
        public static object UserDataOrDefault(this PropertyChangedEventArgs change)
        {
            var extended = change as PropertyChangedEventArgsEx;

            return extended != null ? extended.UserData : null;
        }

        #endregion

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