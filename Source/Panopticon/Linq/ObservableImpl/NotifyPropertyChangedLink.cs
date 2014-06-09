// <copyright file="NotifyPropertyChangedLink.cs" company="million miles per hour ltd">
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
using System.Reactive.Disposables;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    internal class NotifyPropertyChangedLink<TIn, TOut> : IObserver<TIn>, IObservable<TOut>
        where TIn : INotifyPropertyChanged
    {
        private readonly IObservable<TIn> source;
        private readonly string propertyName;
        private readonly Func<TIn, TOut> outValueGetter;
#if USESUBJECT
        private readonly Subject<TOut> observerSubject = new Subject<TOut>();
#else
        private IObserver<TOut> observer;
#endif
        private TIn inValue;

        public NotifyPropertyChangedLink(IObservable<TIn> source, string propertyName, Func<TIn, TOut> outValueGetter)
        {
            Require.NotNull(source, "source");
            Require.NotNull(propertyName, "propertyName");
            Require.NotNull(outValueGetter, "outValueGetter");

            this.source = source;
            this.propertyName = propertyName;
            this.outValueGetter = outValueGetter;
        }

        /// <summary>
        ///     Internal <see cref="PropertyChangedEventHandler" /> to bind to the targets PropertyChanged event.
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
#if USESUBJECT
            if (propertyChangedEventArgs.PropertyName == propertyName)
#else
            if (observer != null && propertyChangedEventArgs.PropertyName == propertyName)
#endif
            {
                NotifyCurrentValue();
            }
        }

        public void NotifyCurrentValue()
        {
            var outValue = outValueGetter(inValue);
#if USESUBJECT
                observerSubject.OnNext(outValue);
#else
            observer.OnNext(outValue);
#endif
        }

        void IObserver<TIn>.OnNext(TIn value)
        {
            if (inValue != null)
                inValue.PropertyChanged -= OnPropertyChanged;
            inValue = value;
            if (inValue != null)
                inValue.PropertyChanged += OnPropertyChanged;

#if !USESUBJECT
            if (observer != null)
#endif
            {
                NotifyCurrentValue();
            }
        }

#if !USESUBJECT

        void IObserver<TIn>.OnError(Exception error)
        {
            if (observer != null)
            {
                observer.OnError(error);
                observer = null;
            }
        }

        void IObserver<TIn>.OnCompleted()
        {
            if (observer != null)
            {
                observer.OnCompleted();
                observer = null;
            }
        }
#endif

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            Require.NotNull(observer, "observer");
            if (this.observer != null)
                throw new Exception("NotifyPropertyChangedLink can only have a single subscriber, and one has already been bound.");
#if USESUBJECT
            observerSubject.Subscribe(observer);
            var sub = source.Subscribe(this.OnNext, observerSubject.OnError, observerSubject.OnCompleted);
#else
            this.observer = observer;
            //  var sub =  source.Subscribe(this.OnNext, this.OnError, this.OnCompleted);
            var sub = source.Subscribe(this); // source.Subscribe(this.OnNext, this.OnError, this.OnCompleted);
#endif
            return new CompositeDisposable(2) {sub, Disposable.Create(() => observer = null)};
        }
    }
}