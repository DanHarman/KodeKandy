using System;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Inherited_Map
    {
        [Test]
        public void When_Mapping_Derived_Instance_Then_Uses_Inherited_Binding()
        {
            // Arrange
            var sut = new Mapper();

            sut.DefineMap<VehicleFrom, VehicleTo>()
               .For(x => x.Name, o => o.Explictly(_ => "Ferrari"));
            sut.DefineMap<CarFrom, CarTo>()
               .InheritsFrom<VehicleFrom, VehicleTo>();

            var from = new CarFrom() { Name = "Porsche", NoSeats = 4 };
            var to = new CarTo();

            // Act
            sut.MapInto(from, to);

            // Assert
            Assert.AreEqual("Ferrari", to.Name);
            Assert.AreEqual(from.NoSeats, to.NoSeats);
        }

        [Test]
        public void When_Inheritence_From_Type_Is_Not_A_Supertype_Of_From_Declaring_Type_Then_Throws()
        {
            // Arrange
            var sut = new Mapper();

            // Assert
            Assert.Throws<ArgumentException>(() => sut.DefineMap<CarFrom, CarTo>().InheritsFrom<SimpleFrom, VehicleTo>(),
                "Cannot inherit from a map whose 'From' type 'SimpleFrom' is not a supertype of this maps 'From' type 'CarFrom'.");
        }

        [Test]
        public void When_Inheritence_To_Type_Is_Not_A_Supertype_Of_To_Declaring_Type_Then_Throws()
        {
            // Arrange
            var sut = new Mapper();

            // Assert
            Assert.Throws<ArgumentException>(() => sut.DefineMap<CarFrom, CarTo>().InheritsFrom<VehicleFrom, SimpleTo>(),
                "Cannot inherit from a map whose 'To' type 'SimpleTo' is not a supertype of this maps 'To' type 'CarTo'.");
        }
        // TODO verify the inheritance guard enforcement on inheritsfrom.
    }
}