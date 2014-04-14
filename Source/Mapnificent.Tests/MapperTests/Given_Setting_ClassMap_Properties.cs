// <copyright file="Given_Setting_ClassMap_Properties.cs" company="million miles per hour ltd">
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
using KodeKandy.QualityTools;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Setting_ClassMap_Properties
    {
        [Test]
        public void When_ConstructUsing_Set_To_Null_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new ClassMap(ProjectionType.Create<SimpleFrom, SimpleTo>(), mapper);

            // Act & Assert
            KKAssert.Throws<ArgumentNullException>(() => sut.ConstructUsing = null, "Value cannot be null.\r\nParameter name: value");
        }

        [Test]
        public void When_InheritsFrom_From_Type_Not_Supertype_Of_Map_From_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new ClassMap(ProjectionType.Create<CarFrom, CarTo>(), mapper);

            // Act
            KKAssert.Throws<Exception>(() => sut.InheritsFrom = ProjectionType.Create<SimpleFrom, VehicleTo>(),
                "Cannot inherit from a ClassMap whose 'From' type 'SimpleFrom' is not a supertype of this maps 'From' type 'CarFrom'.");
        }

        [Test]
        public void When_InheritsFrom_To_Type_Not_Supertype_Of_Map_To_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new ClassMap(ProjectionType.Create<CarFrom, CarTo>(), mapper);

            // Act
            KKAssert.Throws<Exception>(() => sut.InheritsFrom = ProjectionType.Create<VehicleFrom, SimpleTo>(),
                "Cannot inherit from a ClassMap whose 'To' type 'SimpleTo' is not a supertype of this maps 'To' type 'CarTo'.");
        }
    }
}