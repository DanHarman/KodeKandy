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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using KodeKandy.Panopticon.Linq.ObservableImpl;

namespace KodeKandy.Panopticon.Linq
{
    public static class Opticon
    {
        #region When

        public static IObservable<IPropertyValueChanged<TProperty>> When<TClass, TProperty>(this TClass source, string propertyName,
            Func<TClass, TProperty> outValueGetter)
            where TClass : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            return new NotifyPropertyValueChangedObservable<TClass, TProperty>(source.ToPropertyValueChangedObservable(), propertyName, outValueGetter);
        }

        public static IObservable<IPropertyValueChanged<TProperty>> When<TClass, TProperty>(
            this IObservable<IPropertyValueChanged<TClass>> sourceObservable, string propertyName,
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

        public static IObservable<IPropertyValueChanged<TMember>> When<TClass, TMember>(this TClass source,
            Expression<Func<TClass, TMember>> memberPath)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath == null)
                throw new ArgumentNullException("memberPath");

            return MemberObservableFactory.CreateValueObserver(source, memberPath);
        }

        #endregion

        #region WhenValue

        public static IObservable<TProperty> WhenValue<TClass, TProperty>(this TClass source, string propertyName,
            Func<TClass, TProperty> outValueGetter)
            where TClass : class, INotifyPropertyChanged
        {
            return source.When(propertyName, outValueGetter).ToValues();
        }

        public static IObservable<TProperty> WhenValue<TClass, TProperty>(
            this IObservable<IPropertyValueChanged<TClass>> sourceObservable, string propertyName,
            Func<TClass, TProperty> outValueGetter)
            where TClass : class, INotifyPropertyChanged
        {
            return sourceObservable.When(propertyName, outValueGetter).ToValues();
        }

        public static IObservable<TMember> WhenValue<TClass, TMember>(this TClass source,
            Expression<Func<TClass, TMember>> memberPath)
            where TClass : class
        {
            return source.When(memberPath).ToValues();
        }

        #endregion

        #region WhenAny

        public static IObservable<PropertyChanged> WhenAny<TClass>(this TClass source)
            where TClass : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return new NotifyPropertyChangedObservable<TClass>(source.ToPropertyValueChangedObservable());
        }

        public static IObservable<PropertyChanged> WhenAny<TClass>(this IObservable<PropertyValueChanged<TClass>> sourceObservable)
            where TClass : class, INotifyPropertyChanged
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");

            return new NotifyPropertyChangedObservable<TClass>(sourceObservable);
        }

        public static IObservable<PropertyChanged> WhenAny<TClass, TMember>(this TClass source, Expression<Func<TClass, TMember>> memberPath)
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
        ///     Creates an Observable that always returns a specific value and never completes.
        /// </summary>
        /// <typeparam name="T">The type of the observable.</typeparam>
        /// <param name="value">The value to be returned.</param>
        /// <returns>The forever observable.</returns>
        public static IObservable<T> Forever<T>(this T value)
        {
            return new Forever<T>(value);
        }

        #endregion

        #region ForProperty

        public static IObservable<TClass> ForProperty<TClass>(this IObservable<PropertyChanged> source, params string[] propertyNames)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");

            // We need to fire on String.Empty for propertyName too as by convention that indicates a reset.
            var allNames = new string[propertyNames.Length + 1];
            Array.Copy(propertyNames, allNames, propertyNames.Length);
            allNames[propertyNames.Length] = String.Empty;

            return Observable.Create<TClass>(obs =>
                source.Subscribe(pc => { if (allNames.Contains(pc.PropertyChangedEventArgs.PropertyName)) obs.OnNext((TClass) pc.Source); },
                    obs.OnError,
                    obs.OnCompleted));
        }

        #endregion

        #region ToPropertyValueChangedObservable

        public static IObservable<IPropertyValueChanged<TClass>> ToPropertyValueChangedObservable<TClass>(this TClass value)
        {
            return PropertyValueChanged.CreateWithValue(null, default(string), value).Forever();
        }

        #endregion

        #region ToValues

        public static IObservable<T> ToValues<T>(this IObservable<IPropertyValueChanged<T>> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // This is a tiny bit faster than composing where and select.
            return Observable.Create<T>(obs => source.Subscribe(x => { if (x.HasValue) obs.OnNext(x.Value); }, obs.OnError, obs.OnCompleted));
            //return source.Where(pvc => pvc.HasValue).Select(pvc => pvc.Value);
        }

        #endregion

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

//        public static DerivedObservableList<TTargetCollectionItem> Map<TSourceCollection, TSourceCollectionItem, TTargetCollectionItem>(
//            this TSourceCollection source,
//            Func<TSourceCollectionItem, TTargetCollectionItem> mapFunc)
//            where TSourceCollection : INotifyCollectionChanged, ICollection<TSourceCollectionItem>
//        {
//            var derivedCollection = new DerivedObservableList<TTargetCollectionItem>();
//
//            return derivedCollection;
//        }
    }
}