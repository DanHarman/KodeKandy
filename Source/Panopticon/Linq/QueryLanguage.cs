using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
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

            return new ReturnForever<T>(source);
        }

        public static IObservable<TProperty> Observe<TClass, TProperty>(TClass source, Expression<Func<TClass, TProperty>> memberPath)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return PropertyObservableFactory.Create(source, memberPath);
        }
    }

    public static class QueryLanguage
    {
        public static IObservable<TOut> When<TIn, TOut>(this IObservable<TIn> source, string propertyName, Func<TIn, TOut> outValueGetter)
            where TIn : INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (outValueGetter == null)
                throw new ArgumentNullException("outValueGetter");

            return new NotifyPropertyChangedLink<TIn, TOut>(source, propertyName, outValueGetter);
        }

        public static IObservable<Unit> WhenAny<TIn>(this IObservable<TIn> source)
    where TIn : INotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException("source");
            // TODO need a specialised notifyProeprtychanged link it would seem if we want to expose the propertyname and type.
            throw new NotImplementedException();
          //  return new NotifyPropertyChangedLink<TIn, TOut>(source, propertyName, outValueGetter);
        }
    }
}
