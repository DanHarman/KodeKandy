// <copyright file="Given_ObservableObject.cs" company="million miles per hour ltd">
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

using System.Collections.Generic;
using System.ComponentModel;
using KodeKandy.Panopticon.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.ObservableObjectTests
{
    [TestFixture]
    public class Given_ObservableObject
    {
        [Test]
        public void When_Setting_Property_To_New_Value_And_Property_Changed_Suppressed_Then_Does_Not_Fire_PropertyChanged()
        {
            // Arrange            
            var propertyChangedCount = 0;
            var sut = new TestObservableObject {Age = 10};
            sut.PropertyChanged += (sender, args) => ++propertyChangedCount;

            // Act     
            using (sut.SuppressPropertyChanged())
                sut.Age = 3;

            // Assert
            Assert.AreEqual(0, propertyChangedCount);
        }

        [Test]
        public void When_Setting_Property_To_New_Value_Then_Fires_PropertyChanged()
        {
            // Arrange            
            var propertyChanges = new List<PropertyChangedEventArgsEx>();
            var sut = new TestObservableObject {Age = 10};
            sut.PropertyChanged += (sender, args) => propertyChanges.Add((PropertyChangedEventArgsEx) args);
            var expectedPropertyChange = new PropertyChangedEventArgsEx("Age");

            // Act            
            sut.Age = 5;

            // Assert
            Assert.AreEqual(5, sut.Age);
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(expectedPropertyChange, propertyChanges[0]);
        }

        [Test]
        public void When_Setting_Property_To_Same_Value_Then_Does_Not_Fire_PropertyChanged()
        {
            // Arrange            
            var propertyChangedCount = 0;
            var sut = new TestObservableObject {Age = 10};
            sut.PropertyChanged += (sender, args) => ++propertyChangedCount;

            // Act            
            sut.Age = 10;

            // Assert
            Assert.AreEqual(0, propertyChangedCount);
        }
    }
}