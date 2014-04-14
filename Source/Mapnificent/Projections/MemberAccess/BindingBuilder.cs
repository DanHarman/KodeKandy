// <copyright file="BindingBuilder.cs" company="million miles per hour ltd">
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
using System.Linq;
using System.Linq.Expressions;

namespace KodeKandy.Mapnificent.Projections.MemberAccess
{
    /// <summary>
    ///     Builder class to provide convenient type safe definition of member bindings.
    /// </summary>
    /// <typeparam name="TFromDeclaring">Declaring type of the 'from' class.</typeparam>
    /// <typeparam name="TToMember">Type of the 'to' member being set.</typeparam>
    public class BindingBuilder<TFromDeclaring, TToMember>
    {
        public BindingBuilder(Binding binding)
        {
            Binding = binding;
        }

        public Binding Binding { get; private set; }

        /// <summary>
        ///     Merge from a, nested if needed, member of the 'from' class to the specified 'to' class member. When using nested
        ///     and/or reference types, if there is a null in the expression chain, then no mapping will take place for that member
        ///     and the default value for that member will be set if it has been defined.
        /// 
        ///     This expression must be member chain expression, not a plain delegate doing adhoc work to create the value. If you
        ///     wish to fully customise rather than use a member chain use the <see cref="Custom" /> method.
        /// </summary>
        /// <typeparam name="TFromMember"></typeparam>
        /// <param name="fromMember"></param>
        /// <param name="convertFunc"></param>
        /// <returns></returns>
        public BindingBuilder<TFromDeclaring, TToMember> From<TFromMember>(
            Expression<Func<TFromDeclaring, TFromMember>> fromMember, Func<TFromMember, TToMember> convertFunc = null)
        {
            IProjection projection = null;
            
            if (convertFunc != null)
            {
                var anonConvertFunc = (Func<object, object>) (x => convertFunc((TFromMember) x));
                projection = new Conversion(ProjectionType.Create<TFromMember, TToMember>(), Binding.Mapper) { ConversionFunc = anonConvertFunc };
            }

            return From(fromMember, projection);
        }

        /// <summary>
        ///     Merge from a, nested if needed, member of the 'from' class to the specified 'to' class member. When using nested
        ///     and/or reference types, if there is a null in the expression chain, then no mapping will take place for that member
        ///     and the default value for that member will be set if it has been defined.
        /// 
        ///     This expression must be member chain expression, not a plain delegate doing adhoc work to create the value. If you
        ///     wish to fully customise rather than use a member chain use the <see cref="Custom" /> method.
        /// </summary>
        /// <typeparam name="TFromMember"></typeparam>
        /// <param name="fromMember"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public BindingBuilder<TFromDeclaring, TToMember> From<TFromMember>(
            Expression<Func<TFromDeclaring, TFromMember>> fromMember, IProjection projection)
        {
            Require.NotNull(fromMember, "fromMember");

            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos(fromMember);
            var fromMemberPath = String.Join(".", memberInfos.Select(x => x.Name));
            var fromMemberGetter = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);

            Binding.FromDefinition = new FromMemberDefinition(fromMemberPath, typeof(TFromMember), fromMemberGetter);

            if (projection != null)
            {
                Require.IsTrue(projection.ProjectionType.FromType == typeof(TFromMember));
                Require.IsTrue(projection.ProjectionType.ToType == typeof(TToMember));
                Binding.Projection = projection;
            }

            return this;
        }

        /// <summary>
        ///     Ignores this 'to' Member in the mapping.
        ///     If you intend to not ClassMap into a member on the 'to' class you must explicitly define that with
        ///     <see cref="Ignore" />
        ///     so that the mapping definition will validate.
        /// </summary>
        /// <returns></returns>
        public BindingBuilder<TFromDeclaring, TToMember> Ignore()
        {
            Binding.IsIgnore = true;

            return this;
        }

        /// <summary>
        ///     Explicitly define a mapping for a member with a delegate.
        /// </summary>
        /// <typeparam name="TFromResult"></typeparam>
        /// <param name="fromFunc"></param>
        /// <returns></returns>
        public BindingBuilder<TFromDeclaring, TToMember> Custom<TFromResult>(Func<MappingContext, TFromResult> fromFunc)
        {
            Require.NotNull(fromFunc, "fromFunc");

            Binding.FromDefinition = new FromCustomDefinition(ctx => (object) fromFunc(ctx), typeof(TFromResult));

            return this;
        }

        //public BindingBuilder<TToMember> ConstructUsing(Func<TToMember> constructor)
        //{
        //    return this;
        //}
    }
}