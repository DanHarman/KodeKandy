﻿// <copyright file="NestedFrom.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent.Tests.TestEntities
{
    public class NestedFrom
    {
        public NestedChildFrom Child { get; set; }

        #region Nested type: NestedChildFrom

        public class NestedChildFrom
        {
            public string Name { get; set; }
        }

        #endregion
    }
}