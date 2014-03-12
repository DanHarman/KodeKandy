// <copyright file="ConversionDefinition.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.Definitions
{
    /// <summary>
    ///     Conversion definitions encompass all mappings into a value type.
    /// </summary>
    public class ConversionDefinition
    {
        public MappingType MappingType { get; private set; }

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

        public ConversionDefinition(MappingType mappingType)
        {
            Require.NotNull(mappingType);
            Require.IsTrue(mappingType.ToType.IsClass);

            MappingType = mappingType;
        }

        public override string ToString()
        {
            return String.Format("Conversion: {0}", MappingType);
        }
    }
}