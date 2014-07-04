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

        /// <summary>
        ///     Create a PropertyValueChanged Observable for a property on a class instance.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change.
        /// 
        ///     The PropertyValueChangedType captures whether a property has an accessible value with the HasValue property. This
        ///     is generally only relevant when building up a property subscription chain.
        /// 
        ///     Usage: inst.When("Name", x => x.Name);
        /// </summary>
        /// <typeparam name="TClass">The class type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="outValueGetter">The property getter.</param>
        /// <returns>An PropertyValueChanged Observable for the property</returns>
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


        /// <summary>
        ///     Create a PropertyValueChanged Observable for a property on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change.
        /// 
        ///     The PropertyValueChangedType captures whether a property has an accessible value with the HasValue property. This
        ///     is generally only relevant when building up a property subscription chain.
        /// 
        ///     Usage: inst.When("Name", x => x.Name);
        /// 
        ///     n.b. This overload can be composed onto a chain of When() operators for a very fast to instantiate nested property
        ///     observer that captures whether the property path has a broken node in the chain by setting HasValue=false.
        /// 
        ///     inst.When("Child", x => x.Child).When("AnotherChild", x => x.AnotherChild).When(x => x.Name);
        /// </summary>
        /// <typeparam name="TClass">The class type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="sourceObservable">The stream of class instances.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="outValueGetter">The property getter.</param>
        /// <returns>An PropertyValueChanged Observable for the property</returns>
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

        /// <summary>
        ///     Create an PropertyValueChanged Observable for a, potentially nested, property on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     This overload supports nested property paths and will continue to update regardless of which node in the path the
        ///     change occurs at.
        /// 
        ///     n.b. If the path becomes broken, this overload will notify with a PropertyValueChanged where HasValue=false.
        /// 
        ///     Usage: instObservable.When(x => x.Child.AnotherChild.Name);
        /// </summary>
        /// <typeparam name="TClass">Class type.</typeparam>
        /// <typeparam name="TMember">Member type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="memberPath">Path to the target property.</param>
        /// <returns>An PropertyValueChanged Observable for the property</returns>
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

        /// <summary>
        ///     Create an Observable for a property on a class instance.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change.
        /// 
        ///     Usage: inst.WhenValue("Name", x => x.Name);
        /// </summary>
        /// <typeparam name="TClass">The class type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="outValueGetter">The property getter.</param>
        /// <returns>An observable for the property</returns>
        public static IObservable<TProperty> WhenValue<TClass, TProperty>(this TClass source, string propertyName,
            Func<TClass, TProperty> outValueGetter)
            where TClass : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            return source.When(propertyName, outValueGetter).ToValues();
        }

        /// <summary>
        ///     Create an Observable for a property on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     Usage: instObservable.WhenValue("Name", x => x.Name);
        /// 
        ///     n.b. This overload can be composed onto a chain of When() operators for a very fast to instantiate nested property
        ///     observer:
        /// 
        ///     inst.When("Child", x => x.Child).When("AnotherChild", x => x.AnotherChild).WhenValue(x => x.Name);
        /// </summary>
        /// <typeparam name="TClass">The class type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="sourceObservable">The stream of class instances.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="outValueGetter">The property getter.</param>
        /// <returns>An observable for the property</returns>
        public static IObservable<TProperty> WhenValue<TClass, TProperty>(
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

            return sourceObservable.When(propertyName, outValueGetter).ToValues();
        }

        /// <summary>
        ///     Create an Observable for a, potentially nested, property on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     This overload supports nested property paths and will continue to update regardless of which node in the path the
        ///     change occurs at.
        /// 
        ///     n.b. If the path becomes broken, this overload will not emit a value (Use the
        ///     <see cref="When{TClass,TProperty}(TClass,string,System.Func{TClass,TProperty})" /> overload if you wish to be
        ///     notified when the path is broken.)
        /// 
        ///     Usage: instObservable.WhenValue(x => x.Child.AnotherChild.Name);
        /// </summary>
        /// <typeparam name="TClass">Class type.</typeparam>
        /// <typeparam name="TMember">Member type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="memberPath">Path to the target property.</param>
        /// <returns>An observable for the property</returns>
        public static IObservable<TMember> WhenValue<TClass, TMember>(this TClass source,
            Expression<Func<TClass, TMember>> memberPath)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath == null)
                throw new ArgumentNullException("memberPath");

            return source.When(memberPath).ToValues();
        }


        /// <summary>
        ///     Create an Observable for multiple, potentially nested, properties on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     This overload supports nested property paths and will continue to update regardless of which node in the path the
        ///     change occurs at.
        /// 
        ///     n.b. If the path becomes broken, this overload will not emit a value (Use the
        ///     <see cref="When{TClass,TProperty}(TClass,string,System.Func{TClass,TProperty})" /> overload if you wish to be
        ///     notified when the path is broken.)
        /// 
        ///     Usage: instObservable.WhenValue(x => x.Child.AnotherChild.Name, x => x.Child.Age, ... , Tuple.Create);
        /// </summary>
        /// <typeparam name="TClass">Class type.</typeparam>
        /// <typeparam name="TMember1">Member type.</typeparam>
        /// <typeparam name="TMember2">Member type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="memberPath1">Path to the target property.</param>
        /// <param name="memberPath2">Path to the target property.</param>
        /// <param name="resultSelector">Selector to combine the results.</param>
        /// <returns>An observable for the combined properties.</returns>
        public static IObservable<TResult> WhenValue<TClass, TMember1, TMember2, TResult>(this TClass source,
            Expression<Func<TClass, TMember1>> memberPath1,
            Expression<Func<TClass, TMember2>> memberPath2,
            Func<TMember1, TMember2, TResult> resultSelector)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath1 == null)
                throw new ArgumentNullException("memberPath1");
            if (memberPath2 == null)
                throw new ArgumentNullException("memberPath2");
            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");

            return Observable.CombineLatest(
                source.When(memberPath1).ToValues(),
                source.When(memberPath2).ToValues(),
                resultSelector);
        }


        /// <summary>
        ///     Create an Observable for multiple, potentially nested, properties on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     This overload supports nested property paths and will continue to update regardless of which node in the path the
        ///     change occurs at.
        /// 
        ///     n.b. If the path becomes broken, this overload will not emit a value (Use the
        ///     <see cref="When{TClass,TProperty}(TClass,string,System.Func{TClass,TProperty})" /> overload if you wish to be
        ///     notified when the path is broken.)
        /// 
        ///     Usage: instObservable.WhenValue(x => x.Child.AnotherChild.Name, x => x.Child.Age, ... , Tuple.Create);
        /// </summary>
        /// <typeparam name="TClass">Class type.</typeparam>
        /// <typeparam name="TMember1">Member type.</typeparam>
        /// <typeparam name="TMember2">Member type.</typeparam>
        /// <typeparam name="TMember3">Member type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="memberPath1">Path to the target property.</param>
        /// <param name="memberPath2">Path to the target property.</param>
        /// <param name="memberPath3">Path to the target property.</param>
        /// <param name="resultSelector">Selector to combine the results.</param>
        /// <returns>An observable for the combined properties.</returns>
        public static IObservable<TResult> WhenValue<TClass, TMember1, TMember2, TMember3, TResult>(this TClass source,
            Expression<Func<TClass, TMember1>> memberPath1,
            Expression<Func<TClass, TMember2>> memberPath2,
            Expression<Func<TClass, TMember3>> memberPath3,
            Func<TMember1, TMember2, TMember3, TResult> resultSelector)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath1 == null)
                throw new ArgumentNullException("memberPath1");
            if (memberPath2 == null)
                throw new ArgumentNullException("memberPath2");
            if (memberPath3 == null)
                throw new ArgumentNullException("memberPath3");
            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");

            return Observable.CombineLatest(
                source.When(memberPath1).ToValues(),
                source.When(memberPath2).ToValues(),
                source.When(memberPath3).ToValues(),
                resultSelector);
        }

        /// <summary>
        ///     Create an Observable for multiple, potentially nested, properties on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     This overload supports nested property paths and will continue to update regardless of which node in the path the
        ///     change occurs at.
        /// 
        ///     n.b. If the path becomes broken, this overload will not emit a value (Use the
        ///     <see cref="When{TClass,TProperty}(TClass,string,System.Func{TClass,TProperty})" /> overload if you wish to be
        ///     notified when the path is broken.)
        /// 
        ///     Usage: instObservable.WhenValue(x => x.Child.AnotherChild.Name, x => x.Child.Age, ... , Tuple.Create);
        /// </summary>
        /// <typeparam name="TClass">Class type.</typeparam>
        /// <typeparam name="TMember1">Member type.</typeparam>
        /// <typeparam name="TMember2">Member type.</typeparam>
        /// <typeparam name="TMember3">Member type.</typeparam>
        /// <typeparam name="TMember4">Member type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="memberPath1">Path to the target property.</param>
        /// <param name="memberPath2">Path to the target property.</param>
        /// <param name="memberPath3">Path to the target property.</param>
        /// <param name="memberPath4">Path to the target property.</param>
        /// <param name="resultSelector">Selector to combine the results.</param>
        /// <returns>An observable for the combined properties.</returns>
        public static IObservable<TResult> WhenValue<TClass, TMember1, TMember2, TMember3, TMember4, TResult>(this TClass source,
            Expression<Func<TClass, TMember1>> memberPath1,
            Expression<Func<TClass, TMember2>> memberPath2,
            Expression<Func<TClass, TMember3>> memberPath3,
            Expression<Func<TClass, TMember4>> memberPath4,
            Func<TMember1, TMember2, TMember3, TMember4, TResult> resultSelector)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath1 == null)
                throw new ArgumentNullException("memberPath1");
            if (memberPath2 == null)
                throw new ArgumentNullException("memberPath2");
            if (memberPath3 == null)
                throw new ArgumentNullException("memberPath3");
            if (memberPath4 == null)
                throw new ArgumentNullException("memberPath4");
            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");

            return Observable.CombineLatest(
                source.When(memberPath1).ToValues(),
                source.When(memberPath2).ToValues(),
                source.When(memberPath3).ToValues(),
                source.When(memberPath4).ToValues(),
                resultSelector);
        }

        /// <summary>
        ///     Create an Observable for multiple, potentially nested, properties on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     This overload supports nested property paths and will continue to update regardless of which node in the path the
        ///     change occurs at.
        /// 
        ///     n.b. If the path becomes broken, this overload will not emit a value (Use the
        ///     <see cref="When{TClass,TProperty}(TClass,string,System.Func{TClass,TProperty})" /> overload if you wish to be
        ///     notified when the path is broken.)
        /// 
        ///     Usage: instObservable.WhenValue(x => x.Child.AnotherChild.Name, x => x.Child.Age, ... , Tuple.Create);
        /// </summary>
        /// <typeparam name="TClass">Class type.</typeparam>
        /// <typeparam name="TMember1">Member type.</typeparam>
        /// <typeparam name="TMember2">Member type.</typeparam>
        /// <typeparam name="TMember3">Member type.</typeparam>
        /// <typeparam name="TMember4">Member type.</typeparam>
        /// <typeparam name="TMember5">Member type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="memberPath1">Path to the target property.</param>
        /// <param name="memberPath2">Path to the target property.</param>
        /// <param name="memberPath3">Path to the target property.</param>
        /// <param name="memberPath4">Path to the target property.</param>
        /// <param name="memberPath5">Path to the target property.</param>
        /// <param name="resultSelector">Selector to combine the results.</param>
        /// <returns>An observable for the combined properties.</returns>
        public static IObservable<TResult> WhenValue<TClass, TMember1, TMember2, TMember3, TMember4, TMember5, TResult>(this TClass source,
            Expression<Func<TClass, TMember1>> memberPath1,
            Expression<Func<TClass, TMember2>> memberPath2,
            Expression<Func<TClass, TMember3>> memberPath3,
            Expression<Func<TClass, TMember4>> memberPath4,
            Expression<Func<TClass, TMember5>> memberPath5,
            Func<TMember1, TMember2, TMember3, TMember4, TMember5, TResult> resultSelector)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath1 == null)
                throw new ArgumentNullException("memberPath1");
            if (memberPath2 == null)
                throw new ArgumentNullException("memberPath2");
            if (memberPath3 == null)
                throw new ArgumentNullException("memberPath3");
            if (memberPath4 == null)
                throw new ArgumentNullException("memberPath4");
            if (memberPath5 == null)
                throw new ArgumentNullException("memberPath5");
            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");

            return Observable.CombineLatest(
                source.When(memberPath1).ToValues(),
                source.When(memberPath2).ToValues(),
                source.When(memberPath3).ToValues(),
                source.When(memberPath4).ToValues(),
                source.When(memberPath5).ToValues(),
                resultSelector);
        }

        /// <summary>
        ///     Create an Observable for multiple, potentially nested, properties on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     This overload supports nested property paths and will continue to update regardless of which node in the path the
        ///     change occurs at.
        /// 
        ///     n.b. If the path becomes broken, this overload will not emit a value (Use the
        ///     <see cref="When{TClass,TProperty}(TClass,string,System.Func{TClass,TProperty})" /> overload if you wish to be
        ///     notified when the path is broken.)
        /// 
        ///     Usage: instObservable.WhenValue(x => x.Child.AnotherChild.Name, x => x.Child.Age, ... , Tuple.Create);
        /// </summary>
        /// <typeparam name="TClass">Class type.</typeparam>
        /// <typeparam name="TMember1">Member type.</typeparam>
        /// <typeparam name="TMember2">Member type.</typeparam>
        /// <typeparam name="TMember3">Member type.</typeparam>
        /// <typeparam name="TMember4">Member type.</typeparam>
        /// <typeparam name="TMember5">Member type.</typeparam>
        /// <typeparam name="TMember6">Member type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="memberPath1">Path to the target property.</param>
        /// <param name="memberPath2">Path to the target property.</param>
        /// <param name="memberPath3">Path to the target property.</param>
        /// <param name="memberPath4">Path to the target property.</param>
        /// <param name="memberPath5">Path to the target property.</param>
        /// <param name="memberPath6">Path to the target property.</param>
        /// <param name="resultSelector">Selector to combine the results.</param>
        /// <returns>An observable for the combined properties.</returns>
        public static IObservable<TResult> WhenValue<TClass, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TResult>(this TClass source,
            Expression<Func<TClass, TMember1>> memberPath1,
            Expression<Func<TClass, TMember2>> memberPath2,
            Expression<Func<TClass, TMember3>> memberPath3,
            Expression<Func<TClass, TMember4>> memberPath4,
            Expression<Func<TClass, TMember5>> memberPath5,
            Expression<Func<TClass, TMember6>> memberPath6,
            Func<TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TResult> resultSelector)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath1 == null)
                throw new ArgumentNullException("memberPath1");
            if (memberPath2 == null)
                throw new ArgumentNullException("memberPath2");
            if (memberPath3 == null)
                throw new ArgumentNullException("memberPath3");
            if (memberPath4 == null)
                throw new ArgumentNullException("memberPath4");
            if (memberPath5 == null)
                throw new ArgumentNullException("memberPath5");
            if (memberPath6 == null)
                throw new ArgumentNullException("memberPath6");
            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");

            return Observable.CombineLatest(
                source.When(memberPath1).ToValues(),
                source.When(memberPath2).ToValues(),
                source.When(memberPath3).ToValues(),
                source.When(memberPath4).ToValues(),
                source.When(memberPath5).ToValues(),
                source.When(memberPath6).ToValues(),
                resultSelector);
        }

        /// <summary>
        ///     Create an Observable for multiple, potentially nested, properties on a stream of class instances.
        /// 
        ///     When the observable is subscribed to, it will OnNext the initial value of the property, and if the class implements
        ///     INotifyPropertyChanged the observable will continue to OnNext the property's values as they change. If a new
        ///     class instance is emitted on the source stream, then the initial value for the new instance will be OnNexted.
        /// 
        ///     This overload supports nested property paths and will continue to update regardless of which node in the path the
        ///     change occurs at.
        /// 
        ///     n.b. If the path becomes broken, this overload will not emit a value (Use the
        ///     <see cref="When{TClass,TProperty}(TClass,string,System.Func{TClass,TProperty})" /> overload if you wish to be
        ///     notified when the path is broken.)
        /// 
        ///     Usage: instObservable.WhenValue(x => x.Child.AnotherChild.Name, x => x.Child.Age, ... , Tuple.Create);
        /// </summary>
        /// <typeparam name="TClass">Class type.</typeparam>
        /// <typeparam name="TMember1">Member type.</typeparam>
        /// <typeparam name="TMember2">Member type.</typeparam>
        /// <typeparam name="TMember3">Member type.</typeparam>
        /// <typeparam name="TMember4">Member type.</typeparam>
        /// <typeparam name="TMember5">Member type.</typeparam>
        /// <typeparam name="TMember6">Member type.</typeparam>
        /// <typeparam name="TMember7">Member type.</typeparam>
        /// <param name="source">The class instance.</param>
        /// <param name="memberPath1">Path to the target property.</param>
        /// <param name="memberPath2">Path to the target property.</param>
        /// <param name="memberPath3">Path to the target property.</param>
        /// <param name="memberPath4">Path to the target property.</param>
        /// <param name="memberPath5">Path to the target property.</param>
        /// <param name="memberPath7">Path to the target property.</param>
        /// <param name="resultSelector">Selector to combine the results.</param>
        /// <returns>An observable for the combined properties.</returns>
        public static IObservable<TResult> WhenValue<TClass, TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7, TResult>(
            this TClass source,
            Expression<Func<TClass, TMember1>> memberPath1,
            Expression<Func<TClass, TMember2>> memberPath2,
            Expression<Func<TClass, TMember3>> memberPath3,
            Expression<Func<TClass, TMember4>> memberPath4,
            Expression<Func<TClass, TMember5>> memberPath5,
            Expression<Func<TClass, TMember6>> memberPath6,
            Expression<Func<TClass, TMember7>> memberPath7,
            Func<TMember1, TMember2, TMember3, TMember4, TMember5, TMember6, TMember7, TResult> resultSelector)
            where TClass : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (memberPath1 == null)
                throw new ArgumentNullException("memberPath1");
            if (memberPath2 == null)
                throw new ArgumentNullException("memberPath2");
            if (memberPath3 == null)
                throw new ArgumentNullException("memberPath3");
            if (memberPath4 == null)
                throw new ArgumentNullException("memberPath4");
            if (memberPath5 == null)
                throw new ArgumentNullException("memberPath5");
            if (memberPath6 == null)
                throw new ArgumentNullException("memberPath6");
            if (memberPath7 == null)
                throw new ArgumentNullException("memberPath7");
            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");

            return Observable.CombineLatest(
                source.When(memberPath1).ToValues(),
                source.When(memberPath2).ToValues(),
                source.When(memberPath3).ToValues(),
                source.When(memberPath4).ToValues(),
                source.When(memberPath5).ToValues(),
                source.When(memberPath6).ToValues(),
                source.When(memberPath7).ToValues(),
                resultSelector);
        }

        #endregion

        #region WhenAny

        public static IObservable<IPropertyChanged<TClass>> WhenAny<TClass>(this TClass source)
            where TClass : class, INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return new NotifyPropertyChangedObservable<TClass>(source.ToPropertyValueChangedObservable());
        }

        public static IObservable<IPropertyChanged<TClass>> WhenAny<TClass>(this IObservable<PropertyValueChanged<object, TClass>> sourceObservable)
            where TClass : class, INotifyPropertyChanged
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");

            return new NotifyPropertyChangedObservable<TClass>(sourceObservable);
        }

        public static IObservable<IPropertyChanged<TMember>> WhenAny<TClass, TMember>(this TClass source, Expression<Func<TClass, TMember>> memberPath)
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
        ///     Create an Observable that always returns a specific value and never completes.
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

        public static IObservable<TClass> ForProperty<TClass>(this IObservable<IPropertyChanged<TClass>> source, params string[] propertyNames)
            where TClass : class
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
            return PropertyValueChanged.CreateWithValue(default(object), default(string), value).Forever();
        }

        #endregion

        #region ToValues

        public static IObservable<TMember> ToValues<TMember>(this IObservable<IPropertyValueChanged<TMember>> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // This is a tiny bit faster than composing where and select.
            return Observable.Create<TMember>(obs => source.Subscribe(x => { if (x.HasValue) obs.OnNext(x.Value); }, obs.OnError, obs.OnCompleted));
            //return source.Where(pvc => pvc.HasValue).Select(pvc => pvc.Value);
        }

        #endregion

        #region UserDataOrDefault

        /// <summary>
        ///     Retrieves the UserData from a PropertyChangedEventArgs instance if it is in fact a PropertyChangedEventArgsEx,
        ///     otherwise it will return null.
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