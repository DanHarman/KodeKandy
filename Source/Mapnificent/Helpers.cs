namespace KodeKandy.Mapnificent
{
    public static class Helpers
    {
        public static string SafeGetTypeName(this object instance)
        {
            return instance == null ? "<null>" : instance.GetType().Name;
        }
    }
}