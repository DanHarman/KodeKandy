﻿using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.PropertyChangeHelperTests
{
    [TestFixture]
    public class Given_PropertyChangeHelper : ReactiveTest
    {
        [Test]
        public void When_SetPropertyValue_New_Value_Then_Fires_PropertyChanged()
        {
            // Arrange            
            var propertyChanges = new List<PropertyChangedEventArgsEx>();
            var sut = new PropertyChangeHelper(this);
            sut.PropertyChanged += (sender, args) => propertyChanges.Add((PropertyChangedEventArgsEx)args);
            var age = 10;
            var expectedPropertyChange = new PropertyChangedEventArgsEx(this, "Age", "UserData-1");

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
            sut.PropertyChanged += (sender, args) => ++eventCount; ;
            var age = 10;

            // Act            
            sut.SetPropertyValue(ref age, 10, "Age", "UserData-1");

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
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChange_On_Event_With_Correct_Values()
        {
            // Arrange            
            var propertyChanges = new List<PropertyChangedEventArgsEx>();
            var sut = new PropertyChangeHelper(this);
            sut.PropertyChanged += (sender, args) => propertyChanges.Add((PropertyChangedEventArgsEx)args);
            var expectedPropertyChange = new PropertyChangedEventArgsEx(this, "Age", "UserData-2"); 

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
    }
}