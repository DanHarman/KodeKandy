// <copyright file="Binding.cs" company="million miles per hour ltd">
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
using System.Reflection;
using KodeKandy.Mapnificent.Projections;

namespace KodeKandy.Mapnificent.MemberAccess
{
    public enum BindingType
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
    public class Binding
    {
        private bool isIgnore;

        public Binding(MemberInfo toMemberInfo, BindingType bindingType,
            FromDefinition fromDefinition = null, Conversion conversionOverride = null)
        {
            Require.NotNull(toMemberInfo, "toMemberInfo");

            BindingType = bindingType;
            ToDefinition = new ToDefinition(toMemberInfo);
            FromDefinition = fromDefinition ?? FromUndefinedDefinition.Default;
            ConversionOverride = conversionOverride;
        }

        /// <summary>
        ///     Captures whether the binding was explicitly defined in config or automatically inferred.
        /// </summary>
        public BindingType BindingType { get; private set; }

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

        /// <summary>
        ///     Applies a map operation between bound properties on the 'from' and 'to' class.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="fromDeclaring">An instance of the from class.</param>
        /// <param name="toDeclaring">An instance of the to class.</param>
        /// <param name="mapInto"></param>
        public void Apply(Mapper mapper, object fromDeclaring, object toDeclaring, bool mapInto)
        {
            try
            {
                Require.NotNull(mapper, "mapper");
                Require.NotNull(fromDeclaring, "fromDeclaring");
                Require.NotNull(toDeclaring, "toDeclaring");

                object fromValue;

                if (IsIgnore)
                    return;

                // 1. Get the 'from' value.
                var hasValue = FromDefinition.TryGetFromValue(fromDeclaring, mapper, out fromValue);

                if (hasValue)
                {
                    // Project the 'from' value if required.
                    var toValue = ProjectValue(mapper, fromValue, toDeclaring, mapInto);

                    // Set the value.
                    ToDefinition.Accessor.Setter(toDeclaring, toValue);
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error applying binding '{0}'", this);
                throw new MapnificentException(msg, ex);
            }
        }

        /// <summary>
        ///     The binding may require a projection (i.e. a mapping or conversion) between the 'from' and 'to' types.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="fromValue"></param>
        /// <param name="toDeclaringInstance"></param>
        /// <param name="mapInto"></param>
        /// <returns></returns>
        public object ProjectValue(Mapper mapper, object fromValue, object toDeclaringInstance, bool mapInto = false)
        {
            Require.NotNull(mapper, "mapper");
            Require.NotNull(fromValue, "fromValue");
            Require.NotNull(toDeclaringInstance, "toDeclaringInstance");

            object toValue = null;

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
                    toValue = ToDefinition.Accessor.Getter(toDeclaringInstance);

                toValue = map.Apply(fromValue, toValue, mapInto);
            }
            else
            {
                // If this is a value type binding, then order of precedence is:
                // 1) Apply a conversion override if it is defined.
                // 2) If the projection is identity i.e. T -> T then do nothing.
                // 3) Ask the Mapper for a converter.

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