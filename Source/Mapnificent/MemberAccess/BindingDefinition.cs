// <copyright file="BindingDefinition.cs" company="million miles per hour ltd">
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
using KodeKandy.Mapnificent.Projections;

namespace KodeKandy.Mapnificent.MemberAccess
{
    public enum BindingDefinitionType
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
    public class BindingDefinition
    {
        /// <summary>
        ///     Captures whether the binding was explicitly defined in config or automatically inferred.
        /// </summary>
        public BindingDefinitionType BindingDefinitionType { get; private set; }

        /// <summary>
        ///     Defines the 'to' member setter details.
        /// </summary>
        public ToDefinition ToDefinition { get; private set; }

        public Type ToType
        {
            get { return ToDefinition.MemberType; }
        }

        /// <summary>
        ///     Defines the 'from' provider when it is a member on the 'from' class.
        /// </summary>
        public FromDefinition FromDefinition { get; set; }

        public Type FromType
        {
            get { return FromDefinition.MemberType; }
        }

        public bool HasCustomFromDefintion
        {
            get { return FromDefinition is FromCustomDefinition; }
        }

        /// <summary>
        ///     ConversionOverride used to map between the 'from' member to the 'to' member.
        ///     This is an override as by default the Mapper is queried for conversions.
        /// </summary>
        public Conversion ConversionOverride { get; set; }

        /// <summary>
        ///     Indicates if the binding is map based, or conversely Conversion based.
        /// </summary>
        public bool IsMap
        {
            get { return ProjectionType.IsMap; }
        }

        /// <summary>
        ///     A projection tye reflecting the type of the 'from' and 'to' members.
        /// </summary>
        public ProjectionType ProjectionType
        {
            get { return new ProjectionType(FromType, ToType); }
        }

        private bool isIgnore;
        public bool IsIgnore
        {
            get { return isIgnore; }
            set
            {
                isIgnore = value;
                if (isIgnore)
                {
                    FromDefinition = null;
                    ConversionOverride = null;
                }
            }
        }

        public BindingDefinition(MemberInfo toMemberInfo, BindingDefinitionType bindingDefinitionType,
            FromDefinition fromDefinition = null, Conversion conversionOverride = null)
        {
            Require.NotNull(toMemberInfo, "toMemberInfo");

            BindingDefinitionType = bindingDefinitionType;
            ToDefinition = new ToDefinition(toMemberInfo);
            FromDefinition = fromDefinition ?? FromUndefinedDefinition.Default;
            ConversionOverride = conversionOverride;
        }

        /// <summary>
        ///     The binding may require a projection (i.e. a mapping or conversion) between the 'from' and 'to' types.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="fromValue"></param>
        /// <param name="toDeclaringInstance"></param>
        /// <param name="mapInto"></param>
        /// <returns></returns>
        public object ProjectValue(Mapper mapper, object fromValue, object toDeclaringInstance, bool mapInto)
        {
            object toValue;

            if (HasCustomFromDefintion)
            {
                // Custom 'from' methods must return the correct 'to' type so no need for conversion or mapping.
                toValue = fromValue;
            }
            else if (IsMap)
            {
                // TODO support Map override here.

                var map = mapper.GetMap(ProjectionType);

                // If we are mapping into then attempt to get a value to map into.
                if (mapInto)
                    toValue = ToDefinition.Accessor.Getter(toDeclaringInstance) ?? map.CreateInstanceOfTo(fromValue);
                else
                    toValue = map.CreateInstanceOfTo(fromValue);

                map.Apply(fromValue, toValue);
            }
            else
            {
                // If this is a value type binding, then order of precedence is:
                // 1) Apply a conversion override if it is defined.
                // 2) If the projection is identity i.e. T -> T then do nothing.
                // 3) Ask the Mapper for a convereter.

                Conversion conversion;

                if (ConversionOverride != null)
                    conversion = ConversionOverride;
                else if (ProjectionType.IsIdentity)
                    conversion = null;
                else
                    conversion = mapper.GetConversion(ProjectionType);

                toValue = conversion != null ? conversion.Apply(fromValue) : fromValue;
            }

            return toValue;
        }

        public override string ToString()
        {
            var bindingDescription = string.Format("'{0}'->'{1}'", FromDefinition, ToDefinition.MemberName);

            return string.Format("Binding: {0}, Type: {1}", bindingDescription, ProjectionType);
        }
    }
}