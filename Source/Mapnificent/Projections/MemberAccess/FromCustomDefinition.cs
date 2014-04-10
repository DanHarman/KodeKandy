// <copyright file="FromCustomDefinition.cs" company="million miles per hour ltd">
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
    /// <summary>
    ///     A 'from' definition that executes a custom delegate on a 'from' instance to get a value to set into the 'to'
    ///     member.
    /// </summary>
    public class FromCustomDefinition : FromDefinition
    {
        private readonly Func<MappingContext, object> fromFunc;
        private readonly Type memberType;

        public FromCustomDefinition(Func<MappingContext, object> fromFunc, Type fromType)
        {
            Require.NotNull(fromFunc, "fromFunc");

            this.fromFunc = fromFunc;
            memberType = fromType;
        }

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
            fromValue = fromFunc(new MappingContext(mapper, fromDeclaringInstance));

            return true;
        }

        public override string ToString()
        {
            return "<Custom>";
        }
    }
}