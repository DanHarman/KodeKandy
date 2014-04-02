// <copyright file="ReadWriteTestEntity.cs" company="million miles per hour ltd">
// Copyright (c) 2013-2014 All Right Reserved
// 
// This source is subject to the MIT License.
// Please see the License.txt file for more information.
// All other rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>

using System.Reflection;

namespace KodeKandy.TestEntities
{
    public class ReadWriteTestEntity
    {
        public static readonly string ReadWritePropertyName = "ReadWriteProperty";
        public static readonly MemberInfo ReadWritePropertyMemberInfo = typeof(ReadWriteTestEntity).GetProperty(ReadWritePropertyName);

        public static readonly string WriteOnlyPropertyName = "WriteOnlyProperty";
        public static readonly MemberInfo WriteOnlyPropertyMemberInfo = typeof(ReadWriteTestEntity).GetProperty(WriteOnlyPropertyName);

        public static readonly string ReadOnlyPropertyName = "ReadOnlyProperty";
        public static readonly MemberInfo ReadOnlyPropertyMemberInfo = typeof(ReadWriteTestEntity).GetProperty(ReadOnlyPropertyName);

        public static readonly string FieldName = "Field";
        public static readonly MemberInfo FieldMemberInfo = typeof(ReadWriteTestEntity).GetField(FieldName);
        public int Field;
        public int ReadWriteProperty { get; set; }
        public int WriteOnlyProperty
        {
            set { Field = value; }
        }
        public int ReadOnlyProperty
        {
            get { return Field; }
        }
    }
}