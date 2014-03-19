// <copyright file="MemberSetterDefinition.cs" company="million miles per hour ltd">
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
    public class MemberSetterDefinition : MemberAccessorDefinition
    {
        public Action<object, object> MemberSetter { get; private set; }

        public MemberSetterDefinition(MemberInfo memberInfo)
            : base(memberInfo.DeclaringType, memberInfo.Name, memberInfo.GetMemberType())
        {
            Require.NotNull(memberInfo, "memberInfo");

            MemberSetter = ReflectionHelpers.CreateWeakMemberSetter(memberInfo);
        }
    }
}