// <copyright file="Given_Checking_CanRead.cs" company="million miles per hour ltd">
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

using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.MemberInfoExtensionsTests
{
    [TestFixture]
    public class Given_Checking_CanRead
    {
        [Test]
        public void When_Field_Then_True()
        {
            // Act
            var res = ReadWriteTestEntity.FieldMemberInfo.CanRead();

            // Assert
            Assert.IsTrue(res);
        }

        [Test]
        public void When_Property_Is_Readable_Then_True()
        {
            // Act
            var res = ReadWriteTestEntity.ReadWritePropertyMemberInfo.CanRead();

            // Assert
            Assert.IsTrue(res);
        }

        [Test]
        public void When_Property_Is_Unreadable_Then_False()
        {
            // Act
            var res = ReadWriteTestEntity.WriteOnlyPropertyMemberInfo.CanRead();

            // Assert
            Assert.IsFalse(res);
        }
    }
}