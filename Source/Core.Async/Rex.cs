// <copyright file="Rex.cs" company="million miles per hour ltd">
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

namespace KodeKandy
{
    /// <summary>
    ///     Monadic type that represents a result of an operation that may fail, but which must not error its outter stream.
    ///     Instead the error is captured in the <see cref="Rex{T}" /> so that it may be inspected and handled without
    ///     terminating the observable.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    public abstract class Rex<T> : IEquatable<Rex<T>>
    {
        public abstract bool IsError { get; }
        public abstract T Value { get; }
        public abstract Exception Exception { get; }

        public bool Equals(Rex<T> other)
        {
            if (other.IsError != IsError)
                return false;

            return IsError ? Exception.Equals(other.Exception) : Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Rex<T>) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Rex<T> left, Rex<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Rex<T> left, Rex<T> right)
        {
            return !Equals(left, right);
        }

        public class Error : Rex<T>
        {
            private readonly Exception exception;

            public Error(Exception exception)
            {
                this.exception = exception;
            }

            public override bool IsError
            {
                get { return true; }
            }

            public override Exception Exception
            {
                get { return exception; }
            }

            public override T Value
            {
                get { throw new InvalidOperationException("A Rex<T> of type 'Error' does not have a value."); }
            }
        }

        internal class Result : Rex<T>
        {
            private readonly T value;

            public Result(T value)
            {
                this.value = value;
            }

            public override bool IsError
            {
                get { return false; }
            }

            public override Exception Exception
            {
                get { throw new InvalidOperationException("A Rex<T> of type 'Result' does not have an Exception."); }
            }

            public override T Value
            {
                get { return value; }
            }
        }
    }

    public static class Rex
    {
        public static Rex<T> Error<T>(Exception ex)
        {
            return new Rex<T>.Error(ex);
        }

        public static Rex<T> Result<T>(T value)
        {
            return new Rex<T>.Result(value);
        }
    }
}