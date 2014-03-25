using System.Linq;
using System.Resources;
using KodeKandy.Mapnificent.Bindngs;
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
            var sut = MemberBindingDefinition.Create(memberInfo, MemberBindingDefinitionType.Explicit);

            // Act
            var res = MemberBindingDefinitionValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual("Binding definition does not define a 'from' source.", res[0].Reason);
        }

        [Test]
        public void When_Bound_From_Definition_Then_Valid()
        {
            // Arrange
            var memberInfo = typeof(SimpleTo).GetMember("StringProp").Single();
            var sut = MemberBindingDefinition.Create(memberInfo, MemberBindingDefinitionType.Explicit);
            sut.FromMemberDefinition = new MemberGetterDefinition(typeof(SimpleFrom), "StringProp", typeof(string), ReflectionHelpers.CreateSafeWeakMemberChainGetter(new [] {memberInfo}));

            // Act
            var res = MemberBindingDefinitionValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual(0, res.Count);
        }

        [Test]
        public void When_Explicit_From_Definition_Then_Valid()
        {
            // Arrange
            var memberInfo = typeof(SimpleTo).GetMember("StringProp").Single();
            var sut = MemberBindingDefinition.Create(memberInfo, MemberBindingDefinitionType.Explicit);
            sut.FromCustomDefinition = context => "Wow";

            // Act
            var res = MemberBindingDefinitionValidator.Validate(sut, new Mapper());

            // Assert
            Assert.AreEqual(0, res.Count);
        }
    }
}