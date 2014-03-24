using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Mapper
    {
        [Test]
        public void When_Instantiated_Then_Has_String_Map()
        {
            // Arrange
            var sut = new Mapper();

            var x = sut.GetMap(typeof(string), typeof(string));
            var y = x.Bindings;

            // Assert
            Assert.IsTrue(sut.HasMap(typeof(string), typeof(string)));
        }

        public void When_Mapping_Undefined_Class_Then_Throws()
        {
            var sut = new Mapper();

         //   Assert.Throws<MapnificentException>(() => sut.MapInto<Simple>(new Object()));
        }
    }
}