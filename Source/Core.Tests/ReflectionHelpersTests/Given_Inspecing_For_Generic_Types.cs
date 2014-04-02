using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace KodeKandy.ReflectionHelpersTests
{
    [TestFixture]
    public class Given_Inspecing_For_Generic_Types
    {
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

        [Test]
        public void When_GenericType_Param_Is_Not_Generic_Then_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => typeof(string).DoesImplementGenericType(typeof(int)), "Must be a generic type.");
        }
    }
}