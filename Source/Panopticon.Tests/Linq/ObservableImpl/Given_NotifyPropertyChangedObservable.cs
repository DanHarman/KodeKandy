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
                OnNext(10, new TestObservableObject() {Age = 2}),
                OnNext(20, new TestObservableObject() {Age = 3}),
                OnNext(30, new TestObservableObject() {Age = 4}),
                OnCompleted<TestObservableObject>(40)
                );
            var observer = scheduler.CreateObserver<PropertyChange>();
            var expected = new[]
            {
                OnCompleted<PropertyChange>(40)
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
                OnNext(10, new TestObservableObject() {Age = 2}),
                OnNext(20, new TestObservableObject() {Age = 3}),
                OnNext(30, new TestObservableObject() {Age = 4}),
                OnError<TestObservableObject>(40, expectedException)
                );
            var observer = scheduler.CreateObserver<PropertyChange>();
            var expected = new[]
            {
                OnError<PropertyChange>(40, expectedException)
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
            var sourceObs = Opticon.Forever(obj);
            var scheduler = new TestScheduler();
            var firstObserver = scheduler.CreateObserver<PropertyChange>();
            var secondObserver = scheduler.CreateObserver<PropertyChange>();
            var firstObservserExpected = new[]
            {
                OnNext(0, new PropertyChange(obj, "Age")),
                OnNext(20, new PropertyChange(obj, "Age")),
                OnNext(50, new PropertyChange(obj, "Age")),
            };

            var secondObservserExpected = new[]
            {
                OnNext(50, new PropertyChange(obj, "Age")),
            };

            var sut = new NotifyPropertyChangedObservable<TestObservableObject>(sourceObs);

            // Act
            sut.Subscribe(firstObserver);
            obj.Age = 5;
            scheduler.AdvanceTo(20);
            obj.Age = 6;
            scheduler.AdvanceTo(50);
            sut.Subscribe(secondObserver);
            obj.Age = 7;

            // Assert
            firstObserver.Messages.AssertEqual(firstObservserExpected);
            secondObserver.Messages.AssertEqual(secondObservserExpected);
        }

        [Test]
        public void When_Subscribed_And_Source_Does_Not_Complete_Then_Returns_Value_At_Time_Of_Subscribe_And_Subsequent_Values()
        {
            // Arrange
            var obj = new TestObservableObject() {Age = 2};
            var sourceObs = Opticon.Forever(obj);
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChange>();
            var expected = new[]
            {
                OnNext(20, new PropertyChange(obj, "Age")),
                OnNext(30, new PropertyChange(obj, "Age")),
            };

            var sut = new NotifyPropertyChangedObservable<TestObservableObject>(sourceObs);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(20);
            obj.Age = 3;
            scheduler.AdvanceTo(30);
            obj.Age = 7;

            // Assert
            observer.Messages.AssertEqual(expected);
        }
    }
}