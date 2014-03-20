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
// 
// </copyright>

using System;

namespace KodeKandy.Mapnificent
{
    /// <summary>
    ///     Conversion definitions encompass all mappings into a value type.
    /// </summary>
    public class Conversion
    {
        public ProjectionType ProjectionType { get; private set; }

        private Func<object, object> conversionFunc;
        public Func<object, object> ConversionFunc
        {
            get { return conversionFunc; }
            set
            {
                Require.NotNull(value, "value");
                conversionFunc = value;
            }
        }

        public Conversion(ProjectionType projectionType)
        {
            Require.NotNull(projectionType);
            Require.IsTrue(projectionType.ToType.IsClass);

            ProjectionType = projectionType;
        }

        public override string ToString()
        {
            return String.Format("Conversion: {0}", ProjectionType);
        }

        public object Apply(object fromValue)
        {
            return conversionFunc(fromValue);
        }
    }
}