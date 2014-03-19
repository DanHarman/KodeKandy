namespace KodeKandy.Mapnificent.Tests.TestEntities
{
    public class FlatteningFrom
    {
        public FlatteningChildFrom Child { get; set; }

        public class FlatteningChildFrom
        {
            public string Name { get; set; }
        }
    }
}