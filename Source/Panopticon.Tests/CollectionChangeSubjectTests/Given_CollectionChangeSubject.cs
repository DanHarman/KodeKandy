// <copyright file="Given_CollectionChangeSubject.cs" company="million miles per hour ltd">
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
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.CollectionChangeSubjectTests
{
    [TestFixture]
    public class Given_CollectionChangeSubject : ReactiveTest
    {
        [Test]
        public void When_Disposed_Then_OnCompleted()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new CollectionChangeSubject<int>(this);
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
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChange_With_Correct_Values()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new CollectionChangeSubject<int>(this);
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
        public void When_NotifyPropertyValueChanged_Then_Fires_PropertyChangedEventArgs_With_Correct_Values()
        {
            // Arrange            
            var sut = new CollectionChangeSubject<int>(this);
            var propertyChanges = new List<Tuple<object, PropertyChangedEventArgs>>();
            sut.PropertyChanged += (sender, args) => propertyChanges.Add(Tuple.Create(sender, args));


            // Act            
            sut.NotifyPropertyValueChanged(70, "Age", "UserData-2");

            // Assert
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(this, propertyChanges[0].Item1);
            Assert.AreEqual("Age", propertyChanges[0].Item2.PropertyName);
        }

        [Test]
        public void When_SetPropertyValue_Then_Fires_PropertyChange_With_Correct_Values()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new CollectionChangeSubject<int>(this);
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
        public void When_SetPropertyValue_Then_Fires_PropertyChangedEventArgs_With_Correct_Values()
        {
            // Arrange            
            int age = 10;
            var propertyChanges = new List<Tuple<object, PropertyChangedEventArgs, int>>();
            var sut = new CollectionChangeSubject<int>(this);
            sut.PropertyChanged += (sender, args) => propertyChanges.Add(Tuple.Create(sender, args, age));

            // Act            
            sut.SetPropertyValue(ref age, 17, "Age", "UserData-1");

            // Assert
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual(17, age);
            Assert.AreEqual(this, propertyChanges[0].Item1);
            Assert.AreEqual("Age", propertyChanges[0].Item2.PropertyName);
            Assert.AreEqual(17, propertyChanges[0].Item3);
        }
    }
}