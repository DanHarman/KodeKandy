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
// </copyright>

using System;
using System.Collections.Generic;
using KodeKandy.Mapnificent.Projections;

namespace KodeKandy.Mapnificent
{
    /// <summary>
    ///     Defines the types projected in a ClassMap or conversion.
    /// </summary>
    public class ProjectionType
    {
        /// <summary>
        ///     Type indicating that a from projection has not yet been defined.
        /// </summary>
        public static Type Undefined = typeof(UndefinedType);

        /// <summary>
        ///     Type indicating that a projection has a custom from delegate so it is not a simple field binding projection.
        /// </summary>
        public static Type Custom = typeof(CustomType);

        public ProjectionType(Type fromType, Type toType)
        {
            Require.NotNull(fromType, "fromType");
            Require.NotNull(toType, "toType");

            // If this is a list mapping then we upcast the fromType to an IEnumerable<T> and the toType to IList<T>, this saves having to explicitly store
            // every permutation of collection type that derive from these base classes.
            // On second thoughts we are going to be explicit at the moment so the comment above is liable for deletion. TODO delete once certain.
            var fromEnumerableType = fromType.TryGetGenericTypeDefinitionOfType(typeof(IEnumerable<>));
            var toListType = toType.TryGetGenericTypeDefinitionOfType(typeof(IList<>));

            if (fromEnumerableType != null && toListType != null)
            {
                FromItemType = fromEnumerableType.GetGenericArguments()[0];
                ToItemType = toListType.GetGenericArguments()[0];
            }

            FromType = fromType;
            ToType = toType;
        }

        public Type FromType { get; private set; }
        public Type ToType { get; private set; }
        public Type FromItemType { get; private set; }
        public Type ToItemType { get; private set; }

        /// <summary>
        ///     Indicates that the projection corresponds to a <see cref="ClassMap" />.ef
        /// </summary>
        public bool IsClassProjection
        {
            get { return ToType.IsClass; }
        }

        /// <summary>
        ///     Indicates if the projection type is from and to the same type. i.e. a clone. This implies no requirement for a
        ///     conversion.
        /// </summary>
        public bool IsIdentity
        {
            get { return ToType == FromType; }
        }

        /// <summary>
        ///     Indicates if the projection type corresponds to <see cref="IEnumerable{T}" /> to <see cref="IList{T}" />.
        /// </summary>
        public bool IsListProjection
        {
            get { return FromItemType != null && ToItemType != null; }
        }

        public static ProjectionType Create<TFrom, TTo>()
        {
            return new ProjectionType(typeof(TFrom), typeof(TTo));
        }

        protected bool Equals(ProjectionType other)
        {
            return FromType == other.FromType && ToType == other.ToType && FromItemType == other.FromItemType && ToItemType == other.ToItemType;
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

        #region Nested type: CustomType

        public class CustomType
        {
            private CustomType()
            {
            }
        };

        #endregion

        #region Nested type: UndefinedType

        public class UndefinedType
        {
            private UndefinedType()
            {
            }
        };

        #endregion
    }
}