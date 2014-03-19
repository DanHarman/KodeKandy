// <copyright file="MemberBindingDefinitionBuilder.cs" company="million miles per hour ltd">
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
using System.Linq;
using System.Linq.Expressions;

namespace KodeKandy.Mapnificent
{
    /// <summary>
    ///     Builder class to provide convenient type safe definition of member bindings.
    /// </summary>
    /// <typeparam name="TFromDeclaring">Declaring type of the 'from' class.</typeparam>
    /// <typeparam name="TToMember">Type of the 'to' member being set.</typeparam>
    public class MemberBindingDefinitionBuilder<TFromDeclaring, TToMember>
    {
        public MemberBindingDefinition MemberBindingDefinition { get; private set; }

        public MemberBindingDefinitionBuilder(MemberBindingDefinition memberBindingDefinition)
        {
            this.MemberBindingDefinition = memberBindingDefinition;
        }

        /// <summary>
        ///     Map from a, nested if needed, member of the 'from' class to the specified 'to' class member. When using nested
        ///     and/or reference types, if there is a null in the expression chain, then no mapping will take place for that member
        ///     and the default value for that member will be set if it has been defined.
        /// 
        ///     This expression must be member chain expression, not a plain delegate doing adhoc work to create the value. If you
        ///     wish to fully customise rather than use a member chain use the <see cref="Explictly{TFrom}" /> method.
        /// </summary>
        /// <typeparam name="TFromMember"></typeparam>
        /// <param name="fromMember"></param>
        /// <returns></returns>
        public MemberBindingDefinitionBuilder<TFromDeclaring, TToMember> From<TFromMember>(
            Expression<Func<TFromDeclaring, TFromMember>> fromMember)
        {
            Require.NotNull(fromMember, "fromMember");

            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos(fromMember);
            var fromMemberName = String.Join(".", memberInfos.Select(x => x.Name));
            var fromMemberGetter = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);

            MemberBindingDefinition.MemberGetterDefinition = new MemberGetterDefinition(typeof(TFromDeclaring), fromMemberName, typeof(TFromMember),
                fromMemberGetter);

            return this;
        }

        /// <summary>
        ///     Ignores this 'to' Member in the mapping.
        ///     If you intend to not map into a member on the 'to' class you must explicitly define that with <see cref="Ignore" />
        ///     so that the mapping definition will validate.
        /// </summary>
        /// <returns></returns>
        public MemberBindingDefinitionBuilder<TFromDeclaring, TToMember> Ignore()
        {
            MemberBindingDefinition.Ignore = true;

            return this;
        }

        /// <summary>
        ///     Explicitly define a mapping for a member with a delegate.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="fromFunc"></param>
        /// <returns></returns>
        public MemberBindingDefinitionBuilder<TFromDeclaring, TToMember> Explictly<TFrom>(Func<TFrom, TToMember> fromFunc)
        {
            return this;
        }

        //public MemberBindingDefinitionBuilder<TToMember> ConstructedBy(Func<TToMember> constructor)
        //{
        //    return this;
        //}
    }

    //public static class MemberBindingDefinitionBuilderExtensions
    //{

    //}
}