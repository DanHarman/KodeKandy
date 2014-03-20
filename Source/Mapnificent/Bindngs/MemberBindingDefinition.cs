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

using System.Reflection;

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
        public MemberSetterDefinition MemberSetterDefinition { get; private set; }

        /// <summary>
        ///     Defines the 'from' member getter details.
        /// </summary>
        public MemberGetterDefinition MemberGetterDefinition { get; set; }

        /// <summary>
        ///     Conversion used to map between the 'from' member to the 'to' member.
        /// </summary>
        public Conversion Conversion { get; set; }

        /// <summary>
        /// A projection tye reflecting the type of the 'from' and 'to' members.
        /// </summary>
        public ProjectionType ProjectionType
        {
            get { return new ProjectionType(MemberGetterDefinition.MemberType, MemberSetterDefinition.MemberType); }
        }

        private bool ignore;
        public bool Ignore
        {
            get { return ignore; }
            set
            {
                ignore = value;
                if (ignore)
                {
                    MemberSetterDefinition = null;
                    Conversion = null;
                }
            }
        }

        private MemberBindingDefinition(MemberSetterDefinition memberSetterDefinition, MemberBindingDefinitionType memberBindingDefinitionType,
            MemberGetterDefinition memberGetterDefinition = null, Conversion conversion = null)
        {
            Require.NotNull(memberSetterDefinition, "memberSetterDefinition");

            MemberBindingDefinitionType = memberBindingDefinitionType;
            MemberSetterDefinition = memberSetterDefinition;
            MemberGetterDefinition = memberGetterDefinition;
            Conversion = conversion;
        }

        public static MemberBindingDefinition Create(MemberInfo toMemberInfo, MemberBindingDefinitionType memberBindingDefinitionType, MemberGetterDefinition memberGetterDefinition = null, Conversion conversion = null)
        {
            return new MemberBindingDefinition(new MemberSetterDefinition(toMemberInfo), memberBindingDefinitionType, memberGetterDefinition, conversion);
        }

        /// <summary>
        /// Indicates if this binding is reliant on a map or conversion in able to project the type of the 'from' value to the type of the 'to' value.
        /// </summary>
        public bool RequiresMapOrConversion
        {
            get
            {
                if (MemberGetterDefinition == null || Conversion != null)
                    return false;

                var fromMemberType = MemberGetterDefinition.MemberType;
                var toMemberType = MemberSetterDefinition.MemberType;

                return fromMemberType != toMemberType;
            }
        }
    }
}