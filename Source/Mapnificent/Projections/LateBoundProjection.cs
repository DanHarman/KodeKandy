// <copyright file="LateBoundProjection.cs" company="million miles per hour ltd">
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
    /// <summary>
    ///     A late bound projection that is resolved against the mapper when it is needed.
    /// </summary>
    public class LateBoundProjection : Projection
    {
        private readonly Lazy<IProjection> projection;
        private readonly ProjectionType projectionType;

        public LateBoundProjection(ProjectionType projectionType, Mapper mapper) : base(mapper)
        {
            this.projectionType = projectionType;
            projection = new Lazy<IProjection>(() => Mapper.GetProjection(ProjectionType));
        }

        public override ProjectionType ProjectionType
        {
            get { return projectionType; }
        }

        public override object Apply(object from, object to = null, bool mapInto = false)
        {
            try
            {
                return projection.Value.Apply(from, to, mapInto);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error applying LateBoundProjection of type {0}", ProjectionType);
                throw new MapnificentException(msg, ex, Mapper);
            }
        }
    }
}