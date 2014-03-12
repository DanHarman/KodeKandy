// <copyright file="Given_Observing_CollectionChanged_Events.cs" company="million miles per hour ltd">
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
// 
// </copyright>

using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.ObservableListTests
{
    [TestFixture]
    public class Given_Observing_CollectionChanged_Events
    {
        [Test]
        public void When_Add_Then_Action_Add_Raised()
        {
            // Arrange
            var sut = new ObservableList<int>();
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 10, 0),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 99, 1),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.Add(10);
            sut.Add(99);

            // Assert
            KKAssert.AreEqual(expected, results);
        }

        [Test]
        public void When_Remove_Then_Action_Remove_Raised()
        {
            // Arrange
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 12, 1),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 100, 2),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.Remove(12);
            sut.Remove(100);

            // Assert
            KKAssert.AreEqual(expected, results);
        }

        [Test]
        public void When_RemoveAt_Then_Action_Remove_Raised()
        {
            // Arrange
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 12, 1),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 100, 2),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.RemoveAt(1);
            sut.RemoveAt(2);

            // Assert
            KKAssert.AreEqual(expected, results);
        }

        [Test]
        public void When_Insert_Then_Action_Add_Raised()
        {
            // Arrange
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 777, 3),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 101, 0),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.Insert(3, 777);
            sut.Insert(0, 101);

            // Assert
            KKAssert.AreEqual(expected, results);
        }

        [Test]
        public void When_Clear_Then_Action_Reset_Raised()
        {
            // Arrange
            var sut = new ObservableList<int>(new[] {10, 12, 18, 100});
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.Clear();

            // Assert
            KKAssert.AreEqual(expected, results);
        }
    }
}