// <copyright file="WeakEventHandler.cs" company="million miles per hour ltd">
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
using System.ComponentModel;

namespace KodeKandy.Panopticon.Internal
{
    /// <summary>
    ///     Holds a weak reference to a PropertyChanged event preventing the subscriber from blocking GC of the object exposing
    ///     the event by rooting it.
    /// </summary>
    /// <example>
    ///     <c> source.PropertyChanged += new WeakPropertyChangedEventHandler(OnPropertyChanged).Handler; </c>
    /// </example>
    /// <remarks>
    ///     Useful for subscribing to INotifyPropertyChanged exposers without keeping them alive. The WPF framework performs a
    ///     similar trick.
    /// </remarks>
    internal sealed class WeakPropertyChangedEventHandler
    {
        private readonly WeakReference handlerReference;

        public WeakPropertyChangedEventHandler(PropertyChangedEventHandler handler)
        {
            handlerReference = new WeakReference(handler);
        }

        public void Handler(object sender, PropertyChangedEventArgs e)
        {
            var handler = handlerReference.Target as PropertyChangedEventHandler;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}