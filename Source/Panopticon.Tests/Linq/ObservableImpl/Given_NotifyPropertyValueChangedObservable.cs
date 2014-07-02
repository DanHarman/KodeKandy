// <copyright file="Given_NotifyPropertyValueChangedObservable.cs" company="million miles per hour ltd">
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
    public class Given_NotifyPropertyValueChangedObservable : ReactiveTest
    {
        [Test]
        public void When_Source_Observable_Completes_Then_Completes()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sourceObs = scheduler.CreateColdObservable(
                OnCompleted<IPropertyValueChanged<TestObservableObject>>(40)
                );
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnCompleted<IPropertyValueChanged<int>>(40)
            };

            var sut = new NotifyPropertyValueChangedObservable<TestObservableObject, int>(sourceObs, "Age", x => x.Age);

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
                OnError<IPropertyValueChanged<TestObservableObject>>(40, expectedException)
                );
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnError<IPropertyValueChanged<int>>(40, expectedException)
            };

            var sut = new NotifyPropertyValueChangedObservable<TestObservableObject, int>(sourceObs, "Age", x => x.Age);

            // Act            
            sut.Subscribe(observer);
            scheduler.Start();

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_Then_Returns_Value_At_Time_Of_Subscribe_And_Subsequent_Values()
        {
            // Arrange
            var sourceObj = new TestObservableObject() {Age = 2};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(20, PropertyValueChanged.CreateWithValue(sourceObj, "Age", 5)),
                OnNext(30, PropertyValueChanged.CreateWithValue(sourceObj, "Age", 3)),
            };

            var sut = new NotifyPropertyValueChangedObservable<TestObservableObject, int>(sourceObj.ToPropertyValueChangedObservable(), "Age",
                x => x.Age);

            scheduler.AdvanceTo(10);
            sourceObj.Age = 5;
            scheduler.AdvanceTo(20);

            // Act

            sut.Subscribe(observer);
            scheduler.AdvanceTo(30);
            sourceObj.Age = 3;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_Twice_Then_Correct_Notifications_For_Both_Observers()
        {
            // Arrange
            var sourceObj = new TestObservableObject {Age = 3};
            var scheduler = new TestScheduler();
            var firstObserver = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var secondObserver = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var firstObservserExpected = new[]
            {
                OnNext(0, PropertyValueChanged.CreateWithValue(sourceObj, "Age", 3)),
                OnNext(0, PropertyValueChanged.CreateWithValue(sourceObj, "Age", 5)),
                OnNext(20, PropertyValueChanged.CreateWithValue(sourceObj, "Age", 6)),
                OnNext(50, PropertyValueChanged.CreateWithValue(sourceObj, "Age", 7)),
            };
            var secondObservserExpected = new[]
            {
                OnNext(50, PropertyValueChanged.CreateWithValue(sourceObj, "Age", 6)),
                OnNext(50, PropertyValueChanged.CreateWithValue(sourceObj, "Age", 7)),
            };

            var sut = new NotifyPropertyValueChangedObservable<TestObservableObject, int>(sourceObj.ToPropertyValueChangedObservable(), "Age",
                x => x.Age);

            // Act
            sut.Subscribe(firstObserver);
            sourceObj.Age = 5;
            scheduler.AdvanceTo(20);
            sourceObj.Age = 6;
            scheduler.AdvanceTo(50);
            sut.Subscribe(secondObserver);
            sourceObj.Age = 7;

            // Assert
            Assert.AreEqual(firstObservserExpected, firstObserver.Messages);
            Assert.AreEqual(secondObservserExpected, secondObserver.Messages);
        }
    }
}