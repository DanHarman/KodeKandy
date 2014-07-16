using System;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    interface IAdaptor<in TFrom, out TTo> : IObserver<TFrom>, IDisposable
    {
        void NotifyInitialValue(IObserver<TTo> observer);
    }
}