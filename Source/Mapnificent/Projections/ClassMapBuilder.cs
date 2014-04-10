// <copyright file="ClassMapBuilder.cs" company="million miles per hour ltd">
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
    ///     Type safe ClassMap builder.
    /// </summary>
    /// <typeparam name="TFromDeclaring">The type being mapped from.</typeparam>
    /// <typeparam name="TToDeclaring">The type being mapped to.</typeparam>
    public class ClassMapBuilder<TFromDeclaring, TToDeclaring> : MapBuilder<TFromDeclaring, TToDeclaring>
        where TToDeclaring : class
    {
        public ClassMapBuilder(ClassMap classMap)
            : base(classMap)
        {
            Require.IsTrue(classMap.ProjectionType.FromType == typeof(TFromDeclaring));
            Require.IsTrue(classMap.ProjectionType.ToType == typeof(TToDeclaring));
        }

        public new ClassMap Map
        {
            get { return (ClassMap) base.Map; }
        }

        public ClassMapBuilder<TFromDeclaring, TToDeclaring> For<TToMember>(Expression<Func<TToDeclaring, TToMember>> toMember,
            Action<BindingBuilder<TFromDeclaring, TToMember>> options)
        {
            Require.NotNull(toMember, "toMember");
            Require.IsTrue(ExpressionHelpers.IsMemberExpression(toMember), "Parameter 'afterMappingAction' must be a simple expression.");

            // Obtain a Binding.
            var toMemberInfo = ExpressionHelpers.GetMemberInfo(toMember);
            var memberBindingDefinition = Map.GetMemberBindingDefinition(toMemberInfo);

            // Apply the builder options.
            var builder = new BindingBuilder<TFromDeclaring, TToMember>(memberBindingDefinition);
            options(builder);

            return this;
        }

        public new ClassMapBuilder<TFromDeclaring, TToDeclaring> PostMapStep(Action<TFromDeclaring, TToDeclaring> afterMappingAction)
        {
            return (ClassMapBuilder<TFromDeclaring, TToDeclaring>) base.PostMapStep(afterMappingAction);
        }

        public ClassMapBuilder<TFromDeclaring, TToDeclaring> ConstructUsing(Func<ConstructionContext, TToDeclaring> constructedBy)
        {
            Require.NotNull(constructedBy, "constructedBy");

            Map.ConstructUsing = (ctx) => (object) constructedBy(ctx);

            return this;
        }

        public ClassMapBuilder<TFromDeclaring, TToDeclaring> InheritsFrom<TFromInherits, TToInherits>()
        {
            var projectionType = ProjectionType.Create<TFromInherits, TToInherits>();

            if (!projectionType.IsClassProjection)
            {
                var msg = string.Format("Error inheriting from '{0}' for map of type {1}, it is only possible to inherit from a class map",
                    projectionType, Map.ProjectionType);

                throw new MapnificentException(msg, Map.Mapper);
            }

            Map.InheritsFrom = projectionType;

            return this;
        }

        public ClassMapBuilder<TFromDeclaring, TToDeclaring> PolymorhpicFor<TFromDerived, TToDerived>()
        {
            Map.AddPolymorphicFor(ProjectionType.Create<TFromDerived, TToDerived>());

            return this;
        }
    }
}