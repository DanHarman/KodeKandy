// <copyright file="MemberAccessor.cs" company="million miles per hour ltd">
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
using System.Reflection;

namespace KodeKandy
{
    public enum MemberAccessorType
    {
        Field,
        Property
    }

    /// <summary>
    ///     Defines delegates to set a member on a class, and exposes metadata about these members.
    ///     The delegates maybe null if they do not exist.
    /// </summary>
    public class MemberAccessor
    {
        public MemberAccessor(MemberInfo memberInfo)
        {
            Require.NotNull(memberInfo, "memberInfo");

            Type = memberInfo is PropertyInfo ? MemberAccessorType.Property : MemberAccessorType.Field;
            MemberName = memberInfo.Name;
            DeclaringType = memberInfo.DeclaringType;
            MemberType = memberInfo.GetMemberType();
            Getter = memberInfo.CanRead() ? ReflectionHelpers.CreateWeakMemberGetter(memberInfo) : null;
            Setter = memberInfo.CanWrite() ? ReflectionHelpers.CreateWeakMemberSetter(memberInfo) : null;
        }

        /// <summary>
        ///     Whether the member is a property or field.
        /// </summary>
        public MemberAccessorType Type { get; private set; }

        /// <summary>
        ///     The name of the property or field this accessor corresponds to.
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        ///     The type of the class declaring the member.
        /// </summary>
        public Type DeclaringType { get; private set; }

        /// <summary>
        ///     The type of the member.
        /// </summary>
        public Type MemberType { get; private set; }

        /// <summary>
        ///     The member getter delegate. May be null.
        /// </summary>
        public Func<object, object> Getter { get; private set; }

        /// <summary>
        ///     The member setter delegate. May be null.
        /// </summary>
        public Action<object, object> Setter { get; private set; }
    }
}