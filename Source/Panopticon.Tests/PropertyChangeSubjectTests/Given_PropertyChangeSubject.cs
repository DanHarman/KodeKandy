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
        public void When_SetPropertyValue_Then_Fires_PropertyChange_With_Correct_Values()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new PropertyChangeSubject(this);
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnNext(10, PropertyChange.Create(this, 17, "Age", "UserData-1")),
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
        public void When_SetPropertyValue_Then_Fires_PropertyChangedEventArgs_With_Correct_Values()
        {
            // Arrange            
            int age = 10;
            var propertyChanges = new List<Tuple<object, PropertyChangedEventArgs, int>>();
            PropertyChangedEventHandler propertyChanged = (sender, args) => propertyChanges.Add(Tuple.Create(sender, args, age));
            var sut = new PropertyChangeSubject(this, () => propertyChanged);
            
            // Act            
            sut.SetPropertyValue(ref age, 17, "Age", "UserData-1");
            
            // Assert
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(17, age);
            Assert.AreEqual(this, propertyChanges[0].Item1);
            Assert.AreEqual("Age", propertyChanges[0].Item2.PropertyName);
            Assert.AreEqual(17, propertyChanges[0].Item3);
        }

        [Test]
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChange_With_Correct_Values()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new PropertyChangeSubject(this);
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnNext(30, PropertyChange.Create(this, 70, "Age", "UserData-2"))
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
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChangedEventArgs_With_Correct_Values()
        {
            // Arrange            
            var propertyChanges = new List<Tuple<object, PropertyChangedEventArgs>>();
            PropertyChangedEventHandler propertyChanged = (sender, args) => propertyChanges.Add(Tuple.Create(sender, args));
            var sut = new PropertyChangeSubject(this, () => propertyChanged);

            // Act            
            sut.NotifyPropertyValueChanged(70, "Age", "UserData-2");

            // Assert
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(this, propertyChanges[0].Item1);
            Assert.AreEqual("Age", propertyChanges[0].Item2.PropertyName);            
        }
    }
}