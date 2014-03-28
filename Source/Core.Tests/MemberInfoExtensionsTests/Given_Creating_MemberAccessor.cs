using KodeKandy.TestEntities;
using NUnit.Framework;

namespace KodeKandy.MemberInfoExtensionsTests
{
    [TestFixture]
    public class Given_Creating_MemberAccessor
    {
        [Test]
        public void When_Member_Is_ReadWriteProperty_Then_Constructed_Correctly()
        {
            // Act
            var sut = new MemberAccessor(ReadWriteTestEntity.ReadWritePropertyMemberInfo);

            // Asert
            Assert.AreEqual(sut.Type, MemberAccessorType.Property);
            Assert.AreEqual(sut.MemberName, ReadWriteTestEntity.ReadWritePropertyName);
            Assert.AreEqual(typeof(ReadWriteTestEntity), sut.DeclaringType);
            Assert.AreEqual(typeof(int), sut.MemberType);
            Assert.NotNull(sut.Getter);
            Assert.NotNull(sut.Setter);
        }

        [Test]
        public void When_Member_Is_WriteOnlyProperty_Then_Constructed_Correctly()
        {
            // Act
            var sut = new MemberAccessor(ReadWriteTestEntity.WriteOnlyPropertyMemberInfo);

            // Asert
            Assert.AreEqual(sut.Type, MemberAccessorType.Property);
            Assert.AreEqual(sut.MemberName, ReadWriteTestEntity.WriteOnlyPropertyName);
            Assert.AreEqual(typeof(ReadWriteTestEntity), sut.DeclaringType);
            Assert.AreEqual(typeof(int), sut.MemberType);
            Assert.IsNull(sut.Getter);
            Assert.NotNull(sut.Setter);
        }

        [Test]
        public void When_Member_Is_ReadOnlyProperty_Then_Constructed_Correctly()
        {
            // Act
            var sut = new MemberAccessor(ReadWriteTestEntity.ReadOnlyPropertyMemberInfo);

            // Asert
            Assert.AreEqual(sut.Type, MemberAccessorType.Property);
            Assert.AreEqual(sut.MemberName, ReadWriteTestEntity.ReadOnlyPropertyName);
            Assert.AreEqual(typeof(ReadWriteTestEntity), sut.DeclaringType);
            Assert.AreEqual(typeof(int), sut.MemberType);
            Assert.NotNull(sut.Getter);
            Assert.IsNull(sut.Setter);
        }

        [Test]
        public void When_Member_Is_Field_Then_Constructed_Correctly()
        {
            // Act
            var sut = new MemberAccessor(ReadWriteTestEntity.FieldMemberInfo);

            // Asert
            Assert.AreEqual(sut.Type, MemberAccessorType.Field);
            Assert.AreEqual(sut.MemberName, ReadWriteTestEntity.FieldName);
            Assert.AreEqual(typeof(ReadWriteTestEntity), sut.DeclaringType);
            Assert.AreEqual(typeof(int), sut.MemberType);
            Assert.NotNull(sut.Getter);
            Assert.NotNull(sut.Setter);
        }
    }
}