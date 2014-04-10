// <copyright file="Given_Setting_PolymorphicMap_Properties.cs" company="million miles per hour ltd">
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
    public class Given_Setting_PolymorphicMap_Properties
    {
        [Test]
        public void When_PolymorphicFor_From_Type_Not_Subtype_Of_Map_From_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new PolymorphicMap(ProjectionType.Create<VehicleFrom, VehicleTo>(), mapper);

            // Act
            KKAssert.Throws<Exception>(() => sut.AddPolymorph(ProjectionType.Create<SimpleFrom, CarTo>()),
                "Cannot be polymorphic for a Projection whose 'from' type 'SimpleFrom' is not a subtype of this maps 'from' type 'VehicleFrom'.");
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
            var sut = new PolymorphicMap(ProjectionType.Create<VehicleFrom, VehicleTo>(), mapper);
            sut.AddPolymorph(ProjectionType.Create<CarFrom, CarTo>());

            // Act
            KKAssert.Throws<Exception>(() => sut.AddPolymorph(ProjectionType.Create<CarFrom, VehicleTo>()),
                "Illegal polymorph defintion. A definition has already been registered for the 'from' type 'CarFrom' and would be made ambiguous by this one.");
        }

        [Test]
        public void When_PolymorphicFor_To_Type_Not_Subtype_Of_Map_To_Type_Then_Throws()
        {
            // Arrange
            var mapper = new Mapper();
            var sut = new PolymorphicMap(ProjectionType.Create<VehicleFrom, VehicleTo>(), mapper);

            // Act
            KKAssert.Throws<Exception>(() => sut.AddPolymorph(ProjectionType.Create<CarFrom, SimpleTo>()),
                "Cannot be polymorphic for a Projection whose 'to' type 'SimpleTo' is not a subtype of this maps 'to' type 'VehicleTo'.");
        }
    }
}