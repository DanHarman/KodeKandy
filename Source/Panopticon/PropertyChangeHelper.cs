// <copyright file="PropertyChangeHelper.cs" company="million miles per hour ltd">
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading;

namespace KodeKandy.Panopticon
{
    /// <summary>
    ///     Helper class for implementing <see cref="INotifyPropertyChanged" />. To use, compose and instance of this class in,
    ///     and
    ///     on the property changed event property, delegate the add and remove handler calls onto the event exposed on this
    ///     class.
    /// </summary>
    /// <remarks>
    ///     When firing, this class sends a custom event type derived from PropertyChangedEventArgs, but with an additional
    ///     'UserData'
    ///     field that may be useful for solving re-entrancy problems when change observers need to know whether they triggered
    ///     the
    ///     change themselves.
    /// </remarks>
    public class PropertyChangeHelper
    {
        private int suppressPropertyChangedCount;

        public PropertyChangeHelper(object source)
        {
            Require.NotNull(source, "source");

            Source = source;
        }

        public object Source { get; private set; }

        protected bool IsPropertyChangedSuppressed
        {
            get { return Interlocked.CompareExchange(ref suppressPropertyChangedCount, 0, 0) != 0; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Set the property and fire a PropertyChanged event, but only if the value has changed.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="property">A reference to the property backing field that may be modified.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="userData">An optional user data field.</param>
        public void SetPropertyValue<T>(ref T property, T value, string propertyName, object userData = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
                return;

            property = value;

            NotifyPropertyChanged(propertyName, userData);
        }

        /// <summary>
        ///     Fires a PropertyChanged event of type PropertyChangedEventArgsEx, which has an addtional, and optional
        ///     UserData field.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        /// <param name="userData">Optional user data.</param>
        public void NotifyPropertyChanged(string propertyName, object userData = null)
        {
            if (IsPropertyChangedSuppressed)
                return;

            var handlerSnapshot = PropertyChanged;

            if (handlerSnapshot != null)
            {
                handlerSnapshot(Source, new PropertyChangedEventArgsEx(Source, propertyName, userData));
            }
        }

        /// <summary>
        ///     Suppress all PropertyChanged events for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        public IDisposable SuppressPropertyChanged()
        {
            Interlocked.Increment(ref suppressPropertyChangedCount);
            return Disposable.Create(() => Interlocked.Decrement(ref suppressPropertyChangedCount));
        }
    }
}