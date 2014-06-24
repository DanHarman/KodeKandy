using System;
using System.ComponentModel;

namespace KodeKandy.Panopticon
{
    public interface IObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        ///     Suppress all PropertyChanged events for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        IDisposable SuppressPropertyChanged();
    }
}