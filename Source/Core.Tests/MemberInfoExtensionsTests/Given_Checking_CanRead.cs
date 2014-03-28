using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.MemberInfoExtensionsTests
{
    [TestFixture]
    public class Given_Checking_CanRead
    {
        [Test]
        public void When_Property_Is_Readable_Then_True()
        {
            // Act
            var res = ReadWriteTestEntity.ReadWritePropertyMemberInfo.CanRead();

            // Assert
            Assert.IsTrue(res);
        }

        [Test]
        public void When_Property_Is_Unreadable_Then_False()
        {
            // Act
            var res = ReadWriteTestEntity.WriteOnlyPropertyMemberInfo.CanRead();

            // Assert
            Assert.IsFalse(res);
        }

        [Test]
        public void When_Field_Then_True()
        {
            // Act
            var res = ReadWriteTestEntity.FieldMemberInfo.CanRead();

            // Assert
            Assert.IsTrue(res);
        }
    }
}