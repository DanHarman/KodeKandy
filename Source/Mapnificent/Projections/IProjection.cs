// <copyright file="IProjection.cs" company="million miles per hour ltd">
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
    public interface IProjection
    {
        ProjectionType ProjectionType { get; }


        /// <summary>
        ///     Applies a projection.
        /// </summary>
        /// <param name="from">The 'from' class to project from.</param>
        /// <param name="to">
        ///     The 'to' class to project into, this is only set when mapping into an existing hierarchy, or a ConstructUsing
        ///     override has been set at the
        ///     the parent level.
        /// </param>
        /// <param name="mapInto">If true, attemps to ClassMap into an existing object graph rather than recreating all children.</param>
        /// <returns>
        ///     The result of the projection, which may be the 'to' value if that was mapped into, otherwise it is a new
        ///     value.
        /// </returns>
        object Apply(object from, object to = null, bool mapInto = false);
    }
}