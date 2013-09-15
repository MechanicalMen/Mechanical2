using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Events
{
    /// <summary>
    /// Represents an unhandled exception.
    /// </summary>
    public sealed class UnhandledExceptionEvent : IEvent
    {
        private readonly Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionEvent"/> class.
        /// </summary>
        /// <param name="e">The unhandled exception to wrap.</param>
        public UnhandledExceptionEvent( Exception e )
        {
            if( e.NotNullReference() )
                this.exception = e;
            else
                this.exception = new ArgumentNullException("e").StoreDefault();
        }

        /// <summary>
        /// Gets the unhandled exception.
        /// </summary>
        /// <value>The unhandled exception.</value>
        public Exception Exception
        {
            get { return this.exception; }
        }
    }
}
