// <copyright file="PropertyChangeSubject.cs" company="million miles per hour ltd">
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
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace KodeKandy.Panopticon
{
    public class PropertyChangeSubject : IObservable<IPropertyChange>, IDisposable
    {
        private readonly object gate = new object();
        /// <summary>
        ///     Cache of property observablesForProperty filtered by property name.
        /// </summary>
        private Dictionary<string, IObservable<IPropertyChange>> observablesForProperty;
        private Dictionary<string, object> observablesForProperty2;
        private Subject<IPropertyChange> propertyChangeSubject;
        private int suppressNotificationCount;

        public PropertyChangeSubject(object source)
        {
            Require.NotNull(source, "source");

            Source = source;
        }

        public object Source { get; private set; }
        protected bool IsNotificationSuppressed
        {
            get { return Interlocked.CompareExchange(ref suppressNotificationCount, 0, 0) != 0; }
        }

        #region Implementation of IObservable<out IPropertyChange>

        public IDisposable Subscribe(IObserver<IPropertyChange> observer)
        {
            lock (gate)
            {
                if (propertyChangeSubject == null)
                    propertyChangeSubject = new Subject<IPropertyChange>();
            }
            return propertyChangeSubject.Subscribe(observer);
        }

        public IObservable<IPropertyChange> GetObservableForProperty(string propertyName)
        {
//            return Observable.Create<IPropertyChange>(obs =>
//            {
//                var sub = new PropertyChangeSubscription(obs, propertyName);
//                this.PropertyChanged += sub.Handler;
//                sub.UnsubscribeAction = () => this.PropertyChanged -= sub.Handler;
//                    return sub;
//                });

            IObservable<IPropertyChange> res;
            lock (gate)
            {
                // Caching and publishing the filtered subject for a property gives significant performance
                // benefits when multiple subscribers listen to the same property.
                // When there is only one subscriber, the overhead of the dictionary lookup shouldn't matter.
                if (observablesForProperty == null)
                    observablesForProperty = new Dictionary<string, IObservable<IPropertyChange>>();

                observablesForProperty.TryGetValue(propertyName, out res);
                if (res == null)
                {
                    var getter = ReflectionHelpers.CreateWeakMemberGetter(ReflectionHelpers.GetMemberInfo(Source.GetType(), propertyName));

//                    res = Observable
//                        .FromEventPattern<Handler, PropertyChangedEventArgs>(
//                            h => PropertyChanged += new WeakEventHandler(h).Handler,
//                            h => PropertyChanged -= h)
//                        .Where(x => x.EventArgs.PropertyName == propertyName).Select(x => (IPropertyChange) x.EventArgs).Publish().RefCount();

                    res = (new PropertyChangedObservable(h => PropertyChanged += h, h => PropertyChanged -= h))
                       .Where(x => x.PropertyName == propertyName).Select(x => (IPropertyChange)x);

                    var bleh = Observable.Defer(() => Observable.Return(PropertyChange.Create(Source, getter(Source), propertyName)));

                    res = bleh.Concat(res);

                    // var coldInitial = Observable.Defer(() => Observable.Return((T)getter(Source)));


                    observablesForProperty.Add(propertyName, res);
                }

//                if (propertyChangeSubject == null)
//                    propertyChangeSubject = new Subject<IPropertyChange>();
//
//                res = propertyChangeSubject.Where(x => x.PropertyName == propertyName).Publish().RefCount();
            }

            return res;
        }


        public IObservable<TProperty> GetObservableForProperty<TClass, TProperty>(string propertyName)
        {
            //            return Observable.Create<IPropertyChange>(obs =>
            //            {
            //                var sub = new PropertyChangeSubscription(obs, propertyName);
            //                this.PropertyChanged += sub.Handler;
            //                sub.UnsubscribeAction = () => this.PropertyChanged -= sub.Handler;
            //                    return sub;
            //                });

            IObservable<TProperty> res;
            lock (gate)
            {
                // Caching and publishing the filtered subject for a property gives significant performance
                // benefits when multiple subscribers listen to the same property.
                // When there is only one subscriber, the overhead of the dictionary lookup shouldn't matter.
                if (observablesForProperty2 == null)
                    observablesForProperty2 = new Dictionary<string, object>();

                object outRes;
                observablesForProperty2.TryGetValue(propertyName, out outRes);
                if (outRes != null)
                    res = (IObservable<TProperty>) outRes;
                else
                {
                    var getter = (Func<TClass, TProperty>)ReflectionHelpers.CreatePropertyGetter(Source.GetType().GetProperty(propertyName));

//                    var hotLive = Observable
//                        .FromEventPattern<Handler, PropertyChangedEventArgs>(
//                            h => PropertyChanged += new WeakEventHandler(h).Handler,
//                            h => PropertyChanged -= h)
//                        .Where(x => x.EventArgs.PropertyName == propertyName).Select(x => (T) getter(Source)).Publish().RefCount();

//                    var hotLive = (new PropertyChangedObservable(h => PropertyChanged += h, h => PropertyChanged -= h))
//                       .Where(x => x.PropertyName == propertyName).Select(x => (T)getter(Source));//.Publish().RefCount();

                    var hotLive = (new PropertyChangedObservable(h => PropertyChanged += h, h => PropertyChanged -= h))
                                 .Where(x => x.PropertyName == propertyName).Select(x => getter((TClass)Source));

//                    var bleh = Observable.Defer(() => Observable.Return(PropertyChange.Create(Source, getter(Source), propertyName)));
//
//                    res = res.Merge(bleh);
                    var coldInitial = Observable.Defer(() => Observable.Return(getter((TClass)Source)));
                    res = coldInitial.Concat(hotLive);
                    observablesForProperty2.Add(propertyName, res);
                }

                //                if (propertyChangeSubject == null)
                //                    propertyChangeSubject = new Subject<IPropertyChange>();
                //
                //                res = propertyChangeSubject.Where(x => x.PropertyName == propertyName).Publish().RefCount();
            }

            return res;
        }

        #region Nested type: PropertyChangedObservable

        #endregion

        #region Nested type: WeakEventHandler

        #endregion

        #endregion

        #region Implementation of IDisposable

        public virtual void Dispose()
        {
            lock (gate)
            {
                if (propertyChangeSubject != null)
                {
                    propertyChangeSubject.OnCompleted();
                    propertyChangeSubject.Dispose();
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetPropertyValue<T>(ref T property, T value, string propertyName, object userData = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
                return;

            property = value;

            NotifyPropertyValueChanged(value, propertyName, userData);
        }

        public void NotifyPropertyValueChanged<T>(T value, string propertyName, object userData = null)
        {
            if (IsNotificationSuppressed)
                return;

            var notification = PropertyChange.Create(Source, value, propertyName, userData);

            var handlerSnapshot = PropertyChanged;

            if (handlerSnapshot != null)
                handlerSnapshot(Source, notification);

            if (propertyChangeSubject != null)
                propertyChangeSubject.OnNext(notification);
        }

        /// <summary>
        ///     Suppress all change notifications for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <param name="completionAction">
        ///     An action to optionally perform when all suppression scopes are exited (as it supports
        ///     reentrance a count is maintained).
        /// </param>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        public IDisposable BeginNotificationSuppression(Action completionAction = null)
        {
            Interlocked.Increment(ref suppressNotificationCount);
            return Disposable.Create(() =>
            {
                var count = Interlocked.Decrement(ref suppressNotificationCount);
                if (completionAction != null && count == 0)
                {
                    completionAction();
                }
            });
        }
    }

    public class PropertyChangeSubscription : IObservable<IPropertyChange>, IDisposable
    {
        private readonly PropertyChangeSubject propertyChangeSubject;
        private readonly string propertyName;
        private bool isDisposed;
        private IObserver<IPropertyChange> observer;

        public PropertyChangeSubscription(PropertyChangeSubject propertyChangeSubject, string propertyName)
        {
            this.propertyChangeSubject = propertyChangeSubject;
            this.propertyName = propertyName;

            propertyChangeSubject.PropertyChanged += PropertyChangedEventHandler;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            propertyChangeSubject.PropertyChanged -= PropertyChangedEventHandler;
        }

        #endregion

        #region IObservable<IPropertyChange> Members

        public IDisposable Subscribe(IObserver<IPropertyChange> observer)
        {
            this.observer = observer;
            return this;
        }

        #endregion

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (isDisposed || observer == null)
                return;
            if (e.PropertyName == propertyName)
                observer.OnNext((IPropertyChange) e);
        }
    }

    public static class ObservableObjectMixin
    {
        public static IObservable<TProperty> WhenPropertyChanged<TClass, TProperty>(this TClass instance,
            Expression<Func<TClass, TProperty>> memberPath)
            where TClass : ObservableObject
        {
            var name = ExpressionHelpers.GetMemberName(memberPath);
            return instance.propertyChangeSubject.GetObservableForProperty(name).Select(x => (TProperty) x.Value);
        }

        public static IObservable<TProperty> WhenPropertyChangedNu<TClass, TProperty>(this TClass instance,
            Expression<Func<TClass, TProperty>> memberPath)
            where TClass : ObservableObject
        {
            var name = ExpressionHelpers.GetMemberName(memberPath);
            return instance.propertyChangeSubject.GetObservableForProperty<TClass, TProperty>(name);
        }

        public static IObservable<TProperty> WhenPropertyChangedNu<TClass, TProperty>(this ObservableObject instance,
            string propertyName)
        {
            return instance.propertyChangeSubject.GetObservableForProperty<TClass, TProperty>(propertyName);
        }

//        public static IObservable<TProperty> WhenPropertyChanged2<TClass, TProperty>(this TClass instance,
//            Expression<Func<TClass, TProperty>> memberPath)
//            where TClass : ObservableObject2
//
//        {
//            var name = ExpressionHelpers.GetMemberName(memberPath);
//
//            return instance.propertyChangeSubject.GetObservableForProperty(name).Select(x => ((MonadicPropertyChangedEventArgs<TProperty>) x).Value);
//        }

        public static IObservable<IPropertyChange> WhenPropertyChanged3<TClass, TProperty>(this TClass instance,
            Expression<Func<TClass, TProperty>> memberPath)
            where TClass : ObservableObject
        {
            var name = ExpressionHelpers.GetMemberName(memberPath);

            // return Observable.Create<IPropertyChange>(obs => new PropertyChangeSubscription(obs, instance.PropertyChangesSubject, name));

            //  return new PropertyChangeSubscription(instance.PropertyChangesSubject, name);
            return instance.propertyChangeSubject.GetObservableForProperty(name);
        }

        public static IObservable<IPropertyChange> WhenPropertyChanged3<TClass>(this TClass instance,
            string propertyName)
            where TClass : ObservableObject
        {
            return instance.propertyChangeSubject.GetObservableForProperty(propertyName);
        }

        public static IObservable<TProperty> WhenPropertyChanged3<TClass, TProperty>(this TClass instance, string propertyName)
            where TClass : ObservableObject
        {
            return instance.propertyChangeSubject.Select(x => (TProperty) x.Value);
        }
    }
}