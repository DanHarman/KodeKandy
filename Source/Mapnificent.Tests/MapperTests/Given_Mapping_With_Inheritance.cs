using System;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Mapping_With_Inheritance
    {
        [Test]
        public void When_Mapping_Derived_Instance_Then_Uses_Inherited_Binding()
        {
            // Arrange
            var sut = new Mapper();

            sut.DefineMap<VehicleFrom, VehicleTo>()
               .For(x => x.Name, o => o.Custom(_ => "Ferrari"));
            sut.DefineMap<CarFrom, CarTo>()
               .InheritsFrom<VehicleFrom, VehicleTo>();

            var from = new CarFrom() {Name = "Porsche", NumSeats = 4};
            var to = new CarTo();

            // Act
            sut.MapInto(from, to);

            // Assert
            Assert.AreEqual("Ferrari", to.Name);
            Assert.AreEqual(from.NumSeats, to.NumSeats);
        }
    }
}