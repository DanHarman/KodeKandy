using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Mapping_With_Polymorphism
    {
        [Test]
        public void When_Mapping_Derived_Instance_As_Base_Then_Maps_Polymorphically_As_Derived()
        {
            // Arrange
            var sut = new Mapper();

            sut.DefineMap<VehicleFrom, VehicleTo>()
               .For(x => x.Name, o => o.Custom(_ => "Ferrari"))
               .PolymorhpicFor<CarFrom, CarTo>();

            sut.DefineMap<CarFrom, CarTo>()
               .InheritsFrom<VehicleFrom, VehicleTo>();

            var from = new CarFrom() { Name = "Porsche", NumSeats = 4 };

            // Act
            var to = sut.Map<VehicleFrom, VehicleTo>(from);
            var res = to as CarTo;

            // Assert
            Assert.NotNull(res);
            Assert.AreEqual("Ferrari", res.Name);
            Assert.AreEqual(from.NumSeats, res.NumSeats);
        }
    }
}