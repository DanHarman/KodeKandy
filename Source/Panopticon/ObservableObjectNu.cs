// <copyright file="ObservableObjectNu.cs" company="million miles per hour ltd">
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
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon
{
    public abstract class ObservableObjectNu : INotifyPropertyChanged
    {
        private readonly PropertyChangeHelper propertyChangeHelper;

        protected ObservableObjectNu()
        {
            propertyChangeHelper = new PropertyChangeHelper(this);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChangeHelper.PropertyChanged += value; }
            remove { propertyChangeHelper.PropertyChanged -= value; }
        }

        #endregion

        [NotifyPropertyChangedInvocator("propertyName")]
        protected void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            propertyChangeHelper.SetPropertyValue(ref property, value, propertyName);
        }

        [NotifyPropertyChangedInvocator("propertyName")]
        protected void SetValue<TVal>(ref TVal property, TVal value, object userData, [CallerMemberName] string propertyName = null)
        {
            propertyChangeHelper.SetPropertyValue(ref property, value, propertyName, userData);
        }

        public IDisposable SuppressPropertyChanged()
        {
            return propertyChangeHelper.SuppressPropertyChanged();
        }
    }
}