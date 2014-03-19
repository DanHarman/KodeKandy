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

            if (definition.MemberGetterDefinition == null)
                memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.MemberSetterDefinition, "Binding definition does not define 'From' binding."));

            if (definition.MemberGetterDefinition != null && definition.ConversionDefinition == null)
            {
                var fromMemberType = definition.MemberGetterDefinition.MemberType;
                var toMemberType = definition.MemberSetterDefinition.MemberType;

                if (!mapper.HasMapOrConversion(fromMemberType, toMemberType))
                {
                    memberDefinitionErrors.Add(
                        MemberDefinitionError.Create(definition.MemberSetterDefinition, "Mapped from '{0}' but no {1} defined between '{2}'->'{3}'.",
                            definition.MemberGetterDefinition.MemberName,
                            toMemberType.IsClass ? "map" : "conversion",
                            fromMemberType.Name, toMemberType.Name));
                }
            }

            if (definition.MemberGetterDefinition != null && definition.ConversionDefinition != null)
            {
                // TODO validate conversion or make it so instantiation implies validity.

                if (definition.MemberSetterDefinition.MemberType != definition.ConversionDefinition.ProjectionType.ToType)
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.MemberSetterDefinition,
                        "To member type {0} does not match the defined conversion output type ({1})", definition.MemberGetterDefinition.MemberType,
                        definition.ConversionDefinition));

                if (definition.MemberGetterDefinition.MemberType != definition.ConversionDefinition.ProjectionType.FromType)
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(definition.MemberSetterDefinition,
                        "From member type {0} does not match the defined conversion input type ({1})", definition.MemberGetterDefinition.MemberType,
                        definition.ConversionDefinition));
            }

            return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
        }
    }
}