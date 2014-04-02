// <copyright file="Helpers.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent
{
    public static class Helpers
    {
        /// <summary>
        ///     Safely get the name of a type returning <![CDATA[<null>]]> if the instance is null.
        /// </summary>
        /// <param name="instance">Instance to return the type of.</param>
        /// <returns>
        ///     The name of the type or <![CDATA[<null>]]>.
        /// </returns>
        public static string SafeGetTypeName(this object instance)
        {
            return instance == null ? "<null>" : instance.GetType().Name;
        }
    }
}