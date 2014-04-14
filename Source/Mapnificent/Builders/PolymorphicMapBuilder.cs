// <copyright file="PolymorphicMapBuilder.cs" company="million miles per hour ltd">
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
using KodeKandy.Mapnificent.Projections;

namespace KodeKandy.Mapnificent.Builders
{
    public class PolymorphicMapBuilder<TFromDeclaring, TToDeclaring> : MapBuilder<TFromDeclaring, TToDeclaring>
        where TToDeclaring : class
    {
        public PolymorphicMapBuilder(PolymorphicMap classMap)
            : base(classMap)
        {
            Require.IsTrue(classMap.ProjectionType.FromType == typeof(TFromDeclaring));
            Require.IsTrue(classMap.ProjectionType.ToType == typeof(TToDeclaring));
        }

        public new PolymorphicMap Map
        {
            get { return (PolymorphicMap) base.Map; }
        }

        public new PolymorphicMapBuilder<TFromDeclaring, TToDeclaring> AfterMapping(Action<TFromDeclaring, TToDeclaring> afterMappingAction)
        {
            return (PolymorphicMapBuilder<TFromDeclaring, TToDeclaring>) base.AfterMapping(afterMappingAction);
        }

        public PolymorphicMapBuilder<TFromDeclaring, TToDeclaring> AddPolymorph<TFromDerived, TToDerived>()
        {
            Map.AddPolymorph(ProjectionType.Create<TFromDerived, TToDerived>());

            return this;
        }
    }
}