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
using KodeKandy.Mapnificent.MemberAccess;

namespace KodeKandy.Mapnificent.Projections
{
    /// <summary>
    ///     Type safe ConversionOverride builder.
    /// </summary>
    /// <typeparam name="TFromDeclaring">The type being mapped from.</typeparam>
    /// <typeparam name="TToDeclaring">The type being mapped to.</typeparam>
    public class MapDefinitionBuilder<TFromDeclaring, TToDeclaring>
        where TToDeclaring : class
    {
        public Map Map { get; private set; }

        public MapDefinitionBuilder(Map map)
        {
            Require.IsTrue(map.ProjectionType.FromType == typeof(TFromDeclaring));
            Require.IsTrue(map.ProjectionType.ToType == typeof(TToDeclaring));

            Map = map;
        }

        public MapDefinitionBuilder<TFromDeclaring, TToDeclaring> For<TToMember>(Expression<Func<TToDeclaring, TToMember>> toMember,
            Action<BindingBuilder<TFromDeclaring, TToMember>> options)
        {
            Require.NotNull(toMember, "afterMappingAction");
            Require.IsTrue(ExpressionHelpers.IsMemberExpression(toMember), "Parameter 'afterMappingAction' must be a simple expression.");

            // Obtain a Binding.
            var toMemberInfo = ExpressionHelpers.GetMemberInfo(toMember);
            var memberBindingDefinition = Map.GetMemberBindingDefinition(toMemberInfo);

            // Apply the builder options.
            var builder = new BindingBuilder<TFromDeclaring, TToMember>(memberBindingDefinition);
            options(builder);

            return this;
        }

        public MapDefinitionBuilder<TFromDeclaring, TToDeclaring> AfterMapping(Action<TFromDeclaring, TToDeclaring> afterMappingAction)
        {
            Require.NotNull(afterMappingAction, "afterMappingAction");

            Map.PostMapStep = (f, t) => afterMappingAction((TFromDeclaring) f, (TToDeclaring) t);
            
            return this;
        }

        public MapDefinitionBuilder<TFromDeclaring, TToDeclaring> ConstructedBy(Func<ConstructionContext, TToDeclaring> constructedBy)
        {
            Require.NotNull(constructedBy, "constructedBy");

            Map.ConstructedBy = (ctx) => (object) constructedBy(ctx);

            return this;
        }

        public MapDefinitionBuilder<TFromDeclaring, TToDeclaring> InheritsFrom<TFromInherits, TToInherits>()
        {
            return InheritsFrom(new ProjectionType(typeof(TFromInherits), typeof(TToInherits)));
        }

        public MapDefinitionBuilder<TFromDeclaring, TToDeclaring> InheritsFrom(ProjectionType projectionType)
        {
            Require.NotNull(projectionType, "projectionType");
            Require.IsTrue(projectionType.FromType.IsAssignableFrom(Map.ProjectionType.FromType), 
                String.Format("Cannot inherit from a map whose 'From' type '{0}' is not a supertype of this maps 'From' type '{1}'.", projectionType.FromType.Name, Map.ProjectionType.FromType.Name));
            Require.IsTrue(projectionType.ToType.IsAssignableFrom(Map.ProjectionType.ToType),
                String.Format("Cannot inherit from a map whose 'To' type '{0}' is not a supertype of this maps 'To' type '{1}'.", projectionType.FromType.Name, Map.ProjectionType.FromType.Name));

            Map.InheritsFrom = projectionType;

            return this;
        }
    }
}