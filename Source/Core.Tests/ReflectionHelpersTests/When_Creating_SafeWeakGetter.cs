// <copyright file="When_Creating_SafeWeakGetter.cs" company="million miles per hour ltd">
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

namespace KodeKandy.ReflectionHelpersTests
{
    [TestFixture]
    public class When_Creating_SafeWeakGetter
    {
        [Test]
        public void Given_One_Node_Field_Chain_Then_Gets_MemberInfo()
        {
            // Arrange
            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos<Inner, int>(x => x.Field);
            var sut = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);
            var instance = new Inner() {Field = 1234};

            // Act
            object result;
            var success = sut(instance, out result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(instance.Field, result);
        }

        [Test]
        public void Given_One_Node_Property_Chain_Then_Gets_Member()
        {
            // Arrange
            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos<Inner, int>(x => x.Property);
            var sut = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);
            var instance = new Inner() {Property = 1234};

            // Act
            object result;
            var success = sut(instance, out result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(instance.Property, result);
        }

        [Test]
        public void Given_Two_Node_Field_Chain_Then_Gets_MemberInfos()
        {
            // Arrange
            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos<Outter, int>(x => x.InnerField.Field);
            var sut = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);
            var instance = new Outter {InnerField = new Inner {Field = 1234}};

            // Act
            object result;
            var success = sut(instance, out result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(instance.InnerField.Field, result);
        }

        [Test]
        public void Given_Two_Node_Field_Chain_With_Null_Link_Then_HasResult_False()
        {
            // Arrange
            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos<Outter, int>(x => x.InnerField.Field);
            var sut = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);
            var instance = new Outter {InnerField = null};

            // Act
            object result;
            var success = sut(instance, out result);

            // Assert
            Assert.IsFalse(success);
        }

        [Test]
        public void Given_Two_Node_Property_Chain_Then_Gets_MemberInfos()
        {
            // Arrange
            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos<Outter, int>(x => x.InnerProperty.Property);
            var sut = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);
            var instance = new Outter {InnerProperty = new Inner {Property = 1234}};

            // Act
            object result;
            var success = sut(instance, out result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(instance.InnerProperty.Property, result);
        }

        [Test]
        public void Given_Two_Node_Property_Chain_With_Null_Link_Then_HasResult_False()
        {
            // Arrange
            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos<Outter, int>(x => x.InnerProperty.Property);
            var sut = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);
            var instance = new Outter {InnerProperty = null};

            // Act
            object result;
            var success = sut(instance, out result);

            // Assert
            Assert.IsFalse(success);
        }

        [Test]
        public void Given_Zero_Node_Property_Chain_Then_Gets_Root_Instance()
        {
            // Arrange
            var memberInfos = ExpressionHelpers.GetExpressionChainMemberInfos<Outter, Outter>(x => x);
            var sut = ReflectionHelpers.CreateSafeWeakMemberChainGetter(memberInfos);
            var instance = new Outter { InnerProperty = new Inner { Property = 1234 } };

            // Act
            object result;
            var success = sut(instance, out result);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(instance, result);
        }
    }
}