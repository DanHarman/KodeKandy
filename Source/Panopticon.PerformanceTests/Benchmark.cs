// <copyright file="Benchmark.cs" company="million miles per hour ltd">
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
using System.Diagnostics;
using System.Linq;

namespace Panopticon.PerformanceTests
{
    public static class Benchmark
    {
        public static void TimeOperation(int repeats, string label, Action timedOperation)
        {
            var sw = new Stopwatch();
            var res = new List<long>();
            for (var j = 0; j < repeats + 1; ++j)
            {
                GC.Collect();
                sw.Reset();
                sw.Start();
                timedOperation();
                sw.Stop();

                res.Add(sw.ElapsedMilliseconds);
            }

            Console.Error.WriteLine("{0} - Average : {1}ms", label, res.Skip(1).Average());
        }

        public static void TimeOperation<T>(int repeats, string label, Func<T> sutFactory, Action<T> timedOperation)
        {
            var sw = new Stopwatch();
            var res = new List<long>();
            for (var j = 0; j < repeats + 1; ++j)
            {
                GC.Collect();
                var suts = sutFactory();
                sw.Reset();
                sw.Start();
                timedOperation(suts);
                sw.Stop();

                res.Add(sw.ElapsedMilliseconds);
            }

            Console.Error.WriteLine("{0} - Average : {1}ms", label, res.Skip(1).Average());
        }
    }
}