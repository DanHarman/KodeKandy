// <copyright file="ConversionDefinitionBuilder.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.Definitions
{
    /// <summary>
    ///     Type safe ConversionDefinition builder.
    /// </summary>
    /// <typeparam name="TFrom">The type being mapped from.</typeparam>
    /// <typeparam name="TTo">The type being mapped to.</typeparam>
    public class ConversionDefinitionBuilder<TFrom, TTo>
        where TTo : struct
    {
        public ConversionDefinition ConversionDefinition { get; private set; }

        public ConversionDefinitionBuilder(ConversionDefinition conversionDefinition)
        {
            Require.IsTrue(conversionDefinition.MappingType.FromType == typeof(TFrom));
            Require.IsTrue(conversionDefinition.MappingType.ToType == typeof(TTo));

            ConversionDefinition = conversionDefinition;
        }

        /// <summary>
        ///     Explicitly define a mapping for a member with a delegate.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="conversionFunc"></param>
        /// <returns></returns>
        public ConversionDefinitionBuilder<TFrom, TTo> Explictly(Func<TFrom, TTo> conversionFunc)
        {
            Require.NotNull(conversionFunc, "conversionFunc");

            // Need to remove the type specificity to be able to store these in a general way.
            Func<object, object> weakConversionFunc = from => conversionFunc((TFrom) from);

            ConversionDefinition.ConversionFunc = weakConversionFunc;

            return this;
        }
    }
}