// <copyright file="MapBuilder.cs" company="million miles per hour ltd">
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
using System.Linq.Expressions;
using KodeKandy.Mapnificent.MemberAccess;

namespace KodeKandy.Mapnificent.Projections
{
    /// <summary>
    ///     Type safe ConversionOverride builder.
    /// </summary>
    /// <typeparam name="TFromDeclaring">The type being mapped from.</typeparam>
    /// <typeparam name="TToDeclaring">The type being mapped to.</typeparam>
    public class MapBuilder<TFromDeclaring, TToDeclaring>
        where TToDeclaring : class
    {
        public MapBuilder(Map map)
        {
            Require.IsTrue(map.ProjectionType.FromType == typeof(TFromDeclaring));
            Require.IsTrue(map.ProjectionType.ToType == typeof(TToDeclaring));

            Map = map;
        }

        public Map Map { get; private set; }

        public MapBuilder<TFromDeclaring, TToDeclaring> For<TToMember>(Expression<Func<TToDeclaring, TToMember>> toMember,
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

        public MapBuilder<TFromDeclaring, TToDeclaring> AfterMapping(Action<TFromDeclaring, TToDeclaring> afterMappingAction)
        {
            Require.NotNull(afterMappingAction, "afterMappingAction");

            Map.PostMapStep = (f, t) => afterMappingAction((TFromDeclaring) f, (TToDeclaring) t);

            return this;
        }

        public MapBuilder<TFromDeclaring, TToDeclaring> ConstructedBy(Func<ConstructionContext, TToDeclaring> constructedBy)
        {
            Require.NotNull(constructedBy, "constructedBy");

            Map.ConstructedBy = (ctx) => (object) constructedBy(ctx);

            return this;
        }

        public MapBuilder<TFromDeclaring, TToDeclaring> InheritsFrom<TFromInherits, TToInherits>()
        {
            Map.InheritsFrom = ProjectionType.Create<TFromInherits, TToInherits>();

            return this;
        }


        public MapBuilder<TFromDeclaring, TToDeclaring> PolymorhpicFor<TFromDerived, TToDerived>()
        {
            Map.AddPolymorphicFor(ProjectionType.Create<TFromDerived, TToDerived>());

            return this;
        }
    }
}