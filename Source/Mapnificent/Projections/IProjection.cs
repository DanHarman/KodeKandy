namespace KodeKandy.Mapnificent.Projections
{
    public interface IProjection
    {
        /// <summary>
        /// Applies a projection.
        /// </summary>
        /// <param name="from">The 'from' class to project from.</param>
        /// <param name="to">The 'to' class to project into, this is only set when mapping into an existing hierarchy, or a ConstructUsing override has been set at the
        /// the parent level.</param>
        /// <param name="mapInto">If true, attemps to ClassMap into an existing object graph rather than recreating all children.</param>
        /// <returns>The result of the projection, which may be the 'to' value if that was mapped into, otherwise it is a new value.</returns>
        object Apply(object from, object to = null, bool mapInto = false);        
    }
}