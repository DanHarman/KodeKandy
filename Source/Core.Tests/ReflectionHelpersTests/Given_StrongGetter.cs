using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.ReflectionHelpersTests
{
    [TestFixture]
    public class Given_StrongGetter
    {
        [Test]
        public void Given_WeakFieldGetter_When_Getting_Field_Then_Suceeds()
        {
            // Arrange
            var propertyInfo = typeof(Shape).GetProperty("Edges");
            const int expectedEdges = 7;
            var shape = new Shape() { Edges = expectedEdges };

            // Act
            var sut = (Func<Shape, int>)ReflectionHelpers.CreatePropertyGetter(propertyInfo);
            var res = sut(shape);

            // Assert
            Assert.AreEqual(expectedEdges, res);
        }
    }
}
