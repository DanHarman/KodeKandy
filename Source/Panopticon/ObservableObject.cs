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
// 
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon
{
    //public class CompletionMixin
    //{
    //    private Exception Exception = null;
    //}

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

    public interface IObservableObject : IDisposable
    {
        IObservable<IPropertyChange> PropertyChanges { get; }
    }

    public class ObservableObject : IObservableObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly PropertyChangeSubject propertyChangeSubject;

        public ObservableObject()
        {
            // propertyChangeSubject = new PropertyChangeSubject(this, new object(), () => PropertyChanged);
            propertyChangeSubject = new PropertyChangeSubject(this, () => PropertyChanged);
        }

        public IObservable<IPropertyChange> PropertyChanges
        {
            get { return propertyChangeSubject; }
        }

        [NotifyPropertyChangedInvocator("propertyName")]
        public void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            propertyChangeSubject.SetPropertyValue(ref property, value, propertyName);
        }

        public void Dispose()
        {
            propertyChangeSubject.Dispose();
        }
    }
}