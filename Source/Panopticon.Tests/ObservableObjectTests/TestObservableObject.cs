﻿// <copyright file="TestObservableObject.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Panopticon.Tests.ObservableObjectTests
{
    internal class TestObservableObject : ObservableObject
    {
        private int age;
        public int Age
        {
            get { return age; }
            set { SetValue(ref age, value); }
        }
    }

    internal class TestObservableObject2 : ObservableObject
    {
        private int age;
        public int Age
        {
            get { return age; }
            set { SetValue(ref age, value); }
        }
    }

    internal class TestObservableObject3 : ObservableObject
    {
        private int age;
        public int Age
        {
            get { return age; }
            set { SetValue(ref age, value); }
        }
    }
}