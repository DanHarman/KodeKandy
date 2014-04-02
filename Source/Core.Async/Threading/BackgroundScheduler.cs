// <copyright file="BackgroundScheduler.cs" company="million miles per hour ltd">
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

using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace KodeKandy.Threading
{
    /// <summary>
    ///     Unified scheduler for background work.
    /// </summary>
    public class BackgroundScheduler : UnifiedScheduler
    {
        static BackgroundScheduler()
        {
            Default = CreateDefaultScheduler();
        }

        public BackgroundScheduler(string name, IScheduler rxScheduler, TaskScheduler taskScheduler)
            : base(name, rxScheduler, taskScheduler)
        {
        }

        public static BackgroundScheduler Default { get; protected internal set; }

        protected internal static BackgroundScheduler CreateDefaultScheduler()
        {
            return new BackgroundScheduler("DefaultBackgroundScheduler", Scheduler.Default, TaskScheduler.Default);
        }
    }
}