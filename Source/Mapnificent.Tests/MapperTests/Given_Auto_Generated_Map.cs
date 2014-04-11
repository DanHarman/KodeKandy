// <copyright file="Given_Auto_Generated_Map.cs" company="million miles per hour ltd">
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
using KodeKandy.Mapnificent.Projections.MemberAccess;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MapperTests
{
    [TestFixture]
    public class Given_Auto_Generated_Map
    {
        [Test]
        public void When_Enumerating_Bindings_Then_MemberDefinitiontType_Is_Auto()
        {
            // Arrange
            var sut = new Mapper();
            var map = sut.BuildClassMap<SimpleFrom, SimpleTo>().Map;

            // Act
            var res = map.Bindings;

            // Assert
            Assert.IsTrue(res.All(b => b.BindingType == BindingType.Auto));
        }

        [Test]
        public void When_Mapping_Flattening_Class_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.BuildClassMap<FlatteningFrom, FlatteningTo>();
            var from = new FlatteningFrom {Child = new FlatteningFrom.FlatteningChildFrom {Name = "Bob"}};
            var to = new FlatteningTo();

            // Act
            sut.Merge(from, to);

            // Assert
            Assert.AreEqual(from.Child.Name, to.ChildName);
        }

        [Test]
        public void When_Mapping_Nested_Class_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.BuildClassMap<NestedFrom, NestedTo>();
            sut.BuildClassMap<NestedFrom.NestedChildFrom, NestedTo.NestedChildTo>();
            var from = new NestedFrom {Child = new NestedFrom.NestedChildFrom {Name = "Bob"}};
            var to = new NestedTo {Child = new NestedTo.NestedChildTo()};

            // Act
            sut.Merge(from, to);

            // Assert
            Assert.AreEqual(from.Child.Name, to.Child.Name);
            Assert.AreNotEqual(from.Child, to.Child); // Ensure mapping of class not ref copying!
        }

        [Test]
        public void When_Mapping_Simple_Class_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.BuildClassMap<SimpleFrom, SimpleTo>();
            var from = new SimpleFrom() {StringProp = "Bob", IntField = 20};
            var to = new SimpleTo();

            // Act
            sut.Merge(from, to);

            // Assert
            Assert.AreEqual(from.StringProp, to.StringProp);
            Assert.AreEqual(from.IntField, to.IntField);
        }

        [Test]
        public void When_Mapping_With_Conversion_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.BuildClassMap<ConversionFrom, ConversionTo>();
            sut.BuildConversion<string, int>().Explictly(x => x.Count());
            var from = new ConversionFrom {Age = "Twelve"};
            var to = new ConversionTo {Age = 6};

            // Act
            sut.Merge(from, to);

            // Assert
            Assert.AreEqual(from.Age.Count(), to.Age);
        }
    }

    [TestFixture]
    public class Given_Customized_Map
    {
        [Test]
        public void When_Mapping_Value_Type_To_Class_Then_Maps()
        {
            // Arrange
            var sut = new Mapper();
            sut.BuildClassMap<int, SimpleTo>()
               .For(x => x.IntField, o => o.Custom(ctx => (int) ctx.FromInstance))
               .For(x => x.StringProp, o => o.Custom(ctx => ((int) ctx.FromInstance).ToString()));
            const int from = 12;
            var to = new SimpleTo();

            // Act
            sut.Merge(from, to);

            // Assert
            Assert.AreEqual(from, to.IntField);
            Assert.AreEqual("12", to.StringProp);
        }
    }
}