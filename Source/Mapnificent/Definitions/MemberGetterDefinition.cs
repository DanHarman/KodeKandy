// <copyright file="MemberGetterDefinition.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.Definitions
{
    /// <summary>
    ///     A definition for from members that are being mapped from.
    /// </summary>
    public class MemberGetterDefinition : MemberAccessorDefinition
    {
        public ReflectionHelpers.SafeGetterFunc MemberGetter { get; private set; }

        public MemberGetterDefinition(Type declaringType, string memberName, Type memberType, ReflectionHelpers.SafeGetterFunc memberGetter)
            : base(declaringType, memberName, memberType)
        {
            Require.NotNull(memberGetter, "fromMemberSafeGetterFunc");

            MemberGetter = memberGetter;
        }
    }
}