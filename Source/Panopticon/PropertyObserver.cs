#define FAST_DISPOSAL
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace KodeKandy.Panopticon
{
    public static class PropertyObserver
    {
        public static IDisposable Observe<TValue>(INotifyPropertyChanged root, string path, Action<TValue> whenChanged)
        {
            var pathComponents = path.Split('.');
            if (pathComponents.Length == 1)
            {
                return new PropertyObserver<TValue>(root, path, whenChanged);
            }
            return new PropertyChainObserver<TValue>(root, pathComponents, whenChanged);
        }
    }

    public class PropertyObserver<TValue> : IDisposable
    {
#if FAST_DISPOSAL
        private class ObserverList
        {
            private readonly HashSet<PropertyObserver<TValue>> links = new HashSet<PropertyObserver<TValue>>();

            public bool IsEmpty
            {
                get { return links.Count == 0; }
            }

            private void OnChanged(object sender, PropertyChangedEventArgs args)
            {
                foreach (var link in links)
                {
                    link.OnChange(sender, args);
                }
            }

            public void Add(PropertyObserver<TValue> link)
            {
                links.Add(link);
            }

            public void Remove(PropertyObserver<TValue> link)
            {
                links.Remove(link);
            }

            public void Unbind(INotifyPropertyChanged observed)
            {
                observed.PropertyChanged -= OnChanged;
            }

            public void Bind(INotifyPropertyChanged observed)
            {
                observed.PropertyChanged += OnChanged;
            }
        }
#endif

        private static class ObserverCache
        {
#if FAST_DISPOSAL
            private static readonly ConditionalWeakTable<INotifyPropertyChanged, ObserverList> Entries = new ConditionalWeakTable<INotifyPropertyChanged, ObserverList>();

            public static void Unbind(INotifyPropertyChanged observed, PropertyObserver<TValue> link)
            {
                ObserverList observerList;
                if (!Entries.TryGetValue(observed, out observerList)) return;
                observerList.Remove(link);
                if (observerList.IsEmpty)
                {
                    observerList.Unbind(observed);
                    Entries.Remove(observed);
                }
            }

            public static void Bind(INotifyPropertyChanged observed, PropertyObserver<TValue> link)
            {
                ObserverList observerList;
                if (!Entries.TryGetValue(observed, out observerList))
                {
                    observerList = new ObserverList();
                    observerList.Bind(observed);
                    Entries.Add(observed, observerList);
                }
                observerList.Add(link);
            }
#else
            public static void Unbind(INotifyPropertyChanged observed, PropertyObserver<TValue> link)
            {
                observed.PropertyChanged -= link.OnChange;
            }
            public static void Bind(INotifyPropertyChanged observed, PropertyObserver<TValue> link)
            {
                observed.PropertyChanged += link.OnChange;
            }
#endif
        }

        private readonly string sourceProperty;
        private readonly Action<TValue> whenChanged;
        private Func<object, object> getter;
        private INotifyPropertyChanged source;

        public PropertyObserver(string sourceProperty, Action<TValue> whenChanged)
        {
            Require.NotNullOrEmpty(sourceProperty, "sourceProperty");
            Require.NotNull(whenChanged, "whenChanged");

            this.sourceProperty = sourceProperty;
            this.whenChanged = whenChanged;
        }

        public PropertyObserver(INotifyPropertyChanged newSource, string sourceProperty, Action<TValue> whenChanged)
            : this(sourceProperty, whenChanged)
        {
            Rebind(newSource);
        }

        public void Rebind(INotifyPropertyChanged newSource)
        {
            Type oldType = null;
            if (source != null)
            {
                oldType = source.GetType();
                ObserverCache.Unbind(source, this);
            }
            source = newSource;
            if (source == null)
            {
                getter = null;
                whenChanged(default(TValue));
            }
            else if (sourceProperty == "*")
            {
                getter = null;
                ObserverCache.Bind(source, this);
                whenChanged(default(TValue));
            }
            else
            {
                if (source.GetType() != oldType || getter == null)
                {
                    getter = ReflectionHelpers.CreateWeakMemberGetter(ReflectionHelpers.GetMemberInfo(source.GetType(), sourceProperty));
                    //ClassDelegateCache.GetWeakDelegatePair(source.GetType(), sourceProperty).Getter;
                }
                if (getter == null)
                {
                    throw new ArgumentException(string.Format("No setter for property {0} on {1}", sourceProperty, source.GetType()));
                }
                ObserverCache.Bind(source, this);
                whenChanged((TValue)getter(source));
            }
        }

        private void OnChange(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName != sourceProperty && sourceProperty != "*") || source == null || getter == null) return;
            whenChanged((TValue)getter(source));
        }

        public void Dispose()
        {
            if (source != null)
            {
                ObserverCache.Unbind(source, this);
                source = null;
            }
        }
    }

    internal class PropertyChainObserver<TValue> : IDisposable
    {
        private readonly IDisposable lhs;
        private readonly PropertyObserver<TValue> rhs;

        public PropertyChainObserver(INotifyPropertyChanged root, string[] sourcePath, Action<TValue> whenChanged)
        {
            rhs = new PropertyObserver<TValue>(sourcePath[sourcePath.Length - 1], whenChanged);
            if (sourcePath.Length == 1)
            {
                rhs.Rebind(root);
            }
            else if (sourcePath.Length == 2)
            {
                lhs = new PropertyObserver<INotifyPropertyChanged>(root, sourcePath[0], x => rhs.Rebind(x));
            }
            else if (sourcePath.Length > 2)
            {
                var subPath = new string[sourcePath.Length - 1];
                Array.Copy(sourcePath, subPath, sourcePath.Length - 1);
                lhs = new PropertyChainObserver<INotifyPropertyChanged>(root, subPath, x => rhs.Rebind(x));
            }
        }

        public void Dispose()
        {
            rhs.Dispose();
            if (lhs != null) lhs.Dispose();
        }
    }

    public interface IPropertyChangeObserver : IDisposable
    {
        void OnChanged(object sender, PropertyChangedEventArgs args);
    }

    public interface IPropertyChangeObserver<in TClass, out TValue> : IPropertyChangeObserver
        where TClass : class, INotifyPropertyChanged
    {
        void Rebind(TClass o);
        Action<TValue> Action { set; }
    }

    public class ObservableProperty<TClass, TValue>
        where TClass : class, INotifyPropertyChanged
    {
        public readonly Func<TClass, TValue> Getter;
        public readonly string PropertyName;

        public ObservableProperty(Expression<Func<TClass, TValue>> expr)
        {
            Getter = expr.Compile();
            PropertyName = ExpressionHelpers.GetMemberName(expr);
        }
    }

    sealed class ObserverList
    {
        private readonly HashSet<IPropertyChangeObserver> observers = new HashSet<IPropertyChangeObserver>();

        private void OnChanged(object sender, PropertyChangedEventArgs args)
        {
            foreach (var observer in observers)
            {
                observer.OnChanged(sender, args);
            }
        }

        public void Add(INotifyPropertyChanged observed, IPropertyChangeObserver link)
        {
            if (observers.Count == 0) observed.PropertyChanged += OnChanged;
            observers.Add(link);
        }

        public bool Remove(INotifyPropertyChanged observed, IPropertyChangeObserver link)
        {
            observers.Remove(link);
            if (observers.Count != 0) return false;
            observed.PropertyChanged -= OnChanged;
            return true;
        }
    }

    static class ObserverCache
    {
        private static readonly ConditionalWeakTable<INotifyPropertyChanged, ObserverList> Entries = new ConditionalWeakTable<INotifyPropertyChanged, ObserverList>();

        public static void Unbind(INotifyPropertyChanged observed, IPropertyChangeObserver link)
        {
            ObserverList observerList;
            if (!Entries.TryGetValue(observed, out observerList)) return;
            if (observerList.Remove(observed, link))
            {
                Entries.Remove(observed);
            }
        }

        public static void Bind(INotifyPropertyChanged observed, IPropertyChangeObserver link)
        {
            ObserverList observerList;
            if (!Entries.TryGetValue(observed, out observerList))
            {
                observerList = new ObserverList();
                Entries.Add(observed, observerList);
            }
            observerList.Add(observed, link);
        }
    }

    public class Observer<TClass, TValue> : IPropertyChangeObserver<TClass,TValue>
        where TClass : class, INotifyPropertyChanged
    {
        private readonly string propertyName;
        private readonly Func<TClass, TValue> getter;
        private TValue currValue;
        private bool firedAny;

        private TClass observed;
        private Action<TValue> action;

        private TClass Observed
        {
            set
            {
                if (ReferenceEquals(observed, value)) return;
                if (observed != null)
                {
                    ObserverCache.Unbind(observed, this);
                }
                observed = value;
                if (observed != null)
                {
                    ObserverCache.Bind(observed, this);
                }
            }
        }

        public Action<TValue> Action
        {
            set
            {
                action = value;
                if (observed != null)
                {
                    Fire(null);
                }
            }
        }

        public Observer(string propertyName)
        {
            this.propertyName = propertyName;
            getter = (Func<TClass, TValue>) ReflectionHelpers.CreatePropertyGetter(typeof(TClass).GetProperty(propertyName));

           // getter = x => (TValue)DelegateCache.GetPropertyGetter(typeof(TClass), propertyName)(x);
            action = null;
        }

        public Observer(string propertyName, Action<TValue> act)
        {
            this.propertyName = propertyName;
            getter = (Func<TClass, TValue>) ReflectionHelpers.CreatePropertyGetter(typeof(TClass).GetProperty(propertyName));

//            getter = x => (TValue)DelegateCache.GetPropertyGetter(typeof(TClass), propertyName)(x);
            action = act;
        }

        public Observer(Func<TClass, TValue> getter, string propertyName, Action<TValue> action)
        {
            this.propertyName = propertyName;
            this.getter = getter;
            this.action = action;
        }

        public Observer(ObservableProperty<TClass, TValue> property, Action<TValue> action)
        {
            propertyName = property.PropertyName;
            getter = property.Getter;
            this.action = action;
        }

        public Observer(Func<TClass, TValue> getter, string propertyName)
        {
            this.propertyName = propertyName;
            this.getter = getter;
        }

        public Observer(ObservableProperty<TClass, TValue> property)
        {
            propertyName = property.PropertyName;
            getter = property.Getter;
        }

        public Observer(TClass root, string propertyName)
        {
            this.propertyName = propertyName;
            getter = (Func<TClass, TValue>) ReflectionHelpers.CreatePropertyGetter(typeof(TClass).GetProperty(propertyName));

//            getter = (x => (TValue) DelegateCache.GetPropertyGetter(typeof (TClass), propertyName)(x));
            Rebind(root);
        }

        public Observer(TClass root, Func<TClass, TValue> gettr, string propertyName)
        {
            this.propertyName = propertyName;
            getter = gettr;
            Rebind(root);
        }

        public Observer(TClass root, ObservableProperty<TClass, TValue> property)
        {
            propertyName = property.PropertyName;
            getter = property.Getter;
            Rebind(root);
        }

//        public Observer(TClass root, string propertyName, Action<TValue> action)
//            : this(x => (TValue)DelegateCache.GetPropertyGetter(typeof(TClass), propertyName)(x), propertyName, action)
//        {
//            Rebind(root);
//        }

        public Observer(TClass root, Func<TClass, TValue> gettr, string propertyName, Action<TValue> act)
            : this(gettr, propertyName, act)
        {
            Rebind(root);
        }

        public Observer(TClass root, ObservableProperty<TClass, TValue> property, Action<TValue> action)
        {
            propertyName = property.PropertyName;
            getter = property.Getter;
            this.action = action;
            Rebind(root);
        }

        public void Rebind(TClass o)
        {
            Observed = o;
            Fire(null);
        }

        public void OnChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != propertyName) return;
            Fire(args as PropertyChange<TValue>);
        }

        private void Fire(PropertyChange<TValue> args)
        {
            if (action == null) return;            
            var newValue = args == null ? (observed == null ? default(TValue) : getter(observed)) : args.Value;
         //   if (firedAny && EqualityComparer<TValue>.Default.Equals(newValue, currValue)) return;
            firedAny = true;
            currValue = newValue;
            action(newValue);
        }

        public void Dispose()
        {
            Observed = null;
        }
    }


    public class ObserverChain<TClass, TMiddle, TRight> : IPropertyChangeObserver<TClass, TRight>
        where TClass : class, INotifyPropertyChanged 
        where TMiddle : class, INotifyPropertyChanged
    {
        private readonly IPropertyChangeObserver<TClass, TMiddle> lhs;
        private readonly Observer<TMiddle, TRight> rhs;

        public ObserverChain(IPropertyChangeObserver<TClass, TMiddle> lhs, Observer<TMiddle, TRight> rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;        
            lhs.Action = rhs.Rebind;
        }

        public void Dispose()
        {
            lhs.Dispose();
            rhs.Dispose();
        }

        public void OnChanged(object sender, PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void Rebind(TClass o)
        {
            lhs.Rebind(o);
        }

        public Action<TRight> Action
        {
            set { rhs.Action = value; }
        }
    }

    public class ObserverFilter<TClass, TValue> : IPropertyChangeObserver<TClass, TValue> 
        where TClass : class, INotifyPropertyChanged 
    {
        private readonly IPropertyChangeObserver<TClass, TValue> lhs;
        private readonly Func<TValue, bool> predicate;
        private Action<TValue> action;

        public ObserverFilter(IPropertyChangeObserver<TClass,TValue> lhs, Func<TValue,bool> predicate, Action<TValue> act)
        {
            this.lhs = lhs;
            this.predicate = predicate;
            lhs.Action = Fire;
            action = act;
        }

        private void Fire(TValue v)
        {
            if (action != null && predicate(v))
            {
                action(v);
            }
        }

        public void Dispose()
        {
            lhs.Dispose();
        }

        public void OnChanged(object sender, PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void Rebind(TClass o)
        {
            lhs.Rebind(o);
        }

        public Action<TValue> Action
        {
            set { action = value; }
        }
    }

    public static class ObserverExtensions
    {
        public static ObserverChain<TClass, TMiddle, TRight> Chain<TClass, TMiddle, TRight>(this IPropertyChangeObserver<TClass, TMiddle> This, string propertyName)
            where TClass : class, INotifyPropertyChanged
            where TMiddle : class, INotifyPropertyChanged
        {
            return new ObserverChain<TClass, TMiddle, TRight>(This, new Observer<TMiddle, TRight>(propertyName));
        }

        public static ObserverChain<TClass, TMiddle, TRight> Chain<TClass, TMiddle, TRight>(this IPropertyChangeObserver<TClass, TMiddle> This, Func<TMiddle, TRight> func, string propertyName)
            where TClass : class, INotifyPropertyChanged
            where TMiddle : class, INotifyPropertyChanged
        {
            return new ObserverChain<TClass, TMiddle, TRight>(This, new Observer<TMiddle, TRight>(func, propertyName));
        }

        public static ObserverChain<TClass, TMiddle, TRight> Chain<TClass, TMiddle, TRight>(this IPropertyChangeObserver<TClass, TMiddle> This, ObservableProperty<TMiddle, TRight> property)
            where TClass : class, INotifyPropertyChanged
            where TMiddle : class, INotifyPropertyChanged
        {
            return new ObserverChain<TClass, TMiddle, TRight>(This, new Observer<TMiddle, TRight>(property));
        }

        public static ObserverChain<TClass, TMiddle, TRight> Chain<TClass, TMiddle, TRight>(this IPropertyChangeObserver<TClass, TMiddle> This, string property, Action<TRight> act)
            where TClass : class, INotifyPropertyChanged
            where TMiddle : class, INotifyPropertyChanged
        {
            return new ObserverChain<TClass, TMiddle, TRight>(This, new Observer<TMiddle, TRight>(property, act));
        }

        public static ObserverChain<TClass, TMiddle, TRight> Chain<TClass, TMiddle, TRight>(this IPropertyChangeObserver<TClass, TMiddle> This, Func<TMiddle, TRight> func, string property, Action<TRight> act)
            where TClass : class, INotifyPropertyChanged
            where TMiddle : class, INotifyPropertyChanged
        {
            return new ObserverChain<TClass, TMiddle, TRight>(This, new Observer<TMiddle, TRight>(func, property, act));
        }

        public static ObserverChain<TClass, TMiddle, TRight> Chain<TClass, TMiddle, TRight>(this IPropertyChangeObserver<TClass, TMiddle> This, ObservableProperty<TMiddle, TRight> property, Action<TRight> act)
            where TClass : class, INotifyPropertyChanged
            where TMiddle : class, INotifyPropertyChanged
        {
            return new ObserverChain<TClass, TMiddle, TRight>(This, new Observer<TMiddle, TRight>(property, act));
        }

        public static ObserverFilter<TClass, TValue> Where<TClass, TValue>(this IPropertyChangeObserver<TClass, TValue> This, Func<TValue, bool> predicate)
            where TClass : class, INotifyPropertyChanged
        {
            return new ObserverFilter<TClass, TValue>(This, predicate, null);
        }

        public static ObserverFilter<TClass, TValue> Where<TClass, TValue>(this IPropertyChangeObserver<TClass, TValue> This, Func<TValue, bool> predicate, Action<TValue> action)
            where TClass : class, INotifyPropertyChanged
        {
            return new ObserverFilter<TClass, TValue>(This, predicate, action);
        }

        public static IPropertyChangeObserver<TClass,TValue> Subscribe<TClass, TValue>(this IPropertyChangeObserver<TClass, TValue> This, Action<TValue> action)
            where TClass : class, INotifyPropertyChanged
        {
            This.Action = action;
            return This;
        }
    }

}
