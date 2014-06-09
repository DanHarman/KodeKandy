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

        private static Dictionary<MemberInfo, Delegate> cache = new Dictionary<MemberInfo, Delegate>();
  //      private static Dictionary<Tuple<Type, string>, Delegate> cache = new Dictionary<Tuple<Type, string>, Delegate>();
        private static object gate = new object();

        private static Delegate CreateNotifyPropertyChangedLinkMethodDelegate(MemberInfo memberInfo)
        {
            Delegate result;
        //    var key = Tuple.Create(instanceType, memberInfo.Name);
            
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
            where TIn : INotifyPropertyChanged
        {
            var getter = ReflectionHelpers.CreatePropertyGetter((PropertyInfo) outMemberInfo);
            return new NotifyPropertyChangedLink<TIn, TOut>((IObservable<TIn>) source, outMemberInfo.Name, (Func<TIn, TOut>) getter);
        }
    }
}