// <copyright file="Given_PropertyValueChanged.cs" company="million miles per hour ltd">
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

using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests
{
    [TestFixture]
    public class Given_PropertyValueChanged
    {
        [Test]
        public void When_Created_With_Value_Then_Properties_Correct()
        {
            // Arrange
            var expectedSource = new object();
            const string expectedPropertyName = "Name";
            const string expectedPropertyValue = "Ana";

            // Act
            var sut = new PropertyValueChanged<string>(expectedSource, expectedPropertyName, expectedPropertyValue);

            // Assert
            Assert.AreEqual(expectedSource, sut.Source);
            Assert.AreEqual(expectedPropertyName, sut.PropertyChangedEventArgs.PropertyName);
            Assert.AreEqual(expectedPropertyValue, sut.Value);
            Assert.IsTrue(sut.HasValue);
        }

        [Test]
        public void When_Created_Without_Value_Then_Properties_Correct()
        {
            // Arrange
            var expectedSource = new object();
            const string expectedPropertyName = "Name";

            // Act
            var sut = new PropertyValueChanged<string>(expectedSource, expectedPropertyName);

            // Assert
            Assert.AreEqual(expectedSource, sut.Source);
            Assert.AreEqual(expectedPropertyName, sut.PropertyChangedEventArgs.PropertyName);
            Assert.IsFalse(sut.HasValue);
        }

        [Test]
        public void When_Evaluating_Equality_And_Different_Property_Name_Then_False()
        {
            // Arrange
            var expectedSource = new object();
            var a = new PropertyValueChanged<string>(expectedSource, "Name", "Rose");
            var b = new PropertyValueChanged<string>(expectedSource, "Name", "Belle");

            // Act
            var res = a.Equals(b);

            // Assert
            Assert.IsFalse(res);
        }

        [Test]
        public void When_Evaluating_Equality_And_One_Without_Value_Then_False()
        {
            // Arrange
            var expectedSource = new object();
            var a = new PropertyValueChanged<string>(expectedSource, "Name", "Rose");
            var b = new PropertyValueChanged<string>(expectedSource, "Name");

            // Act
            var res = a.Equals(b);

            // Assert
            Assert.IsFalse(res);
        }

        [Test]
        public void When_Evaluating_Equality_And_Same_Then_False()
        {
            // Arrange
            var expectedSource = new object();
            var a = new PropertyValueChanged<string>(expectedSource, "Name", "Rose");
            var b = new PropertyValueChanged<string>(expectedSource, "Name", "Rose");

            // Act
            var res = a.Equals(b);

            // Assert
            Assert.IsTrue(res);
        }
    }
}