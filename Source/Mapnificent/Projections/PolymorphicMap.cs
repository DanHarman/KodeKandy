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
        private readonly List<IProjection> polymorphs = new List<IProjection>();

        private readonly ProjectionType projectionType;

        public PolymorphicMap(ProjectionType projectionType, Mapper mapper)
            : base(projectionType, mapper)
        {
            this.projectionType = projectionType;
        }

        public override ProjectionType ProjectionType
        {
            get { return projectionType; }
        }

        public ReadOnlyCollection<IProjection> Polymorphs
        {
            get { return new ReadOnlyCollection<IProjection>(polymorphs); }
        }

        public override object Apply(object from, object to = null, bool mapInto = false)
        {
            Require.NotNull(from, "from");

            var matchedProjection = polymorphs.FirstOrDefault(pt => pt.ProjectionType.FromType == from.GetType());
            if (matchedProjection == null)
            {
                var msg = string.Format("Error applying polymorphic map {0} as no polymorphic map defined for 'from' type {1}.", ProjectionType,
                    from.GetType().Name);
                throw new MapnificentException(msg, Mapper);
            }

            return matchedProjection.Apply(from, to, mapInto);
        }

        public void AddPolymorph(ProjectionType polymorphProjectionType)
        {
            Require.NotNull(polymorphProjectionType, "polymorphProjectionType");

            Require.IsTrue(ProjectionType.FromType.IsAssignableFrom(polymorphProjectionType.FromType),
                String.Format("Cannot be polymorphic for a Projection whose 'from' type '{0}' is not a subtype of this maps 'from' type '{1}'.",
                    polymorphProjectionType.FromType.Name, ProjectionType.FromType.Name));

            Require.IsTrue(ProjectionType.ToType.IsAssignableFrom(polymorphProjectionType.ToType),
                String.Format("Cannot be polymorphic for a Projection whose 'to' type '{0}' is not a subtype of this maps 'to' type '{1}'.",
                    polymorphProjectionType.ToType.Name, ProjectionType.ToType.Name));

            Require.IsFalse(polymorphs.Any(pt => pt.ProjectionType.FromType == polymorphProjectionType.FromType),
                String.Format(
                    "Illegal polymorph defintion. A definition has already been registered for the 'from' type '{0}' and would be made ambiguous by this one.",
                    polymorphProjectionType.FromType.Name));

            polymorphs.Add(new LateBoundProjection(polymorphProjectionType, Mapper));
        }
    }
}