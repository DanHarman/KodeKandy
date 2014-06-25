// <copyright file="Given_Observe.cs" company="million miles per hour ltd">
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

using System.Reactive.Linq;
using KodeKandy.Panopticon.Linq;
using KodeKandy.Panopticon.Tests.TestEntities;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq.QueryLanguageTests
{
    [TestFixture]
    public class Given_When : ReactiveTest
    {
        [Test]
        public void When_Subscribe_For_INotifyPropertyChanged_Implementor_Then_OnNext_Values()
        {
            // Arrange
            var obj = new TestObservableObject { Age = 5 };
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 5),
                OnNext(15, 3),
            };

            var sut = Observable.Return(obj).Concat(Observable.Never<TestObservableObject>()).When("Age", x => x.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(15);
            obj.Age = 3;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}