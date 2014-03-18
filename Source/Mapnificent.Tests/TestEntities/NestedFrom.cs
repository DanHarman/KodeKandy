namespace KodeKandy.Mapnificent.Tests.TestEntities
{
    public class NestedFrom
    {
        public NestedChildFrom Child { get; set; }

        public class NestedChildFrom
        {
            public string Name { get; set; }
        }
    }
}