// <copyright file="ConstructionContext.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Mapnificent
{
    public class MappingContext
    {
        public Mapper Mapper { get; private set; }
        public object FromInstance { get; private set; }


        public MappingContext(Mapper mapper, object fromInstance)
        {
            Mapper = mapper;
            FromInstance = fromInstance;
        }
    }


    public class ConstructionContext
    {
        public ConstructionContext(Mapper mapper, object fromInstance, object parent)
        {
            Mapper = mapper;
            FromInstance = fromInstance;
            Parent = parent;
        }

        public Mapper Mapper { get; private set; }
        public Type TypeOfTarget { get; private set; }
        public object FromInstance { get; private set; }
        public object Parent { get; private set; }
    }
}