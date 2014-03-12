// <copyright file="When_Getting_MemberNames.cs" company="million miles per hour ltd">
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

namespace KodeKandy.ExpressionHelpersTests
{
    [TestFixture]
    public class When_Getting_MemberNames
    {
        [Test]
        public void Given_PropertyChain_Then_Gets_Expected()
        {
            // Arrange
            var expected = new[] {"InnerProperty", "Property"};

            // Act
            var res = ExpressionHelpers.GetMemberNames<Outter, int>(x => x.InnerProperty.Property);

            // Assert
            Assert.AreEqual(expected, res);
        }

        [Test]
        public void Given_PropertyChain_With_Function_Then_Throws()
        {
            // Act
            var res = ExpressionHelpers.GetMemberNames<Outter, string>(x => x.InnerProperty.GetName());

            // Assert
            // Assert.IsTrue(res);
        }
    }
}