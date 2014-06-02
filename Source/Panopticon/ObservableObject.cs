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
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon
{
    //public class ObservableCommand<T> : ICommand, IObservable<T>, IObserver<bool>
    //{
    //    private readonly Subject<T> executeSubject = new Subject<T>();
    //    private readonly BehaviorSubject<bool> canExecuteSubject = new BehaviorSubject<bool>(true);

    //    public bool CanExecute(object parameter)
    //    {
    //        return canExecuteSubject.Value;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        try
    //        {
    //            executeSubject.OnNext((T) parameter);
    //        }
    //        catch (Exception ex)
    //        {
    //            executeSubject.OnError(ex);
    //        }
    //    }

    //    public event EventHandler CanExecuteChanged;

    //    public IDisposable Subscribe(IObserver<T> observer)
    //    {
    //        return executeSubject.Subscribe(observer);
    //    }

    //    public void OnNext(bool value)
    //    {
    //        canExecuteSubject.OnNext(value);
    //    }

    //    public void OnError(Exception error)
    //    {
    //        canExecuteSubject.OnError(error);
    //    }

    //    public void OnCompleted()
    //    {
    //        canExecuteSubject.OnCompleted();
    //    }
    //}


    public class ObservableObject : INotifyPropertyChanged, IObservableObject
    {
        public readonly PropertyChangeSubject propertyChangeSubject;

        public ObservableObject()
        {
            propertyChangeSubject = new PropertyChangeSubject(this);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChangeSubject.PropertyChanged += value; }
            remove { propertyChangeSubject.PropertyChanged -= value; }
        }

        #endregion

        #region IObservableObject Members

        /// <summary>
        ///     An observable providing notification of property changes. It will complete when the object is
        ///     disposed.
        /// </summary>
        public IObservable<IPropertyChange> PropertyChanges
        {
            get { return propertyChangeSubject; }
        }

        internal PropertyChangeSubject PropertyChangesSubject
        {
            get { return propertyChangeSubject; }
        }

        /// <summary>
        ///     Suppress all change notifications for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        public IDisposable BeginNotificationSuppression()
        {
            return propertyChangeSubject.BeginNotificationSuppression();
        }

        public void Dispose()
        {
            propertyChangeSubject.Dispose();
        }

        #endregion

        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            propertyChangeSubject.SetPropertyValue(ref property, value, propertyName);
        }

        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<TVal>(ref TVal property, TVal value, object userData, [CallerMemberName] string propertyName = null)
        {
            propertyChangeSubject.SetPropertyValue(ref property, value, propertyName, userData);
        }
    }
}