// <copyright file="ProjectionType.cs" company="million miles per hour ltd">
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
// 
// </copyright>

using System;

namespace KodeKandy.Mapnificent
{
    /// <summary>
    /// Defines the types projected in a map or conversion.
    /// </summary>
    public class ProjectionType
    {
        public Type FromType { get; set; }
        public Type ToType { get; set; }

        public bool IsMap
        {
            get { return ToType.IsClass; }
        }

        public bool IsIdentity
        {
            get { return ToType == FromType; }
        }

        public ProjectionType(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");

            FromType = fromType;
            ToType = toType;
        }

        public static ProjectionType Create<TFrom, TTo>()
        {
            return new ProjectionType(typeof(TFrom), typeof(TTo));
        }

        protected bool Equals(ProjectionType other)
        {
            return FromType == other.FromType && ToType == other.ToType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ProjectionType) obj);
        }

        public override int GetHashCode()
        {
            return FromType.GetHashCode() ^ ToType.GetHashCode();
        }

        public static bool operator ==(ProjectionType left, ProjectionType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectionType left, ProjectionType right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("'{0}'->'{1}'",
                FromType != null ? FromType.Name : "<Undefined>",
                ToType != null ? ToType.Name : "<Undefined>");
        }
    }
}