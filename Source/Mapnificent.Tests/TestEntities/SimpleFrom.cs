// <copyright file="SimpleFrom.cs" company="million miles per hour ltd">
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
    public class SimpleFrom
    {
        public int IntField;
        public string StringProp { get; set; }
    }

    public class VehicleFrom
    {
        public string Name { get; set; }
    }

    public class CarFrom : VehicleFrom
    {
        public int NumSeats { get; set; }
    }

    public class VehicleTo
    {
        public string Name { get; set; }
    }

    public class CarTo : VehicleTo
    {
        public int NumSeats { get; set; }
    }
}