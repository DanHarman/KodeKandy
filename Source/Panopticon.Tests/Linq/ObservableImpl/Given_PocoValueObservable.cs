// <copyright file="Given_PocoValueObservable.cs" company="million miles per hour ltd">
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
    public class Given_PocoValueObservable : ReactiveTest
    {
        [Test]
        public void When_Source_Has_Null_Node_Then_Propagates_ProvertyValueChanged_With_HasValue_False()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sourceOne = new TestPoco {Age = 5};
            var sourceTwo = new TestPoco {Age = 17};
            var sourceObs = scheduler.CreateColdObservable(
                OnNext(100, PropertyValueChanged.CreateWithValue(null, "Age", sourceOne)),
                OnNext(200, PropertyValueChanged.CreateWithoutValue<TestPoco>(null, "Age")),
                OnNext(300, PropertyValueChanged.CreateWithValue(null, "Age", sourceTwo)),
                OnCompleted<IPropertyValueChanged<TestPoco>>(400)
                );
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(100, PropertyValueChanged.CreateWithValue(sourceOne, "Age", sourceOne.Age)),
                OnNext(200, PropertyValueChanged.CreateWithoutValue<int>(null, "Age")),
                OnNext(300, PropertyValueChanged.CreateWithValue(sourceTwo, "Age", sourceTwo.Age)),
                OnCompleted<IPropertyValueChanged<int>>(400),
            };

            var sut = new PocoValueObservable<TestPoco, int>(sourceObs, "Age", x => x.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.Start();

            // Assert
            Assert.AreEqual(expected, observer.Messages);
            Assert.IsFalse(observer.Messages[1].Value.Value.HasValue);
        }

        [Test]
        public void When_Source_Observable_Completes_Then_Completes()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sourceObs = scheduler.CreateColdObservable(
                OnCompleted<IPropertyValueChanged<TestPoco>>(400)
                );
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnCompleted<IPropertyValueChanged<int>>(400),
            };

            var sut = new PocoValueObservable<TestPoco, int>(sourceObs, "Age", x => x.Age);

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
                OnError<IPropertyValueChanged<TestPoco>>(400, expectedException)
                );
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnError<IPropertyValueChanged<int>>(400, expectedException),
            };

            var sut = new PocoValueObservable<TestPoco, int>(sourceObs, "Age", x => x.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.Start();

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_Then_Returns_Value_At_Time_Of_Subscribe()
        {
            // Arrange
            var obj = new TestPoco() {Age = 2};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(20, PropertyValueChanged.CreateWithValue(obj, "Age", 5)),
            };

            var sut = new PocoValueObservable<TestPoco, int>(obj.ToPropertyValueChangedObservable(), "Age", x => x.Age);

            // Act
            scheduler.AdvanceTo(10);
            obj.Age = 5;
            scheduler.AdvanceTo(20);
            sut.Subscribe(observer);
            scheduler.AdvanceTo(30);
            obj.Age = 3; // This should not fire through.

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}