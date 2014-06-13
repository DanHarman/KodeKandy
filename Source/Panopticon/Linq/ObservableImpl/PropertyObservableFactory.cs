using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using KodeKandy.Panopticon.Properties;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    public static class PropertyObservableFactory
    {
        private static readonly MethodInfo CreateNotifyPropertyChangedLinkMethod =
            typeof(PropertyObservableFactory).GetMethod("CreateNotifyPropertyChangedLink",
                BindingFlags.Static | BindingFlags.NonPublic);

        public static IObservable<TProperty> Create<TClass, TProperty>(TClass source, Expression<Func<TClass, TProperty>> memberPath)
        {
            var memberInfos = ExpressionHelpers.GetMemberInfos(memberPath);

            object currObserver = ReturnForever<TClass>.Value(source);
            foreach (var memberInfo in memberInfos)
            {
                var create = (Func<object, MemberInfo, object>) CreateNotifyPropertyChangedLinkMethodDelegate(memberInfo);
                currObserver = create(currObserver, memberInfo);
            }

            return (IObservable<TProperty>) currObserver;
        }

        private static readonly Dictionary<MemberInfo, Delegate> cache = new Dictionary<MemberInfo, Delegate>();
        private static readonly object gate = new object();

        private static Delegate CreateNotifyPropertyChangedLinkMethodDelegate(MemberInfo memberInfo)
        {
            Delegate result;
            
            lock (gate)
            {
                if (cache.TryGetValue(memberInfo, out result))
                    return result;

                var instanceType = memberInfo.ReflectedType;
                var memberType = memberInfo.GetMemberType();
                var specialisedMethod = CreateNotifyPropertyChangedLinkMethod.MakeGenericMethod(instanceType, memberType);

                result = Delegate.CreateDelegate(typeof(Func<,,>).MakeGenericType(typeof(object), typeof(MemberInfo), typeof(object)),
                    specialisedMethod);
                cache.Add(memberInfo, result);
            }

            return result;
        }

        [UsedImplicitly]
        private static object CreateNotifyPropertyChangedLink<TIn, TOut>(object source, MemberInfo outMemberInfo)
            where TIn : class, INotifyPropertyChanged
        {
            var getter = ReflectionHelpers.CreatePropertyGetter((PropertyInfo) outMemberInfo);
            return new NotifyPropertyChangedLink<TIn, TOut>((IObservable<TIn>) source, outMemberInfo.Name, (Func<TIn, TOut>) getter);
        }
    }
}