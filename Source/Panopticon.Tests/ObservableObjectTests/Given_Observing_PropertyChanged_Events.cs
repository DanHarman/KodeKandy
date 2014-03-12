// <copyright file="Given_Observing_PropertyChanged_Events.cs" company="million miles per hour ltd">
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

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.ObservableObjectTests
{
    [TestFixture]
    public class Given_Observing_PropertyChanged_Events
    {
        [Test]
        public void When_SetValue_With_New_Values_Then_Event_Raised()
        {
            // Arrange
            var sut = new TestObservableObject {Age = 10};
            var results = new List<Tuple<string, int>>();
            var expected = new[]
            {
                Tuple.Create("Age", 17),
                Tuple.Create("Age", 70)
            };
            sut.PropertyChanged += (o, pc) => results.Add(Tuple.Create(pc.PropertyName, sut.Age));

            // Act
            sut.Age = 17;
            sut.Age = 70;

            // Assert
            CollectionAssert.AreEqual(expected, results);
        }

        [Test]
        public void When_SetValue_With_Same_Values_Then_Event_Raised_Once_Per_Value()
        {
            // Arrange
            var sut = new TestObservableObject {Age = 10};
            var results = new List<Tuple<string, int>>();
            var expected = new[]
            {
                Tuple.Create("Age", 70)
            };
            sut.PropertyChanged += (o, pc) => results.Add(Tuple.Create(pc.PropertyName, sut.Age));

            // Act
            sut.Age = 10;
            sut.Age = 10;
            sut.Age = 70;
            sut.Age = 70;

            // Assert
            CollectionAssert.AreEqual(expected, results);
        }
    }
}