// <copyright file="Given_Mapping_With_Inheritance.cs" company="million miles per hour ltd">
// Copyright (c) 2013-2014 All Right Reserved
// 
// This source is subject to the MIT License.
// Please see the License.txt file for more information.
// All other rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>

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

            sut.BuildClassMap<VehicleFrom, VehicleTo>()
               .For(x => x.Name, o => o.Custom(_ => "Ferrari"));
            sut.BuildClassMap<CarFrom, CarTo>()
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