using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading;

namespace KodeKandy.Panopticon
{
    public class PropertyChangeHelper
    {
        public PropertyChangeHelper(object source)
        {
            Require.NotNull(source, "source");

            Source = source;
        }

        public object Source { get; private set; }
        private int suppressNotificationCount;

        protected bool IsNotificationSuppressed
        {
            get { return Interlocked.CompareExchange(ref suppressNotificationCount, 0, 0) != 0; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetPropertyValue<T>(ref T property, T value, string propertyName, object userData = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
                return;

            property = value;

            NotifyPropertyValueChanged(propertyName, userData);
        }

        public void NotifyPropertyValueChanged(string propertyName, object userData = null)
        {
            if (IsNotificationSuppressed)
                return;

            var notification = new PropertyChangeEventArgsEx(Source, propertyName, userData);

            var handlerSnapshot = PropertyChanged;

            if (handlerSnapshot != null)
                handlerSnapshot(Source, notification);
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
}