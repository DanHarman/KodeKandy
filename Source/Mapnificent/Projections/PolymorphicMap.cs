// <copyright file="PolymorphicMap.cs" company="million miles per hour ltd">
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KodeKandy.Mapnificent.Projections
{
    public class PolymorphicMap : Map
    {
        private readonly List<ProjectionType> polymorphs = new List<ProjectionType>();

        public PolymorphicMap(ProjectionType projectionType, Mapper mapper)
            : base(projectionType, mapper)
        {
        }

        public ReadOnlyCollection<ProjectionType> Polymorphs
        {
            get { return new ReadOnlyCollection<ProjectionType>(polymorphs); }
        }

        public override object Apply(object from, object to = null, bool mapInto = false)
        {
            Require.NotNull(from, "from");

            var matchedProjection = polymorphs.FirstOrDefault(pt => pt.FromType == from.GetType());
            if (matchedProjection == null)
            {
                var msg = string.Format("Error applying polymorphic map {0} as no polymorphic map defined for 'from' type {1}.", ProjectionType,
                    from.GetType().Name);
                throw new MapnificentException(msg, Mapper);
            }

            var map = Mapper.GetProjection(matchedProjection);
            return map.Apply(from, to, mapInto);
        }

        public void AddPolymorph(ProjectionType projectionType)
        {
            Require.NotNull(projectionType, "projectionType");

            Require.IsTrue(ProjectionType.FromType.IsAssignableFrom(projectionType.FromType),
                String.Format("Cannot be polymorphic for a Projection whose 'from' type '{0}' is not a subtype of this maps 'from' type '{1}'.",
                    projectionType.FromType.Name, ProjectionType.FromType.Name));

            Require.IsTrue(ProjectionType.ToType.IsAssignableFrom(projectionType.ToType),
                String.Format("Cannot be polymorphic for a Projection whose 'to' type '{0}' is not a subtype of this maps 'to' type '{1}'.",
                    projectionType.ToType.Name, ProjectionType.ToType.Name));

            Require.IsFalse(polymorphs.Any(pt => pt.FromType == projectionType.FromType),
                String.Format(
                    "Illegal polymorph defintion. A definition has already been registered for the 'from' type '{0}' and would be made ambiguous by this one.",
                    projectionType.FromType.Name));

            polymorphs.Add(projectionType);
        }
    }
}