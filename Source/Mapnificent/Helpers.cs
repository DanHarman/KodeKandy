namespace KodeKandy.Mapnificent
{
    public static class Helpers
    {
        /// <summary>
        /// Safely get the name of a type returning <![CDATA[<null>]]> if the instance is null.
        /// </summary>
        /// <param name="instance">Instance to return the type of.</param>
        /// <returns>The name of the type or <![CDATA[<null>]]>.</returns>
        public static string SafeGetTypeName(this object instance)
        {
            return instance == null ? "<null>" : instance.GetType().Name;
        }
    }
}