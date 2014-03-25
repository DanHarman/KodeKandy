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

using System;
using System.Reflection;
using System.ServiceModel.Channels;
using KodeKandy.Mapnificent.Bindngs;

namespace KodeKandy.Mapnificent
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
    public class MemberBindingDefinition
    {
        /// <summary>
        ///     Captures whether the binding was explicitly defined in config or automatically inferred.
        /// </summary>
        public MemberBindingDefinitionType MemberBindingDefinitionType { get; private set; }

        /// <summary>
        ///     Defines the 'to' member setter details.
        /// </summary>
        public MemberSetterDefinition ToMemberDefinition { get; private set; }

        private MemberGetterDefinition fromMemberDefinition;
        /// <summary>
        ///     Defines the 'from' provider when it is a member on the 'from' class.
        /// </summary>
        public MemberGetterDefinition FromMemberDefinition
        {
            get { return fromMemberDefinition; }
            set
            {
                customFromDefinition = null;
                fromMemberDefinition = value;
            }
        }

        private Func<MappingContext, object> customFromDefinition;
        /// <summary>
        /// Defines that the 'from' value comes from a custom delegate rather than to a member on the 'from' type.
        /// </summary>
        public Func<MappingContext, object> CustomFromDefinition
        {
            get { return customFromDefinition; }
            set
            {
                fromMemberDefinition = null;
                customFromDefinition = value;
            }
        }

        public bool HasCustomFromDefintion
        {
            get { return CustomFromDefinition != null; }
        }

        /// <summary>
        ///     ConversionOverride used to map between the 'from' member to the 'to' member.
        /// This is an override as by default the Mapper is queried for conversions.
        /// </summary>
        public Conversion ConversionOverride { get; set; }

        /// <summary>
        /// Indicates if the binding is map based, or conversely Conversion based.
        /// </summary>
        public bool IsMap
        {
            get { return ProjectionType.IsMap; }
        }

        /// <summary>
        /// A projection tye reflecting the type of the 'from' and 'to' members.
        /// </summary>
        public ProjectionType ProjectionType
        {
            get
            {
                return new ProjectionType(FromType, ToType);
            }
        }

       // public Func<> 

        private bool ignore;
        public bool Ignore
        {
            get { return ignore; }
            set
            {
                ignore = value;
                if (ignore)
                {
                    FromMemberDefinition = null;
                    ConversionOverride = null;
                }
            }
        }

        private MemberBindingDefinition(MemberSetterDefinition toMemberDefinition, MemberBindingDefinitionType memberBindingDefinitionType,
            MemberGetterDefinition fromMemberDefinition = null, Conversion conversionOverride = null)
        {
            Require.NotNull(toMemberDefinition, "ToMemberDefinition");

            MemberBindingDefinitionType = memberBindingDefinitionType;
            ToMemberDefinition = toMemberDefinition;
            FromMemberDefinition = fromMemberDefinition;
            ConversionOverride = conversionOverride;
        }

        public static MemberBindingDefinition Create(MemberInfo toMemberInfo, MemberBindingDefinitionType memberBindingDefinitionType, MemberGetterDefinition memberGetterDefinition = null, Conversion conversion = null)
        {
            Require.NotNull(toMemberInfo, "toMemberInfo");

            return new MemberBindingDefinition(new MemberSetterDefinition(toMemberInfo), memberBindingDefinitionType, memberGetterDefinition, conversion);
        }

        /// <summary>
        /// Indicates if this binding is reliant on a map or ConversionOverride in able to project the type of the 'from' value to the type of the 'to' value.
        /// </summary>
        public bool RequiresMapOrConversion
        {
            get
            {
                if (FromMemberDefinition == null || ConversionOverride != null)
                    return false;

                var fromMemberType = FromMemberDefinition.MemberType;
                var toMemberType = ToMemberDefinition.MemberType;

                return fromMemberType != toMemberType;
            }
        }

        /// <summary>
        /// Attempts to get a member value from a 'from' instance.
        /// </summary>
        /// <param name="fromDeclaringInstance">The class instance from which the member should be fetched.</param>
        /// <param name="mapper">The current mapper (required to provide context to custom From definitions).</param>
        /// <param name="fromValue">The out value set if anything is a value can be retrieved.</param>
        /// <returns>True if a value was retrieved, otherwise false.</returns>
        public bool TryGetFromValue(object fromDeclaringInstance, Mapper mapper, out object fromValue)
        {
            // TODO if we add support for Default values, then this would be the place to provide it.

            bool hasValue;
            // 1. Get the 'from' value.
            if (HasCustomFromDefintion)
            {
                fromValue = CustomFromDefinition(new MappingContext(mapper, fromDeclaringInstance));
                hasValue = true;
            }
            else
            {
                hasValue = FromMemberDefinition.MemberGetter(fromDeclaringInstance, out fromValue);
            }
            return hasValue;
        }

        public Type FromType
        {
            get
            {
                if (HasCustomFromDefintion)
                    return ProjectionType.Custom;
                if (FromMemberDefinition == null)
                    return ProjectionType.Undefined;
                return FromMemberDefinition.MemberType;
            }
        }

        public Type ToType
        {
            get { return ToMemberDefinition.MemberType; }
        }

        public override string ToString()
        {
            string bindingDescription;
            if (HasCustomFromDefintion)
                bindingDescription = string.Format("'<Custom>'->'{0}'", ToMemberDefinition.MemberName);
            else if (fromMemberDefinition != null)
            {
                bindingDescription = string.Format("'{0}'->'{1}'", FromMemberDefinition.MemberName, ToMemberDefinition.MemberName);
            }
            else
            {
                bindingDescription = string.Format("'<Undefined>'->'{0}'", ToMemberDefinition.MemberName); 
            }

            return string.Format("Binding: {0}, Type: {1}", bindingDescription, ProjectionType);
        }
    }
}