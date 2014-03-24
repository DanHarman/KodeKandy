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
                fromCustomDefinition = null;
                fromMemberDefinition = value;
            }
        }

        private Func<MappingContext, object> fromCustomDefinition;
        /// <summary>
        /// Defines the 'from' provider when it is a custom delegate.
        /// </summary>
        public Func<MappingContext, object> FromCustomDefinition
        {
            get { return fromCustomDefinition; }
            set
            {
                fromMemberDefinition = null;
                fromCustomDefinition = value;
            }
        }

        public bool IsFromCustom
        {
            get { return FromCustomDefinition != null; }
        }

        /// <summary>
        ///     Conversion used to map between the 'from' member to the 'to' member.
        /// </summary>
        public Conversion Conversion { get; set; }

        /// <summary>
        /// A projection tye reflecting the type of the 'from' and 'to' members.
        /// </summary>
        public ProjectionType ProjectionType
        {
            get { return new ProjectionType(FromMemberDefinition == null ? null : FromMemberDefinition.MemberType, ToMemberDefinition.MemberType); }
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
                    ToMemberDefinition = null;
                    Conversion = null;
                }
            }
        }

        private MemberBindingDefinition(MemberSetterDefinition toMemberDefinition, MemberBindingDefinitionType memberBindingDefinitionType,
            MemberGetterDefinition fromMemberDefinition = null, Conversion conversion = null)
        {
            Require.NotNull(toMemberDefinition, "ToMemberDefinition");

            MemberBindingDefinitionType = memberBindingDefinitionType;
            ToMemberDefinition = toMemberDefinition;
            FromMemberDefinition = fromMemberDefinition;
            Conversion = conversion;
        }

        public static MemberBindingDefinition Create(MemberInfo toMemberInfo, MemberBindingDefinitionType memberBindingDefinitionType, MemberGetterDefinition memberGetterDefinition = null, Conversion conversion = null)
        {
            Require.NotNull(toMemberInfo, "toMemberInfo");

            return new MemberBindingDefinition(new MemberSetterDefinition(toMemberInfo), memberBindingDefinitionType, memberGetterDefinition, conversion);
        }

        /// <summary>
        /// Indicates if this binding is reliant on a map or conversion in able to project the type of the 'from' value to the type of the 'to' value.
        /// </summary>
        public bool RequiresMapOrConversion
        {
            get
            {
                if (FromMemberDefinition == null || Conversion != null)
                    return false;

                var fromMemberType = FromMemberDefinition.MemberType;
                var toMemberType = ToMemberDefinition.MemberType;

                return fromMemberType != toMemberType;
            }
        }
    }
}