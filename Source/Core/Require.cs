// <copyright file="Require.cs" company="million miles per hour ltd">
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
using KodeKandy.Properties;

namespace KodeKandy
{
    public static class Require
    {
        /// <summary>
        ///     Ensures that a value is not null.
        /// </summary>
        public static void NotNull<T>(T parameter, [InvokerParameterName] string parameterName = null, string message = null)
            where T : class
        {
            if (parameter != null)
                return;

            message = message ?? String.Format("Argument '{0}' of type {1} was null", parameterName ?? "<undefined>", typeof(T));

            throw new ArgumentNullException(parameterName, message);
        }


        /// <summary>
        ///     Require tha that the string value is not null or empty.
        /// </summary>
        public static void NotNullOrEmpty(string parameter, [InvokerParameterName] string parameterName = null, string message = null)
        {
            if (!String.IsNullOrEmpty(parameter))
                return;

            message = message ?? String.Format("String argument {0} was null or empty", parameterName ?? "<undefined>");

            throw new ArgumentNullException(parameterName, message);
        }

        /// <summary>
        ///     Require that the boolean value is true.
        /// </summary>
        public static void IsTrue(bool predicateResult, string message = null, [InvokerParameterName] string parameterName = null)
        {
            if (predicateResult)
                return;

            throw new ArgumentException(message ?? "Require.IsTrue predicate result was false", parameterName);
        }

        /// <summary>
        ///     Require that the boolean value is false.
        /// </summary>
        public static void IsFalse(bool predicateResult, string message = null, [InvokerParameterName] string parameterName = null)
        {
            if (!predicateResult)
                return;

            throw new ArgumentException(message ?? "Require.IsFalse predicate result was true", parameterName);
        }
    }
}