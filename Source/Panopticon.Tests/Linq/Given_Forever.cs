// <copyright file="Given_Forever.cs" company="million miles per hour ltd">
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
using KodeKandy.QualityTools;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq
{
    [TestFixture]
    public class Given_Forever : ReactiveTest
    {
        [Test]
        public void When_Subscribed_To_Then_Returns_Value_And_Does_Not_Complete()
        {
            // Arrange
            var scheduler = new TestScheduler();
            var sut = 100.Forever();
            var observer = scheduler.CreateObserver<int>();
            var expected = new[]
            {
                OnNext(0, 100),
            };
            sut.Subscribe(observer);

            // Act
            scheduler.Start();

            // Assert
            KKAssert.AreEqualByValue(expected, observer.Messages);
        }
    }
}