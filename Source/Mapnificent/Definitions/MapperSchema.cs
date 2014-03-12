// <copyright file="MapperSchema.cs" company="million miles per hour ltd">
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
using System.Collections.Generic;

namespace KodeKandy.Mapnificent.Definitions
{
    /// <summary>
    ///     Contains a series of map and conversion definitions.
    /// </summary>
    public class MapperSchema
    {
        /// <summary>
        ///     Map definitions encompass all mappings into a reference type.
        /// </summary>
        private readonly Dictionary<MappingType, MapDefinition> mapDefinitions = new Dictionary<MappingType, MapDefinition>();

        /// <summary>
        ///     Conversion definitions encompass all mappings into a value type.
        /// </summary>
        private readonly Dictionary<MappingType, ConversionDefinition> conversionDefinitions = new Dictionary<MappingType, ConversionDefinition>();

        public MapDefinitionBuilder<TFrom, TTo> DefineMap<TFrom, TTo>()
            where TTo : class
        {
            var mappingType = new MappingType(typeof(TFrom), typeof(TTo));
            MapDefinition definition;

            if (!mapDefinitions.TryGetValue(mappingType, out definition))
            {
                definition = new MapDefinition(mappingType);
                mapDefinitions.Add(mappingType, definition);
            }

            return new MapDefinitionBuilder<TFrom, TTo>(definition);
        }

        public void DefineConversion<TFrom, TTo>()
            where TTo : struct
        {
            var mappingType = new MappingType(typeof(TFrom), typeof(TTo));
            ConversionDefinition definition;

            if (!conversionDefinitions.TryGetValue(mappingType, out definition))
            {
                definition = new ConversionDefinition(mappingType);
                conversionDefinitions.Add(mappingType, definition);
            }
        }

        public bool HasMap(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");
            Require.IsTrue(toType.IsClass, "toType must be a class.");

            return mapDefinitions.ContainsKey(new MappingType(fromType, toType));
        }

        public bool HasConversion(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");
            Require.IsTrue(toType.IsValueType, "toType must be a value type.");

            // If we decide to allow automatic type conversion look at:
            // Convert.ChangeType
            // & System.ComponentModel.TypeDescriptor.GetConverter(typeof(int))

            return conversionDefinitions.ContainsKey(new MappingType(fromType, toType));
        }

        public bool HasMapOrConversion(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");

            if (toType.IsClass)
                return HasMap(fromType, toType);
            else
                return HasConversion(fromType, toType);
        }

        // TODO Add Import(MapperSchema mapperSchema) method.
    }
}