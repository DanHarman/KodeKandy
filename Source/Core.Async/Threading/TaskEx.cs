// <copyright file="TaskEx.cs" company="million miles per hour ltd">
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
using System.Linq;
using System.Threading.Tasks;

namespace KodeKandy.Threading
{
    public static class TaskEx
    {
        /// <summary>
        ///     The delegate is always invoked, so whilst not really a true 'Always' since there is nothing ensuring it is
        ///     attached to the end of a continuation chain, unlike Then() it does not skip its delegate if the antecedent
        ///     is faulted.
        ///     Defaults to background scheduler so be explicit if modifying a sync context object.
        /// </summary>
        public static Task Always(this Task task, Action finallyAction, TaskScheduler scheduler = null)
        {
            Require.NotNull(task, "task");
            Require.NotNull(finallyAction, "finallyAction");

            var tcs = new TaskCompletionSource<object>();

            task.ContinueWith(_ =>
            {
                Exception finallyException = null;
                try
                {
                    finallyAction();
                }
                catch (Exception ex)
                {
                    finallyException = ex;
                }

                if (finallyException != null || task.IsFaulted)
                {
                    var exceptions = new[] {task.Exception, finallyException}.Where(e => e != null);
                    tcs.TrySetException(exceptions);
                }
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else tcs.TrySetResult(null);
            }, scheduler ?? BackgroundScheduler.Default);

            return tcs.Task;
        }

        public static Task<T> FromResult<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        /// <summary>
        ///     Creates a faulted task without performing any async work.
        /// </summary>
        /// <typeparam name="T">The task type.</typeparam>
        /// <param name="exception">The exception that faults the task.</param>
        /// <returns>A task faulted with exception.</returns>
        public static Task<T> FromException<T>(Exception exception)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}