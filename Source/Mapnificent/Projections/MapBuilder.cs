using System;

namespace KodeKandy.Mapnificent.Projections
{
    public abstract class MapBuilder<TFromDeclaring, TToDeclaring>
    {
        public IMap Map { get; private set; }

        protected MapBuilder(IMap map)
        {
            Map = map;
        }

        public MapBuilder<TFromDeclaring, TToDeclaring> PostMapStep(Action<TFromDeclaring, TToDeclaring> afterMappingAction)
        {
            Require.NotNull(afterMappingAction, "afterMappingAction");

            Map.PostMapStep = (f, t) => afterMappingAction((TFromDeclaring)f, (TToDeclaring)t);

            return this;
        }
    }
}