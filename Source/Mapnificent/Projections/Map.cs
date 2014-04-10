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
// </copyright>

using System;

namespace KodeKandy.Mapnificent.Projections
{
    public abstract class Map : Projection, IMap
    {
        private Func<ConstructionContext, object> constructUsing;

        protected Map(ProjectionType projectionType, Mapper mapper)
            : base(projectionType, mapper)
        {
            // We can only create a default constructor if the toType is concrete.
            if (!projectionType.ToType.IsInterface)
                ConstructUsing = _ => Activator.CreateInstance(ProjectionType.ToType);
        }

        #region IMap Members

        public Func<ConstructionContext, object> ConstructUsing
        {
            get { return constructUsing; }
            set
            {
                Require.NotNull(value, "value");
                constructUsing = value;
            }
        }

        /// <summary>
        ///     Action applied to the mapping target after mapping has been performed.
        /// </summary>
        public Action<object, object> PostMapStep { get; set; }

        #endregion
    }
}