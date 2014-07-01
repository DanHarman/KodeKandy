// <copyright file="ObservableCommand.cs" company="million miles per hour ltd">
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

namespace KodeKandy.Panopticon
{
//    public class ObservableCommand<T> : ICommand, IObservable<T>, IObserver<bool>
//    {
//        private readonly Subject<T> executeSubject = new Subject<T>();
//        private readonly BehaviorSubject<bool> canExecuteSubject = new BehaviorSubject<bool>(true);
//
//        public bool CanExecute(object parameter)
//        {
//            return canExecuteSubject.Value;
//        }
//
//        public void Execute(object parameter)
//        {
//            try
//            {
//                executeSubject.OnNext((T) parameter);
//            }
//            catch (Exception ex)
//            {
//                executeSubject.OnError(ex);
//            }
//        }
//
//        public event EventHandler CanExecuteChanged;
//
//        public IDisposable Subscribe(IObserver<T> observer)
//        {
//            return executeSubject.Subscribe(observer);
//        }
//
//        public void OnNext(bool value)
//        {
//            canExecuteSubject.OnNext(value);
//        }
//
//        public void OnError(Exception error)
//        {
//            canExecuteSubject.OnError(error);
//        }
//
//        public void OnCompleted()
//        {
//            canExecuteSubject.OnCompleted();
//        }
//    }
}