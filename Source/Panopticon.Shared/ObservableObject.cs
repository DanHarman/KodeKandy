// <copyright file="ObservableObject.cs" company="million miles per hour ltd">
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
using System.Runtime.CompilerServices;
using KodeKandy.Properties;

namespace KodeKandy.Panopticon
{
    public abstract class ObservableObject : IObservableObject
    {
        private readonly PropertyChangeHelper propertyChangeHelper;

        protected ObservableObject()
        {
            propertyChangeHelper = new PropertyChangeHelper(this);
        }

        #region IObservableObject Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChangeHelper.PropertyChanged += value; }
            remove { propertyChangeHelper.PropertyChanged -= value; }
        }

        /// <summary>
        ///     Suppress all PropertyChanged events for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        public IDisposable SuppressPropertyChanged()
        {
            return propertyChangeHelper.SuppressPropertyChanged();
        }

        #endregion

        [NotifyPropertyChangedInvocator("propertyName")]
        protected void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            // CallerMemberNameAttribute is not supported on all platforms so we have to check for null here to avoid build errors
            Require.NotNull(propertyName, nameof(propertyName));

            propertyChangeHelper.SetPropertyValue(ref property, value, propertyName);
        }

        [NotifyPropertyChangedInvocator("propertyName")]
        protected void SetValue<TVal>(ref TVal property, TVal value, object userData, [CallerMemberName] string propertyName = null)
        {
            // CallerMemberNameAttribute is not supported on all platforms so we have to check for null here to avoid build errors
            Require.NotNull(propertyName, nameof(propertyName));

            propertyChangeHelper.SetPropertyValue(ref property, value, propertyName, userData);
        }
    }
}