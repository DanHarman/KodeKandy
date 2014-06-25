// <copyright file="Given_CollectionChangedHelper.cs" company="million miles per hour ltd">
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

using System.Collections.Generic;
using System.Collections.Specialized;
using KodeKandy.QualityTools;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.CollectionChangeHelperTests
{
    [TestFixture]
    public class Given_CollectionChangedHelper
    {
        [Test]
        public void When_NotifyAdd_Then_Action_Add_Raised()
        {
            // Arrange
            var sut = new CollectionChangedHelper<int>(this);
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 10, 0),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 99, 1),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.NotifyAdd(10, 0);
            sut.NotifyAdd(99, 1);

            // Assert
            KKAssert.AreEqualByValue(expected, results);
        }

        [Test]
        public void When_NotifyRemove_Then_Action_Remove_Raised()
        {
            // Arrange
            var sut = new CollectionChangedHelper<int>(this);
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 12, 1),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 100, 2),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.NotifyRemove(12, 1);
            sut.NotifyRemove(100, 2);

            // Assert
            KKAssert.AreEqualByValue(expected, results);
        }

        [Test]
        public void When_NotifyReplace_Then_Action_Replace_Raised()
        {
            // Arrange
            var sut = new CollectionChangedHelper<int>(this);
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, 12, (object) 14, 9),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, 100, (object) 200, 80),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.NotifyReplace(12, 14, 9);
            sut.NotifyReplace(100, 200, 80);

            // Assert
            KKAssert.AreEqualByValue(expected, results);
        }

        [Test]
        public void When_NotifyReset_Then_Action_Reset_Raised()
        {
            // Arrange
            var sut = new CollectionChangedHelper<int>(this);
            var results = new List<NotifyCollectionChangedEventArgs>();
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
            };
            sut.CollectionChanged += (o, e) => results.Add(e);

            // Act
            sut.NotifyReset();

            // Assert
            KKAssert.AreEqualByValue(expected, results);
        }
    }
}