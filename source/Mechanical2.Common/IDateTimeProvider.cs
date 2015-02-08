using System;

namespace Mechanical.Common
{
    /// <summary>
    /// Provides date and time related functionality.
    /// </summary>
    public interface IDateTimeProvider
    {
        //// NOTE: this interface is mainly useful for unit testing
        ////       (where instead of the current time one can test for leap years,
        ////       and the code does not spend actual time sleeping.)

        /// <summary>
        /// Gets the current date and time in UTC.
        /// </summary>
        /// <value>The current date and time in UTC.</value>
        DateTime UtcNow { get; }

        /// <summary>
        /// Suspends the current thread for the specified amount of time.
        /// </summary>
        /// <param name="timeout">The amount of time for which the thread is suspended. If the value of the <paramref name="timeout"/> argument is <see cref="TimeSpan.Zero"/>, the thread relinquishes the remainder of its time slice to any thread of equal priority that is ready to run. If there are no other threads of equal priority that are ready to run, execution of the current thread is not suspended.</param>
        void Sleep( TimeSpan timeout );

        /// <summary>
        /// Suspends the current thread for the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds for which the thread is suspended. If the value of the <paramref name="millisecondsTimeout"/> argument is zero, the thread relinquishes the remainder of its time slice to any thread of equal priority that is ready to run. If there are no other threads of equal priority that are ready to run, execution of the current thread is not suspended.</param>
        void Sleep( int millisecondsTimeout );
    }
}
