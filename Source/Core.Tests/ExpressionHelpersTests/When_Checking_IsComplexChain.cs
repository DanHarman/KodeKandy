// <copyright file="When_Checking_IsComplexChain.cs" company="million miles per hour ltd">
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

namespace KodeKandy.ExpressionHelpersTests
{
    [TestFixture]
    public class When_Checking_IsComplexChain
    {
        [Test]
        public void Given_Function_Property_Chain_Then_True()
        {
            // Act
            var res = ExpressionHelpers.IsComplexChain<Inner, string>(x => x.GetName());

            // Assert
            Assert.IsTrue(res);
        }

        [Test]
        public void Given_One_Node_Field_Chain_Then_False()
        {
            // Act
            var res = ExpressionHelpers.IsComplexChain<Inner, int>(x => x.Field);

            // Assert
            Assert.IsFalse(res);
        }

        [Test]
        public void Given_One_Node_Property_Chain_Then_False()
        {
            // Act
            var res = ExpressionHelpers.IsComplexChain<Inner, int>(x => x.Property);

            // Assert
            Assert.IsFalse(res);
        }

        [Test]
        public void Given_One_Node_With_Convert_Field_Chain_Then_False()
        {
            // Act
            var res = ExpressionHelpers.IsComplexChain<Inner, object>(x => (object) x.Field);

            // Assert
            Assert.IsFalse(res);
        }

        [Test]
        public void Given_One_Node_With_Convert_Property_Chain_Then_False()
        {
            // Act
            var res = ExpressionHelpers.IsComplexChain<Inner, object>(x => (object) x.Property);

            // Assert
            Assert.IsFalse(res);
        }

        [Test]
        public void Given_Two_Node_Property_Chain_Then_True()
        {
            // Act
            var res = ExpressionHelpers.IsComplexChain<Outter, int>(x => x.InnerProperty.Property);

            // Assert
            Assert.IsTrue(res);
        }
    }
}