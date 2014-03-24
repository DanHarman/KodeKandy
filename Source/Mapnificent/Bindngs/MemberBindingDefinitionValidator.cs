using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KodeKandy.Mapnificent
{
    public static class MemberBindingDefinitionValidator
    {
        public static ReadOnlyCollection<MemberDefinitionError> Validate(MemberBindingDefinition definition, Mapper mapper)
        {
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            if (definition.Ignore)
                return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);

            if (definition.FromMemberDefinition == null && definition.FromCustomDefinition == null)
                memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.ToMemberDefinition, "Binding definition does not define a 'from' source."));

            if (definition.FromMemberDefinition != null && definition.Conversion == null)
            {
                var fromMemberType = definition.FromMemberDefinition.MemberType;
                var toMemberType = definition.ToMemberDefinition.MemberType;

                if (!mapper.HasMapOrConversion(fromMemberType, toMemberType))
                {
                    memberDefinitionErrors.Add(
                        MemberDefinitionError.Create(definition.ToMemberDefinition, "Mapped from '{0}' but no {1} defined between '{2}'->'{3}'.",
                            definition.FromMemberDefinition.MemberName,
                            toMemberType.IsClass ? "map" : "conversion",
                            fromMemberType.Name, toMemberType.Name));
                }
            }

            if (definition.FromMemberDefinition != null && definition.Conversion != null)
            {
                // TODO validate conversion or make it so instantiation implies validity.

                if (definition.ToMemberDefinition.MemberType != definition.Conversion.ProjectionType.ToType)
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.ToMemberDefinition,
                        "To member type {0} does not match the defined conversion output type ({1})", definition.FromMemberDefinition.MemberType,
                        definition.Conversion));

                if (definition.FromMemberDefinition.MemberType != definition.Conversion.ProjectionType.FromType)
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.ToMemberDefinition,
                        "From member type {0} does not match the defined conversion input type ({1})", definition.FromMemberDefinition.MemberType,
                        definition.Conversion));
            }

            return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
        }
    }
}