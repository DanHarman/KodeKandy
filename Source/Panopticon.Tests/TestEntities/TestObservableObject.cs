// <copyright file="TestObservableObject.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Panopticon.Tests.TestEntities
{
    internal class TestObservableObject : ObservableObject
    {
        private TestObservableObject _observableChild;
        private TestPoco _pocoChild;
        private int age;
        private string _name;

        public int Age
        {
            get { return age; }
            set { SetValue(ref age, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }

        public TestObservableObject ObservableChild
        {
            get { return _observableChild; }
            set { SetValue(ref _observableChild, value); }
        }

        public TestPoco PocoChild
        {
            get { return _pocoChild; }
            set { SetValue(ref _pocoChild, value); }
        }
    }
}