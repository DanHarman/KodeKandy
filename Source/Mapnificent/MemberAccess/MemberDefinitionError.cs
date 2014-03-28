// <copyright file="MemberDefinitionError.cs" company="million miles per hour ltd">
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
    public class MemberDefinitionError
    {
        public Type DeclaringType { get; private set; }

        public Type ToMemberType { get; private set; }

        public string ToMemberName { get; set; }

        public string Reason { get; set; }

        public MemberDefinitionError(Type declaringType, Type toMemberType, string toMemberName, string reason)
        {
            Require.NotNull(declaringType, "declaringType");
            Require.NotNull(toMemberType, "toMemberType");
            Require.NotNull(toMemberName, "toMemberName");
            Require.NotNull(reason, "reason");

            DeclaringType = declaringType;
            ToMemberType = toMemberType;
            ToMemberName = toMemberName;
            Reason = reason;
        }

        public static MemberDefinitionError Create(MemberInfo toMemberInfo, string reason)
        {
            return new MemberDefinitionError(toMemberInfo.DeclaringType, toMemberInfo.GetMemberType(), toMemberInfo.Name, reason);
        }

        public static MemberDefinitionError Create(MemberInfo toMemberInfo, string reasonFormat, params object[] reasonArgs)
        {
            var reason = string.Format(reasonFormat, reasonArgs);
            return Create(toMemberInfo, reason);
        }

        public static MemberDefinitionError Create(ToDefinition toDefinition, string reason)
        {
            return new MemberDefinitionError(toDefinition.DeclaringType, toDefinition.MemberType, toDefinition.MemberName, reason);
        }

        public static MemberDefinitionError Create(ToDefinition toDefinition, string reasonFormat, params object[] reasonArgs)
        {
            var reason = string.Format(reasonFormat, reasonArgs);
            return Create(toDefinition, reason);
        }
    }
}