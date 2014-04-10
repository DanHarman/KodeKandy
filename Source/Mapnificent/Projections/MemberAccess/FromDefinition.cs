// <copyright file="FromDefinition.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.Projections.MemberAccess
{
    public abstract class FromDefinition
    {
        /// <summary>
        ///     The type of the member.
        /// </summary>
        public abstract Type MemberType { get; }

        /// <summary>
        ///     Attempts to get a member value from a 'from' instance.
        /// </summary>
        /// <param name="fromDeclaringInstance">The class instance from which the member should be fetched.</param>
        /// <param name="mapper">The current mapper (required to provide context to custom From definitions).</param>
        /// <param name="fromValue">The out value set if anything is a value can be retrieved.</param>
        /// <returns>True if a value was retrieved, otherwise false.</returns>
        public abstract bool TryGetFromValue(object fromDeclaringInstance, Mapper mapper, out object fromValue);
    }

    /// <summary>
    ///     A 'from' definition that binds to a member on the 'from' instance to generate a value to set into the 'to' class's
    ///     member.
    /// </summary>
    public class FromMemberDefinition : FromDefinition
    {
        private readonly Type memberType;

        public FromMemberDefinition(string memberPath, Type memberType, SafeGetterFunc memberGetter)
        {
            Require.NotNull(memberPath, "memberPath");
            Require.NotNull(memberType, "memberType");
            Require.NotNull(memberGetter, "memberGetter");

            MemberPath = memberPath;
            this.memberType = memberType;
            MemberGetter = memberGetter;
        }

        public SafeGetterFunc MemberGetter { get; private set; }

        /// <summary>
        ///     The path to the property or field this accessor corresponds to. This may be a 'chain'.
        /// </summary>
        public string MemberPath { get; private set; }

        /// <summary>
        ///     The type of the member.
        /// </summary>
        public override Type MemberType
        {
            get { return memberType; }
        }

        /// <summary>
        ///     Attempts to get a member value from a 'from' instance.
        /// </summary>
        /// <param name="fromDeclaringInstance">The class instance from which the member should be fetched.</param>
        /// <param name="mapper">The current mapper (required to provide context to custom From definitions).</param>
        /// <param name="fromValue">The out value set if anything is a value can be retrieved.</param>
        /// <returns>True if a value was retrieved, otherwise false.</returns>
        public override bool TryGetFromValue(object fromDeclaringInstance, Mapper mapper, out object fromValue)
        {
            return MemberGetter(fromDeclaringInstance, out fromValue);
        }

        public override string ToString()
        {
            return string.Format("Member:{0}", MemberPath);
        }
    }
}