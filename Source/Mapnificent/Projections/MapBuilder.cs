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

namespace KodeKandy.Mapnificent.Projections
{
    public abstract class MapBuilder<TFromDeclaring, TToDeclaring>
    {
        protected MapBuilder(IMap map)
        {
            Map = map;
        }

        public IMap Map { get; private set; }

        public MapBuilder<TFromDeclaring, TToDeclaring> PostMapStep(Action<TFromDeclaring, TToDeclaring> afterMappingAction)
        {
            Require.NotNull(afterMappingAction, "afterMappingAction");

            Map.PostMapStep = (f, t) => afterMappingAction((TFromDeclaring) f, (TToDeclaring) t);

            return this;
        }
    }
}