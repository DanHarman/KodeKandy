// <copyright file="Given_PropertyChangeHelper.cs" company="million miles per hour ltd">
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
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.PropertyChangeHelperTests
{
    [TestFixture]
    public class Given_PropertyChangeHelper : ReactiveTest
    {
        [Test]
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChange_On_Event_With_Correct_Values()
        {
            // Arrange            
            var propertyChanges = new List<PropertyChangedEventArgsEx>();
            var sut = new PropertyChangeHelper(this);
            sut.PropertyChanged += (sender, args) => propertyChanges.Add((PropertyChangedEventArgsEx) args);
            var expectedPropertyChange = new PropertyChangedEventArgsEx("Age", "UserData-2");

            // Act            
            sut.NotifyPropertyChanged("Age", "UserData-2");

            // Assert
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(expectedPropertyChange, propertyChanges[0]);
        }

        [Test]
        public void When_PropertyChanged_Suppressed_Then_NotifyPropertyChanged_Does_Not_Fire_PropertyChanged()
        {
            // Arrange
            var eventCount = 0;
            var sut = new PropertyChangeHelper(this);
            sut.PropertyChanged += (sender, args) => ++eventCount;

            // Act
            using (sut.SuppressPropertyChanged())
                sut.NotifyPropertyChanged("Age", "UserData-1");

            // Assert
            Assert.AreEqual(0, eventCount);
        }

        [Test]
        public void When_PropertyChanged_Suppressed_Then_SetPropertyValue_Does_Not_Fire_PropertyChanged()
        {
            // Arrange
            var eventCount = 0;
            var sut = new PropertyChangeHelper(this);
            sut.PropertyChanged += (sender, args) => ++eventCount;
            var age = 10;

            // Act
            using (sut.SuppressPropertyChanged())
                sut.SetPropertyValue(ref age, 17, "Age", "UserData-1");

            // Assert
            Assert.AreEqual(0, eventCount);
        }

        [Test]
        public void When_SetPropertyValue_New_Value_Then_Fires_PropertyChanged()
        {
            // Arrange            
            var propertyChanges = new List<PropertyChangedEventArgsEx>();
            var sut = new PropertyChangeHelper(this);
            sut.PropertyChanged += (sender, args) => propertyChanges.Add((PropertyChangedEventArgsEx) args);
            var age = 10;
            var expectedPropertyChange = new PropertyChangedEventArgsEx("Age", "UserData-1");

            // Act            
            sut.SetPropertyValue(ref age, 17, "Age", "UserData-1");

            // Assert
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(expectedPropertyChange, propertyChanges[0]);
        }

        [Test]
        public void When_SetPropertyValue_Same_Value_Then_Does_Not_Fire_PropertyChanged()
        {
            // Arrange            
            var eventCount = 0;
            var sut = new PropertyChangeHelper(this);
            sut.PropertyChanged += (sender, args) => ++eventCount;
            ;
            var age = 10;

            // Act            
            sut.SetPropertyValue(ref age, 10, "Age", "UserData-1");

            // Assert
            Assert.AreEqual(0, eventCount);
        }
    }
}