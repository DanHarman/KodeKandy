namespace KodeKandy.Panopticon.Tests.TestEntities
{
    class TestObservableObject : ObservableObject
    {
        private int age;
        private TestObservableObject child;
        public int Age
        {
            get { return age; }
            set { SetValue(ref age, value); }
        }

        public TestObservableObject Child
        {
            get { return child; }
            set { SetValue(ref child, value); }
        }
    }
}