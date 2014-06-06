using System;
using System.Reactive.Subjects;
using System.Windows.Input;

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