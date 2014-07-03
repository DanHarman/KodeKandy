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
        [Test]
        public void When_Subscribe_To_Property_With_Broken_Path_Then_OnNext_Initial_Image_With_Null_Source()
        {
            // Arrange
            var obj = new TestObservableObject();
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyChanged<TestObservableObject>>();
            var expected = new[]
            {
                OnNext(000, PropertyChanged.Create(default(TestObservableObject))),
            };

            var sut = obj.WhenAny(x => x.PocoChild.ObservableChild);

            // Act
            sut.Subscribe(observer);

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_One_Node_Path_To_Property_Then_OnNext_Initial_Image_And_Changes()
        {
            // Arrange
            var obj = new TestObservableObject {Age = 5};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyChanged<TestObservableObject>>();
            var expected = new[]
            {
                OnNext(000, PropertyChanged.Create(obj)),
                OnNext(000, PropertyChanged.Create(obj, "Age")),
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
            var observer = scheduler.CreateObserver<IPropertyChanged<TestObservableObject>>();
            var expected = new[]
            {
                OnNext(000, PropertyChanged.Create(obj.ObservableChild)),
                OnNext(010, PropertyChanged.Create(replacementChild)),
                OnNext(010, PropertyChanged.Create(replacementChild, "Age")),
            };

            var sut = obj.WhenAny(x => x.ObservableChild);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.ObservableChild = replacementChild;
            obj.ObservableChild.Age = 17;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }

        [Test]
        public void When_Subscribe_With_Two_Node_Path_To_Property_And_Modify_Property_Then_OnNext_Changes()
        {
            // Arrange
            var obj = new TestObservableObject {ObservableChild = new TestObservableObject {Age = 3}};
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<IPropertyChanged<TestObservableObject>>();
            var expected = new[]
            {
                OnNext(000, PropertyChanged.Create(obj.ObservableChild)),
                OnNext(010, PropertyChanged.Create(obj.ObservableChild, "Age")),
            };

            var sut = obj.WhenAny(x => x.ObservableChild);

            // Act
            sut.Subscribe(observer);
            scheduler.AdvanceTo(10);
            obj.ObservableChild.Age = 5;

            // Assert
            Assert.AreEqual(expected, observer.Messages);
        }
    }
}