// <copyright file="Given_Mapper.cs" company="million miles per hour ltd">
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

using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Mapper
    {
        public void When_Mapping_Undefined_Class_Then_Throws()
        {
            var sut = new Mapper();

            //   Assert.Throws<MapnificentException>(() => sut.MapInto<Simple>(new Object()));
        }

        [Test]
        public void When_Instantiated_Then_Has_String_Map()
        {
            // Arrange
            var sut = new Mapper();

            var x = sut.GetMap(typeof(string), typeof(string));
            var y = x.Bindings;

            // Assert
            Assert.IsTrue(sut.HasMap(typeof(string), typeof(string)));
        }
    }
}