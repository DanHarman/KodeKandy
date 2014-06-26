// <copyright file="Given_NotifyPropertyChangedValueObservable2.cs" company="million miles per hour ltd">
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
    public class Given_NotifyPropertyChangedValueObservable2 : ReactiveTest
    {
        [Test]
        public void When_Source_Observable_Completes_Then_Completes()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sourceObj = new[]
            {
                new TestObservableObject() {Age = 2},
                new TestObservableObject() {Age = 3},
                new TestObservableObject() {Age = 4}
            };
            var sourceObs = scheduler.CreateColdObservable(
                OnNext(10, sourceObj[0].ToPropertyValueChanged()),
                OnNext(20, sourceObj[1].ToPropertyValueChanged()),
                OnNext(30, sourceObj[2].ToPropertyValueChanged()),
                OnCompleted<PropertyValueChanged<TestObservableObject>>(40)
                );
            var observer = scheduler.CreateObserver<PropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(10, new PropertyValueChanged<int>(sourceObj[0], "Age", sourceObj[0].Age)),
                OnNext(20, new PropertyValueChanged<int>(sourceObj[1], "Age", sourceObj[1].Age)),
                OnNext(30, new PropertyValueChanged<int>(sourceObj[2], "Age", sourceObj[2].Age)),
                OnCompleted<PropertyValueChanged<int>>(40)
            };

            var sut = new NotifyPropertyChangedValueObservable2<TestObservableObject, int>(sourceObs, "Age", x => x.Age);

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
            var sourceObj = new[]
            {
                new TestObservableObject() {Age = 2},
                new TestObservableObject() {Age = 3},
                new TestObservableObject() {Age = 4}
            };
            var sourceObs = scheduler.CreateColdObservable(
                OnNext(10, sourceObj[0].ToPropertyValueChanged()),
                OnNext(20, sourceObj[1].ToPropertyValueChanged()),
                OnNext(30, sourceObj[2].ToPropertyValueChanged()),
                OnError<PropertyValueChanged<TestObservableObject>>(40, expectedException)
                );
            var observer = scheduler.CreateObserver<PropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(10, new PropertyValueChanged<int>(sourceObj[0], "Age", sourceObj[0].Age)),
                OnNext(20, new PropertyValueChanged<int>(sourceObj[1], "Age", sourceObj[1].Age)),
                OnNext(30, new PropertyValueChanged<int>(sourceObj[2], "Age", sourceObj[2].Age)),
                OnError<PropertyValueChanged<int>>(40, expectedException)
            };

            var sut = new NotifyPropertyChangedValueObservable2<TestObservableObject, int>(sourceObs, "Age", x => x.Age);

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
            var sourceObj = new TestObservableObject {Age = 3};
            var scheduler = new TestScheduler();
            var firstObserver = scheduler.CreateObserver<PropertyValueChanged<int>>();
            var secondObserver = scheduler.CreateObserver<PropertyValueChanged<int>>();
            var firstObservserExpected = new[]
            {
                OnNext(0, new PropertyValueChanged<int>(sourceObj, "Age", 3)),
                OnNext(0, new PropertyValueChanged<int>(sourceObj, "Age", 5)),
                OnNext(20, new PropertyValueChanged<int>(sourceObj, "Age", 6)),
                OnNext(50, new PropertyValueChanged<int>(sourceObj, "Age", 7)),
            };
            var secondObservserExpected = new[]
            {
                OnNext(50, new PropertyValueChanged<int>(sourceObj, "Age", 6)),
                OnNext(50, new PropertyValueChanged<int>(sourceObj, "Age", 7)),
            };

            var sut = new NotifyPropertyChangedValueObservable2<TestObservableObject, int>(sourceObj.ToPropertyValueChanged().Forever(), "Age", x => x.Age);

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

        [Test]
        public void When_Subscribed_And_Source_Does_Not_Complete_Then_Returns_Value_At_Time_Of_Subscribe_And_Subsequent_Values()
        {
            // Arrange
            var sourceObj = new TestObservableObject() {Age = 2};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyValueChanged<int>>();
            var expected = new[]
            {
                 OnNext(20, new PropertyValueChanged<int>(sourceObj, "Age", 5)),
                OnNext(30, new PropertyValueChanged<int>(sourceObj, "Age", 3)),
            };

            var sut = new NotifyPropertyChangedValueObservable2<TestObservableObject, int>(sourceObj.ToPropertyValueChanged().Forever(), "Age", x => x.Age);

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
    }
}