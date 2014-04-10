namespace KodeKandy.Mapnificent.Projections
{
    /// <summary>
    /// This is the base class for every type of projection and encompasses both value and reference type projections.
    /// </summary>
    public abstract class Projection : IProjection
    {
        protected Projection(ProjectionType projectionType, Mapper mapper)
        {
            Require.NotNull(projectionType, "projectionType");
            Require.NotNull(mapper, "mapper");

            ProjectionType = projectionType;
            Mapper = mapper;
        }


        public ProjectionType ProjectionType { get; private set; }

        /// <summary>
        ///     The Mapper this projection is associated with.
        /// </summary>
        public Mapper Mapper { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="mapInto">If true, attemps to ClassMap into an existing object graph rather than recreating all children.</param>
        public abstract object Apply(object from, object to = null, bool mapInto = false);
    }
}