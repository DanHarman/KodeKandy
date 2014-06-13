// <copyright file="ImmutableList.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Panopticon.Internal
{
    /// <summary>
    ///     Immutable list used to safely manage lists of items in an immutable fashion.
    /// </summary>
    internal class ImmutableList<T>
    {
        private readonly T[] data;

        public ImmutableList()
        {
            data = new T[0];
        }

        public ImmutableList(T[] data)
        {
            this.data = data;
        }

        public T[] Data
        {
            get { return data; }
        }

        public ImmutableList<T> Add(T value)
        {
            var newData = new T[data.Length + 1];
            Array.Copy(data, newData, data.Length);
            newData[data.Length] = value;
            return new ImmutableList<T>(newData);
        }

        public ImmutableList<T> Remove(T value)
        {
            var i = Array.IndexOf(data, value);
            if (i < 0)
                return this;
            var newData = new T[data.Length - 1];
            Array.Copy(data, 0, newData, 0, i);
            Array.Copy(data, i + 1, newData, i, data.Length - i - 1);
            return new ImmutableList<T>(newData);
        }
    }
}