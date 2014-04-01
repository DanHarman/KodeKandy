using System;
using KodeKandy.Mapnificent.Projections;
using KodeKandy.Mapnificent.Tests.TestEntities;
using KodeKandy.QualityTools;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Setting_Map_Properties
    {
        [Test]
        public void When_InheritsFrom_From_Type_Not_Supertype_Of_Map_From_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new Map(ProjectionType.Create<CarFrom, CarTo>(), mapper);

            // Act
            KKAssert.Throws<Exception>(() => sut.InheritsFrom = ProjectionType.Create<SimpleFrom, VehicleTo>(),
                "Cannot inherit from a map whose 'From' type 'SimpleFrom' is not a supertype of this maps 'From' type 'CarFrom'.");
        }

        [Test]
        public void When_InheritsFrom_To_Type_Not_Supertype_Of_Map_To_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new Map(ProjectionType.Create<CarFrom, CarTo>(), mapper);

            // Act
            KKAssert.Throws<Exception>(() => sut.InheritsFrom = ProjectionType.Create<VehicleFrom, SimpleTo>(),
                "Cannot inherit from a map whose 'To' type 'SimpleTo' is not a supertype of this maps 'To' type 'CarTo'.");
        }

        [Test]
        public void When_PolymorphicFor_From_Type_Not_Subtype_Of_Map_From_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new Map(ProjectionType.Create<VehicleFrom, VehicleTo>(), mapper);

            // Act
            KKAssert.Throws<Exception>(() => sut.AddPolymorphicFor(ProjectionType.Create<SimpleFrom, CarTo>()),
                "Cannot be polymorphic for a map whose 'From' type 'SimpleFrom' is not a subtype of this maps 'From' type 'VehicleFrom'.");
        }

        [Test]
        public void When_PolymorphicFor_To_Type_Not_Subtype_Of_Map_To_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new Map(ProjectionType.Create<VehicleFrom, VehicleTo>(), mapper);

            // Act
            KKAssert.Throws<Exception>(() => sut.AddPolymorphicFor(ProjectionType.Create<CarFrom, SimpleTo>()),
                "Cannot be polymorphic for a map whose 'To' type 'SimpleTo' is not a subtype of this maps 'To' type 'VehicleTo'.");
        }

        /// <summary>
        ///     This needs to be verified as in polymophic scenarios we don't know the 'to' type so must infer it form the 'form'
        ///     type, hence only one polymophic definition per 'from' type.
        /// </summary>
        [Test]
        public void When_PolymorphicFor_From_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new Map(ProjectionType.Create<VehicleFrom, VehicleTo>(), mapper);
            sut.AddPolymorphicFor(ProjectionType.Create<CarFrom, CarTo>());

            // Act
            KKAssert.Throws<Exception>(() => sut.AddPolymorphicFor(ProjectionType.Create<CarFrom, VehicleTo>()),
                "Illegal 'polymorphic for' defintion. A definition has already been registered for the 'from' type 'CarFrom' and would be made ambiguous by this one.");
        }

        [Test]
        public void When_AddPolymorphicFor_Then_Is_Added_To_Map()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new Map(ProjectionType.Create<VehicleFrom, VehicleTo>(), mapper);
            var expected = ProjectionType.Create<CarFrom, CarTo>();

            // Act
            sut.AddPolymorphicFor(expected);

            // Assert
            CollectionAssert.Contains(sut.PolymorphicFor, expected);
        }

        [Test]
        public void When_ConstructedBy_Set_To_Null_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new Map(ProjectionType.Create<SimpleFrom, SimpleTo>(), mapper);

            // Act & Assert
            KKAssert.Throws<ArgumentNullException>(() => sut.ConstructedBy = null, "Value cannot be null.\r\nParameter name: value");
        }
    }
}