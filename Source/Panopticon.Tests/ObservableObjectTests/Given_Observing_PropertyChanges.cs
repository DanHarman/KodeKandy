// <copyright file="Given_Observing_PropertyChanges.cs" company="million miles per hour ltd">
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

using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.ObservableObjectTests
{
    [TestFixture]
    public class Given_Observing_PropertyChanges : ReactiveTest
    {
        [Test]
        public void When_Disposed_Then_OnCompleted()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new TestObservableObject {Age = 10};
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnCompleted<IPropertyChange>(90)
            };
            sut.PropertyChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(90);
            sut.Dispose();

            // Assert
            observer.Messages.AssertEqual(expected);
        }

        [Test]
        public void When_SetValue_With_New_Values_Then_Event_Fires()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new TestObservableObject {Age = 10};
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnNext(10, PropertyChange.Create(sut, 17, "Age")),
                OnNext(30, PropertyChange.Create(sut, 70, "Age"))
            };
            sut.PropertyChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.Age = 17;
            scheduler.AdvanceTo(30);
            sut.Age = 70;
            scheduler.AdvanceTo(90);

            // Assert
            observer.Messages.AssertEqual(expected);
        }

        [Test]
        public void When_SetValue_With_Same_Values_Then_OnNext_Invoked_Once_Per_Value()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new TestObservableObject {Age = 10};
            var observer = scheduler.CreateObserver<IPropertyChange>();
            var expected = new[]
            {
                OnNext(30, PropertyChange.Create(sut, 70, "Age"))
            };
            sut.PropertyChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.Age = 10;
            scheduler.AdvanceTo(20);
            sut.Age = 10;
            scheduler.AdvanceTo(30);
            sut.Age = 70;
            scheduler.AdvanceTo(40);
            sut.Age = 70;

            // Assert
            observer.Messages.AssertEqual(expected);
        }
    }
}