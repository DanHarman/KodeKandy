using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.MemberInfoExtensionsTests
{
    [TestFixture]
    public class Given_Checking_CanRWrite
    {
        [Test]
        public void When_Property_Is_Writeable_Then_True()
        {
            // Act
            var res = ReadWriteTestEntity.ReadWritePropertyMemberInfo.CanWrite();

            // Assert
            Assert.IsTrue(res);
        }

        [Test]
        public void When_Property_Is_Unwriteable_Then_False()
        {
            // Act
            var res = ReadWriteTestEntity.ReadOnlyPropertyMemberInfo.CanWrite();

            // Assert
            Assert.IsFalse(res);
        }

        [Test]
        public void When_Field_Then_True()
        {
            // Act
            var res = ReadWriteTestEntity.FieldMemberInfo.CanWrite();

            // Assert
            Assert.IsTrue(res);
        }
    }
}