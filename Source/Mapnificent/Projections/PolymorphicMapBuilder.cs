using System;

namespace KodeKandy.Mapnificent.Projections
{
    public class PolymorphicMapBuilder<TFromDeclaring, TToDeclaring> : MapBuilder<TFromDeclaring, TToDeclaring>
        where TToDeclaring : class
    {
        public PolymorphicMapBuilder(PolymorphicMap classMap)
            : base(classMap)
        {
            Require.IsTrue(classMap.ProjectionType.FromType == typeof(TFromDeclaring));
            Require.IsTrue(classMap.ProjectionType.ToType == typeof(TToDeclaring));
        }

        public new PolymorphicMap Map { get { return (PolymorphicMap) base.Map; } }

        public new PolymorphicMapBuilder<TFromDeclaring, TToDeclaring> PostMapStep(Action<TFromDeclaring, TToDeclaring> afterMappingAction)
        {
            return (PolymorphicMapBuilder<TFromDeclaring, TToDeclaring>) base.PostMapStep(afterMappingAction);
        }

        public PolymorphicMapBuilder<TFromDeclaring, TToDeclaring> PolymorhpicFor<TFromDerived, TToDerived>()
        {
            Map.AddPolymorph(ProjectionType.Create<TFromDerived, TToDerived>());

            return this;
        }
    }
}