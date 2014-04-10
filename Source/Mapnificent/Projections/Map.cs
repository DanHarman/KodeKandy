using System;

namespace KodeKandy.Mapnificent.Projections
{
    public abstract class Map : IMap
    {
        private Func<ConstructionContext, object> constructUsing;

        protected Map(ProjectionType projectionType, Mapper mapper)
        {
            Require.NotNull(projectionType, "projectionType");
            Require.NotNull(mapper, "mapper");

            ProjectionType = projectionType;
            Mapper = mapper;

            // We can only create a default constructor if the toType is concrete.
            if (!projectionType.ToType.IsInterface)
                ConstructUsing = _ => Activator.CreateInstance(ProjectionType.ToType);
        }

        #region IMap Members

        public ProjectionType ProjectionType { get; private set; }

        /// <summary>
        ///     The Mapper this ClassMap is associated with.
        /// </summary>
        public Mapper Mapper { get; private set; }

        public Func<ConstructionContext, object> ConstructUsing
        {
            get { return constructUsing; }
            set
            {
                Require.NotNull(value, "value");
                constructUsing = value;
            }
        }

        /// <summary>
        ///     Action applied to the mapping target after mapping has been performed.
        /// </summary>
        public Action<object, object> PostMapStep { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapInto">If true, attemps to ClassMap into an existing object graph rather than recreating all children.</param>
        public abstract object Apply(object from, object to = null, bool mapInto = false);

        #endregion
    }
}