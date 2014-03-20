// <copyright file="Map.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent
{
    /// <summary>
    ///     Defines a mapping from type to a class.
    /// </summary>
    public class Map 
    {
        // User defined definitions. Does not include automatic definitions - those are composed separately so as to avoid
        // confusion when inheriting maps.
        private readonly Dictionary<string, MemberBindingDefinition> explicitBindings = new Dictionary<string, MemberBindingDefinition>();

        public ProjectionType InheritsFrom { get; set; }

        private readonly List<ProjectionType> polymorphicFor = new List<ProjectionType>();
        public ReadOnlyCollection<ProjectionType> PolymoprhicFor { get { return new ReadOnlyCollection<ProjectionType>(polymorphicFor); } }

        public ProjectionType ProjectionType { get; private set; }

        // Flag indicating if the bindings need to be re-evaluated due to a change.
        private bool bindingsDirty = true;
        private ReadOnlyCollection<MemberBindingDefinition> bindings;
        public ReadOnlyCollection<MemberBindingDefinition> Bindings
        {
            get
            {
                if (bindingsDirty || bindings == null)
                    RefreshBindings();

                return bindings;
            }
        }

        public Map(ProjectionType projectionType)
        {
            Require.NotNull(projectionType);
            Require.IsTrue(projectionType.ToType.IsClass);

            ProjectionType = projectionType;
        }

        public MemberBindingDefinition GetMemberBindingDefinition(MemberInfo toMemberInfo)
        {
            MemberBindingDefinition memberBindingDefinition;
            if (!explicitBindings.TryGetValue(toMemberInfo.Name, out memberBindingDefinition))
            {
                memberBindingDefinition = MemberBindingDefinition.Create(toMemberInfo, MemberBindingDefinitionType.Explicit);
                explicitBindings[toMemberInfo.Name] = memberBindingDefinition;
                bindingsDirty = true;
            }

            return memberBindingDefinition;
        }

        public void AddPolymorphic(ProjectionType projectionType)
        {
            polymorphicFor.Add(projectionType);
        }

        /// <summary>
        /// Refreshes the cached bindings collection, which includes the auto generated ones. This is cached and lazily evaluated
        /// for performance reasons and to avoid trying to autogenerate bindings when the user may be intending to manually specify them.
        /// </summary>
        private void RefreshBindings()
        {
            var autoMemberBindingsResult = GenerateAutoMemberBindings();

            var combined = new List<MemberBindingDefinition>(explicitBindings.Values);
            combined.AddRange(autoMemberBindingsResult.Item1);

            bindings = new ReadOnlyCollection<MemberBindingDefinition>(combined);
        }

        public void AssertValid(Mapper mapper)
        {
            var mapDefinitionErrors = Validate(mapper);

            if (mapDefinitionErrors.Any())
            {
                throw new Exception();
            }
        }

        private Tuple<ReadOnlyCollection<MemberBindingDefinition>, ReadOnlyCollection<MemberDefinitionError>> GenerateAutoMemberBindings()
        {
            var autoMemberBindings = new List<MemberBindingDefinition>();
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            // Discover which 'to' class members do not have explicit definitions, so that they can be automapped.
            var undefinedToMemberInfos = ReflectionHelpers.GetMemberInfos(ProjectionType.ToType).Where(m => !explicitBindings.ContainsKey(m.Name));

            // Iterate all the undefined members on the 'to' class and try to automatically create matches from
            // inspecting members on the 'from' class.
            foreach (var toMemberInfo in undefinedToMemberInfos)
            {
                var unflattenedMemberInfos = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(ProjectionType.FromType, toMemberInfo.Name);

                if (unflattenedMemberInfos.Any())
                {
                    var fromMemberType = unflattenedMemberInfos.Last().GetMemberType();
                    var fromMemberName = String.Join(".", unflattenedMemberInfos.GetMemberNames());
                    var fromMemberGetter = ReflectionHelpers.CreateSafeWeakMemberChainGetter(unflattenedMemberInfos);

                    var memberGetterDefinition = new MemberGetterDefinition(ProjectionType.FromType, fromMemberName, fromMemberType,
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

        public ReadOnlyCollection<MemberDefinitionError> Validate(Mapper mapper)
        {
            Require.NotNull(mapper, "Mapper");

            var errors = Bindings.SelectMany(b => MemberBindingDefinitionValidator.Validate(b, mapper)).ToList();
        
            return new ReadOnlyCollection<MemberDefinitionError>(errors);
        }

        public void Apply(object from, object to, Mapper mapper)
        {
            Require.NotNull(from, "from");
            Require.NotNull(to, "to");
            Require.NotNull(mapper, "mapper");

            foreach (var binding in Bindings)
            {
                ApplyBinding(from, to, binding, mapper);
            }
        }

        /// <summary>
        /// Applies a map operation between bound properties on the 'from' and 'to' class.
        /// </summary>
        /// <param name="fromDeclaring">An instance of the from class.</param>
        /// <param name="toDeclaring">An instance of the to class.</param>
        /// <param name="binding"></param>
        private void ApplyBinding(object fromDeclaring, object toDeclaring, MemberBindingDefinition binding, Mapper mapper)
        {
            Require.NotNull(fromDeclaring, "fromDeclaring");
            Require.NotNull(toDeclaring, "toDeclaring");

            object fromValue, toValue;
            var hasValue = binding.MemberGetterDefinition.MemberGetter(fromDeclaring, out fromValue);
            if (hasValue)
            {
                if (binding.Conversion != null)
                {
                    toValue = binding.Conversion.ConversionFunc(fromValue);
                }
                else
                {
                    var projectionType = binding.ProjectionType;
                    if (!projectionType.IsIdentity)
                    {
                        if (projectionType.IsMap)
                        {
                            toValue = Activator.CreateInstance(binding.MemberSetterDefinition.MemberType);
                            mapper.GetMap(projectionType).Apply(fromValue, toValue, mapper);
                        }
                        else
                        {
                            toValue = mapper.GetConversion(projectionType).Apply(fromValue);
                        }
                    }
                    else
                    {
                        toValue = fromValue;
                    }
                }

                // TODO need to handle non explicit conversions.

                binding.MemberSetterDefinition.MemberSetter(toDeclaring, toValue);
            }
        }
    }
}