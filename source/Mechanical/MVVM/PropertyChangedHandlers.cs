using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM
{
    /// <summary>
    /// Manages handlers of multiple property changed events, from any number of sources.
    /// </summary>
    public class PropertyChangedHandlers : DisposableObject
    {
        #region Handler

        private class Handler
        {
            internal Handler( INotifyPropertyChanged eventSource, string propertyName, Action<PropertyChangedEventArgs> action )
            {
                Ensure.Debug(eventSource, e => e.NotNull());
                Ensure.Debug(propertyName, p => p.NotNullOrLengthy());
                Ensure.Debug(action, a => a.NotNull());

                this.weakEventSource = new WeakReference(eventSource);
                this.propertyName = propertyName;
                this.action = action;

                eventSource.PropertyChanged += this.OnPropertyChanged;
            }

            private readonly WeakReference weakEventSource;
            private readonly string propertyName;
            private readonly Action<PropertyChangedEventArgs> action;

            internal INotifyPropertyChanged EventSource
            {
                get { return (INotifyPropertyChanged)this.weakEventSource.Target; }
            }

            private void OnPropertyChanged( object sender, PropertyChangedEventArgs e )
            {
                if( string.Equals(e.PropertyName, this.propertyName, StringComparison.Ordinal) )
                    this.action(e);
            }

            internal void Unsubscribe()
            {
                var source = this.EventSource;
                if( source.NotNullReference() )
                    source.PropertyChanged -= this.OnPropertyChanged;
            }
        }

        #endregion

        #region Private Fields

        private readonly List<Handler> handlers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedHandlers"/> class.
        /// </summary>
        public PropertyChangedHandlers()
        {
            this.handlers = new List<Handler>();
        }

        #endregion

        #region IDisposableObject

        /// <summary>
        /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
        protected override void OnDispose( bool disposing )
        {
            if( disposing )
            {
                //// dispose-only (i.e. non-finalizable) logic
                //// (managed, disposable resources you own)

                this.Clear();
            }

            //// shared cleanup logic
            //// (unmanaged resources)


            base.OnDispose(disposing);
        }

        #endregion

        #region Private Methods

        private int IndexOf( INotifyPropertyChanged eventSource )
        {
            for( int i = 0; i < this.handlers.Count; ++i )
            {
                if( object.ReferenceEquals(this.handlers[i].EventSource, eventSource) )
                    return i;
            }

            return -1;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers the specified <see cref="Action"/> to be executed, when the property of the source changes.
        /// </summary>
        /// <param name="eventSource">The event source to listen to.</param>
        /// <param name="propertyName">The property change to handle.</param>
        /// <param name="action">The handler to register.</param>
        public void Handle( INotifyPropertyChanged eventSource, string propertyName, Action<PropertyChangedEventArgs> action )
        {
            Ensure.That(eventSource).NotNull();
            Ensure.That(propertyName).NotNullOrLengthy();
            Ensure.That(action).NotNull();

            this.handlers.Add(new Handler(eventSource, propertyName, action));
        }

        /// <summary>
        /// Registers the specified <see cref="Action"/> to be executed, when the property of the source changes.
        /// </summary>
        /// <param name="eventSource">The event source to listen to.</param>
        /// <param name="propertyName">The property change to handle.</param>
        /// <param name="action">The handler to register.</param>
        public void Handle( INotifyPropertyChanged eventSource, string propertyName, Action action )
        {
            Ensure.That(action).NotNull();

            this.Handle(eventSource, propertyName, e => action());
        }

        /// <summary>
        /// Unsubscribes all handlers of the specified source.
        /// </summary>
        /// <param name="eventSource">The source to remove handlers of.</param>
        public void UnsubscribeAll( INotifyPropertyChanged eventSource )
        {
            int at;
            while( (at = this.IndexOf(eventSource)) != -1 )
            {
                var handler = this.handlers[at];
                handler.Unsubscribe();
                this.handlers.RemoveAt(at);
            }
        }

        /// <summary>
        /// Unsubscribes all handlers.
        /// </summary>
        public void Clear()
        {
            foreach( var handler in this.handlers )
                handler.Unsubscribe();

            this.handlers.Clear();
        }

        #endregion
    }
}
