// <copyright file="TargetClassSchemaTests.cs" company="million miles per hour ltd">
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

using KodeKandy.Mapnificent.Tests.TestEntities.Inheritance;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests
{
    [TestFixture]
    public class TargetClassSchemaTests
    {
        [Test]
        public void When_Creating_Schema_Then_Schema_Is_Correct()
        {
            // Arrange
            var sut = new ToClassSchema(typeof(Circle));

            // Assert
            Assert.IsTrue(sut.Members.ContainsKey("memberName"));
            Assert.IsTrue(sut.Members.ContainsKey("Radius"));
        }
    }
}