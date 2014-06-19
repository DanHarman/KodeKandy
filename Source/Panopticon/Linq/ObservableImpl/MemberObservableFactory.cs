// <copyright file="MemberObservableFactory.cs" company="million miles per hour ltd">
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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    /// <summary>
    ///     Factory class to create member observables, that supports multi node paths to child members.
    ///     It caches delegate to minimise the use of reflection. Reflection is used to avoid unnecessary boxing of value
    ///     types.
    /// </summary>
    public static class MemberObservableFactory
    {
        private static readonly MethodInfo _createNotifyPropertyChangedLinkMethod =
            typeof(MemberObservableFactory).GetMethod("CreateNotifyPropertyChangedLink",
                BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo _createPocoLinkMethod =
            typeof(MemberObservableFactory).GetMethod("CreatePocoLink",
                BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly Dictionary<MemberInfo, Delegate> _cache = new Dictionary<MemberInfo, Delegate>();
        private static readonly object _gate = new object();

        public static IObservable<TProperty> Create<TClass, TProperty>(TClass source, Expression<Func<TClass, TProperty>> memberPath)
        {
            var memberInfos = ExpressionHelpers.GetMemberInfos(memberPath);

            object currObserver = Forever<TClass>.Value(source);
            foreach (var memberInfo in memberInfos)
            {
                var create = (Func<object, MemberInfo, object>) CreateLinkDelegate(memberInfo);
                currObserver = create(currObserver, memberInfo);
            }

            return (IObservable<TProperty>) currObserver;
        }

        private static Delegate CreateLinkDelegate(MemberInfo memberInfo)
        {
            Delegate result;

            lock (_gate)
            {
                // See if its in the cache?
                if (_cache.TryGetValue(memberInfo, out result))
                    return result;

                var instanceType = memberInfo.ReflectedType;
                var memberType = memberInfo.GetMemberType();

                MethodInfo specialisedMethod;

                // Create the appropriate Link type.
                if (typeof(INotifyPropertyChanged).IsAssignableFrom(instanceType))
                    specialisedMethod = _createNotifyPropertyChangedLinkMethod.MakeGenericMethod(instanceType, memberType);
                else
                    specialisedMethod = _createPocoLinkMethod.MakeGenericMethod(instanceType, memberType);

                result = Delegate.CreateDelegate(typeof(Func<,,>).MakeGenericType(typeof(object), typeof(MemberInfo), typeof(object)),
                    specialisedMethod);

                _cache.Add(memberInfo, result);
            }

            return result;
        }

        [UsedImplicitly]
        private static object CreateNotifyPropertyChangedLink<TClass, TMember>(object source, MemberInfo outMemberInfo)
            where TClass : class, INotifyPropertyChanged
        {
            var getter = ReflectionHelpers.CreateMemberGetter(outMemberInfo);

            return new NotifyPropertyChangedLink<TClass, TMember>((IObservable<TClass>) source, outMemberInfo.Name, (Func<TClass, TMember>) getter);
        }

        [UsedImplicitly]
        private static object CreatePocoLink<TClass, TMember>(object source, MemberInfo memberInfo)
            where TClass : class
        {
            var getter = ReflectionHelpers.CreateMemberGetter(memberInfo);

            return new PocoLink<TClass, TMember>((IObservable<TClass>) source, (Func<TClass, TMember>) getter);
        }
    }
}