// <copyright file="Given_Observing_CollectionChanges.cs" company="million miles per hour ltd">
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

using KodeKandy.QualityTools;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.ObservableListTests
{
    [TestFixture]
    public class Given_Observing_CollectionChanges : ReactiveTest
    {
        [Test]
        public void When_Add_Then_OnNext_Action_Add()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new ObservableList<int>();
            var observer = scheduler.CreateObserver<CollectionChange<int>>();
            var expected = new[]
            {
                OnNext(10, CollectionChange.CreateAdd(sut, 10)),
                OnNext(30, CollectionChange.CreateAdd(sut, 99)),
            };
            sut.CollectionChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.Add(10);
            scheduler.AdvanceTo(30);
            sut.Add(99);

            // Assert
            KKAssert.AreEqualByValue(expected, observer.Messages);
        }

        [Test]
        public void When_Clear_Then_OnNext_Action_Image()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var observer = scheduler.CreateObserver<CollectionChange<int>>();
            var expected = new[]
            {
                OnNext(10, CollectionChange.CreateImage<int>(sut))
            };
            sut.CollectionChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.Clear();

            // Assert
            KKAssert.AreEqualByValue(expected, observer.Messages);
        }

        [Test]
        public void When_Disposed_Then_OnCompleted()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var observer = scheduler.CreateObserver<CollectionChange<int>>();
            var expected = new[]
            {
                OnCompleted(90, CollectionChange.CreateImage<int>(sut))
            };
            sut.CollectionChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(90);
            sut.Dispose();

            // Assert
            observer.Messages.AssertEqual(expected);
        }

        [Test]
        public void When_Insert_Then_OnNext_Action_Add()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var observer = scheduler.CreateObserver<CollectionChange<int>>();
            var expected = new[]
            {
                OnNext(10, CollectionChange.CreateAdd(sut, 777)),
                OnNext(30, CollectionChange.CreateAdd(sut, 101)),
            };
            sut.CollectionChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.Insert(3, 777);
            scheduler.AdvanceTo(30);
            sut.Insert(0, 101);

            // Assert
            KKAssert.AreEqualByValue(expected, observer.Messages);
        }

        [Test]
        public void When_RemoveAt_Then_OnNext_Action_Remove()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var observer = scheduler.CreateObserver<CollectionChange<int>>();
            var expected = new[]
            {
                OnNext(10, CollectionChange.CreateRemove(sut, 12)),
                OnNext(30, CollectionChange.CreateRemove(sut, 100)),
            };
            sut.CollectionChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.RemoveAt(1);
            scheduler.AdvanceTo(30);
            sut.RemoveAt(2);

            // Assert
            KKAssert.AreEqualByValue(expected, observer.Messages);
        }

        [Test]
        public void When_Remove_Then_OnNext_Action_Remove()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var observer = scheduler.CreateObserver<CollectionChange<int>>();
            var expected = new[]
            {
                OnNext(10, CollectionChange.CreateRemove(sut, 12)),
                OnNext(30, CollectionChange.CreateRemove(sut, 100)),
            };
            sut.CollectionChanges.Subscribe(observer);

            // Act
            scheduler.Start();
            scheduler.AdvanceTo(10);
            sut.Remove(12);
            scheduler.AdvanceTo(30);
            sut.Remove(100);

            // Assert
            KKAssert.AreEqualByValue(expected, observer.Messages);
        }
    }
}