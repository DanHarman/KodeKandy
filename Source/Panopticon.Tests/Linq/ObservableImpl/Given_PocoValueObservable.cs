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
        public void When_Subscribed_Then_Returns_Value_At_Time_Of_Subscribe()
        {
            // Arrange
            var obj = new TestPoco() {Age = 2};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(20, 5),
            };

            var sut = new PocoValueObservable<TestPoco, int>(Opticon.Forever(obj), x => x.Age);

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