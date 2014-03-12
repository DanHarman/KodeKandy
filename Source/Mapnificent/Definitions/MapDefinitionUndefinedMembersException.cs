// <copyright file="MapDefinitionUndefinedMembersException.cs" company="million miles per hour ltd">
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
using System.Reflection;
using System.Text;

namespace KodeKandy.Mapnificent.Definitions
{
    public class MapDefinitionUndefinedMembersException : Exception
    {
        public IEnumerable<MemberInfo> UndefinedMemberInfos { get; private set; }

        public MapDefinitionUndefinedMembersException(IEnumerable<MemberInfo> undefinedMemberInfos) : base(CreateErrorString(undefinedMemberInfos))
        {
            UndefinedMemberInfos = undefinedMemberInfos;
        }

        private static string CreateErrorString(IEnumerable<MemberInfo> undefinedMemberInfos)
        {
            var builder = new StringBuilder("Member definitions are not set for the following:");

            return builder.ToString();
        }
    }
}