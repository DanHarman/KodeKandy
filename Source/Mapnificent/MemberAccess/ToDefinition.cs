// <copyright file="ToDefinition.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.MemberAccess
{
    public class ToDefinition
    {
        public KodeKandy.MemberAccessor Accessor { get; private set; }

        public string MemberName { get { return Accessor.MemberName; } }

        public Type DeclaringType { get { return Accessor.DeclaringType; } }

        public Type MemberType { get { return Accessor.MemberType; } }

        public ToDefinition(MemberInfo memberInfo)
        {
            Require.NotNull(memberInfo, "memberInfo");

            // We need a getter as well for situations where we are mapping into an existing object and need to merge into
            // 'child' members, rather than just creating new instances of them.
            Accessor = new KodeKandy.MemberAccessor(memberInfo);
        }
    }
}