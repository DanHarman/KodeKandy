using System.Linq;
using KodeKandy.Mapnificent.Projections;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    public class Given_Building_PolymorphicMap
    {
        [Test]
        public void When_AddPolymorph_Then_Is_Added_To_Map()
        {
            // Arrange
            var map = new PolymorphicMap(ProjectionType.Create<VehicleFrom, VehicleTo>(), new Mapper());
            var sut = new PolymorphicMapBuilder<VehicleFrom, VehicleTo>(map);
            var expected = ProjectionType.Create<CarFrom, CarTo>();

            // Act
            sut.AddPolymorph<CarFrom, CarTo>();

            // Assert
            Assert.NotNull(map.Polymorphs.Single(x => x.ProjectionType == expected));
        }
    }
}