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
        public ToMemberDefinition ToToMemberDefinition { get; private set; }

        private FromDefinition fromDefinition;
        /// <summary>
        ///     Defines the 'from' provider when it is a member on the 'from' class.
        /// </summary>
        public FromDefinition FromDefinition
        {
            get { return fromDefinition; }
            set { fromDefinition = value; }
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

        public MemberBindingDefinition(MemberInfo toMemberInfo, MemberBindingDefinitionType memberBindingDefinitionType,
            FromDefinition fromDefinition = null, Conversion conversionOverride = null)
        {
            Require.NotNull(toMemberInfo, "toMemberInfo");

            MemberBindingDefinitionType = memberBindingDefinitionType;
            ToToMemberDefinition = new ToMemberDefinition(toMemberInfo);
            FromDefinition = fromDefinition ?? FromUndefinedDefinition.Default;
            ConversionOverride = conversionOverride;
        }

        public Type FromType
        {
            get
            {
                if (HasCustomFromDefintion)
                    return ProjectionType.Custom;
                if (FromDefinition == null)
                    return ProjectionType.Undefined;
                return ((FromMemberDefinition) FromDefinition).MemberType;
            }
        }

        public Type ToType
        {
            get { return ToToMemberDefinition.MemberType; }
        }

        public override string ToString()
        {
            string bindingDescription;

            if (fromDefinition != null)
            {
                bindingDescription = string.Format("'{0}'->'{1}'", FromDefinition.Description, ToToMemberDefinition.MemberName);
            }
            else
            {
                bindingDescription = string.Format("'<Undefined>'->'{0}'", ToToMemberDefinition.MemberName);
            }

            return string.Format("Binding: {0}, Type: {1}", bindingDescription, ProjectionType);
        }
    }
}