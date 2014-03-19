// <copyright file="MemberBindingDefinition.cs" company="million miles per hour ltd">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace KodeKandy.Mapnificent.Definitions
{
    public enum MemberBindingDefinitionType
    {
        /// <summary>
        ///     Binding explicitly defined by config.
        /// </summary>
        Explicit,
        /// <summary>
        ///     Binding automatically inferred by the library.
        /// </summary>
        Auto
    }

    /// <summary>
    ///     Defines the mapping for a member in a 'to' class from a 'from' class.
    /// </summary>
    //    public class MemberBindingDefinition
    //    {
    //        /// <summary>
    //        ///     Captures whether the binding was explicitly defined in config or automatically inferred.
    //        /// </summary>
    //        public MemberBindingDefinitionType MemberBindingDefinitionType { get; private set; }
    //
    //        /// <summary>
    //        ///     Defines the 'to' member setter details.
    //        /// </summary>
    //        public MemberSetterDefinition MemberSetterDefinition { get; private set; }
    //
    //        /// <summary>
    //        ///     Defines the 'from' member getter details.
    //        /// </summary>
    //        public MemberGetterDefinition MemberGetterDefinition { get; set; }
    //
    //        /// <summary>
    //        ///     Conversion used to map between the 'from' member to the 'to' member.
    //        /// </summary>
    //        public ConversionDefinition ConversionDefinition { get; set; }
    //
    //        private bool ignore;
    //        public bool Ignore
    //        {
    //            get { return ignore; }
    //            set
    //            {
    //                ignore = value;
    //                if (ignore)
    //                {
    //                    MemberSetterDefinition = null;
    //                    ConversionDefinition = null;
    //                }
    //            }
    //        }
    //
    //        private MemberBindingDefinition(MemberSetterDefinition memberSetterDefinition, MemberBindingDefinitionType memberBindingDefinitionType,
    //            MemberGetterDefinition memberGetterDefinition = null, ConversionDefinition conversionDefinition = null)
    //        {
    //            Require.NotNull(memberSetterDefinition, "memberSetterDefinition");
    //
    //            MemberBindingDefinitionType = memberBindingDefinitionType;
    //            MemberSetterDefinition = memberSetterDefinition;
    //            MemberGetterDefinition = memberGetterDefinition;
    //            ConversionDefinition = conversionDefinition;
    //        }
    //
    //        public static MemberBindingDefinition Create(MemberInfo toMemberInfo, MemberBindingDefinitionType memberBindingDefinitionType, MemberGetterDefinition memberGetterDefinition = null, ConversionDefinition conversionDefinition = null)
    //        {
    //            return new MemberBindingDefinition(new MemberSetterDefinition(toMemberInfo), memberBindingDefinitionType, memberGetterDefinition, conversionDefinition);
    //        }
    //
    //        public ReadOnlyCollection<MemberDefinitionError> Validate(MapperSchema mapperSchema)
    //        {
    //            var memberDefinitionErrors = new List<MemberDefinitionError>();
    //
    //            if (Ignore)
    //                return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
    //
    //            if (MemberGetterDefinition == null)
    //                memberDefinitionErrors.Add(MemberDefinitionError.Create(MemberSetterDefinition, "Binding definition does not define 'From' binding."));
    //
    //            if (MemberGetterDefinition != null && ConversionDefinition == null)
    //            {
    //                var fromMemberType = MemberGetterDefinition.MemberType;
    //                var toMemberType = MemberSetterDefinition.MemberType;
    //
    //                if (!mapperSchema.HasMapOrConversion(fromMemberType, toMemberType))
    //                {
    //                    memberDefinitionErrors.Add(
    //                        MemberDefinitionError.Create(MemberSetterDefinition, "Mapped from '{0}' but no {1} defined between '{2}'->'{3}'.",
    //                            MemberGetterDefinition.MemberName,
    //                            toMemberType.IsClass ? "map" : "conversion",
    //                            fromMemberType.Name, toMemberType.Name));
    //                }
    //            }
    //
    //            if (MemberGetterDefinition != null && ConversionDefinition != null)
    //            {
    //                // TODO validate conversion or make it so instantiation implies validity.
    //
    //                if (MemberSetterDefinition.MemberType != ConversionDefinition.MappingType.ToType)
    //                    memberDefinitionErrors.Add(MemberDefinitionError.Create(MemberSetterDefinition,
    //                        "To member type {0} does not match the defined conversion output type ({1})", MemberGetterDefinition.MemberType,
    //                        ConversionDefinition));
    //
    //                if (MemberGetterDefinition.MemberType != ConversionDefinition.MappingType.FromType)
    //                    memberDefinitionErrors.Add(MemberDefinitionError.Create(MemberSetterDefinition,
    //                        "From member type {0} does not match the defined conversion input type ({1})", MemberGetterDefinition.MemberType,
    //                        ConversionDefinition));
    //            }
    //
    //            return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
    //        }
    //
    //        // TODO returns a mapping delegate to go in the map.
    //        public void Apply(object fromDeclaring, object toDeclaring)
    //        {
    //            Require.NotNull(fromDeclaring, "fromDeclaring");
    //            Require.NotNull(toDeclaring, "toDeclaring");
    //
    //            object value;
    //            var hasValue = MemberGetterDefinition.MemberGetter(fromDeclaring, out value);
    //            if (hasValue)
    //            {
    //                if (ConversionDefinition != null)
    //                    value = ConversionDefinition.ConversionFunc(value);
    //                MemberSetterDefinition.MemberSetter(toDeclaring, value);
    //            }
    //        }
    //    }

    /// <summary>
    ///     Immutable object defining the mapping for a member in a 'to' class from a 'from' class.
    /// </summary>
    public class MemberBindingDefinition
    {
        /// <summary>
        ///     Captures whether the binding was explicitly defined in config or automatically inferred.
        /// </summary>
        public MemberBindingDefinitionType MemberBindingDefinitionType { get; private set; }

        /// <summary>
        ///     Defines the 'to' member setter details.
        /// </summary>
        public MemberSetterDefinition MemberSetterDefinition { get; private set; }

        /// <summary>
        ///     Defines the 'from' member getter details.
        /// </summary>
        public MemberGetterDefinition MemberGetterDefinition { get; private set; }

        /// <summary>
        ///     Conversion used to map between the 'from' member to the 'to' member.
        /// </summary>
        public ConversionDefinition ConversionDefinition { get; private set; }

        /// <summary>
        /// Defines that this 'to' member should be ignored.
        /// </summary>
        public bool Ignore { get; private set; }

        private MemberBindingDefinition(MemberSetterDefinition memberSetterDefinition, MemberBindingDefinitionType memberBindingDefinitionType,
            MemberGetterDefinition memberGetterDefinition = null, ConversionDefinition conversionDefinition = null)
        {
            Require.NotNull(memberSetterDefinition, "memberSetterDefinition");

            MemberBindingDefinitionType = memberBindingDefinitionType;
            MemberSetterDefinition = memberSetterDefinition;
            MemberGetterDefinition = memberGetterDefinition;
            ConversionDefinition = conversionDefinition;
        }

        public static MemberBindingDefinition Create(MemberInfo toMemberInfo, MemberBindingDefinitionType memberBindingDefinitionType, MemberGetterDefinition memberGetterDefinition = null, ConversionDefinition conversionDefinition = null)
        {
            return new MemberBindingDefinition(new MemberSetterDefinition(toMemberInfo), memberBindingDefinitionType, memberGetterDefinition, conversionDefinition);
        }

        public MemberBindingDefinition WithIgnore()
        {
            var newDefinition = new MemberBindingDefinition(MemberSetterDefinition, MemberBindingDefinitionType, null, null) { Ignore = true };
            return newDefinition;
        }

        public MemberBindingDefinition WithConversionDefinition(ConversionDefinition conversionDefinition)
        {
            return new MemberBindingDefinition(MemberSetterDefinition, MemberBindingDefinitionType, MemberGetterDefinition, conversionDefinition);
        }

        public MemberBindingDefinition WithMemberGetterDefinition(MemberGetterDefinition memberGetterDefinition)
        {
            return new MemberBindingDefinition(MemberSetterDefinition, MemberBindingDefinitionType, memberGetterDefinition, ConversionDefinition);
        }

        internal ReadOnlyCollection<MemberDefinitionError> Validate(MapperSchema mapperSchema)
        {
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            if (Ignore)
                return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);

            if (MemberGetterDefinition == null)
                memberDefinitionErrors.Add(MemberDefinitionError.Create(MemberSetterDefinition, "Binding definition does not define 'From' binding."));

            if (MemberGetterDefinition != null && ConversionDefinition == null)
            {
                var fromMemberType = MemberGetterDefinition.MemberType;
                var toMemberType = MemberSetterDefinition.MemberType;

                if (!mapperSchema.HasMapOrConversion(fromMemberType, toMemberType))
                {
                    memberDefinitionErrors.Add(
                        MemberDefinitionError.Create(MemberSetterDefinition, "Mapped from '{0}' but no {1} defined between '{2}'->'{3}'.",
                            MemberGetterDefinition.MemberName,
                            toMemberType.IsClass ? "map" : "conversion",
                            fromMemberType.Name, toMemberType.Name));
                }
            }

            if (MemberGetterDefinition != null && ConversionDefinition != null)
            {
                // TODO validate conversion or make it so instantiation implies validity.

                if (MemberSetterDefinition.MemberType != ConversionDefinition.MappingType.ToType)
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(MemberSetterDefinition,
                        "To member type {0} does not match the defined conversion output type ({1})", MemberGetterDefinition.MemberType,
                        ConversionDefinition));

                if (MemberGetterDefinition.MemberType != ConversionDefinition.MappingType.FromType)
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(MemberSetterDefinition,
                        "From member type {0} does not match the defined conversion input type ({1})", MemberGetterDefinition.MemberType,
                        ConversionDefinition));
            }

            return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
        }
    }

    public static class MemberBindingDefinitionValidator
    {
        public static ReadOnlyCollection<MemberDefinitionError> Validate(MemberBindingDefinition definition, MapperSchema mapperSchema)
        {
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            if (definition.Ignore)
                return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);

            if (definition.MemberGetterDefinition == null)
                memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.MemberSetterDefinition, "Binding definition does not define 'From' binding."));

            if (definition.MemberGetterDefinition != null && definition.ConversionDefinition == null)
            {
                var fromMemberType = definition.MemberGetterDefinition.MemberType;
                var toMemberType = definition.MemberSetterDefinition.MemberType;

                if (!mapperSchema.HasMapOrConversion(fromMemberType, toMemberType))
                {
                    memberDefinitionErrors.Add(
                        MemberDefinitionError.Create(definition.MemberSetterDefinition, "Mapped from '{0}' but no {1} defined between '{2}'->'{3}'.",
                            definition.MemberGetterDefinition.MemberName,
                            toMemberType.IsClass ? "map" : "conversion",
                            fromMemberType.Name, toMemberType.Name));
                }
            }

            if (definition.MemberGetterDefinition != null && definition.ConversionDefinition != null)
            {
                // TODO validate conversion or make it so instantiation implies validity.

                if (definition.MemberSetterDefinition.MemberType != definition.ConversionDefinition.MappingType.ToType)
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.MemberSetterDefinition,
                        "To member type {0} does not match the defined conversion output type ({1})", definition.MemberGetterDefinition.MemberType,
                        definition.ConversionDefinition));

                if (definition.MemberGetterDefinition.MemberType != definition.ConversionDefinition.MappingType.FromType)
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.MemberSetterDefinition,
                        "From member type {0} does not match the defined conversion input type ({1})", definition.MemberGetterDefinition.MemberType,
                        definition.ConversionDefinition));
            }

            return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
        }
    }
}