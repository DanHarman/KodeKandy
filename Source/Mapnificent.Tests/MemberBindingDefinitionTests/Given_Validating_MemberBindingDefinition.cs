using System.Linq;
using KodeKandy.Mapnificent.MemberAccess;
using KodeKandy.Mapnificent.Tests.TestEntities;
using NUnit.Framework;

namespace KodeKandy.Mapnificent.Tests.MemberBindingDefinitionTests
{
    [TestFixture]
    public class Given_Validating_MemberBindingDefinition
    {
        [Test]
        public void When_No_From_Definition_Then_Invalid()
        {
            // Arrange
            var memberInfo = typeof(SimpleTo).GetMember("StringProp").Single();
            var sut = new BindingDefinition(memberInfo, BindingDefinitionType.Explicit);

            // Act
            var res = BindingDefinitionValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual("Binding definition does not define a 'from' source.", res[0].Reason);
        }

        [Test]
        public void When_Bound_From_Definition_Then_Valid()
        {
            // Arrange
            var memberInfo = typeof(SimpleTo).GetMember("StringProp").Single();
            var sut = new BindingDefinition(memberInfo, BindingDefinitionType.Explicit);
            sut.FromDefinition = new FromMemberDefinition("StringProp", typeof(string),
                ReflectionHelpers.CreateSafeWeakMemberChainGetter(new[] {memberInfo}));

            // Act
            var res = BindingDefinitionValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual(0, res.Count);
        }

        [Test]
        public void When_Explicit_From_Definition_Then_Valid()
        {
            // Arrange
            var memberInfo = typeof(SimpleTo).GetMember("StringProp").Single();
            var sut = new BindingDefinition(memberInfo, BindingDefinitionType.Explicit)
            {
                FromDefinition = new FromCustomDefinition(context => "Wow")
            };

            // Act
            var res = BindingDefinitionValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual(0, res.Count);
        }
    }
}