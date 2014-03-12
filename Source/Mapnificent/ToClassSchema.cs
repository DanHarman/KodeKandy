// <copyright file="ToClassSchema.cs" company="million miles per hour ltd">
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
using System.Collections.Generic;
using System.Reflection;

namespace KodeKandy.Mapnificent
{
    public class ToClassSchema
    {
        private readonly Type classType;
        private readonly Dictionary<string, Action<object, object>> members = new Dictionary<string, Action<object, object>>();

        public ToClassSchema(Type classType)
        {
            this.classType = classType;

            EnumeratePropertyMembers();
            EnumerateFieldMembers();
        }

        public Dictionary<string, Action<object, object>> Members
        {
            get { return members; }
        }

        private void EnumeratePropertyMembers()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            var props = classType.GetProperties(flags);

            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                    continue;

                members.Add(prop.Name, ReflectionHelpers.CreateWeakPropertySetter(prop));
            }
        }

        private void EnumerateFieldMembers()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            var fields = classType.GetFields(flags);

            foreach (var field in fields)
            {
                members.Add(field.Name, ReflectionHelpers.CreateWeakFieldSetter(field));
            }
        }
    }
}