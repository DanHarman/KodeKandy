// <copyright file="Given_Building_Map_With_MapBuilder.cs" company="million miles per hour ltd">
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

using System;
using KodeKandy.Mapnificent.Projections;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Building_Map_With_MapBuilder
    {
        [Test]
        public void When_ConstructUsing_Then_Is_Set_On_Map()
        {
            // Arrange
            var map = new ClassMap(ProjectionType.Create<VehicleFrom, VehicleTo>(), new Mapper());
            var sut = new ClassMapBuilder<VehicleFrom, VehicleTo>(map);
            var flag = false;
            var constructor = (Func<ConstructionContext, VehicleTo>) (_ =>
            {
                flag = true;
                return new VehicleTo();
            });

            // Act
            // Have to do this in indirect fashion as on the map the typing is removed with a wrapping delegate on object.
            sut.ConstructUsing(constructor);
            map.ConstructUsing(null);

            // Assert
            Assert.IsTrue(flag);
        }

        [Test]
        public void When_InheritsFrom_Then_Is_Set_On_Map()
        {
            // Arrange
            var map = new ClassMap(ProjectionType.Create<CarFrom, CarTo>(), new Mapper());
            var sut = new ClassMapBuilder<CarFrom, CarTo>(map);
            var expected = ProjectionType.Create<VehicleFrom, VehicleTo>();

            // Act
            sut.InheritsFrom<VehicleFrom, VehicleTo>();

            // Assert
            Assert.AreEqual(expected, map.InheritsFrom);
        }

        [Test]
        public void When_PolymorphicFor_Then_Is_Added_To_Map()
        {
            // Arrange
            var map = new ClassMap(ProjectionType.Create<VehicleFrom, VehicleTo>(), new Mapper());
            var sut = new ClassMapBuilder<VehicleFrom, VehicleTo>(map);
            var expected = ProjectionType.Create<CarFrom, CarTo>();

            // Act
            sut.PolymorhpicFor<CarFrom, CarTo>();

            // Assert
            CollectionAssert.Contains(map.PolymorphicFor, expected);
        }

        [Test]
        public void When_PostMapStep_Then_Is_Set_On_Map()
        {
            // Arrange
            var map = new ClassMap(ProjectionType.Create<VehicleFrom, VehicleTo>(), new Mapper());
            var sut = new ClassMapBuilder<VehicleFrom, VehicleTo>(map);
            var flag = false;
            var postMapStep = (Action<VehicleFrom, VehicleTo>) ((_, __) => flag = true);

            // Act
            sut.PostMapStep(postMapStep);
            map.PostMapStep(null, null);

            // Assert
            Assert.IsTrue(flag);
        }
    }
}