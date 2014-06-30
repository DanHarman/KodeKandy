// <copyright file="Given_NotifyPropertyChangedObservable.cs" company="million miles per hour ltd">
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
using KodeKandy.Panopticon.Linq;
using KodeKandy.Panopticon.Linq.ObservableImpl;
using KodeKandy.Panopticon.Tests.TestEntities;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq.ObservableImpl
{
    [TestFixture]
    public class Given_NotifyPropertyChangedObservable : ReactiveTest
    {
        [Test]
        public void When_Source_Observable_Completes_Then_Completes()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sourceObs = scheduler.CreateColdObservable(
                OnCompleted<PropertyValueChanged<TestObservableObject>>(40)
                );
            var observer = scheduler.CreateObserver<PropertyChanged>();
            var expected = new[]
            {
                OnCompleted<PropertyChanged>(40)
            };

            var sut = new NotifyPropertyChangedObservable<TestObservableObject>(sourceObs);

            // Act
            sut.Subscribe(observer);
            scheduler.Start();

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Source_Observable_Errors_Then_Errors()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var expectedException = new Exception("Expected");
            var sourceObs = scheduler.CreateColdObservable(
                OnError<PropertyValueChanged<TestObservableObject>>(40, expectedException)
                );
            var observer = scheduler.CreateObserver<PropertyChanged>();
            var expected = new[]
            {
                OnError<PropertyChanged>(40, expectedException)
            };

            var sut = new NotifyPropertyChangedObservable<TestObservableObject>(sourceObs);

            // Act            
            sut.Subscribe(observer);
            scheduler.Start();

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_Twice_Then_Correct_Notifications_For_Both_Observers()
        {
            // Arrange
            var obj = new TestObservableObject {Age = 3};
            var scheduler = new TestScheduler();
            var firstObserver = scheduler.CreateObserver<PropertyChanged>();
            var secondObserver = scheduler.CreateObserver<PropertyChanged>();
            var firstObservserExpected = new[]
            {
                OnNext(0, new PropertyChanged(obj)),
                OnNext(0, new PropertyChanged(obj, "Age")),
                OnNext(20, new PropertyChanged(obj, "Age")),
                OnNext(50, new PropertyChanged(obj, "Age")),
            };

            var secondObservserExpected = new[]
            {
                OnNext(50, new PropertyChanged(obj)),
                OnNext(50, new PropertyChanged(obj, "Age")),
            };

            var sut = new NotifyPropertyChangedObservable<TestObservableObject>(obj.ToPropertyValueChangedObservable());

            // Act
            sut.Subscribe(firstObserver);
            obj.Age = 5;
            scheduler.AdvanceTo(20);
            obj.Age = 6;
            scheduler.AdvanceTo(50);
            sut.Subscribe(secondObserver);
            obj.Age = 7;

            // Assert
            Assert.AreEqual(firstObservserExpected, firstObserver.Messages);
            Assert.AreEqual(secondObservserExpected, secondObserver.Messages);
        }

        [Test]
        public void When_Subscribe_Then_Returns_Default_Value_At_Time_Of_Subscribe_And_Subsequent_Values()
        {
            // Arrange
            var obj = new TestObservableObject() {Age = 2};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChanged>();
            var expected = new[]
            {
                OnNext(00, new PropertyChanged(obj)),
                OnNext(20, new PropertyChanged(obj, "Age")),
                OnNext(30, new PropertyChanged(obj, "Age")),
                OnNext(40, new PropertyChanged(obj, "Name")),
            };

            var sut = new NotifyPropertyChangedObservable<TestObservableObject>(obj.ToPropertyValueChangedObservable());

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(20);
            obj.Age = 3;
            scheduler.AdvanceTo(30);
            obj.Age = 7;
            scheduler.AdvanceTo(40);
            obj.Name = "Fi";

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}