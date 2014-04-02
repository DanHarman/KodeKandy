// <copyright file="When_ReverseEnumeratingBinaryPartitions.cs" company="million miles per hour ltd">
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

using System;
using System.Linq;
using NUnit.Framework;

namespace KodeKandy.ExpressionHelpersTests
{
    [TestFixture]
    public class When_ReverseEnumeratingBinaryPartitions
    {
        [Test]
        public void Given_Empty_String_Then_Enumeration_Empty()
        {
            // Act
            var res = ExpressionHelpers.ReverseEnumerateBinaryPartitions(string.Empty);

            // Assert
            Assert.IsEmpty(res);
        }

        [Test]
        public void Given_String_Then_Yields_Expected_Partitions()
        {
            // Arrange
            var input = "Hello";
            var expected = new[]
            {
                Tuple.Create("Hello", String.Empty),
                Tuple.Create("Hell", "o"),
                Tuple.Create("Hel", "lo"),
                Tuple.Create("He", "llo"),
                Tuple.Create("H", "ello"),
                Tuple.Create(String.Empty, "Hello"),
            };

            // Act
            var res = ExpressionHelpers.ReverseEnumerateBinaryPartitions(input).ToArray();

            // Assert
            CollectionAssert.AreEqual(expected, res);
        }
    }
}