// <copyright file="ReflectionHelpers.cs" company="million miles per hour ltd">
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
// 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KodeKandy
{
    public delegate bool SafeGetterFunc(object instance, out object value);

    public static class ReflectionHelpers
    {
        public class SafeWeakMemberGetterResult
        {
            public bool HasResult { get; private set; }
            public object Result { get; private set; }

            public SafeWeakMemberGetterResult(bool hasResult, object result)
            {
                HasResult = hasResult;
                Result = result;
            }

            public static SafeWeakMemberGetterResult WithResult(object result)
            {
                return new SafeWeakMemberGetterResult(true, result);
            }

            public static readonly SafeWeakMemberGetterResult NoResult = new SafeWeakMemberGetterResult(false, null);
        }

        public static SafeGetterFunc CreateSafeWeakMemberChainGetter(IEnumerable<MemberInfo> memberInfos)
        {
            Require.NotNull(memberInfos, "memberInfos");
            Require.IsTrue(memberInfos.Any());

            var getters = memberInfos.Select(CreateWeakMemberGetter).ToArray();

            // If there is no chain then avoid unnecessary looping etc.
            if (getters.Length == 1)
                return (object inst, out object value) =>
                {
                    value = getters[0](inst);
                    return true;
                };

            // With a chain we have to loop.
            return (object inst, out object value) =>
            {
                // All but the terminal need to be treated as dereferences.
                for (var i = 0; i < getters.Length - 1; ++i)
                {
                    var derefGetter = getters[i];

                    // Follow the chain
                    inst = derefGetter(inst);

                    if (inst == null)
                    {
                        value = null;
                        return false;
                    }
                }

                value = getters[getters.Length - 1](inst);
                return true;
            };
        }

        /// <summary>
        ///     Get a MemberInfo for a simple member. The MemberInfo will be either a FieldInfo or PropertyInfo.
        /// </summary>
        /// <returns>Either a PropertyInfo or FieldInfo</returns>
        public static MemberInfo GetMemberInfo(Type classType, string memberName)
        {
            Require.NotNull(classType, "classType");
            Require.NotNull(memberName, "memberName");

            return (MemberInfo) classType.GetProperty(memberName) ?? classType.GetField(memberName);
        }

        /// <summary>
        ///     Get all the properties and fields on a class.
        /// </summary>
        public static ReadOnlyCollection<MemberInfo> GetMemberInfos(Type classType, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            Require.NotNull(classType, "classType");

            var memberInfos = new List<MemberInfo>();

            var propertyInfos = classType.GetProperties(flags);

            foreach (var propertyInfo in propertyInfos)
            {
                if (!propertyInfo.CanWrite)
                    continue;

                memberInfos.Add(propertyInfo);
            }

            var fieldInfos = classType.GetFields(flags);

            foreach (var fieldInfo in fieldInfos)
            {
                memberInfos.Add(fieldInfo);
            }

            return new ReadOnlyCollection<MemberInfo>(memberInfos);
        }

        /// <summary>
        ///     Create a weakly typed member getter delegate.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Func<object, object> CreateWeakMemberGetter(MemberInfo memberInfo)
        {
            Require.NotNull(memberInfo, "memberInfo");

            if (memberInfo is PropertyInfo)
                return CreateWeakPropertyGetter((PropertyInfo) memberInfo);

            return CreateWeakFieldGetter((FieldInfo) memberInfo);
        }

        /// <summary>
        ///     Create a weakly typed member setter delegate.
        /// </summary>
        public static Action<object, object> CreateWeakMemberSetter(MemberInfo memberInfo)
        {
            Require.NotNull(memberInfo, "memberInfo");

            if (memberInfo is PropertyInfo)
                return CreateWeakPropertySetter((PropertyInfo) memberInfo);

            return CreateWeakFieldSetter((FieldInfo) memberInfo);
        }

        /// <summary>
        ///     Create a weakly typed property getter delegate.
        /// </summary>
        public static Func<object, object> CreateWeakPropertyGetter(PropertyInfo propertyInfo)
        {
            Require.NotNull(propertyInfo, "propertyInfo");

            var getterMethodInfo = propertyInfo.GetGetMethod(true);

            if (getterMethodInfo == null)
                throw new Exception(string.Format("No get method defined for property '{0}' on class '{1}", propertyInfo.Name,
                    propertyInfo.DeclaringType));

            // Parameters
            var weakInstanceParam = Expression.Parameter(typeof(object));

            // Convert Params
            var typedInstanceParam = Expression.Convert(weakInstanceParam, propertyInfo.DeclaringType);

            var callExpression = Expression.Call(typedInstanceParam, getterMethodInfo);
            var boxedCallExpression = Expression.Convert(callExpression, typeof(object));

            // Build the delegate
            var weakGetter = Expression.Lambda<Func<object, object>>(boxedCallExpression, weakInstanceParam).Compile();

            return weakGetter;
        }

        /// <summary>
        ///     Create a weakly typed property setter delegate.
        /// </summary>
        public static Action<object, object> CreateWeakPropertySetter(PropertyInfo propertyInfo)
        {
            Require.NotNull(propertyInfo, "propertyInfo");

            var setterMethodInfo = propertyInfo.GetSetMethod(true);

            if (setterMethodInfo == null)
                throw new Exception(string.Format("No set method defined for property '{0}' on class '{1}", propertyInfo.Name,
                    propertyInfo.DeclaringType));

            // Parameters
            var weakInstanceParam = Expression.Parameter(typeof(object));
            var weakValueParam = Expression.Parameter(typeof(object));

            // Convert Params
            var typedInstanceParam = Expression.Convert(weakInstanceParam, propertyInfo.DeclaringType);
            var typedValueParam = Expression.Convert(weakValueParam, propertyInfo.PropertyType);

            // Build the delegate
            var callExpression = Expression.Call(typedInstanceParam, setterMethodInfo, typedValueParam);
            var weakSetter = Expression.Lambda<Action<object, object>>(callExpression, weakInstanceParam, weakValueParam).Compile();

            return weakSetter;
        }

        /// <summary>
        ///     Create a weakly typed field getter delegate.
        /// </summary>
        public static Func<object, object> CreateWeakFieldGetter(FieldInfo fieldInfo)
        {
            Require.NotNull(fieldInfo, "fieldInfo");

            // Parameters
            var weakInstanceParam = Expression.Parameter(typeof(object));
            var typedInstanceParam = Expression.Convert(weakInstanceParam, fieldInfo.DeclaringType);

            // Build the weak getter expression
            var fieldExpression = Expression.Field(typedInstanceParam, fieldInfo);
            var weakFieldGetterExpression = Expression.Convert(fieldExpression, typeof(object));

            // Build the delgate
            var weakGetter = Expression.Lambda<Func<object, object>>(weakFieldGetterExpression, weakInstanceParam).Compile();

            return weakGetter;
        }

        /// <summary>
        ///     Create a weakly typed field setter delegate.
        /// </summary>
        public static Action<object, object> CreateWeakFieldSetter(FieldInfo fieldInfo)
        {
            Require.NotNull(fieldInfo, "fieldInfo");

            // Parameters
            var weakInstanceParam = Expression.Parameter(typeof(object));
            var weakValueParam = Expression.Parameter(typeof(object));

            // Convert Params
            var typedInstanceParam = Expression.Convert(weakInstanceParam, fieldInfo.DeclaringType);
            var typedValueParam = Expression.Convert(weakValueParam, fieldInfo.FieldType);

            // Build the weak getter expression
            var fieldExpression = Expression.Field(typedInstanceParam, fieldInfo);
            var fieldSetterExpression = Expression.Assign(fieldExpression, typedValueParam);

            // Build the delegate
            var weakSetter = Expression.Lambda<Action<object, object>>(fieldSetterExpression, weakInstanceParam, weakValueParam).Compile();

            return weakSetter;
        }


        /// <summary>
        ///     Discover if a type implements an open generic type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="genericType">The type to look for.</param>
        /// <returns>True if the generic type is implemented, otherwise false.</returns>
        public static bool DoesImplementsGenericType(this Type type, Type genericType)
        {
            Require.NotNull(type, "type");
            Require.NotNull(genericType, "genericType");
            Require.IsTrue(genericType.IsGenericType, "Must be a generic type.", "genericType");

            return TryGetGenericTypeDefinitionOfType(type, genericType) != null;
        }

        /// <summary>
        ///     Try to get the generic definition of a type implemented on type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="genericType">The generic type to look for.</param>
        /// <returns>The generic type definition if found, otherwise null.</returns>
        public static Type TryGetGenericTypeDefinitionOfType(this Type type, Type genericType)
        {
            Require.NotNull(type, "type");
            Require.NotNull(genericType, "genericType");
            Require.IsTrue(genericType.IsGenericType, "Must be a generic type.", "genericType");

            if (type.IsInterface)
                return type.IsGenericType && type.GetGenericTypeDefinition() == genericType ? type.GetGenericTypeDefinition() : null;

            return type.GetInterfaces().SingleOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == genericType);
        }
    }
}