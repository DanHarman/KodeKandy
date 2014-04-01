// <copyright file="UnifiedScheduler.cs" company="million miles per hour ltd">
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
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace KodeKandy.Threading
{
    [DebuggerDisplay("{Name}")]
    public abstract class UnifiedScheduler : IScheduler
    {
        private readonly IScheduler rxScheduler;
        private readonly TaskScheduler taskScheduler;
        public string Name { get; private set; }

        protected UnifiedScheduler(string name, IScheduler rxScheduler, TaskScheduler taskScheduler)
        {
            Require.NotNullOrEmpty(name, "name");
            Require.NotNull(rxScheduler, "rxScheduler");
            Require.NotNull(taskScheduler, "taskScheduler");

            Name = name;
            this.rxScheduler = rxScheduler;
            this.taskScheduler = taskScheduler;
        }

        public DateTimeOffset Now
        {
            get { return rxScheduler.Now; }
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return rxScheduler.Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return rxScheduler.Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            return rxScheduler.Schedule(state, action);
        }

        public static implicit operator TaskScheduler(UnifiedScheduler contextScheduler)
        {
            return contextScheduler.taskScheduler;
        }
    }
}