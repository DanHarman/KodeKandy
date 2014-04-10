// <copyright file="FromUndefinedDefinition.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.MemberAccess
{
    /// <summary>
    ///     Used to represent undefined 'from' definitions on bindings.
    /// </summary>
    public class FromUndefinedDefinition : FromDefinition
    {
        public static readonly FromUndefinedDefinition Default = new FromUndefinedDefinition();

        private FromUndefinedDefinition()
        {
        }

        /// <summary>
        ///     The type of the member.
        /// </summary>
        public override Type MemberType
        {
            get { return ProjectionType.Undefined; }
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
            fromValue = null;
            return false;
        }

        public override string ToString()
        {
            return "<Undefined>";
        }
    }
}