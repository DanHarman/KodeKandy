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
        /// <summary>
        ///     Test to ensure co-variance works correctly. Resolves defect caused by subscribing to property defined on a base
        ///     class.
        /// </summary>
        [Test]
        public void When_Property_Declared_On_Base_Class_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new DerivedTestPoco() {Age = 100};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(obj, "Age", 100)),
            };

            var sut = obj.When(x => x.Age);

            // Act
            sut.Subscribe(observer);
            obj.Age = 20;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_One_Node_Path_To_Property_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestPoco() {Age = 5};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(obj, "Age", 5)),
            };

            var sut = obj.When(x => x.Age);

            // Act
            sut.Subscribe(observer);
            obj.Age = 3; // This should not fire through.

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Node_One_Then_OnNext_Changes()
        {
            // Arrange
            var childOne = new TestPoco {Age = 3};
            var childTwo = new TestPoco() {Age = 5};
            var obj = new TestObservableObject {PocoChild = childOne};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(childOne, "Age", 3)),
                OnNext(010, PropertyValueChanged.CreateWithValue(childTwo, "Age", 5)),
            };

            var sut = obj.When(x => x.PocoChild.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.PocoChild = childTwo;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}