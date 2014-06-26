// <copyright file="Given_UserDataOrDefault.cs" company="million miles per hour ltd">
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
using System.ComponentModel;
using KodeKandy.Panopticon.Linq;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.Linq
{
    [TestFixture]
    public class Given_UserDataOrDefault
    {
        [Test]
        public void When_PropertyChangedEventArgsEx_Then_Returns_UserData()
        {
            // Arrange
            var sut = new PropertyChangedEventArgsEx("A Property", new Object());

            // Act
            var res = sut.UserDataOrDefault();

            // Assert
            Assert.AreEqual(sut.UserData, res);
        }

        [Test]
        public void When_PropertyChangedEventArgs_Then_Returns_Null()
        {
            // Arrange
            var sut = new PropertyChangedEventArgs("A Property");

            // Act
            var res = sut.UserDataOrDefault();

            // Assert
            Assert.IsNull(res);
        }
    }
}