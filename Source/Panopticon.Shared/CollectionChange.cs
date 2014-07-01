// <copyright file="CollectionChange.cs" company="million miles per hour ltd">
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
using System.Collections.ObjectModel;
using System.Linq;

namespace KodeKandy.Panopticon
{
    public enum CollectionChangeOperation
    {
        Image,
        Add,
        Remove,
        Update,
        ItemChange
    }

//    public interface IOrderedCollectionChange
//    {
//        /// <summary>
//        ///     The originator of the change notification.
//        /// </summary>
//        object Source { get; }
//
//        /// <summary>
//        ///     The operation that was performed.
//        /// </summary>
//        CollectionChangeOperation Operation { get; }
//
//        /// <summary>
//        ///     The index of the first item in the ordered collection to be impacted by the operation.
//        /// </summary>
//        long FirstImpactedIndex { get; }
//    }

    public class CollectionChange<T> //: IOrderedCollectionChange
        : IEquatable<CollectionChange<T>>
    {
        public static readonly ReadOnlyCollection<T> EmptyItemps = new ReadOnlyCollection<T>(new T[] {});

        public CollectionChange(object source, CollectionChangeOperation operation, ReadOnlyCollection<T> items)
            : this(source, operation, items, EmptyItemps)
        {
        }

        public CollectionChange(object source, CollectionChangeOperation operation, ReadOnlyCollection<T> items, ReadOnlyCollection<T> replacedItems)
        {
            Source = source;
            Operation = operation;
            Items = items;
            ReplacedItems = replacedItems;
        }

        #region Equality members

        public bool Equals(CollectionChange<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Operation == other.Operation && Source.Equals(other.Source) && Items.SequenceEqual(other.Items) &&
                   ReplacedItems.SequenceEqual(other.ReplacedItems) && FirstImpactedIndex == other.FirstImpactedIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CollectionChange<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Operation;
                hashCode = (hashCode * 397) ^ Source.GetHashCode();
                hashCode = (hashCode * 397) ^ Items.GetHashCode();
                hashCode = (hashCode * 397) ^ ReplacedItems.GetHashCode();
                hashCode = (hashCode * 397) ^ FirstImpactedIndex.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(CollectionChange<T> left, CollectionChange<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CollectionChange<T> left, CollectionChange<T> right)
        {
            return !Equals(left, right);
        }

        #endregion

        /// <summary>
        ///     The originator of the change notification.
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        ///     The operation that was performed.
        /// </summary>
        public CollectionChangeOperation Operation { get; private set; }

        /// <summary>
        ///     The items impacted by the operation.
        /// </summary>
        public ReadOnlyCollection<T> Items { get; private set; }

        /// <summary>
        ///     The items that were replaced by the update.
        /// </summary>
        public ReadOnlyCollection<T> ReplacedItems { get; private set; }

        /// <summary>
        ///     The index of the first item in the ordered collection to be impacted by the operation.
        /// </summary>
        public long FirstImpactedIndex { get; private set; }
    }

    public static class CollectionChange
    {
        public static CollectionChange<T> CreateImage<T>(object source, ReadOnlyCollection<T> items)
        {
            return new CollectionChange<T>(source, CollectionChangeOperation.Image, items);
        }

        public static CollectionChange<T> CreateImage<T>(object source, params T[] items)
        {
            return new CollectionChange<T>(source, CollectionChangeOperation.Image, new ReadOnlyCollection<T>(items));
        }

        public static CollectionChange<T> CreateAdd<T>(object source, ReadOnlyCollection<T> items)
        {
            return new CollectionChange<T>(source, CollectionChangeOperation.Add, items);
        }

        public static CollectionChange<T> CreateAdd<T>(object source, params T[] items)
        {
            return new CollectionChange<T>(source, CollectionChangeOperation.Add, new ReadOnlyCollection<T>(items));
        }

        public static CollectionChange<T> CreateRemove<T>(object source, ReadOnlyCollection<T> items)
        {
            return new CollectionChange<T>(source, CollectionChangeOperation.Remove, items);
        }

        public static CollectionChange<T> CreateRemove<T>(object source, params T[] items)
        {
            return new CollectionChange<T>(source, CollectionChangeOperation.Remove, new ReadOnlyCollection<T>(items));
        }

        public static CollectionChange<T> CreateUpdate<T>(object source, ReadOnlyCollection<T> items, ReadOnlyCollection<T> replacedItem)
        {
            return new CollectionChange<T>(source, CollectionChangeOperation.Update, items, replacedItem);
        }
    }
}