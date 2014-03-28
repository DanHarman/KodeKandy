using System.Collections.Generic;
using System.Collections.ObjectModel;
using KodeKandy.Mapnificent.Bindngs;

namespace KodeKandy.Mapnificent
{
    public static class MemberBindingDefinitionValidator
    {
        public static ReadOnlyCollection<MemberDefinitionError> Validate(MemberBindingDefinition definition, Mapper mapper)
        {
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            if (definition.IsIgnore)
                return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);

            if (definition.FromDefinition == FromUndefinedDefinition.Default)
                memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.ToToMemberDefinition, "Binding definition does not define a 'from' source."));

//            if (definition.FromDefinition != null && definition.ConversionOverride == null)
//            {
//                var fromMemberType = definition.FromDefinition.MemberType;
//                var toMemberType = definition.toToMemberDefinition.MemberType;
//
//                if (!mapper.HasMapOrConversion(fromMemberType, toMemberType))
//                {
//                    memberDefinitionErrors.Add(
//                        MemberDefinitionError.Create(definition.toToMemberDefinition, "Mapped from '{0}' but no {1} defined between '{2}'->'{3}'.",
//                            definition.FromDefinition.memberPath,
//                            toMemberType.IsClass ? "map" : "conversion",
//                            fromMemberType.Name, toMemberType.Name));
//                }
//            }
//
//            if (definition.FromDefinition != null && definition.ConversionOverride != null)
//            {
//                // TODO validate conversion or make it so instantiation implies validity.
//
//                if (definition.toToMemberDefinition.MemberType != definition.ConversionOverride.ProjectionType.ToType)
//                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.toToMemberDefinition,
//                        "To member type {0} does not match the defined conversion output type ({1})", definition.FromDefinition.MemberType,
//                        definition.ConversionOverride));
//
//                if (definition.FromDefinition.MemberType != definition.ConversionOverride.ProjectionType.FromType)
//                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.toToMemberDefinition,
//                        "From member type {0} does not match the defined conversion input type ({1})", definition.FromDefinition.MemberType,
//                        definition.ConversionOverride));
//            }

            return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
        }
    }
}