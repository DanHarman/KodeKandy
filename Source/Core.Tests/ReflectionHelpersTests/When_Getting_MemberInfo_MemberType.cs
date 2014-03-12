// <copyright file="When_Getting_MemberInfo_MemberType.cs" company="million miles per hour ltd">
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

using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.ReflectionHelpersTests
{
    [TestFixture]
    public class When_Getting_MemberInfo_MemberType
    {
        [Test]
        public void Given_PropertyInfo_Then_Gets_PropertyType()
        {
            // Act
            var res = typeof(Inner).GetProperty("Property").GetMemberType();

            // Assert
            Assert.AreEqual(typeof(int), res);
        }

        [Test]
        public void Given_FieldInfo_Then_Gets_FieldType()
        {
            // Act
            var res = typeof(Inner).GetField("Field").GetMemberType();

            // Assert
            Assert.AreEqual(typeof(int), res);
        }
    }
}