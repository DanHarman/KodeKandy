using System.Linq;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Auto_Generated_Map
    {
        [Test]
        public void When_Enumerating_Bindings_Then_MemberDefinitiontType_Is_Auto()
        {
            // Arrange
            var sut = new Mapper();
            var map = sut.DefineMap<SimpleFrom, SimpleTo>().Map;
           
            // Act
            var res = map.Bindings;

            // Assert
            Assert.IsTrue(res.All(b => b.MemberBindingDefinitionType == MemberBindingDefinitionType.Auto));
        }

        [Test]
        public void When_Mapping_Simple_Class_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.DefineMap<SimpleFrom, SimpleTo>();
            var from = new SimpleFrom() {Name = "Bob", Age = 20};
            var to = new SimpleTo();

            // Act
            sut.MapInto(from, to);

            // Assert
            Assert.AreEqual(from.Name, to.Name);
            Assert.AreEqual(from.Age, to.Age);
        }

        [Test]
        public void When_Mapping_Flattening_Class_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.DefineMap<FlatteningFrom, FlatteningTo>();
            var from = new FlatteningFrom { Child = new FlatteningFrom.FlatteningChildFrom { Name = "Bob" } };
            var to = new FlatteningTo();

            // Act
            sut.MapInto(from, to);

            // Assert
            Assert.AreEqual(from.Child.Name, to.ChildName);
        }

        [Test]
        public void When_Mapping_Nested_Class_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.DefineMap<NestedFrom, NestedTo>();
            sut.DefineMap<NestedFrom.NestedChildFrom, NestedTo.NestedChildTo>();
            var from = new NestedFrom { Child = new NestedFrom.NestedChildFrom { Name = "Bob" } };
            var to = new NestedTo { Child = new NestedTo.NestedChildTo() };

            // Act
            sut.MapInto(from, to);

            // Assert
            Assert.AreEqual(from.Child.Name, to.Child.Name);
            Assert.AreNotEqual(from.Child, to.Child); // Ensure mapping of class not ref copying!
        }

        [Test]
        public void When_Mapping_With_Conversion_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.DefineMap<ConversionFrom, ConversionTo>();
            sut.DefineConversion<string, int>().Explictly(x => x.Count());
            var from = new ConversionFrom { Age = "Twelve" };
            var to = new ConversionTo { Age = 6 };

            // Act
            sut.MapInto(from, to);

            // Assert
            Assert.AreEqual(from.Age.Count(), to.Age);
        }

        // TODO MapInto from a value type to a ref type.
    }
}