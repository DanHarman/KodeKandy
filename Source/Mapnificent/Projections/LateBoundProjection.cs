namespace KodeKandy.Mapnificent.Projections
{
    public class LateBoundProjection : Projection
    {
        public LateBoundProjection(ProjectionType projectionType, Mapper mapper) : base(projectionType, mapper)
        {   
        }

        public override object Apply(object from, object to = null, bool mapInto = false)
        {
            return to;
        }
    }
}