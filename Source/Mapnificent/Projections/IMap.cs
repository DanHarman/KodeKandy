// <copyright file="IMap.cs" company="million miles per hour ltd">
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
    public interface IMap : IProjection
    {
        /// <summary>
        ///     Factory method to create an instance of the 'to' type. Defaults to <c>Activator.CreateInstance()</c> when the
        ///     toType is concrete, otherwise defaults to null.
        /// </summary>
        Func<ConstructionContext, object> ConstructUsing { get; set; }

        /// <summary>
        ///     Action applied to the mapping target after mapping has been performed.
        /// </summary>
        Action<object, object> PostMapStep { get; set; }
        ProjectionType ProjectionType { get; }

        /// <summary>
        ///     The Mapper this ClassMap is associated with.
        /// </summary>
        Mapper Mapper { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapInto">If true, attemps to ClassMap into an existing object graph rather than recreating all children.</param>
        object Apply(object from, object to = null, bool mapInto = false);
    }
}