// <copyright file="Given_Creating_MemberBindingDefinition.cs" company="million miles per hour ltd">
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
using KodeKandy.Mapnificent.MemberAccess;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MemberBindingDefinitionTests
{
    [TestFixture]
    public class Given_Creating_MemberBindingDefinition
    {
        [Test]
        public void When_ToMemberInfo_Null_Then_Throws_ArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Binding(null, BindingType.Auto));
        }
    }
}