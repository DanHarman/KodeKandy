﻿namespace KodeKandy.Mapnificent.Tests.TestEntities
{
    public class NestedTo
    {
        public NestedChildTo Child { get; set; }

        public class NestedChildTo
        {
            public string Name { get; set; }
        }
    }
}