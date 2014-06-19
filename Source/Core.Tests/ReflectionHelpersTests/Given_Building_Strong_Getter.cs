// <copyright file="Given_StrongGetter.cs" company="million miles per hour ltd">
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
using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.ReflectionHelpersTests
{
    [TestFixture]
    public class Given_Building_Strong_Getter
    {
        [Test]
        public void When_CreatePropertyGetter_Delegate_Used_Then_Succeeds()
        {
            // Arrange
            var propertyInfo = typeof(Shape).GetProperty("Edges");
            const int expectedEdges = 7;
            var shape = new Shape() {Edges = expectedEdges};

            // Act
            var sut = (Func<Shape, int>) ReflectionHelpers.CreatePropertyGetter(propertyInfo);
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedEdges, res);
        }

        [Test]
        public void When_CreateMemberGetter_For_Property_Delegate_Used_Then_Succeeds()
        {
            // Arrange
            var propertyInfo = typeof(Shape).GetProperty("Edges");
            const int expectedEdges = 7;
            var shape = new Shape() { Edges = expectedEdges };

            // Act
            var sut = (Func<Shape, int>)ReflectionHelpers.CreateMemberGetter(propertyInfo);
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedEdges, res);
        }

        [Test]
        public void When_CreateFieldGetter_Delegate_Used_Then_Succeeds()
        {
            // Arrange
            var fieldInfo = typeof(Shape).GetField("Id");
            const int expectedId = 7;
            var shape = new Shape() { Id = expectedId };

            // Act
            var sut = (Func<Shape, int>)ReflectionHelpers.CreateFieldGetter(fieldInfo);
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedId, res);
        }

        [Test]
        public void When_CreateMemberGetter_For_Field_Delegate_Used_Then_Succeeds()
        {
            // Arrange
            var fieldInfo = typeof(Shape).GetField("Id");
            const int expectedId = 7;
            var shape = new Shape() { Id = expectedId };

            // Act
            var sut = (Func<Shape, int>)ReflectionHelpers.CreateMemberGetter(fieldInfo);
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedId, res);
        }
    }
}