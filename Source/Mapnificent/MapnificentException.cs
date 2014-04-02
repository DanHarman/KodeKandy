// <copyright file="MapnificentException.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent
{
    public class MapnificentException : Exception
    {
        public MapnificentException(string message) : base(message)
        {
        }

        public MapnificentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    ///     An error associated with an error in the definition of a map.
    /// </summary>
    public class MapDefinitionError : MapnificentException
    {
        public MapDefinitionError(string message) : base(message)
        {
        }

        public MapDefinitionError(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}