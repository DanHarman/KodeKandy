using System;
using System.Collections;

namespace KodeKandy.Mapnificent.Projections
{
    public class ListMap : Map
    {
        public ListMap(ProjectionType projectionType, Mapper mapper) : base(projectionType, mapper)
        {
            Require.IsTrue(projectionType.IsListProjection);

            ItemProjectionType = new ProjectionType(projectionType.FromItemType, projectionType.ToItemType);
        }

        public ProjectionType ItemProjectionType { get; private set; }

        public override object Apply(object from, object to = null, bool mapInto = false)
        {
            Require.NotNull(from, "from");
            Require.IsFalse(mapInto, "mapInto not currenlty support on collection");

            // need to cope with empty to type - if we pass in the expected type then we could instantiate it if its a concrete collection type.

            if (to == null)
                to = ConstructUsing(new ConstructionContext(Mapper, from, null));

            var fromEnumerable = (IEnumerable) from;
            var toCollection = (IList) to;

            // Abstact if it is a map or conversion.
            Func<object, object> projectFunc;

            if (ItemProjectionType.IsClassProjection)
            {
                var itemMap = Mapper.GetMap(ItemProjectionType);
                projectFunc = (f) => itemMap.Apply(f);
            }
            else
            {
                var conversion = Mapper.GetConversion(ItemProjectionType);
                projectFunc = conversion.Apply;
            }

            foreach (var item in fromEnumerable)
            {
                var mappedItem = projectFunc(item);
                toCollection.Add(mappedItem);
            }

            return to;
        }

        public void Validate()
        {
            if (!Mapper.HasProjection(ProjectionType.FromItemType, ProjectionType.ToItemType))
                throw new Exception(string.Format("Mapped not defined for the item type of ListMap '{0}'", ProjectionType));
        }
    }
}