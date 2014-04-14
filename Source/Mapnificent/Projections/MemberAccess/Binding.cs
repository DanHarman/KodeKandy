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

namespace KodeKandy.Mapnificent.Projections.MemberAccess
{
    /// <summary>
    ///     Defines the source of data for a member in the 'to' class. This may be a member of the 'from' class, a custom
    ///     function or merely specify that this member is to be ignored.
    /// </summary>
    /// <remarks>
    ///     n.b. These bound members will likely need to be projected with either a map or convserion of some form.
    /// </remarks>
    public class Binding : IProjection
    {
        private FromDefinition fromDefinition;
        private bool isIgnore;

        public Binding(MemberInfo toMemberInfo, BindingType bindingType, Mapper mapper,
            FromDefinition fromDefinition = null)
        {
            Require.NotNull(toMemberInfo, "toMemberInfo");

            BindingType = bindingType;
            Mapper = mapper;
            ToDefinition = new ToDefinition(toMemberInfo);
            FromDefinition = fromDefinition ?? FromUndefinedDefinition.Default;
        }

        /// <summary>
        ///     Captures whether the binding was explicitly defined in config or automatically inferred.
        /// </summary>
        public BindingType BindingType { get; private set; }

        public Mapper Mapper { get; private set; }

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
        public FromDefinition FromDefinition
        {
            get { return fromDefinition; }
            set
            {
                fromDefinition = value;

                // Update the projection if its not set or just an automatic LateBoundProjection, as it may be stale if the member
                // type has changed.
                if (Projection == null || Projection is LateBoundProjection)
                    Projection = new LateBoundProjection(ProjectionType, Mapper);
            }
        }

        public Type FromType
        {
            get { return FromDefinition.MemberType; }
        }

        public bool HasCustomFromDefintion
        {
            get { return FromDefinition is FromCustomDefinition; }
        }

        /// <summary>
        ///     A projection the reflecting the type of the 'from' and 'to' members.
        /// </summary>
        public ProjectionType ProjectionType
        {
            get { return new ProjectionType(FromType, ToType); }
        }

        public IProjection Projection { get; set; }

        public bool IsIgnore
        {
            get { return isIgnore; }
            set
            {
                isIgnore = value;
                if (isIgnore)
                {
                    FromDefinition = null;
                    Projection = null;
                }
            }
        }

        /// <summary>
        ///     Applies a ClassMap operation between bound properties on the 'from' and 'to' class.
        /// </summary>
        /// <param name="fromDeclaring">An instance of the from class.</param>
        /// <param name="toDeclaring">An instance of the to class.</param>
        /// <param name="mapInto"></param>
        public object Apply(object fromDeclaring, object toDeclaring, bool mapInto = false)
        {
            try
            {
                Require.NotNull(fromDeclaring, "fromDeclaring");
                Require.NotNull(toDeclaring, "toDeclaring");

                if (IsIgnore)
                    return toDeclaring;

                object fromValue;

                // 1. Get the 'from' value.
                var hasValue = FromDefinition.TryGetFromValue(fromDeclaring, Mapper, out fromValue);

                if (hasValue)
                {
                    object toValue = null;

                    // 2. Project.
                    // If we are 'mapping into' then attempt to get a value to map into.
                    if (ProjectionType.IsClassProjection && mapInto)
                        toValue = ToDefinition.Accessor.Getter(toDeclaring);

                    toValue = Projection.Apply(fromValue, toValue, mapInto);

                    // 3. Set the 'to' value.
                    ToDefinition.Accessor.Setter(toDeclaring, toValue);
                }

                return toDeclaring;
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error applying binding '{0}'", this);
                throw new MapnificentException(msg, ex, Mapper);
            }
        }

        public override string ToString()
        {
            var bindingDescription = string.Format("'{0}'->'{1}'", FromDefinition, ToDefinition.MemberName);

            return string.Format("Binding: {0}, Type: {1}", bindingDescription, ProjectionType);
        }
    }
}