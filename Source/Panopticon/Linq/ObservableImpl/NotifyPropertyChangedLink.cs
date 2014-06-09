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
using System.Threading;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    internal class NotifyPropertyChangedLink<TIn, TOut> : IObserver<TIn>, IObservable<TOut>
        where TIn : INotifyPropertyChanged
    {
        private readonly IObservable<TIn> source;
        private readonly string propertyName;
        private readonly Func<TIn, TOut> outValueGetter;
        private IObserver<TOut> observer;
        private TIn inValue;

        public NotifyPropertyChangedLink(IObservable<TIn> source, string propertyName, Func<TIn, TOut> outValueGetter)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            this.source = source;
            this.propertyName = propertyName;
            this.outValueGetter = outValueGetter;
        }

        /// <summary>
        ///     Internal <see cref="PropertyChangedEventHandler" /> to bind to the targets PropertyChanged event.
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (observer != null && propertyChangedEventArgs.PropertyName == propertyName)
            {
                NotifyCurrentValue();
            }
        }

        public void NotifyCurrentValue()
        {
            var outValue = outValueGetter(inValue);
            observer.OnNext(outValue);
        }

        void IObserver<TIn>.OnNext(TIn value)
        {
            if (inValue != null)
                inValue.PropertyChanged -= OnPropertyChanged;
            inValue = value;
            if (inValue != null)
                inValue.PropertyChanged += OnPropertyChanged;

            if (observer != null)
            {
                NotifyCurrentValue();
            }
        }

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

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");
           
            if (this.observer != null)
                throw new Exception("NotifyPropertyChangedLink can only have a single subscriber, and one has already been bound.");

            this.observer = observer;
            var sub = source.Subscribe(this); 

            return new Subscription(this, observer);
            return Disposable.Create(() =>
            {
                sub.Dispose();
                observer = null;
            });
        }

        class Subscription : IDisposable
        {
            private NotifyPropertyChangedLink<TIn, TOut> link;
            private IObserver<TOut> observer;

            public Subscription(NotifyPropertyChangedLink<TIn, TOut> link, IObserver<TOut> observer)
            {
                this.link = link;
                this.observer = observer;
            }

            public void Dispose()
            {
                var currObserver = Interlocked.Exchange(ref this.observer, null);
                if (currObserver == null)
                    return;

               // link.Unsubscribe(observer);
                link = null;
            }
        }
    }
}