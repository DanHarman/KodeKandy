// <copyright file="Given_WhenAny.cs" company="million miles per hour ltd">
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
    public class Given_WhenAny : ReactiveTest
    {
//        [Test]
//        public void When_Path_Has_Null_Intermediary_Node_Then_Skips_When_Invalid_Path()
//        {
//            // Arrange
//            var childOne = new TestObservableObject {Age = 5};
//            var childTwo = new TestObservableObject {Age = 17};
//            var obj = new TestObservableObject {ObservableChild = childOne};
//            var scheduler = new TestScheduler();
//
//            var observer = scheduler.CreateObserver<int>();
//            var expected = new[]
//            {
//                OnNext(000, childOne.Age),
//                OnNext(000, 20),
//                OnNext(300, childTwo.Age),
//            };
//
//            var sut = obj.WhenValue(x => x.ObservableChild.Age);
//
//            // Act
//            sut.Subscribe(observer);
//            obj.ObservableChild.Age = 20;
//            scheduler.AdvanceTo(100);
//            obj.ObservableChild = null;
//            scheduler.AdvanceTo(300);
//            obj.ObservableChild = childTwo;
//
//            // Assert
//            observer.Messages.AssertEqual(expected);
//        }

        [Test]
        public void When_Subscribe_With_One_Node_Path_To_Property_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject {Age = 5};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChanged>();
            var expected = new[]
            {
                OnNext(000, new PropertyChanged(obj, "Age")),
            };

            var sut = obj.WhenAny();

            // Act
            sut.Subscribe(observer);
            obj.Age = 10;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Node_One_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject {ObservableChild = new TestObservableObject {Age = 3}};
            var replacementChild = new TestObservableObject {Age = 5};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChanged>();
            var expected = new[]
            {
                OnNext(010, new PropertyChanged(obj, "Age")),
            };

            var sut = obj.WhenAny(x => x.ObservableChild);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.ObservableChild = replacementChild;
        
            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Property_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject {ObservableChild = new TestObservableObject {Age = 3}};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(000, 3),
                OnNext(010, 5),
            };

            var sut = obj.WhenValue(x => x.ObservableChild.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.ObservableChild.Age = 5;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}