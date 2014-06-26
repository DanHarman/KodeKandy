using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading;
using KodeKandy.Panopticon.Internal;

namespace KodeKandy.Panopticon.Linq.ObservableImpl
{
    internal class NotifyPropertyValueChangedObservable<TClass, TProperty> : IObserver<PropertyValueChanged<TClass>>, IObservable<PropertyValueChanged<TProperty>>
        where TClass : class, INotifyPropertyChanged
    {
        private readonly object _gate = new object();
        private readonly string _propertyName;
        private readonly Func<TClass, TProperty> _propertyValueGetter;
        private readonly IObservable<PropertyValueChanged<TClass>> _sourceObservable;
        private IObserver<PropertyValueChanged<TProperty>> _observer = NopObserver<PropertyValueChanged<TProperty>>.Instance;
        private PropertyValueChanged<TProperty> _propertyValueChanged;
        private TClass _source;
        private IDisposable _sourceSubscriptionDisposable;

        public NotifyPropertyValueChangedObservable(IObservable<PropertyValueChanged<TClass>> sourceObservable, string propertyName,
            Func<TClass, TProperty> propertyValueGetter)
        {
            if (sourceObservable == null)
                throw new ArgumentNullException("sourceObservable");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (propertyValueGetter == null)
                throw new ArgumentNullException("propertyValueGetter");

            _sourceObservable = sourceObservable;
            _propertyName = propertyName;
            _propertyValueGetter = propertyValueGetter;
        }

        #region IObservable<TProperty> Members

        public IDisposable Subscribe(IObserver<PropertyValueChanged<TProperty>> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            lock (_gate)
            {
                if (!(_observer is CompletedObserver<PropertyValueChanged<TProperty>>))
                {
                    if (_observer == NopObserver<PropertyValueChanged<TProperty>>.Instance)
                    {
                        // If we have no _observer then make it our _observer and subscribe to _sourceObservable. Because there is no
                        // current value and the sourceObservable should send us its initial value immediately, we don't need to
                        // call observer.OnNext().
                        _observer = observer;
                        _sourceSubscriptionDisposable = _sourceObservable.Subscribe(this);
                    }
                    else
                    {
                        var multiObserver = _observer as ImmutableMultiObserver<PropertyValueChanged<TProperty>>;
                        if (multiObserver != null)
                        {
                            // If we already have a ImmutableMultiObserver then add the new _observer to it.
                            _observer = multiObserver.Add(observer);
                        }
                        else
                        {
                            // We didn't have a multiobserver, so we must have just had a single observer, so replace it with a multiobserver containing
                            // both the old and new observer.
                            var oldObserver = _observer;
                            _observer =
                                new ImmutableMultiObserver<PropertyValueChanged<TProperty>>(new ImmutableList<IObserver<PropertyValueChanged<TProperty>>>(new[] { oldObserver, observer }));
                        }

                        // Send the new observer the current property value. This is done inside the lock to prevent race conditions around the initial
                        // value. Behaviour subject does similar, although I'm not sure we need to make this guarantee as it is stronger than the
                        // guarantee for the property changing once already subscribed (whereby ordering is not guaranteed).
                        observer.OnNext(_propertyValueChanged);
                    }

                    return new Subscription(this, observer);
                }
            }

            // If we got here we have a completed observer and that means _observer won't be modified so we can work outside the lock safely.
            var completedObserver = _observer as CompletedObserver<TProperty>;

            if (completedObserver == CompletedObserver<TProperty>.Instance)
                observer.OnCompleted();
            else
                observer.OnError(completedObserver.Error);

            return Disposable.Empty;
        }

        #endregion

        #region IObserver<TClass> Members

        void IObserver<PropertyValueChanged<TClass>>.OnNext(PropertyValueChanged<TClass> newSource)
        {
            IObserver<PropertyValueChanged<TProperty>> oldObserver;
            TClass oldSource;
            PropertyValueChanged<TProperty> initialPropertyValueChanged;

            lock (_gate)
            {
                if (_observer is CompletedObserver<PropertyValueChanged<TProperty>>)
                    return;

                oldObserver = _observer;
                oldSource = _source;
                _source = newSource.Value;

                // We need to get the current property value from this new source if it is not null, but if it is propagate the nullness
                // with a PropertyValueChanged with HasValue == false.
                initialPropertyValueChanged = newSource.HasValue
                    ? PropertyValueChanged.CreateWithValue(_source, _propertyName, _propertyValueGetter(_source))
                    : PropertyValueChanged.CreateWithoutValue<TProperty>(_source, _propertyName);

                _propertyValueChanged = initialPropertyValueChanged;
            }

            // Connect to INotifyPropertyChanged on the new _source.
            if (newSource.Value != null)
                newSource.Value.PropertyChanged += OnPropertyChanged;

            // Disconnect from INotifyPropertyChanged on previous _source. Because _source has been changed it doesn't matter
            // if we end up with an extra event from this old sourceObservable sneaking through as it will be filtered out.
            if (oldSource != null)
                oldSource.PropertyChanged -= OnPropertyChanged;

            oldObserver.OnNext(initialPropertyValueChanged);
        }

        void IObserver<PropertyValueChanged<TClass>>.OnError(Exception error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            IObserver<PropertyValueChanged<TProperty>> oldObserver;

            lock (_gate)
            {
                if (_observer is CompletedObserver<PropertyValueChanged<TProperty>>)
                    return;

                oldObserver = _observer;
                CleanUp(new CompletedObserver<PropertyValueChanged<TProperty>>(error));
            }

            oldObserver.OnError(error);
        }

        void IObserver<PropertyValueChanged<TClass>>.OnCompleted()
        {
            IObserver<PropertyValueChanged<TProperty>> oldObserver;

            lock (_gate)
            {
                if (_observer is CompletedObserver<PropertyValueChanged<TProperty>>)
                    return;

                oldObserver = _observer;
                CleanUp(CompletedObserver<PropertyValueChanged<TProperty>>.Instance);
            }

            oldObserver.OnCompleted();
        }

        #endregion

        /// <summary>
        ///     Internal <see cref="PropertyChangedEventHandler" /> to bind to the targets PropertyChanged event.
        ///     Gets the current property value from the current sourceObservable and pushes it to the observers.
        /// </summary>
        /// <remarks>
        ///     We keep a local copy of the sourceObservable value so that we don't end up with concurrency issues which
        ///     would occur if we pulled it form the sourceObservable whenever it was needed e.g. when adding a new observer, as
        ///     we do not have a lock around the sourceObservable, but we can control when it pushes new values into us with our
        ///     lock.
        /// </remarks>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_source == sender && _propertyName == propertyChangedEventArgs.PropertyName)
            {
                IObserver<PropertyValueChanged<TProperty>> currObserver;
                PropertyValueChanged<TProperty> propertyValueChanged;

                lock (_gate)
                {
                    currObserver = _observer;
                    propertyValueChanged = new PropertyValueChanged<TProperty>(_source, propertyChangedEventArgs, _propertyValueGetter(_source));
                    _propertyValueChanged = propertyValueChanged;
                }
                
                currObserver.OnNext(propertyValueChanged);
            }
        }

        /// <summary>
        ///     Removes a subscription, disposing of the upstream subscription if no longer required.
        /// </summary>
        /// <remarks>
        ///     This function is intentionally tolerant of observers being supplied that are not subscribed, or
        ///     thid observable having already completed.
        /// </remarks>
        /// <param name="observer">The observer to unsubscribe.</param>
        private void Unsubscribe(IObserver<PropertyValueChanged<TProperty>> observer)
        {
            lock (_gate)
            {
                if (_observer is CompletedObserver<PropertyValueChanged<TProperty>>)
                    return;

                var multiObserver = _observer as ImmutableMultiObserver<PropertyValueChanged<TProperty>>;
                if (multiObserver != null)
                {
                    _observer = multiObserver.Remove(observer);
                    return;
                }

                if (_observer != observer)
                    return;

                // If we got here we only had one observer, so clear our upstream subscription.
                CleanUp(NopObserver<PropertyValueChanged<TProperty>>.Instance);
            }
        }

        /// <summary>
        ///     Cleans up by unsubscribing to the sourceObservable, and releases references.
        /// </summary>
        /// <remarks>Must be called inside the lock.</remarks>
        private void CleanUp(IObserver<PropertyValueChanged<TProperty>> observer)
        {
            _observer = observer;
            _source = default(TClass);
            _propertyValueChanged = null;
            if (_sourceSubscriptionDisposable != null)
            {
                _sourceSubscriptionDisposable.Dispose();
                _sourceSubscriptionDisposable = null;
            }
        }

        #region Nested type: Subscription

        /// <summary>
        ///     Lightweight subscription class that unsubscribes from the NotifyPropertyChangedLink when disposed.
        ///     This is much more performant than using a Disposable.Create().
        /// </summary>
        private class Subscription : IDisposable
        {
            private IObserver<PropertyValueChanged<TProperty>> _observer;
            private NotifyPropertyValueChangedObservable<TClass, TProperty> _valueObservable;

            public Subscription(NotifyPropertyValueChangedObservable<TClass, TProperty> valueObservable, IObserver<PropertyValueChanged<TProperty>> observer)
            {
                _valueObservable = valueObservable;
                _observer = observer;
            }

            #region IDisposable Members

            public void Dispose()
            {
                var currObserver = Interlocked.Exchange(ref _observer, null);
                if (currObserver == null)
                    return;

                _valueObservable.Unsubscribe(currObserver);
                _valueObservable = null;
            }

            #endregion
        }

        #endregion
    }
}