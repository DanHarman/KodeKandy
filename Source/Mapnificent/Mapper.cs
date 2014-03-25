// <copyright file="Mapper.cs" company="million miles per hour ltd">
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
using System.Collections.ObjectModel;

namespace KodeKandy.Mapnificent
{
    /// <summary>
    ///     A mapper that can copy values between two class based on defined and implied relationships.
    /// A projection from a class to a class is called a 'map'.
    /// A projection from a class to a value type is called a 'conversion'.
    /// </summary>
    public class Mapper
    {
        /// <summary>
        ///     MapInto definitions encompass all mappings into a reference type.
        /// </summary>
        private readonly Dictionary<ProjectionType, Map> mapDefinitions = new Dictionary<ProjectionType, Map>();

        /// <summary>
        ///     ConversionOverride definitions encompass all mappings into a value type.
        /// </summary>
        private readonly Dictionary<ProjectionType, Conversion> conversionDefinitions = new Dictionary<ProjectionType, Conversion>();

        public Mapper()
        {
            // Default map for string which just copies the instance by using a custom 'ConstructedBy' which simply returns the from instance.
            DefineMap<string, string>().ConstructedBy(ctx => (string) ctx.FromInstance);
        }

        public MapDefinitionBuilder<TFrom, TTo> DefineMap<TFrom, TTo>()
            where TTo : class
        {
            var mappingType = new ProjectionType(typeof(TFrom), typeof(TTo));
            Map definition;

            if (!mapDefinitions.TryGetValue(mappingType, out definition))
            {
                definition = new Map(mappingType, this);
                mapDefinitions.Add(mappingType, definition);
            }

            return new MapDefinitionBuilder<TFrom, TTo>(definition);
        }

        public ConversionDefinitionBuilder<TFrom, TTo> DefineConversion<TFrom, TTo>()
            where TTo : struct
        {
            var projectionType = new ProjectionType(typeof(TFrom), typeof(TTo));
            Conversion definition;

            if (!conversionDefinitions.TryGetValue(projectionType, out definition))
            {
                definition = new Conversion(projectionType);
                conversionDefinitions.Add(projectionType, definition);
            }

            return new ConversionDefinitionBuilder<TFrom, TTo>(definition);
        }

        public bool HasMap(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");
            Require.IsTrue(toType.IsClass, "toType must be a class.");

            return mapDefinitions.ContainsKey(new ProjectionType(fromType, toType));
        }

        public bool HasConversion(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");
            Require.IsTrue(toType.IsValueType, "toType must be a value type.");

            // If we decide to allow automatic type conversion look at:
            // Convert.ChangeType
            // & System.ComponentModel.TypeDescriptor.GetConverter(typeof(int))

            return conversionDefinitions.ContainsKey(new ProjectionType(fromType, toType));
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

        public Map GetMap(ProjectionType projectionType)
        {
            Map map;
            if (!mapDefinitions.TryGetValue(projectionType, out map))
            {
                var msg = string.Format("Unable to get map of type {0}, as no map defined.", projectionType);
                throw new MapnificentException(msg);
            }
            return map;
        }

        public Map GetMap(Type fromType, Type toType)
        {
            return GetMap(new ProjectionType(fromType, toType));
        }

        public Conversion GetConversion(ProjectionType projectionType)
        {
            Conversion conversion;
            if (!conversionDefinitions.TryGetValue(projectionType, out conversion))
            {
                var msg = string.Format("Unable to get converesion of type {0}, as no conversion defined.", projectionType);
                throw new MapnificentException(msg);
            }
            return conversion;
        }

        public void MapInto(object from, object to)
        {
            try
            {
                Require.NotNull(from, "from");
                Require.NotNull(to, "to");

                var map = GetMap(from.GetType(), to.GetType());
                map.Apply(from, to);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error mapping between object of type '{0}' -> '{1}'", from.SafeGetTypeName(), to.SafeGetTypeName());
                throw new MapnificentException(msg, ex);
            }
        }

        public void AddMap(Map map)
        {
            Require.NotNull(map, "map");

            mapDefinitions.Add(map.ProjectionType, map);
        }

        // TODO Add Import(Mapper mapperSchema) method.
    }
}