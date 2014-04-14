using System;
using System.Linq;
using KodeKandy.Mapnificent.Builders;
using KodeKandy.Mapnificent.Projections;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.BuilderTests
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

        [Test]
        public void When_AfterMapping_Then_Is_Set_On_Map()
        {
            // Arrange
            var map = new PolymorphicMap(ProjectionType.Create<VehicleFrom, VehicleTo>(), new Mapper());
            var sut = new PolymorphicMapBuilder<VehicleFrom, VehicleTo>(map);
            var flag = false;
            var postMapStep = (Action<VehicleFrom, VehicleTo>)((_, __) => flag = true);

            // Act
            sut.AfterMapping(postMapStep);
            map.AfterMapping(null, null);

            // Assert
            Assert.IsTrue(flag);
        }
    }
}