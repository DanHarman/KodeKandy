// <copyright file="Given_When_And_Final_Node_Poco.cs" company="million miles per hour ltd">
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
using KodeKandy.Panopticon.Tests.TestEntities;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq
{
    [TestFixture]
    public class Given_When_And_Final_Node_Poco : ReactiveTest
    {
        [Test]
        public void When_Subscribe_With_One_Node_Path_To_Property_Then_OnNext_And_No_Complete()
        {
            // Arrange
            var obj = new TestPoco() {Age = 5};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 5),
            };

            var sut = obj.When(x => x.Age).ToValues();

            // Act
            sut.Subscribe(observer);
            obj.Age = 3; // This should not fire through.

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Node_One_Then_OnNext_And_No_Complete()
        {
            // Arrange
            var obj = new TestObservableObject {PocoChild = new TestPoco {Age = 3}};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 3),
                OnNext(10, 5),
            };

            var sut = obj.When(x => x.PocoChild.Age).ToValues();

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.PocoChild = new TestPoco {Age = 5};

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}