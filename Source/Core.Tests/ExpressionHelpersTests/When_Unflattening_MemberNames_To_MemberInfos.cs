// <copyright file="When_Unflattening_MemberNames_To_MemberInfos.cs" company="million miles per hour ltd">
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

using System.Reflection;
using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.ExpressionHelpersTests
{
    [TestFixture]
    public class When_Unflattening_MemberNames_To_MemberInfos
    {
        [Test]
        public void Given_Three_Node_Property_Chain_With_Non_Existent_Third_Node_Then_Does_Not_Get_MemberInfos()
        {
            // Arrange
            var expected = new MemberInfo[] {};

            // Act
            var res = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(typeof(Outter), "InnerPropertyPropertyNonExistent");

            // Assert
            Assert.AreEqual(expected, res);
        }

        [Test]
        public void Given_Two_Node_Field_Chain_Then_Gets_MemberInfos()
        {
            // Arrange
            var expected = new[]
            {
                typeof(Outter).GetField("InnerField"),
                typeof(Inner).GetField("Field")
            };

            // Act
            var res = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(typeof(Outter), "InnerFieldField");

            // Assert
            Assert.AreEqual(expected, res);
        }

        [Test]
        public void Given_Two_Node_Property_Chain_Then_Gets_MemberInfos()
        {
            // Arrange
            var expected = new[]
            {
                typeof(Outter).GetProperty("InnerProperty"),
                typeof(Inner).GetProperty("Property")
            };

            // Act
            var res = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(typeof(Outter), "InnerPropertyProperty");

            // Assert
            Assert.AreEqual(expected, res);
        }

        [Test]
        public void Given_Two_Node_Property_Chain_With_Invalid_First_Node_Then_Does_Not_Get_MemberInfos()
        {
            // Arrange
            var expected = new MemberInfo[] {};

            // Act
            var res = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(typeof(Outter), "InnerxxxPropertyProperty");

            // Assert
            Assert.AreEqual(expected, res);
        }

        [Test]
        public void Given_Two_Node_Property_Chain_With_Invalid_Second_Node_Then_Does_Not_Get_MemberInfos()
        {
            // Arrange
            var expected = new MemberInfo[] {};

            // Act
            var res = ExpressionHelpers.UnflattenMemberNamesToMemberInfos(typeof(Outter), "InnerPropertyxxxProperty");

            // Assert
            Assert.AreEqual(expected, res);
        }
    }
}