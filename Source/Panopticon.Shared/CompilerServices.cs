// <copyright file="CompilerServices.cs" company="million miles per hour ltd">
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

namespace System.Runtime.CompilerServices
{
    // Summary:
    //     Allows you to obtain the method or property name of the caller to the method.
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class CallerMemberNameAttribute : Attribute
    {
    }

    // Summary:
    //     Allows you to obtain the line number in the source file at which the method
    //     is called.
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
    }

    // Summary:
    //     Allows you to obtain the full path of the source file that contains the caller.
    //     This is the file path at the time of compile.
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
    }
}