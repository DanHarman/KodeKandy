// <copyright file="MapDefinition.cs" company="million miles per hour ltd">
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using KodeKandy.Mapnificent.Maps;

namespace KodeKandy.Mapnificent.Definitions
{
    /// <summary>
    ///     Defines a mapping from type to a class.
    /// </summary>
    public class MapDefinition : IMapDefinition
    {
        // User defined definitions. Does not include automatic definitions - those are composed separately so as to avoid
        // confusion when inheriting maps.
        private readonly Dictionary<string, MemberBindingDefinition> explicitBindings = new Dictionary<string, MemberBindingDefinition>();

        public MappingType MappingType { get; private set; }

        public MapDefinition(MappingType mappingType)
        {
            Require.NotNull(mappingType);
            Require.IsTrue(mappingType.ToType.IsClass);

            MappingType = mappingType;
        }

        public MemberBindingDefinition GetMemberBindingDefinition(MemberInfo toMemberInfo)
        {
            MemberBindingDefinition memberBindingDefinition;
            if (!explicitBindings.TryGetValue(toMemberInfo.Name, out memberBindingDefinition))
                explicitBindings[toMemberInfo.Name] = MemberBindingDefinition.Create(toMemberInfo, MemberBindingDefinitionType.Explicit);

            return memberBindingDefinition;
        }


        public ReadOnlyCollection<MemberBindingDefinition> Bindings
        {
            get
            {
                var autoMemberBindingsResult = GenerateAutoMemberBindings();

                var combined = new List<MemberBindingDefinition>(explicitBindings.Values);
                combined.AddRange(autoMemberBindingsResult.Item1);

                return new ReadOnlyCollection<MemberBindingDefinition>(combined);
            }
        }

        public void AssertValid(MapperSchema mapperSchema)
        {
            var mapDefinitionErrors = Validate(mapperSchema);

            if (mapDefinitionErrors.Any())
            {
                throw new Exception();
            }
        }

        public Map ToMap(MapperSchema mapperSchema)
        {
            AssertValid(mapperSchema);

            var autoMemberBindingsResult = GenerateAutoMemberBindings();

            var combined = new List<MemberBindingDefinition>(explicitBindings.Values);
            combined.AddRange(autoMemberBindingsResult.Item1);

            throw new NotImplementedException();
        }

        private Tuple<ReadOnlyCollection<MemberBindingDefinition>, ReadOnlyCollection<MemberDefinitionError>> GenerateAutoMemberBindings()
        {
            var autoMemberBindings = new List<MemberBindingDefinition>();
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            // Discover which 'to' class members do not have explicit definitions, so that they can be automapped.
            var undefinedToMemberInfos = ReflectionHelpers.GetMemberInfos(MappingType.ToType).Where(m => !explicitBindings.ContainsKey(m.Name));

            // Iterate all the undefined members on the 'to' class and try to automatically create matches from
            // inspecting members on the 'from' class.
            foreach (var toMemberInfo in undefinedToMemberInfos)
            {
                var unflattenedMemberInfos = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(MappingType.FromType, toMemberInfo.Name);

                if (unflattenedMemberInfos.Any())
                {

                    var fromMemberType = unflattenedMemberInfos.Last().GetMemberType();
                    var fromMemberName = String.Join(".", unflattenedMemberInfos.GetMemberNames());
                    var fromMemberGetter = ReflectionHelpers.CreateSafeWeakMemberChainGetter(unflattenedMemberInfos);

                    var memberGetterDefinition = new MemberGetterDefinition(MappingType.FromType, fromMemberName, fromMemberType,
                        fromMemberGetter);

                    var memberBindingDefinition = MemberBindingDefinition.Create(toMemberInfo, MemberBindingDefinitionType.Auto, memberGetterDefinition);

                    autoMemberBindings.Add(memberBindingDefinition);
                }
                else
                {
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(toMemberInfo, "No '{0}' defined and unable to find auto-match.",
                        toMemberInfo.GetMemberType().IsClass ? "map" : "conversion"));
                }
            }

            return Tuple.Create(new ReadOnlyCollection<MemberBindingDefinition>(autoMemberBindings),
                new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors));
        }

        public ReadOnlyCollection<MemberDefinitionError> Validate(MapperSchema mapperSchema)
        {
            Require.NotNull(mapperSchema, "mapperSchema");

            var memberDefinitionErrors = new List<MemberDefinitionError>();

            var generateAutoMemberBindingsResult = GenerateAutoMemberBindings(); //.ToDictionary(x => x.MemberSetterDefinition.MemberName);

            memberDefinitionErrors.AddRange(ValidateMemberBindingDefinitions(mapperSchema, explicitBindings.Values));
            memberDefinitionErrors.AddRange(ValidateMemberBindingDefinitions(mapperSchema, generateAutoMemberBindingsResult.Item1));
            memberDefinitionErrors.AddRange(generateAutoMemberBindingsResult.Item2);

            return new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors);
        }

        private static IEnumerable<MemberDefinitionError> ValidateMemberBindingDefinitions(MapperSchema mapperSchema,
            IEnumerable<MemberBindingDefinition> memberBindingDefinitions)
        {
            var errors = memberBindingDefinitions.SelectMany(md => md.Validate(mapperSchema)).ToList();

            return errors;
        }
    }
}