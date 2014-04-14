// <copyright file="Conversion.cs" company="million miles per hour ltd">
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
    ///     Conversions encompass all projections into a value type.
    /// n.b. This is still relevent when projecting into classes when you want to treat them as value types.
    /// </summary>
    public class Conversion : Projection
    {
        private Func<object, object> conversionFunc;

        public Conversion(ProjectionType projectionType, Mapper mapper)
            : base(mapper)
        {
            this.projectionType = projectionType;
        }

        private readonly ProjectionType projectionType;
        public override ProjectionType ProjectionType
        {
            get { return projectionType; }
        }

        public Func<object, object> ConversionFunc
        {
            get { return conversionFunc; }
            set
            {
                Require.NotNull(value, "value");
                conversionFunc = value;
            }
        }

        public override string ToString()
        {
            return String.Format("ConvertUsing: {0}", ProjectionType);
        }

        public override object Apply(object from, object to = null, bool mapInto = false)
        {
            return conversionFunc(from);
        }
    }
}