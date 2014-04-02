// <copyright file="Given_Validating_MemberBindingDefinition.cs" company="million miles per hour ltd">
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

using System.Linq;
using KodeKandy.Mapnificent.MemberAccess;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MemberBindingDefinitionTests
{
    [TestFixture]
    public class Given_Validating_MemberBindingDefinition
    {
        [Test]
        public void When_Bound_From_Definition_Then_Valid()
        {
            // Arrange
            var memberInfo = typeof(SimpleTo).GetMember("StringProp").Single();
            var sut = new Binding(memberInfo, BindingType.Explicit);
            sut.FromDefinition = new FromMemberDefinition("StringProp", typeof(string),
                ReflectionHelpers.CreateSafeWeakMemberChainGetter(new[] {memberInfo}));

            // Act
            var res = BindingValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual(0, res.Count);
        }

        [Test]
        public void When_Explicit_From_Definition_Then_Valid()
        {
            // Arrange
            var memberInfo = typeof(SimpleTo).GetMember("StringProp").Single();
            var sut = new Binding(memberInfo, BindingType.Explicit)
            {
                FromDefinition = new FromCustomDefinition(context => "Wow")
            };

            // Act
            var res = BindingValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual(0, res.Count);
        }

        [Test]
        public void When_No_From_Definition_Then_Invalid()
        {
            // Arrange
            var memberInfo = typeof(SimpleTo).GetMember("StringProp").Single();
            var sut = new Binding(memberInfo, BindingType.Explicit);

            // Act
            var res = BindingValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual("Binding definition does not define a 'from' source.", res[0].Reason);
        }
    }
}