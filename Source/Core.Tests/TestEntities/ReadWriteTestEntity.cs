using System.Reflection;

namespace KodeKandy.TestEntities
{
    public class ReadWriteTestEntity
    {
        public int ReadWriteProperty { get; set; }
        public static readonly string ReadWritePropertyName = "ReadWriteProperty";
        public static readonly MemberInfo ReadWritePropertyMemberInfo = typeof(ReadWriteTestEntity).GetProperty(ReadWritePropertyName);

        public int WriteOnlyProperty { set { Field = value; } }
        public static readonly string WriteOnlyPropertyName = "WriteOnlyProperty";
        public static readonly MemberInfo WriteOnlyPropertyMemberInfo = typeof(ReadWriteTestEntity).GetProperty(WriteOnlyPropertyName);

        public int ReadOnlyProperty { get { return Field; } }
        public static readonly string ReadOnlyPropertyName = "ReadOnlyProperty";
        public static readonly MemberInfo ReadOnlyPropertyMemberInfo = typeof(ReadWriteTestEntity).GetProperty(ReadOnlyPropertyName);

        public int Field;
        public static readonly string FieldName = "Field";
        public static readonly MemberInfo FieldMemberInfo = typeof(ReadWriteTestEntity).GetField(FieldName);

    }
}