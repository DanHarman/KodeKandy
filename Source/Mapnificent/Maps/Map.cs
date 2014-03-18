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
// 
// </copyright>

using System;
using System.Collections.ObjectModel;
using KodeKandy.Mapnificent.Definitions;

namespace KodeKandy.Mapnificent.Maps
{
    /// <summary>
    /// The map class allows the mapping between two defined classes.
    /// Unlike a <see cref="MapDefinition"/> a map is immutable.
    /// </summary>
    public class Map
    {
        private readonly Mapper mapper;
        public ReadOnlyCollection<MemberBindingDefinition> Bindings { get; private set; }
        public MappingType MappingType { get; private set; }

        public Map(MapDefinition mapDefinition, Mapper mapper)
        {
            // We need the mapper so that when resolving this maps dependencies (i.e. other maps)
            // we can get hold of them.
            Require.NotNull(mapDefinition, "mapDefinition");
            Require.NotNull(mapper, "mapper");

            // Copy the binding definitions as they currenlty stand. Since they are immutable objets, if they are later
            // mutated this map will not be affected.
            Bindings = mapDefinition.Bindings;

            MappingType = mapDefinition.MappingType;

            this.mapper = mapper;
        }

        public void Apply(object from, object to)
        {
            Require.NotNull(from, "from");
            Require.NotNull(to, "to");

            foreach (var binding in Bindings)
            {
                ApplyBinding(from, to, binding);
            }
        }

        /// <summary>
        /// Applies a map operation between bound properties on the 'from' and 'to' class.
        /// </summary>
        /// <param name="fromDeclaring">An instance of the from class.</param>
        /// <param name="toDeclaring">An instance of the to class.</param>
        /// <param name="binding"></param>
        private void ApplyBinding(object fromDeclaring, object toDeclaring, MemberBindingDefinition binding)
        {
            Require.NotNull(fromDeclaring, "fromDeclaring");
            Require.NotNull(toDeclaring, "toDeclaring");

            object value;
            var hasValue = binding.MemberGetterDefinition.MemberGetter(fromDeclaring, out value);
            if (hasValue)
            {
                if (binding.ConversionDefinition != null)
                    value = binding.ConversionDefinition.ConversionFunc(value);
                binding.MemberSetterDefinition.MemberSetter(toDeclaring, value);
            }
        }
    }
}