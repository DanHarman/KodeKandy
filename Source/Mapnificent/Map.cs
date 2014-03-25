﻿// <copyright file="MapInto.cs" company="million miles per hour ltd">
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
using System.Xml.Serialization;
using KodeKandy.Mapnificent.Bindngs;

namespace KodeKandy.Mapnificent
{
    /// <summary>
    ///     Defines a mapping from type to a class.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// The Mapper this map is associated with.
        /// </summary>
        public Mapper Mapper { get; private set; }
            
        public Func<ConstructionContext, object> ConstructedBy { get; set; }

        /// <summary>
        /// Action applied to the mapping target after mapping has been performed.
        /// </summary>
        public Action<object, object> PostMapStep { get; set; }

        // User defined definitions. Does not include automatic definitions - those are composed separately so as to avoid
        // confusion when inheriting maps.
        private readonly Dictionary<string, MemberBindingDefinition> explicitBindings = new Dictionary<string, MemberBindingDefinition>();


        /// <summary>
        /// Cached bindings for a given Mapper. This is required since a map may be placed into more than one Mapper.
        /// </summary>
        private ReadOnlyCollection<MemberBindingDefinition> cachedBindings;

        public ReadOnlyCollection<MemberBindingDefinition> Bindings
        {
            get { return cachedBindings ?? (cachedBindings = GenerateBindings()); }
        }

        public ProjectionType InheritsFrom { get; set; }

        private readonly List<ProjectionType> polymorphicFor = new List<ProjectionType>();
        public ReadOnlyCollection<ProjectionType> PolymoprhicFor { get { return new ReadOnlyCollection<ProjectionType>(polymorphicFor); } }

        public ProjectionType ProjectionType { get; private set; }
  
        public Map(ProjectionType projectionType, Mapper mapper)
        {
            this.Mapper = mapper;
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
            }

            return memberBindingDefinition;
        }

        public void AddPolymorphic(ProjectionType projectionType)
        {
            polymorphicFor.Add(projectionType);
        }

        /// <summary>
        /// Generates the maps bindings taking into account inherited bindings, explicit bindings and autogenerated bindings to fill in any gaps.
        /// </summary>
        private ReadOnlyCollection<MemberBindingDefinition> GenerateBindings()
        {
            // Get any inherited bindings.
            var newBindingsDict = InheritsFrom != null
                ? Mapper.GetMap(InheritsFrom).Bindings.ToDictionary(x => x.ToMemberDefinition.MemberName)
                : new Dictionary<string, MemberBindingDefinition>();

            // Merge in the explicit bindings in this map overridings inherited ones.
            foreach (var b in explicitBindings)
                newBindingsDict[b.Key] = b.Value;

            var newBindings = newBindingsDict.Values.ToList();

            // Merge in any automated bindings that are required.
            var autobindings = GenerateAutoMemberBindings(newBindings).Item1;

            newBindings.AddRange(autobindings);

            return new ReadOnlyCollection<MemberBindingDefinition>(newBindings);
        }

        public void AssertValid()
        {
            var mapDefinitionErrors = Validate();

            if (mapDefinitionErrors.Any())
            {
                throw new Exception();
            }
        }

        private Tuple<ReadOnlyCollection<MemberBindingDefinition>, ReadOnlyCollection<MemberDefinitionError>> GenerateAutoMemberBindings(
            IEnumerable<MemberBindingDefinition> excludedBindings)
        {
            var autoMemberBindings = new List<MemberBindingDefinition>();
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            // Discover which 'to' class members do not already have definitions so that they can be automapped.
            var undefinedToMemberInfos = ReflectionHelpers.GetMemberInfos(ProjectionType.ToType)
                .Where(m => excludedBindings.All(b => b.ToMemberDefinition.MemberName != m.Name));

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

        public ReadOnlyCollection<MemberDefinitionError> Validate()
        {
            Require.NotNull(Mapper, "Mapper");

            // TODO Validate inheritance type is available in Mapper.

            var errors = Bindings.SelectMany(b => MemberBindingDefinitionValidator.Validate(b, Mapper)).ToList();

            return new ReadOnlyCollection<MemberDefinitionError>(errors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapInto">If true, attemps to map into an existing object graph rather than recreating all children.</param>
        public void Apply(object from, object to, bool mapInto = false)
        {
            Require.NotNull(from, "from");
            Require.NotNull(to, "to");
            Require.NotNull(Mapper, "Mapper");

            foreach (var binding in Bindings)
            {
                ApplyBinding(from, to, binding, mapInto);
            }

            if (PostMapStep != null)
                PostMapStep(from, to);
        }

        public object CreateInstanceOfTo(object fromInstance)
        {
            var instance = ConstructedBy != null
                     ? ConstructedBy(new ConstructionContext(this.Mapper, fromInstance, null))
                     : Activator.CreateInstance(ProjectionType.ToType);

            return instance;
        }

        /// <summary>
        /// Applies a map operation between bound properties on the 'from' and 'to' class.
        /// </summary>
        /// <param name="fromDeclaring">An instance of the from class.</param>
        /// <param name="toDeclaring">An instance of the to class.</param>
        /// <param name="binding"></param>
        /// <param name="mapInto"></param>
        private void ApplyBinding(object fromDeclaring, object toDeclaring, MemberBindingDefinition binding, bool mapInto)
        {
            try
            {
                Require.NotNull(fromDeclaring, "fromDeclaring");
                Require.NotNull(toDeclaring, "toDeclaring");

                object fromValue;

                if (binding.Ignore)
                    return;

                // 1. Get the 'from' value.
                var hasValue = binding.TryGetFromValue(fromDeclaring, Mapper, out fromValue);

                if (hasValue)
                {
                    // Project the 'from' value.
                    object toValue;

                    if (binding.HasCustomFromDefintion)
                    {
                        // Custom 'from' methods must return the correct 'to' type so no need for conversion or mapping.
                        toValue = fromValue;
                    }
                    else if (binding.IsMap)
                    {
                        // TODO support Map override here.

                        var map = Mapper.GetMap(binding.ProjectionType);

                        // If we are mapping into then attempt to get a value to map into.
                        if (mapInto)
                            toValue = binding.ToMemberDefinition.MemberGetter(toDeclaring) ?? map.CreateInstanceOfTo(fromValue);
                        else
                            toValue = map.CreateInstanceOfTo(fromValue);

                        map.Apply(fromValue, toValue);
                    }
                    else
                    {
                        // If this is a value type binding, then order of precedence is:
                        // 1) Apply a conversion override if it is defined.
                        // 2) If the projection is identity i.e. T -> T then do nothing.
                        // 3) Ask the Mapper for a convereter.

                        Conversion conversion;

                        if (binding.ConversionOverride != null)
                            conversion = binding.ConversionOverride;
                        else if (binding.ProjectionType.IsIdentity)
                            conversion = null;
                        else
                            conversion = Mapper.GetConversion(binding.ProjectionType);

                        toValue = conversion != null ? conversion.Apply(fromValue) : fromValue;

                    }

                    // Set the value.
                    binding.ToMemberDefinition.MemberSetter(toDeclaring, toValue);
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error applying binding '{0}'", binding);
                throw new MapnificentException(msg, ex);
            }
        }
    }
}