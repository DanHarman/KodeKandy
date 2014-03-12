// <copyright file="SynchronisedScheduler.cs" company="million miles per hour ltd">
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

using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace KodeKandy.Threading
{
    /// <summary>
    ///     Unified scheduler for syncronised work. A typical example would be a DispatcherScheduler.
    /// </summary>
    public abstract class SynchronisedScheduler : UnifiedScheduler
    {
        protected SynchronisedScheduler(string name, IScheduler rxScheduler, TaskScheduler taskScheduler)
            : base(name, rxScheduler, taskScheduler)
        {
        }

        /// <summary>
        ///     Verify if the current thread is on the correct syncronised thread.
        /// </summary>
        public abstract bool CheckAccess();
    }
}