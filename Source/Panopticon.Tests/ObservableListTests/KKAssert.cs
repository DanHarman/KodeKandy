// <copyright file="KKAssert.cs" company="million miles per hour ltd">
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

using Newtonsoft.Json;
using NUnit.Framework;

namespace KodeKandy.Panopticon.Tests.ObservableListTests
{
    public static class KKAssert
    {
        /// <summary>
        ///     Assert that two objects are identical by value comparison.
        /// </summary>
        /// <remarks>Serializes to JSON to perform the comparison.</remarks>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        public static void AreEqual(object expected, object actual)
        {
            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }
    }
}