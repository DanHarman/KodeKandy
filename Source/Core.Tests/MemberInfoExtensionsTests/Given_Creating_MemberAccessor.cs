// <copyright file="Given_Creating_MemberAccessor.cs" company="million miles per hour ltd">
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
    public class Given_Creating_MemberAccessor
    {
        [Test]
        public void When_Member_Is_Field_Then_Constructed_Correctly()
        {
            // Act
            var sut = new MemberAccessor(ReadWriteTestEntity.FieldMemberInfo);

            // Asert
            Assert.AreEqual(sut.Type, MemberAccessorType.Field);
            Assert.AreEqual(sut.MemberName, ReadWriteTestEntity.FieldName);
            Assert.AreEqual(typeof(ReadWriteTestEntity), sut.DeclaringType);
            Assert.AreEqual(typeof(int), sut.MemberType);
            Assert.NotNull(sut.Getter);
            Assert.NotNull(sut.Setter);
        }

        [Test]
        public void When_Member_Is_ReadOnlyProperty_Then_Constructed_Correctly()
        {
            // Act
            var sut = new MemberAccessor(ReadWriteTestEntity.ReadOnlyPropertyMemberInfo);

            // Asert
            Assert.AreEqual(sut.Type, MemberAccessorType.Property);
            Assert.AreEqual(sut.MemberName, ReadWriteTestEntity.ReadOnlyPropertyName);
            Assert.AreEqual(typeof(ReadWriteTestEntity), sut.DeclaringType);
            Assert.AreEqual(typeof(int), sut.MemberType);
            Assert.NotNull(sut.Getter);
            Assert.IsNull(sut.Setter);
        }

        [Test]
        public void When_Member_Is_ReadWriteProperty_Then_Constructed_Correctly()
        {
            // Act
            var sut = new MemberAccessor(ReadWriteTestEntity.ReadWritePropertyMemberInfo);

            // Asert
            Assert.AreEqual(sut.Type, MemberAccessorType.Property);
            Assert.AreEqual(sut.MemberName, ReadWriteTestEntity.ReadWritePropertyName);
            Assert.AreEqual(typeof(ReadWriteTestEntity), sut.DeclaringType);
            Assert.AreEqual(typeof(int), sut.MemberType);
            Assert.NotNull(sut.Getter);
            Assert.NotNull(sut.Setter);
        }

        [Test]
        public void When_Member_Is_WriteOnlyProperty_Then_Constructed_Correctly()
        {
            // Act
            var sut = new MemberAccessor(ReadWriteTestEntity.WriteOnlyPropertyMemberInfo);

            // Asert
            Assert.AreEqual(sut.Type, MemberAccessorType.Property);
            Assert.AreEqual(sut.MemberName, ReadWriteTestEntity.WriteOnlyPropertyName);
            Assert.AreEqual(typeof(ReadWriteTestEntity), sut.DeclaringType);
            Assert.AreEqual(typeof(int), sut.MemberType);
            Assert.IsNull(sut.Getter);
            Assert.NotNull(sut.Setter);
        }
    }
}