// <copyright file="BindingValidator.cs" company="million miles per hour ltd">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KodeKandy.Mapnificent.Projections.MemberAccess
{
    public static class BindingValidator
    {
        public static ReadOnlyCollection<MemberDefinitionError> Validate(Binding definition, Mapper mapper)
        {
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            if (definition.IsIgnore)
                return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);

            if (definition.FromDefinition == FromUndefinedDefinition.Default)
                memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.ToDefinition, "Binding definition does not define a 'from' source."));

//            if (definition.FromDefinition != null && definition.ConvertUsing == null)
//            {
//                var fromMemberType = definition.FromDefinition.MemberType;
//                var toMemberType = definition.toToMemberDefinition.MemberType;
//
//                if (!mapper.HasProjection(fromMemberType, toMemberType))
//                {
//                    memberDefinitionErrors.Add(
//                        MemberDefinitionError.Create(definition.toToMemberDefinition, "Mapped from '{0}' but no {1} defined between '{2}'->'{3}'.",
//                            definition.FromDefinition.memberPath,
//                            toMemberType.IsClass ? "ClassMap" : "conversion",
//                            fromMemberType.Name, toMemberType.Name));
//                }
//            }
//
//            if (definition.FromDefinition != null && definition.ConvertUsing != null)
//            {
//                // TODO validate conversion or make it so instantiation implies validity.
//
//                if (definition.toToMemberDefinition.MemberType != definition.ConvertUsing.ProjectionType.ToType)
//                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.toToMemberDefinition,
//                        "To member type {0} does not match the defined conversion output type ({1})", definition.FromDefinition.MemberType,
//                        definition.ConvertUsing));
//
//                if (definition.FromDefinition.MemberType != definition.ConvertUsing.ProjectionType.FromType)
//                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.toToMemberDefinition,
//                        "From member type {0} does not match the defined conversion input type ({1})", definition.FromDefinition.MemberType,
//                        definition.ConvertUsing));
//            }

            return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
        }
    }
}