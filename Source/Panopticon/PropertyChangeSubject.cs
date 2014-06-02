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
        private readonly Func<PropertyChangedEventHandler> getPropertyChangedEventHandler;
        private readonly Subject<IPropertyChange> propertyChangeSubject = new Subject<IPropertyChange>();
        private int suppressNotificationCount;

        public PropertyChangeSubject(object source, Func<PropertyChangedEventHandler> getPropertyChangedEventHandler = null)
        {
            Require.NotNull(source, "source");

            Source = source;
            this.getPropertyChangedEventHandler = getPropertyChangedEventHandler ?? (() => null);
        }

        public object Source { get; private set; }
        protected bool IsNotificationSuppressed
        {
            get { return Interlocked.CompareExchange(ref suppressNotificationCount, 0, 0) != 0; }
        }

        #region Implementation of IObservable<out IPropertyChange>

        public IDisposable Subscribe(IObserver<IPropertyChange> observer)
        {
            return propertyChangeSubject.Subscribe(observer);
        }

        #endregion

        #region Implementation of IDisposable

        public virtual void Dispose()
        {
            propertyChangeSubject.OnCompleted();
            propertyChangeSubject.Dispose();
        }

        #endregion

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

            var handlerSnapshot = getPropertyChangedEventHandler();

            if (handlerSnapshot != null)
                handlerSnapshot(Source, new PropertyChangedEventArgs(propertyName));

            propertyChangeSubject.OnNext(PropertyChange.Create(Source, value, propertyName, userData));
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

    public static class MonadicPropertyChangedEventArgs
    {
        public static MonadicPropertyChangedEventArgs<T> Create<T>(string propertyName, T value)
        {
            return new MonadicPropertyChangedEventArgs<T>(propertyName, value);
        }

        public static MonadicPropertyChangedEventArgs<T> Create<T>(string propertyName, T value, object source, object userData = null)
        {
            return new MonadicPropertyChangedEventArgs<T>(propertyName, value, source, userData);
        }
    }

    public interface IMonadicPropertyChangedEventArgs
    {
        /// <summary>
        ///     The originator of the change notification.
        /// </summary>
        object Source { get; }

        /// <summary>
        ///     The name of the changed property.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        ///     The properties new value.
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     Additional user data on the notification. Useful if dealing with re-entrancy issues.
        /// </summary>
        object UserData { get; }
    }

    public class MonadicPropertyChangedEventArgs<T> : PropertyChangedEventArgs, IMonadicPropertyChangedEventArgs
    {
        private readonly object source;
        private readonly object userData;
        private readonly T value;

        public MonadicPropertyChangedEventArgs(string propertyName, T value) : base(propertyName)
        {
            this.value = value;
        }

        public MonadicPropertyChangedEventArgs(string propertyName, T value, object source, object userData = null)
            : base(propertyName)
        {
            this.value = value;
            this.source = source;
            this.userData = userData;
        }

        /// <summary>
        ///     The properties new value.
        /// </summary>
        public new T Value
        {
            get { return value; }
        }

        #region IMonadicPropertyChangedEventArgs Members

        /// <summary>
        ///     The properties new value.
        /// </summary>
        object IMonadicPropertyChangedEventArgs.Value
        {
            get { return value; }
        }

        /// <summary>
        ///     The originator of the change notification.
        /// </summary>
        public object Source
        {
            get { return source; }
        }

        /// <summary>
        ///     Additional user data on the notification. Useful if dealing with re-entrancy issues.
        /// </summary>
        public object UserData
        {
            get { return userData; }
        }

        #endregion
    }

    public class PropertyChangeSubject2 : IObservable<IPropertyChange>, IDisposable
    {
        private readonly Dictionary<string, IObservable<IMonadicPropertyChangedEventArgs>> observables =
            new Dictionary<string, IObservable<IMonadicPropertyChangedEventArgs>>();
        private readonly Subject<IPropertyChange> propertyChangeSubject = new Subject<IPropertyChange>();
        private int suppressNotificationCount;

        public PropertyChangeSubject2(object source)
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
            return propertyChangeSubject.Subscribe(observer);
        }

        #endregion

        #region Implementation of IDisposable

        public virtual void Dispose()
        {
            propertyChangeSubject.OnCompleted();
            propertyChangeSubject.Dispose();
        }

        #endregion

        public IObservable<IMonadicPropertyChangedEventArgs> GetObservableForProperty(string propertyName)
        {
            IObservable<IMonadicPropertyChangedEventArgs> res;
            observables.TryGetValue(propertyName, out res);
            if (res == null)
            {
                res = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => PropertyChanged += h,
                    h => PropertyChanged -= h).Where(x => x.EventArgs.PropertyName == propertyName).Select(
                        x => ((IMonadicPropertyChangedEventArgs) x.EventArgs)).Publish().RefCount();

                observables.Add(propertyName, res);
            }

            return res;
        }

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

            var handlerSnapshot = PropertyChanged;

            if (handlerSnapshot != null)
                handlerSnapshot(Source, MonadicPropertyChangedEventArgs.Create(propertyName, value));

            //   propertyChangeSubject.OnNext(PropertyChange.Create(Source, value, propertyName, userData));
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

    public class PropertyChangeSubject3 : IObservable<IMonadicPropertyChangedEventArgs>, IDisposable
    {
        private readonly object gate = new object();
        /// <summary>
        ///     Cache of property observables filtered by property name.
        /// </summary>
        private readonly Dictionary<string, IObservable<IMonadicPropertyChangedEventArgs>> observables =
            new Dictionary<string, IObservable<IMonadicPropertyChangedEventArgs>>();
        private readonly Subject<IMonadicPropertyChangedEventArgs> propertyChangeSubject = new Subject<IMonadicPropertyChangedEventArgs>();

        private int suppressNotificationCount;

        public PropertyChangeSubject3(object source)
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

        public IDisposable Subscribe(IObserver<IMonadicPropertyChangedEventArgs> observer)
        {
            return propertyChangeSubject.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IMonadicPropertyChangedEventArgs> observer, string propertyName)
        {
    return GetObservableForProperty(propertyName).Subscribe(observer);
        }

        public IObservable<IMonadicPropertyChangedEventArgs> GetObservableForProperty(string propertyName)
        {
            IObservable<IMonadicPropertyChangedEventArgs> res;
            lock (gate)
            {
                // Caching and publishling the filtered subject for a property gives significant performance
                // benefits when multiple subscribers. When there are only a few the overhead doesn't matter.

                observables.TryGetValue(propertyName, out res);
                if (res != null) return res;

                res = propertyChangeSubject.Where(x => x.PropertyName == propertyName).Publish().RefCount();

                observables.Add(propertyName, res);
            }

            return res;
        }

        #endregion

        #region Implementation of IDisposable

        public virtual void Dispose()
        {
            propertyChangeSubject.OnCompleted();
            propertyChangeSubject.Dispose();
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

            var handlerSnapshot = PropertyChanged;

            var notification = MonadicPropertyChangedEventArgs.Create(propertyName, value, Source, userData);

            if (handlerSnapshot != null)
                handlerSnapshot(Source, notification);

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

    public static class ObservableObjectMixin
    {
        public static IObservable<TProperty> WhenPropertyChanged<TClass, TProperty>(this TClass instance,
            Expression<Func<TClass, TProperty>> memberPath)
            where TClass : IObservableObject
        {
            var name = ExpressionHelpers.GetMemberName(memberPath);
            return instance.PropertyChanges.Where(x => x.PropertyName == name).Select(x => (TProperty) x.Value);
        }


        public static IObservable<TProperty> WhenPropertyChanged2<TClass, TProperty>(this TClass instance,
            Expression<Func<TClass, TProperty>> memberPath)
            where TClass : ObservableObject2

        {
            var name = ExpressionHelpers.GetMemberName(memberPath);

            return instance.propertyChangeSubject.GetObservableForProperty(name).Select(x => ((MonadicPropertyChangedEventArgs<TProperty>) x).Value);
        }

        public static IObservable<IMonadicPropertyChangedEventArgs> WhenPropertyChanged3<TClass, TProperty>(this TClass instance,
            Expression<Func<TClass, TProperty>> memberPath)
            where TClass : ObservableObject3
        {
            var name = ExpressionHelpers.GetMemberName(memberPath);

            return instance.propertyChangeSubject.GetObservableForProperty(name);
        }

        public static IObservable<TProperty> WhenPropertyChanged3<TClass, TProperty>(this TClass instance, string propertyName)
            where TClass : ObservableObject3
        {
            return instance.propertyChangeSubject.Select(x => (TProperty)x.Value);
        }
    }
}