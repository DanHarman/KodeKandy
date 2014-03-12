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
        private readonly Dictionary<string, MemberBindingDefinition> definedBindings = new Dictionary<string, MemberBindingDefinition>();

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
            if (!definedBindings.TryGetValue(toMemberInfo.Name, out memberBindingDefinition))
                definedBindings[toMemberInfo.Name] = new MemberBindingDefinition(toMemberInfo, MemberBindingDefinitionType.Explicit);

            return memberBindingDefinition;
        }

        //public ConversionDefinition<TFromDeclaring, TToDeclaring> For<TToMember>(Expression<Func<TToDeclaring, TToMember>> toMember,
        //    Action<MemberBindingDefinitionBuilder<TFromDeclaring, TToMember>> options)
        //{
        //    Require.NotNull(toMember, "toMember");
        //    Require.IsTrue(ExpressionHelpers.IsMemberExpression(toMember), "Parameter 'toMember' must be a simple expression.");

        //    // Obtain a bindingDefinition.
        //    var memberName = ExpressionHelpers.GetMemberName(toMember);
        //    MemberBindingDefinition bindingDefinition;
        //    if (!definedBindings.TryGetValue(memberName, out bindingDefinition))
        //        definedBindings[memberName] = MemberBindingDefinition.Create(toMember);

        //    // Apply the builder options.
        //    var builder = new MemberBindingDefinitionBuilder<TFromDeclaring, TToMember>(bindingDefinition);
        //    options(builder);

        //    return this;
        //}

        //public ConversionDefinition<TFromDeclaring, TToDeclaring> ConstructUsing(Func<ConstructionContext, TToDeclaring> factory)
        //{
        //    return this;
        //}

        /// <summary>
        ///     Indicates that this type is a collection type and it's entities should be mapped.
        /// </summary>
        /// <returns></returns>
        //public ConversionDefinition<TFromDeclaring, TToDeclaring> AsCollection()
        //{
        //    return this;
        //}
        //public ConversionDefinition<TFromDeclaring, TToDeclaring> Inherits<TFromBase, TToBase>(ConversionDefinition<TFromDeclaring, TToDeclaring> definition)
        //{
        //    Require.IsTrue(typeof(TFromBase).IsAssignableFrom(typeof(TFromDeclaring)));
        //    Require.IsTrue(typeof(TToBase).IsAssignableFrom(typeof(TToDeclaring)));
        //    return this;
        //}
        //public Map CreateMap()
        //{
        //    var map = new Map(typeof(TFromDeclaring), typeof(TToDeclaring));
        //    // Discover which to class members do not have explicit definitions,
        //    // and try to automap them.
        //    var allMembers = ReflectionHelpers.GetMemberInfos(typeof(TToDeclaring));
        //    var undefined = allMembers.Where(m => !definedBindings.ContainsKey(m.Name));
        //    var unmatched = new List<string>();
        //    var newDefinitions = new Dictionary<string, MemberBindingDefinition>();
        //    // Iterate all the undefined members on the 'to' class and try to automatically create matches from
        //    // inspecting fields on the 'from' class.
        //    foreach (var toMemberInfo in undefined)
        //    {
        //        var toMemberName = toMemberInfo.Name;
        //        var unflattenedMemberInfos = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(typeof(TFromDeclaring), toMemberName);
        //        if (unflattenedMemberInfos.Any())
        //        {
        //            var getter = ReflectionHelpers.CreateSafeWeakMemberChainGetter(new[] {toMemberInfo});
        //            newDefinitions[toMemberName] = MemberBindingDefinition.Create(toMemberInfo).SetFrom(typeof(TFromDeclaring), toMemberName,
        //                toMemberInfo.GetMemberType(), getter);
        //        }
        //        else
        //        {
        //            unmatched.Add(toMemberInfo.Name);
        //        }
        //    }
        //    // TODO use the definitions to create the map.
        //    return map;
        //}
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

            var combined = new List<MemberBindingDefinition>(definedBindings.Values);
            combined.AddRange(GenerateAutoMemberBindings().Item1);

            throw new NotImplementedException();
        }

        private Tuple<ReadOnlyCollection<MemberBindingDefinition>, ReadOnlyCollection<MemberDefinitionError>> GenerateAutoMemberBindings()
        {
            var autoMemberBindings = new List<MemberBindingDefinition>();
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            // Discover which 'to' class members do not have explicit definitions, so that they can be automapped.
            var undefinedToMemberInfos = ReflectionHelpers.GetMemberInfos(MappingType.ToType).Where(m => !definedBindings.ContainsKey(m.Name));

            // Iterate all the undefined members on the 'to' class and try to automatically create matches from
            // inspecting members on the 'from' class.
            foreach (var toMemberInfo in undefinedToMemberInfos)
            {
                var unflattenedMemberInfos = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(MappingType.FromType, toMemberInfo.Name);

                if (unflattenedMemberInfos.Any())
                {
                    var memberBindingDefinition = new MemberBindingDefinition(toMemberInfo, MemberBindingDefinitionType.Auto);

                    var fromMemberType = unflattenedMemberInfos.Last().GetMemberType();
                    var fromMemberName = String.Join(".", unflattenedMemberInfos.GetMemberNames());
                    var fromMemberGetter = ReflectionHelpers.CreateSafeWeakMemberChainGetter(unflattenedMemberInfos);

                    memberBindingDefinition.MemberGetterDefinition = new MemberGetterDefinition(MappingType.FromType, fromMemberName, fromMemberType,
                        fromMemberGetter);

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
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            var generateAutoMemberBindingsResult = GenerateAutoMemberBindings(); //.ToDictionary(x => x.MemberSetterDefinition.MemberName);

            memberDefinitionErrors.AddRange(ValidateMemberBindingDefinitions(mapperSchema, definedBindings.Values));
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