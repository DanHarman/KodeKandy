// <copyright file="Given_When_And_Final_Node_INotifyPropertyChanged.cs" company="million miles per hour ltd">
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
    public class Given_When_And_Final_Node_INotifyPropertyChanged : ReactiveTest
    {
        [Test]
        public void When_Path_Has_Null_Intermediary_Node_Then_Propagates_ProvertyValueChanged_With_HasValue_False()
        {
            // Arrange
            var childOne = new TestObservableObject {Age = 5};
            var childTwo = new TestObservableObject {Age = 17};
            var obj = new TestObservableObject {ObservableChild = childOne};
            var scheduler = new TestScheduler();

            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(childOne, "Age", childOne.Age)),
                OnNext(000, PropertyValueChanged.CreateWithValue(childOne, "Age", 20)),
                OnNext(100, PropertyValueChanged.CreateWithoutValue<int>(null, "Age")),
                OnNext(300, PropertyValueChanged.CreateWithValue(childTwo, "Age", childTwo.Age)),
            };

            var sut = obj.When(x => x.ObservableChild.Age);

            // Act
            sut.Subscribe(observer);
            obj.ObservableChild.Age = 20;
            scheduler.AdvanceTo(100);
            obj.ObservableChild = null;
            scheduler.AdvanceTo(300);
            obj.ObservableChild = childTwo;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        /// <summary>
        ///     Test to ensure co-variance works correctly. Resolves defect caused by subscribing to property defined on a base
        ///     class.
        /// </summary>
        [Test]
        public void When_Property_Declared_On_Base_Class_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new DerivedTestObservableObject {Age = 100};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(obj, "Age", 100)),
                OnNext(000, PropertyValueChanged.CreateWithValue(obj, "Age", 20)),
            };

            var sut = obj.When(x => x.Age);

            // Act
            sut.Subscribe(observer);
            obj.Age = 20;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_Via_Interface_To_Source_That_Impl_INotifyPropertyChanged_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject {Age = 3} as ITestObject;
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(obj, "Age", 3)),
                OnNext(010, PropertyValueChanged.CreateWithValue(obj, "Age", 5)),
            };

            var sut = obj.When(x => x.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.Age = 5;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_One_Node_Path_To_Property_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject {Age = 5};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(obj, "Age", 5)),
            };

            var sut = obj.When(x => x.Age);

            // Act
            sut.Subscribe(observer);

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
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(obj.ObservableChild, "Age", 3)),
                OnNext(010, PropertyValueChanged.CreateWithValue(replacementChild, "Age", replacementChild.Age)),
            };

            var sut = obj.When(x => x.ObservableChild.Age);

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
            var observer = scheduler.CreateObserver<IPropertyValueChanged<int>>();
            var expected = new[]
            {
                OnNext(000, PropertyValueChanged.CreateWithValue(obj.ObservableChild, "Age", 3)),
                OnNext(010, PropertyValueChanged.CreateWithValue(obj.ObservableChild, "Age", 5)),
            };

            var sut = obj.When(x => x.ObservableChild.Age);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.ObservableChild.Age = 5;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}