#define FAST_DISPOSAL
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
}
