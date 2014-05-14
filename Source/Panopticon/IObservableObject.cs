using System;

namespace KodeKandy.Panopticon
{
    public interface IObservableObject : IDisposable
    {
        /// <summary>
        ///     An observable providing notification of property changes. It will complete when the object is
        ///     disposed.
        /// </summary>
        IObservable<IPropertyChange> PropertyChanges { get; }

        /// <summary>
        ///     Suppress all change notifications for the lifetime of the returned disposable.
        ///     Typically used within a 'using' block.
        /// </summary>
        /// <returns>A disposable that should be disposed when notification suppression is over.</returns>
        IDisposable BeginNotificationSuppression();
    }
}