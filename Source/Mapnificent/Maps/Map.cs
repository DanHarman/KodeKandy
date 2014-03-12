// <copyright file="Map.cs" company="million miles per hour ltd">
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
using KodeKandy.Mapnificent.Definitions;

namespace KodeKandy.Mapnificent.Maps
{
    public class Map
    {
        private readonly MapDefinition mapDefinition;
        private readonly Mapper mapper;

        public Map(MapDefinition mapDefinition, Mapper mapper)
        {
            // We need the mapper so that when resolving this maps dependencies (i.e. other maps)
            // we can get hold of them.
            Require.NotNull(mapDefinition, "mapDefinition");
            Require.NotNull(mapper, "mapper");

            this.mapDefinition = mapDefinition;
            this.mapper = mapper;
        }

        public MappingType MappingType
        {
            get { return mapDefinition.MappingType; }
        }

        public void Apply(object from, object to)
        {
            Require.NotNull(from, "from");
            Require.NotNull(to, "to");


            throw new NotImplementedException();
        }
    }
}