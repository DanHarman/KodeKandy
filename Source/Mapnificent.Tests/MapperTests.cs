// <copyright file="MapperTests.cs" company="million miles per hour ltd">
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
// 
// </copyright>

using System;
using KodeKandy.Mapnificent.Tests.TestEntities.Inheritance;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests
{
    [TestFixture]
    public class MapperTests
    {
        public void When_Mapping_Undefined_Class_Then_Throws()
        {
            var sut = new Mapper();

            //Assert.Throws<MapnificentException>(() => sut.Map<Circle>(new Object()));
        }

        [Test]
        public void Mapping()
        {
            // Given CircleDto
            var sut = new Mapper();
            sut.DefineMap<CircleDto, Circle>()
               .For(to => to.Name, opt => opt.From(from => from.Name));
            //  sut.DefineMap<CircleDto, Circle>(opt => opt.For(to => to.memberName, opt2 => opt2.From(from => from.memberName)));

            var dto = new CircleDto {Name = "Circle", Radius = 15.0};

            // When Mapping
//            var result = sut.Map<Circle>(dto);

            // Assert
        }

        [Test]
        public void Given_Default_Mapping_When_Mapped_Then()
        {
            // Arrange
            var sut = new Mapper();
            sut.DefineMap<ShapeDto, Shape>();
            sut.DefineMap<CircleDto, Circle>();
            //  .Inherits<ShapeDto, Shape>();
            sut.DefineMap<RectangleDto, Rectangle>();

//            var props = ReflectionHelpers.GetProperties(typeof(Rectangle));
            //       var props2 = ReflectionHelpers.GetProperties(typeof(Shape));

            // Act

            // Assert
        }
    }
}