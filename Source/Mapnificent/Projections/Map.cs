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
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using KodeKandy.Mapnificent.MemberAccess;

namespace KodeKandy.Mapnificent.Projections
{
    /// <summary>
    ///     Defines a mapping from type to a class.
    /// </summary>
    public class Map
    {
        private readonly Dictionary<string, Binding> explicitBindings = new Dictionary<string, Binding>();
        private readonly List<ProjectionType> polymorphicFor = new List<ProjectionType>();
        /// <summary>
        ///     Cached bindings for a given Mapper. This is required since a map may be placed into more than one Mapper.
        /// </summary>
        private ReadOnlyCollection<Binding> cachedBindings;
        private Func<ConstructionContext, object> constructedBy;
        private ProjectionType inheritsFrom;

        public Map(ProjectionType projectionType, Mapper mapper)
        {
            Require.NotNull(projectionType, "projectionType");
            Require.NotNull(mapper, "mapper");
            Require.IsTrue(projectionType.ToType.IsClass);

            ProjectionType = projectionType;
            Mapper = mapper;
            ConstructedBy = _ => Activator.CreateInstance(ProjectionType.ToType);
        }

        /// <summary>
        ///     The Mapper this map is associated with.
        /// </summary>
        public Mapper Mapper { get; private set; }

        public Func<ConstructionContext, object> ConstructedBy
        {
            get { return constructedBy; }
            set
            {
                Require.NotNull(value, "value");
                constructedBy = value;
            }
        }

        /// <summary>
        ///     Action applied to the mapping target after mapping has been performed.
        /// </summary>
        public Action<object, object> PostMapStep { get; set; }

        // User defined definitions. Does not include automatic definitions - those are composed separately so as to avoid
        // confusion when inheriting maps.

        public ReadOnlyCollection<Binding> Bindings
        {
            get { return cachedBindings ?? (cachedBindings = GenerateBindings()); }
        }

        public ProjectionType InheritsFrom
        {
            get { return inheritsFrom; }
            set
            {
                Require.NotNull(value, "value");
                Require.IsTrue(value.FromType.IsAssignableFrom(ProjectionType.FromType),
                    String.Format("Cannot inherit from a map whose 'From' type '{0}' is not a supertype of this maps 'From' type '{1}'.",
                        value.FromType.Name, ProjectionType.FromType.Name));
                Require.IsTrue(value.ToType.IsAssignableFrom(ProjectionType.ToType),
                    String.Format("Cannot inherit from a map whose 'To' type '{0}' is not a supertype of this maps 'To' type '{1}'.",
                        value.ToType.Name, ProjectionType.ToType.Name));

                inheritsFrom = value;
            }
        }

        public ReadOnlyCollection<ProjectionType> PolymorphicFor
        {
            get { return new ReadOnlyCollection<ProjectionType>(polymorphicFor); }
        }

        public ProjectionType ProjectionType { get; private set; }

        public void AddPolymorphicFor(ProjectionType projectionType)
        {
            Require.NotNull(projectionType, "projectionType");

            Require.IsTrue(ProjectionType.FromType.IsAssignableFrom(projectionType.FromType),
                String.Format("Cannot be polymorphic for a map whose 'From' type '{0}' is not a subtype of this maps 'From' type '{1}'.",
                    projectionType.FromType.Name, ProjectionType.FromType.Name));

            Require.IsTrue(ProjectionType.ToType.IsAssignableFrom(projectionType.ToType),
                String.Format("Cannot be polymorphic for a map whose 'To' type '{0}' is not a subtype of this maps 'To' type '{1}'.",
                    projectionType.ToType.Name, ProjectionType.ToType.Name));

            Require.IsFalse(polymorphicFor.Any(pt => pt.FromType == projectionType.FromType),
                String.Format(
                    "Illegal 'polymorphic for' defintion. A definition has already been registered for the 'from' type '{0}' and would be made ambiguous by this one.",
                    projectionType.FromType.Name));

            polymorphicFor.Add(projectionType);
        }

        public Binding GetMemberBindingDefinition(MemberInfo toMemberInfo)
        {
            Binding binding;
            if (!explicitBindings.TryGetValue(toMemberInfo.Name, out binding))
            {
                binding = new Binding(toMemberInfo, BindingType.Explicit);
                explicitBindings[toMemberInfo.Name] = binding;
            }

            return binding;
        }

        /// <summary>
        ///     Generates the maps bindings taking into account inherited bindings, explicit bindings and autogenerated bindings to
        ///     fill in any gaps.
        /// </summary>
        private ReadOnlyCollection<Binding> GenerateBindings()
        {
            // Get any inherited bindings.
            var newBindingsDict = InheritsFrom != null
                ? Mapper.GetMap(InheritsFrom).Bindings.ToDictionary(x => x.ToDefinition.MemberName)
                : new Dictionary<string, Binding>();

            // Merge in the explicit bindings in this map overridings inherited ones.
            foreach (var b in explicitBindings)
                newBindingsDict[b.Key] = b.Value;

            var newBindings = newBindingsDict.Values.ToList();

            // Merge in any automated bindings that are required.
            var autobindings = GenerateAutoMemberBindings(newBindings).Item1;

            newBindings.AddRange(autobindings);

            return new ReadOnlyCollection<Binding>(newBindings);
        }

        public void AssertValid()
        {
            var mapDefinitionErrors = Validate();


            // TODO check defined poly maps exist

            if (mapDefinitionErrors.Any())
            {
                throw new Exception();
            }
        }

        private Tuple<ReadOnlyCollection<Binding>, ReadOnlyCollection<MemberDefinitionError>> GenerateAutoMemberBindings(
            IEnumerable<Binding> excludedBindings)
        {
            var autoMemberBindings = new List<Binding>();
            var memberDefinitionErrors = new List<MemberDefinitionError>();

            // Discover which 'to' class members do not already have definitions so that they can be automapped.
            var undefinedToMemberInfos = ReflectionHelpers.GetMemberInfos(ProjectionType.ToType)
                                                          .Where(m => excludedBindings.All(b => b.ToDefinition.MemberName != m.Name));

            // Iterate all the undefined members on the 'to' class and try to automatically create matches from
            // inspecting members on the 'from' class.
            foreach (var toMemberInfo in undefinedToMemberInfos)
            {
                var unflattenedMemberInfos = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(ProjectionType.FromType, toMemberInfo.Name);

                if (unflattenedMemberInfos.Any())
                {
                    var fromMemberType = unflattenedMemberInfos.Last().GetMemberType();
                    var fromMemberPath = String.Join(".", unflattenedMemberInfos.GetMemberNames());
                    var fromMemberGetter = ReflectionHelpers.CreateSafeWeakMemberChainGetter(unflattenedMemberInfos);

                    var memberGetterDefinition = new FromMemberDefinition(fromMemberPath, fromMemberType, fromMemberGetter);

                    var memberBindingDefinition = new Binding(toMemberInfo, BindingType.Auto)
                    {
                        FromDefinition = memberGetterDefinition
                    };

                    autoMemberBindings.Add(memberBindingDefinition);
                }
                else
                {
                    memberDefinitionErrors.Add(MemberDefinitionError.Create(toMemberInfo, "No '{0}' defined and unable to find auto-match.",
                        toMemberInfo.GetMemberType().IsClass ? "map" : "conversion"));
                }
            }

            return Tuple.Create(new ReadOnlyCollection<Binding>(autoMemberBindings),
                new ReadOnlyCollection<MemberDefinitionError>(memberDefinitionErrors));
        }

        public ReadOnlyCollection<MemberDefinitionError> Validate()
        {
            // TODO Validate inheritance type is available in Mapper.

            var errors = Bindings.SelectMany(b => BindingValidator.Validate(b, Mapper)).ToList();

            return new ReadOnlyCollection<MemberDefinitionError>(errors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapInto">If true, attemps to map into an existing object graph rather than recreating all children.</param>
        public object Apply(object from, object to = null, bool mapInto = false)
        {
            Require.NotNull(from, "from");

            // Redirect to polymorphic map if there is a match.
            Map polyMap;
            if (TryGetPolymorphicMap(from.GetType(), out polyMap))
                return polyMap.Apply(from, to, mapInto);

            if (to == null || to.GetType() != ProjectionType.ToType)
                to = ConstructedBy(new ConstructionContext(Mapper, from, null));

            foreach (var binding in Bindings)
            {
                binding.Apply(Mapper, from, to, mapInto);
            }

            if (PostMapStep != null)
                PostMapStep(from, to);

            return to;
        }

        /// <summary>
        ///     Attempt to get a polymorphic map for the supplied 'from' type.
        /// </summary>
        /// <param name="fromType">
        ///     The 'from' type that we are considering polymophic, the 'to' type is implied as we do not allow
        ///     polymophism from multiple 'from' types. This would cause ambiguity.
        /// </param>
        /// <param name="map">The map if there is one, otherwise null.</param>
        /// <returns>True if a polymorphic map was defined for the 'from' type, otherwise False.</returns>
        private bool TryGetPolymorphicMap(Type fromType, out Map map)
        {
            var projectionType = PolymorphicFor.FirstOrDefault(pt => pt.FromType == fromType);
            if (projectionType != null)
            {
                map = Mapper.GetMap(projectionType);
                return true;
            }

            map = null;
            return false;
        }
    }

    public class CollectionMap
    {
        private readonly Map itemMap;

        public CollectionMap(Map itemMap)
        {
            Require.NotNull(itemMap, "itemMap");

            this.itemMap = itemMap;
        }

        public ProjectionType ProjectionType
        {
            get
            {
                var fromType = typeof(IEnumerable<>).MakeGenericType(itemMap.ProjectionType.FromType);
                var toType = typeof(IList<>).MakeGenericType(itemMap.ProjectionType.ToType);

                return new ProjectionType(fromType, toType);
            }
        }

        public static bool IsMappable(ProjectionType projectionType)
        {
            return true;
        }

        public object Apply(object from, object to = null, bool mapInto = false)
        {
            Require.NotNull(from, "from");
            Require.IsTrue(from is IEnumerable);
            Require.IsFalse(mapInto, "Map into not currenlty support on collection");

            // need to cope with empty to type - if we pass in the expected type then we could instantiate it if its a concrete collection type.

            var fromEnumerable = (IEnumerable) from;
            var toCollection = (IList) to;

            foreach (var item in fromEnumerable)
            {
                var mappedItem = itemMap.Apply(item);
                toCollection.Add(mappedItem);
            }

            return to;
        }
    }
}