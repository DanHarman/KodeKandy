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
// 
// </copyright>

using System;
using System.Reflection;

namespace KodeKandy.Mapnificent
{
    /// <summary>
    ///     Base class for member access definitions that capture memberPath, GetMemberType and appropritate delegate to
    ///     access a member.
    /// </summary>
    public abstract class MemberAccessorDefinition
    {
        /// <summary>
        ///     memberPath or description this accessor corresponds to.
        /// </summary>
        public string MemberPath { get; private set; }

        public Type DeclaringType { get; private set; }

        public Type MemberType { get; private set; }

        protected MemberAccessorDefinition(Type declaringType, string memberPath, Type memberType)
        {
            Require.NotNull(declaringType, "declaringType");
            Require.NotNull(memberPath, "memberPath");
            Require.NotNull(memberType, "memberType");

            MemberPath = memberPath;
            DeclaringType = declaringType;
            MemberType = memberType;
        }
    }
}