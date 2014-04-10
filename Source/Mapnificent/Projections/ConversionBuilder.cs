// <copyright file="ConversionBuilder.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.Projections
{
    /// <summary>
    ///     Type safe ConvertUsing builder.
    /// </summary>
    /// <typeparam name="TFrom">The type being mapped from.</typeparam>
    /// <typeparam name="TTo">The type being mapped to.</typeparam>
    public class ConversionBuilder<TFrom, TTo>
        where TTo : struct
    {
        public ConversionBuilder(Conversion conversion)
        {
            Require.IsTrue(conversion.ProjectionType.FromType == typeof(TFrom));
            Require.IsTrue(conversion.ProjectionType.ToType == typeof(TTo));

            Conversion = conversion;
        }

        public Conversion Conversion { get; private set; }

        /// <summary>
        ///     Explicitly define a mapping for a member with a delegate.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="conversionFunc"></param>
        /// <returns></returns>
        public ConversionBuilder<TFrom, TTo> Explictly(Func<TFrom, TTo> conversionFunc)
        {
            Require.NotNull(conversionFunc, "conversionFunc");

            // Need to remove the type specificity to be able to store these in a general way.
            Func<object, object> weakConversionFunc = from => conversionFunc((TFrom) from);

            Conversion.ConversionFunc = weakConversionFunc;

            return this;
        }
    }
}