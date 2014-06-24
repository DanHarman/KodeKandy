using System;
using System.Linq.Expressions;
using KodeKandy.Panopticon.Linq.ObservableImpl;

namespace KodeKandy.Panopticon.Linq
{
    public static class Opticon
    {
        public static IObservable<T> Observe<T>(T source)
            where T : class
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return new Forever<T>(source);
        }

        public static IObservable<TProperty> Observe<TClass, TProperty>(TClass source, Expression<Func<TClass, TProperty>> memberPath)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return MemberObservableFactory.Create(source, memberPath);
        }
    }
}