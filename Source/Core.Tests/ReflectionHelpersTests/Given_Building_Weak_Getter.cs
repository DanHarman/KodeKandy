// <copyright file="Given_Building_Weak_Getter.cs" company="million miles per hour ltd">
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

using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.ReflectionHelpersTests
{
    [TestFixture]
    public class Given_Building_Weak_Getter
    {
        [Test]
        public void When_CreateWeakFieldGetter_Delegate_Used_Then_Suceeds()
        {
            // Arrange
            var fieldInfo = typeof(Shape).GetField("Id");
            var sut = ReflectionHelpers.CreateWeakFieldGetter(fieldInfo);
            const int expectedId = 777;
            var shape = new Shape() {Id = expectedId};

            // Act
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedId, res);
        }

        [Test]
        public void When_CreateWeakMemberGetter_For_Field_Delegate_Used_Then_Suceeds()
        {
            // Arrange
            var fieldInfo = typeof(Shape).GetField("Id");
            var sut = ReflectionHelpers.CreateWeakMemberGetter(fieldInfo);
            const int expectedId = 777;
            var shape = new Shape() {Id = expectedId};

            // Act
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedId, res);
        }

        [Test]
        public void When_CreateWeakMemberGetter_For_Property_Delegate_Used_Then_Suceeds()
        {
            // Arrange
            var propertyInfo = typeof(Shape).GetProperty("Name");
            var sut = ReflectionHelpers.CreateWeakMemberGetter(propertyInfo);
            const string expectedName = "Cube";
            var shape = new Shape() {Name = expectedName};

            // Act
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedName, res);
        }

        [Test]
        public void When_CreateWeakPropertyGetter_Delegate_Used_Then_Suceeds()
        {
            // Arrange
            var propertyInfo = typeof(Shape).GetProperty("Name");
            var sut = ReflectionHelpers.CreateWeakPropertyGetter(propertyInfo);
            const string expectedName = "Cube";
            var shape = new Shape() {Name = expectedName};

            // Act
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedName, res);
        }
    }
}