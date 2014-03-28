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
// 
// </copyright>

using System;

namespace KodeKandy.Mapnificent.Bindngs
{
    public abstract class FromDefinition
    {
        /// <summary>
        ///     Attempts to get a member value from a 'from' instance.
        /// </summary>
        /// <param name="fromDeclaringInstance">The class instance from which the member should be fetched.</param>
        /// <param name="mapper">The current mapper (required to provide context to custom From definitions).</param>
        /// <param name="fromValue">The out value set if anything is a value can be retrieved.</param>
        /// <returns>True if a value was retrieved, otherwise false.</returns>
        public abstract bool TryGetFromValue(object fromDeclaringInstance, Mapper mapper, out object fromValue);

        public abstract string Description { get; }
    }

    /// <summary>
    ///     A 'from' definition that binds to a member on the 'from' instance to generate a value to set into the 'to' class's member.
    /// </summary>
    public class FromMemberDefinition : FromDefinition
    {
        public SafeGetterFunc MemberGetter { get; private set; }

        /// <summary>
        ///     The path to the property or field this accessor corresponds to. This may be a 'chain'.
        /// </summary>
        public string MemberPath { get; private set; }

        /// <summary>
        ///     The type of the member.
        /// </summary>
        public Type MemberType { get; private set; }

        public FromMemberDefinition(string memberPath, Type memberType, SafeGetterFunc memberGetter)
           // : base(declaringType, memberPath, memberType)
        {
            Require.NotNull(memberPath, "memberPath");
            Require.NotNull(memberType, "memberType");
            Require.NotNull(memberGetter, "memberGetter");

            MemberPath = memberPath;
            MemberType = memberType;
            MemberGetter = memberGetter;

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

        public override string Description
        {
            get { return string.Format("Member:{0}", MemberPath); }
        }
    }

    /// <summary>
    ///     A 'from' definition that executes a custom delegate on a 'from' instance to get a value to set into the 'to' member.
    /// </summary>
    public class FromCustomDefinition : FromDefinition
    {
        private readonly Func<MappingContext, object> fromFunc;

        public FromCustomDefinition(Func<MappingContext, object> fromFunc)
        {
            Require.NotNull(fromFunc, "fromFunc");

            this.fromFunc = fromFunc;
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

        public override string Description
        {
            get { return "<Custom>"; }
        }
    }

    public class FromUndefinedDefinition : FromDefinition
    {
        public static readonly FromUndefinedDefinition Default = new FromUndefinedDefinition();

        private FromUndefinedDefinition()
        {}

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

        public override string Description
        {
            get { return "<Undefined>"; }
        }
    }
}