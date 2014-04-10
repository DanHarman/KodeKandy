// <copyright file="Projection.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.Projections
{
    /// <summary>
    ///     This is the base class for every type of projection and encompasses both value and reference type projections.
    /// </summary>
    public abstract class Projection : IProjection
    {
        protected Projection(ProjectionType projectionType, Mapper mapper)
        {
            Require.NotNull(projectionType, "projectionType");
            Require.NotNull(mapper, "mapper");

            ProjectionType = projectionType;
            Mapper = mapper;
        }


        public ProjectionType ProjectionType { get; private set; }

        /// <summary>
        ///     The Mapper this projection is associated with.
        /// </summary>
        public Mapper Mapper { get; private set; }

        #region IProjection Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapInto">If true, attemps to ClassMap into an existing object graph rather than recreating all children.</param>
        public abstract object Apply(object from, object to = null, bool mapInto = false);

        #endregion
    }
}