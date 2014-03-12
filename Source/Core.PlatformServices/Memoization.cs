// <copyright file="Memoization.cs" company="million miles per hour ltd">
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
using System.Collections.Concurrent;

namespace KodeKandy
{
    /// <summary>
    ///     A class to cache function results to increase performance when the result is the same and without side effect.
    ///     i.e. Pure http://en.wikipedia.org/wiki/Pure_function
    ///     This elegant memoizer class is based on Eric Lippert code on stack overflow.
    ///     http://stackoverflow.com/questions/2852161/c-sharp-memoization-of-functions-with-arbitrary-number-of-arguments
    ///     Enhanced to use concurrent dictionary so it is thread safe, although it is important
    ///     that the memoized func is idempotent and side effect free as it may in a race condition be executed more than
    ///     once for a given key (similar to the ConcurrentDictionary behaviour).
    /// </summary>
    public static class Memoization
    {
        /// <summary>
        ///     Memoize a single param function.
        /// </summary>
        public static Func<T, TRes> Memoize<T, TRes>(this Func<T, TRes> func)
        {
            var cache = new ConcurrentDictionary<T, TRes>();

            return a => cache.GetOrAdd(a, func);
        }

        /// <summary>
        ///     Memoize a two param function.
        /// </summary>
        public static Func<T1, T2, TRes> Memoize<T1, T2, TRes>(this Func<T1, T2, TRes> func)
        {
            Func<Tuple<T1, T2>, TRes> tuplified = t => func(t.Item1, t.Item2);
            Func<Tuple<T1, T2>, TRes> memoized = tuplified.Memoize();
            return (a, b) => memoized(Tuple.Create(a, b));
        }

        /// <summary>
        ///     Memoize a three param function.
        /// </summary>
        public static Func<T1, T2, T3, TRes> Memoize<T1, T2, T3, TRes>(this Func<T1, T2, T3, TRes> func)
        {
            Func<Tuple<T1, T2, T3>, TRes> tuplified = t => func(t.Item1, t.Item2, t.Item3);
            Func<Tuple<T1, T2, T3>, TRes> memoized = tuplified.Memoize();
            return (a, b, c) => memoized(Tuple.Create(a, b, c));
        }

        /// <summary>
        ///     Memoize a four param function.
        /// </summary>
        public static Func<T1, T2, T3, T4, TRes> Memoize<T1, T2, T3, T4, TRes>(this Func<T1, T2, T3, T4, TRes> func)
        {
            Func<Tuple<T1, T2, T3, T4>, TRes> tuplified = t => func(t.Item1, t.Item2, t.Item3, t.Item4);
            Func<Tuple<T1, T2, T3, T4>, TRes> memoized = tuplified.Memoize();
            return (a, b, c, d) => memoized(Tuple.Create(a, b, c, d));
        }
    }
}