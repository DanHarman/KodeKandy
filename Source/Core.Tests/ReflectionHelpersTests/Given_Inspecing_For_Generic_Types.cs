// <copyright file="Given_Inspecing_For_Generic_Types.cs" company="million miles per hour ltd">
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace KodeKandy.ReflectionHelpersTests
{
    [TestFixture]
    public class Given_Inspecing_For_Generic_Types
    {
        [Test]
        public void When_GenericType_Param_Is_Not_Generic_Then_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => typeof(string).DoesImplementGenericType(typeof(int)), "Must be a generic type.");
        }

        [TestCase(typeof(List<string>), true)]
        [TestCase(typeof(Collection<string>), true)]
        [TestCase(typeof(int), false)]
        [Test]
        public void When_Inspecting_Type_For_IEnumerable_Then_Result_Expected(Type type, bool expected)
        {
            // Act
            var res = type.DoesImplementGenericType(typeof(IEnumerable<>));

            // Assert
            Assert.AreEqual(expected, res);
        }

        [TestCase(typeof(List<string>), typeof(IEnumerable<>), typeof(IEnumerable<string>))]
        [TestCase(typeof(Collection<int>), typeof(ICollection<>), typeof(ICollection<int>))]
        [TestCase(typeof(Dictionary<int, int>), typeof(IList<>), null)]
        [Test]
        public void When_Trying_To_Get_Generic_Definition_Then_Result_Expected(Type type, Type genericType, Type expected)
        {
            // Act
            var res = type.TryGetGenericTypeDefinitionOfType(genericType);

            // Assert
            Assert.AreEqual(expected, res);
        }
    }
}