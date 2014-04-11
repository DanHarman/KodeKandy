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

using System.Collections.Generic;
using KodeKandy.Mapnificent.Tests.TestEntities;
using KodeKandy.QualityTools;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Mapping_Collection
    {
        private Mapper sut;

        [Test]
        public void When_Mapping_Then_Mapped()
        {
            // Arrange
            sut = new Mapper();
            var from = new List<string> {"one", "two", "three"};

            // Act
            var res = sut.Map<List<string>>(from);

            // Assert
            Assert.NotNull(res);
            CollectionAssert.AreEqual(from, res);
        }
    }

    [TestFixture]
    public class Given_Mapping_With_ChildCollections
    {
        [Test]
        public void When_Mapping_Object_With_Complex_Enumerable_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.BuildClassMap<SimpleFrom, SimpleTo>();
            sut.BuildClassMap<ContainsEnumerableTFrom, ContainsListTTo>();
            var from = new ContainsEnumerableTFrom
            {
                Collection = new List<SimpleFrom>
                {
                    new SimpleFrom {StringProp = "One", IntField = 1},
                    new SimpleFrom {StringProp = "Two", IntField = 2},
                    new SimpleFrom {StringProp = "Three", IntField = 3},
                }
            };

            // Act
            var res = sut.Map<ContainsListTTo>(from);

            // Assert
            KKAssert.AreEqualByValue(from, res);
        }
    }

    [TestFixture]
    public class Given_Mapper
    {
        public void When_Mapping_Undefined_Class_Then_Throws()
        {
            var sut = new Mapper();

            //   Assert.Throws<MapnificentException>(() => sut.Merge<Simple>(new Object()));
        }

        [Test]
        public void When_Instantiated_Then_Has_String_Map()
        {
            // Arrange
            var sut = new Mapper();

            var x = sut.GetClassMap(typeof(string), typeof(string));
            var y = x.Bindings;

            // Assert
            Assert.IsTrue(sut.HasMap(typeof(string), typeof(string)));
        }
    }
}