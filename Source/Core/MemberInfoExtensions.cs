using System;
using System.Reflection;

namespace KodeKandy
{
    /// <summary>
    ///     Extensions to ease working with <see cref="MemberInfo" /> objects.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        ///     Returns the type of the property or field described by the <see cref="MemberInfo" />.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo" /> to inspect.</param>
        /// <returns>The type of the property or field.</returns>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            Require.NotNull(memberInfo, "memberInfo");

            var propertyInfo = memberInfo as PropertyInfo;

            if (propertyInfo != null)
                return propertyInfo.PropertyType;

            var fieldInfo = memberInfo as FieldInfo;

            if (fieldInfo != null)
                return fieldInfo.FieldType;

            throw new Exception(String.Format("Unknown MemberInfoType '{0}'", memberInfo.DeclaringType));
        }

        /// <summary>
        ///     Check whether a member is readable.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo" /> to inspect.</param>
        /// <returns>True if the member is readable, otherwise false.</returns>
        public static bool CanRead(this MemberInfo memberInfo)
        {
            Require.NotNull(memberInfo, "memberInfo");

            var propertyInfo = memberInfo as PropertyInfo;

            return propertyInfo == null || propertyInfo.CanRead;
        }

        /// <summary>
        ///     Check whether a member is writeable.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo" /> to inspect.</param>
        /// <returns>True if the member is writeable, otherwise false.</returns>
        public static bool CanWrite(this MemberInfo memberInfo)
        {
            var propertyInfo = memberInfo as PropertyInfo;

            return propertyInfo == null || propertyInfo.CanWrite;
        }
    }
}