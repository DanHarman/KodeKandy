// <copyright file="RexExtensions.cs" company="million miles per hour ltd">
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
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace KodeKandy
{
    public static class RexExtensions
    {
        public static IObservable<Rex<T>> TakeErrors<T>(this IObservable<Rex<T>> source)
        {
            return Observable.Create<Rex<T>>(obs =>
                source.Subscribe(x =>
                {
                    if (x.IsError)
                        obs.OnNext(x);
                },
                    obs.OnError,
                    obs.OnCompleted));
        }

        public static IObservable<Rex<T>> TakeResults<T>(this IObservable<Rex<T>> source)
        {
            return Observable.Create<Rex<T>>(obs =>
                source.Subscribe(x =>
                {
                    if (!x.IsError)
                        obs.OnNext(x);
                },
                    obs.OnError,
                    obs.OnCompleted));
        }

        /// <summary>
        ///     Applies a selector to a Result amplified in a <see cref="Rex{T}" />, if there is an error in the hope, it just
        ///     passes through.
        ///     Any exceptions thrown by the selector are placed into a <see cref="Rex{T}.Error" />.
        /// </summary>
        /// <typeparam name="T">The type of element in the <see cref="Rex{T}" />.</typeparam>
        /// <typeparam name="TResult">
        ///     The type of the elements in the result sequence, obtained by running the selector function
        ///     for each element in the source sequence.
        /// </typeparam>
        /// <param name="source">A sequence of elements to invoke a transform function on.</param>
        /// <param name="resultSelector">A transform function to apply to each source element.</param>
        /// <returns>
        ///     An observable sequence whose elements are the result of invoking the transform function on each element of
        ///     source.
        /// </returns>
        public static IObservable<Rex<TResult>> SelectResult<T, TResult>(this IObservable<Rex<T>> source, Func<T, TResult> resultSelector)
        {
            return source.Select(x =>
            {
                try
                {
                    return x.IsError ? Rex.Error<TResult>(x.Exception) : Rex.Result(resultSelector(x.Value));
                }
                catch (Exception ex)
                {
                    // This should never arrive here is the selected value is an Error, but just in case.
                    return Rex.Error<TResult>(x.IsError ? new AggregateException(ex, x.Exception) : ex);
                }
            });
        }

        public static IObservable<Rex<T>> ToRex<T>(this IObservable<T> source)
        {
            return Observable.Create<Rex<T>>(obs =>
                source.Subscribe(
                    x => obs.OnNext(Rex.Result(x)),
                    ex => obs.OnNext(Rex.Error<T>(ex)),
                    obs.OnCompleted));
        }

        public static IObservable<Rex<T>> ToRex<T>(this Task<T> task)
        {
            return Observable.Create<Rex<T>>(obs =>
                task.ToObservable().Subscribe(
                    x => obs.OnNext(Rex.Result(x)),
                    ex =>
                    {
                        obs.OnNext(Rex.Error<T>(ex));
                        obs.OnCompleted();
                    },
                    obs.OnCompleted));
        }

        public static IDisposable SubscribeRex<T>(this IObservable<Rex<T>> source, Action<T> onNextResult, Action<Exception> onNextError)
        {
            return source.Subscribe(x =>
            {
                if (x.IsError)
                    onNextError(x.Exception);
                else
                    onNextResult(x.Value);
            });
        }

        public static IDisposable SubscribeRex<T>(this IObservable<Rex<T>> source, Action<T> onNextResult, Action<Exception> onNextError,
            Action<Exception> onError)
        {
            return source.Subscribe(x =>
            {
                if (x.IsError)
                    onNextError(x.Exception);
                else
                    onNextResult(x.Value);
            },
                onError);
        }

        public static IDisposable SubscribeRex<T>(this IObservable<Rex<T>> source, Action<T> onNextResult, Action<Exception> onNextError,
            Action<Exception> onError,
            Action onCompleted)
        {
            return source.Subscribe(x =>
            {
                if (x.IsError)
                    onNextError(x.Exception);
                else
                    onNextResult(x.Value);
            },
                onError,
                onCompleted);
        }

        /// <summary>
        ///     Defer an observable until a value has materialised.
        /// </summary>
        /// <typeparam name="T">The type of the notification.</typeparam>
        /// <param name="source">The source to defer.</param>
        /// <returns>The defered observable as a higher order observable.</returns>
        //public static IObservable<IObservable<T>> DeferUntilFirst<T>(this IObservable<T> source)
        //{
        //    return Observable.Create<IObservable<T>>(obs =>
        //        source.Take(1).Subscribe(first => obs.OnNext(source.StartWith(first)), obs.OnError, obs.OnCompleted));
        //}
    }
}