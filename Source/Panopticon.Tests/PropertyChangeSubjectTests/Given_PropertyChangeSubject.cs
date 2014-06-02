using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.PropertyChangeSubjectTests
{
    [TestFixture]
    public class Given_PropertyChangeSubject : ReactiveTest
    {
        [Test]
        public void When_Disposed_Then_OnCompleted()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new PropertyChangeSubject(this);
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnCompleted<IPropertyChange>(90)
            };
            sut.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(90);
            sut.Dispose();

            // Assert
            observer.Messages.AssertEqual(expected);
        }

        [Test]
        public void When_SetPropertyValue_Then_Fires_PropertyChange_On_Observable_With_Correct_Values()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new PropertyChangeSubject(this);
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnNext(10, (IPropertyChange) PropertyChange.Create(this, 17, "Age", "UserData-1")),
            };
            int age = 10;
            sut.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.SetPropertyValue(ref age, 17, "Age", "UserData-1");
            scheduler.AdvanceTo(90);

            // Assert
            Assert.AreEqual(17, age);
            observer.Messages.AssertEqual(expected);
        }

        [Test]
        public void When_SetPropertyValue_Then_Fires_PropertyChange_On_Event_With_Correct_Values()
        {
            // Arrange            
            var propertyChanges = new List<IPropertyChange>();
            var sut = new PropertyChangeSubject(this);
            sut.PropertyChanged += (sender, args) => propertyChanges.Add((IPropertyChange) args);
            var age = 10;
            var expectedPropertyChange = PropertyChange.Create(this, 17, "Age", "UserData-1");

            // Act            
            sut.SetPropertyValue(ref age, 17, "Age", "UserData-1");
            
            // Assert
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(expectedPropertyChange, propertyChanges[0]);
        }

        [Test]
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChange_On_Observable_With_Correct_Values()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new PropertyChangeSubject(this);
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnNext(30, (IPropertyChange) PropertyChange.Create(this, 70, "Age", "UserData-2"))
            };
            sut.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(30);
            sut.NotifyPropertyValueChanged(70, "Age", "UserData-2");
            scheduler.AdvanceTo(90);

            // Assert
            observer.Messages.AssertEqual(expected);
        }

        [Test]
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChange_On_Event_With_Correct_Values()
        {
            // Arrange            
            var propertyChanges = new List<IPropertyChange>();
            var sut = new PropertyChangeSubject(this);
            sut.PropertyChanged += (sender, args) => propertyChanges.Add((IPropertyChange) args);
            var expectedPropertyChange = PropertyChange.Create(this, 70, "Age", "UserData-2");

            // Act            
            sut.NotifyPropertyValueChanged(70, "Age", "UserData-2");

            // Assert
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(expectedPropertyChange, propertyChanges[0]);
        }
    }
}