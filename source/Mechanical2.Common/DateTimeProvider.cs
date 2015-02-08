using System;
using System.Threading;

namespace Mechanical.Common
{
    /// <summary>
    /// Provides current date and time related functionality.
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeProvider"/> class.
        /// </summary>
        public DateTimeProvider()
        {
        }

        /// <summary>
        /// Gets the current date and time in UTC.
        /// </summary>
        /// <value>The current date and time in UTC.</value>
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        /// <summary>
        /// Suspends the current thread for the specified amount of time.
        /// </summary>
        /// <param name="timeout">The amount of time for which the thread is suspended. If the value of the <paramref name="timeout"/> argument is <see cref="TimeSpan.Zero"/>, the thread relinquishes the remainder of its time slice to any thread of equal priority that is ready to run. If there are no other threads of equal priority that are ready to run, execution of the current thread is not suspended.</param>
        public void Sleep( TimeSpan timeout )
        {
            Thread.Sleep(timeout);
        }

        /// <summary>
        /// Suspends the current thread for the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds for which the thread is suspended. If the value of the <paramref name="millisecondsTimeout"/> argument is zero, the thread relinquishes the remainder of its time slice to any thread of equal priority that is ready to run. If there are no other threads of equal priority that are ready to run, execution of the current thread is not suspended.</param>
        public void Sleep( int millisecondsTimeout )
        {
            Thread.Sleep(millisecondsTimeout);
        }
    }
}
