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
            Assert.Throws<ArgumentNullException>(() => new BindingDefinition(null, BindingDefinitionType.Auto));
        }
    }
}