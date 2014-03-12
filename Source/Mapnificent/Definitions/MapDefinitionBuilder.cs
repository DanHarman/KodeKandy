// <copyright file="MapDefinitionBuilder.cs" company="million miles per hour ltd">
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
using System.Linq.Expressions;

namespace KodeKandy.Mapnificent.Definitions
{
    /// <summary>
    ///     Type safe ConversionDefinition builder.
    /// </summary>
    /// <typeparam name="TFromDeclaring">The type being mapped from.</typeparam>
    /// <typeparam name="TToDeclaring">The type being mapped to.</typeparam>
    public class MapDefinitionBuilder<TFromDeclaring, TToDeclaring>
        where TToDeclaring : class
    {
        public MapDefinition MapDefinition { get; private set; }

        public MapDefinitionBuilder(MapDefinition mapDefinition)
        {
            Require.IsTrue(mapDefinition.MappingType.FromType == typeof(TFromDeclaring));
            Require.IsTrue(mapDefinition.MappingType.ToType == typeof(TToDeclaring));

            MapDefinition = mapDefinition;
        }

        public MapDefinitionBuilder<TFromDeclaring, TToDeclaring> For<TToMember>(Expression<Func<TToDeclaring, TToMember>> toMember,
            Action<MemberBindingDefinitionBuilder<TFromDeclaring, TToMember>> options)
        {
            Require.NotNull(toMember, "toMember");
            Require.IsTrue(ExpressionHelpers.IsMemberExpression(toMember), "Parameter 'toMember' must be a simple expression.");

            // Obtain a bindingDefinition.
            var toMemberInfo = ExpressionHelpers.GetMemberInfo(toMember);
            var memberBindingDefinition = MapDefinition.GetMemberBindingDefinition(toMemberInfo);

            // Apply the builder options.
            var builder = new MemberBindingDefinitionBuilder<TFromDeclaring, TToMember>(memberBindingDefinition);
            options(builder);

            return this;
        }
    }
}