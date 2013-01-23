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
    public class PropertyHandlers
    {
        #region Handler

        private class Handler
        {
            internal Handler( INotifyPropertyChanged eventSource, string propertyName, Action action )
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
            private readonly Action action;

            internal INotifyPropertyChanged EventSource
            {
                get { return (INotifyPropertyChanged)this.weakEventSource.Target; }
            }

            private void OnPropertyChanged( object sender, PropertyChangedEventArgs e )
            {
                if( string.Equals(e.PropertyName, this.propertyName, StringComparison.Ordinal) )
                    this.action();
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
        /// Initializes a new instance of the <see cref="PropertyHandlers"/> class.
        /// </summary>
        public PropertyHandlers()
        {
            this.handlers = new List<Handler>();
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
        public void Handle( INotifyPropertyChanged eventSource, string propertyName, Action action )
        {
            Ensure.That(eventSource).NotNull();
            Ensure.That(propertyName).NotNullOrLengthy();
            Ensure.That(action).NotNull();

            this.handlers.Add(new Handler(eventSource, propertyName, action));
        }

        /// <summary>
        /// Unsubscribes all handlers of the specified source.
        /// </summary>
        /// <param name="eventSource">The source to remove handlers of.</param>
        public void Unsubscribe( INotifyPropertyChanged eventSource )
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
