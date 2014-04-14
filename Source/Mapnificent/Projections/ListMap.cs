// <copyright file="ListMap.cs" company="million miles per hour ltd">
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
using System.Collections;

namespace KodeKandy.Mapnificent.Projections
{
    public class ListMap : Map
    {
        public ListMap(ProjectionType projectionType, Mapper mapper) : base(projectionType, mapper)
        {
            Require.IsTrue(projectionType.IsListProjection);

            ItemProjectionType = new ProjectionType(projectionType.FromItemType, projectionType.ToItemType);
        }

        public ProjectionType ItemProjectionType { get; private set; }

        public override object Apply(object from, object to = null, bool mapInto = false)
        {
            Require.NotNull(from, "from");
            // TODO Reinstate     Require.IsFalse(mapInto, "mapInto not currenlty support on collection");

            // TODO: need to cope with empty to type - if we pass in the expected type then we could instantiate it if its a 
            // concrete collection type. This is only a concern if there is no constructUsing defined.


            if (to == null)
                to = ConstructUsing(new ConstructionContext(Mapper, from, null));

            var fromEnumerable = (IEnumerable) from;
            var toCollection = (IList) to;

            var projection = Mapper.GetProjection(ItemProjectionType);

            foreach (var item in fromEnumerable)
            {
                var mappedItem = projection.Apply(item, null, false);
                toCollection.Add(mappedItem);
            }

            return to;
        }

        public void Validate()
        {
            if (!Mapper.HasProjection(ProjectionType.FromItemType, ProjectionType.ToItemType))
                throw new Exception(string.Format("Projection not defined for the item type of ListMap '{0}'", ProjectionType));
        }
    }
}