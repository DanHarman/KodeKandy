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
// </copyright>

using System;
using System.Collections.Generic;
using KodeKandy.Mapnificent.Projections;

namespace KodeKandy.Mapnificent
{
    /// <summary>
    ///     A mapper that can copy values between two entities based on defined and implied relationships.
    ///     - A projection from a class to a class (i.e 'to' type is a reference type) is called a 'ClassMap'.
    ///     - A projection that polymorphically redirects reference type maps depending on the 'from' type is called a
    ///     'PolymorphicMap'.
    ///     - A projection from an Enumerable to a List is called a 'ListMap'.
    ///     - A projection from a class to a value type is called a 'conversion'.
    /// </summary>
    public class Mapper
    {
        private readonly Dictionary<ProjectionType, IProjection> projections = new Dictionary<ProjectionType, IProjection>();

        public Mapper()
        {
            // Default ClassMap for string which just copies the instance by using a custom 'ConstructUsing' which simply returns the from instance.
            BuildClassMap<string, string>().ConstructUsing(ctx => (string) ctx.FromInstance);
        }

        public ClassMapBuilder<TFrom, TTo> BuildClassMap<TFrom, TTo>(bool register = true)
            where TTo : class
        {
            var projectionType = new ProjectionType(typeof(TFrom), typeof(TTo));

            if (projectionType.IsListProjection)
                throw new Exception(string.Format("Error, {0} is a list projection, not a class projection", projectionType));

            var definition = new ClassMap(projectionType, this);

            if (register)
                projections[projectionType] = definition;

            return new ClassMapBuilder<TFrom, TTo>(definition);
        }

        public PolymorphicMapBuilder<TFrom, TTo> BuildPolymorphicMap<TFrom, TTo>(bool register = true)
            where TTo : class
        {
            var projectionType = new ProjectionType(typeof(TFrom), typeof(TTo));

            var definition = new PolymorphicMap(projectionType, this);

            if (register)
                projections[projectionType] = definition;

            return new PolymorphicMapBuilder<TFrom, TTo>(definition);
        }

        public ClassMapBuilder<TFrom, TTo> BuildListMap<TFrom, TTo>(bool register = true)
            where TFrom : class
            where TTo : class
        {
            var projectionType = new ProjectionType(typeof(TFrom), typeof(TTo));

            if (!projectionType.IsListProjection)
            {
                throw new Exception(string.Format("{0} is not a list projection.", projectionType));
            }

            var definition = new ListMap(projectionType, this);

            if (register)
                projections[projectionType] = definition;

            return null;

            //return new ClassMapBuilder<TFrom, TTo>(definition);
        }

        public ConversionBuilder<TFrom, TTo> BuildConversion<TFrom, TTo>(bool register = true)
            where TTo : struct
        {
            var projectionType = new ProjectionType(typeof(TFrom), typeof(TTo));
            var definition = new Conversion(projectionType, this);

            if (register)
                projections[projectionType] = definition;

            return new ConversionBuilder<TFrom, TTo>(definition);
        }

        public bool HasMap(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");
            Require.IsTrue(toType.IsClass, "toType must be a class.");

            return projections.ContainsKey(new ProjectionType(fromType, toType));
        }

        public bool HasConversion(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");
            Require.IsTrue(toType.IsValueType, "toType must be a value type.");

            // If we decide to allow automatic type conversion look at:
            // Convert.ChangeType
            // & System.ComponentModel.TypeDescriptor.GetConverter(typeof(int))

            return projections.ContainsKey(new ProjectionType(fromType, toType));
        }

        public bool HasProjection(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");

            if (toType.IsClass)
                return HasMap(fromType, toType);
            else
                return HasConversion(fromType, toType);
        }

        public IProjection GetProjection(ProjectionType projectionType)
        {
            IProjection projection;

            if (projections.TryGetValue(projectionType, out projection))
                return projection;

            // Try to get a default projection
            // TODO: For cloning we need to support default map for identity class projections.
            if (projectionType.IsIdentity && !projectionType.IsClassProjection)
            {
                projection = new Conversion(projectionType, this) {ConversionFunc = x => x};
                projections.Add(projectionType, projection);
            }
            else if (projectionType.IsListProjection)
            {
                projection = new ListMap(projectionType, this);
                projections.Add(projectionType, projection);
            }
            else
            {
                var msg = string.Format("Unable to get projection of type {0}, as none defined.", projectionType);
                throw new MapnificentException(msg, this);
            }
            return projection;
        }

        public IProjection GetProjection(Type fromType, Type toType)
        {
            return GetProjection(new ProjectionType(fromType, toType));
        }

        public ClassMap GetClassMap(ProjectionType projectionType)
        {
            if (!projectionType.IsClassProjection)
            {
                var msg = string.Format("Unable to get ClassMap for projection {0}, as it is not a class projection.", projectionType);
                throw new MapnificentException(msg, this);
            }
            return (ClassMap) GetProjection(projectionType);
        }

        /// <summary>
        ///     Gets a list map for the specified <see cref="ProjectionType" />.
        ///     Unlike with <see cref="ClassMap" />s or <see cref="Conversion" />s, these are autogenerated if they have not been
        ///     manually defined.
        /// </summary>
        /// <param name="projectionType"></param>
        /// <returns></returns>
        public ListMap GetListMap(ProjectionType projectionType)
        {
            if (!projectionType.IsListProjection)
            {
                var msg = string.Format("Unable to get ListMap for projection {0}, as it is not a list projection.", projectionType);
                throw new MapnificentException(msg, this);
            }
            return (ListMap) GetProjection(projectionType);
        }

        public ClassMap GetClassMap(Type fromType, Type toType)
        {
            return GetClassMap(new ProjectionType(fromType, toType));
        }

        public ListMap GetListMap(Type fromType, Type toType)
        {
            return GetListMap(new ProjectionType(fromType, toType));
        }

        public Conversion GetConversion(ProjectionType projectionType)
        {
            IProjection conversion;
            if (!projections.TryGetValue(projectionType, out conversion))
            {
                var msg = string.Format("Unable to get converesion of type {0}, as no conversion defined.", projectionType);
                throw new MapnificentException(msg, this);
            }
            return (Conversion) conversion;
        }

        public void MapInto(object from, object to)
        {
            try
            {
                Require.NotNull(from, "from");
                Require.NotNull(to, "to");

                var map = GetProjection(from.GetType(), to.GetType());
                map.Apply(from, to, true);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error mapping into between objects of type '{0}' -> '{1}'", from.SafeGetTypeName(), to.SafeGetTypeName());
                throw new MapnificentException(msg, ex, this);
            }
        }

        public TTo Map<TFrom, TTo>(TFrom from)
            where TFrom : class
            where TTo : class
        {
            try
            {
                Require.NotNull(from, "from");

                var map = GetProjection(typeof(TFrom), typeof(TTo));
                return (TTo) map.Apply(from);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error mapping between objects of type '{0}' -> '{1}'", typeof(TFrom).Name, typeof(TTo).Name);
                throw new MapnificentException(msg, ex, this);
            }
        }

        public object Map(object from, Type toType)
        {
            try
            {
                Require.NotNull(from, "from");
                Require.IsTrue(toType.IsClass);

                var map = GetProjection(from.GetType(), toType);
                return map.Apply(from);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error mapping between objects of type '{0}' -> '{1}'", from.GetType().Name, toType.Name);
                throw new MapnificentException(msg, ex, this);
            }
        }

        public TTo Map<TTo>(object from)
            where TTo : class
        {
            return (TTo) Map(from, typeof(TTo));
        }

        // TODO Add Import(Mapper mapperSchema) method.
    }
}