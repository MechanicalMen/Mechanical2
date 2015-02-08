using System;
using System.Threading;
using Mechanical.Common;
using Mechanical.Conditions;

namespace Mechanical.Tests
{
    public class TestDateTimeProvider : IDateTimeProvider
    {
        #region Private Fields

        private DateTime utcNow = new DateTime(0L, DateTimeKind.Utc);

        #endregion

        #region Constructors

        public TestDateTimeProvider()
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the current date and time in UTC.
        /// </summary>
        /// <value>The current date and time in UTC.</value>
        public DateTime UtcNow
        {
            get
            {
                return this.utcNow;
            }
            set
            {
                if( value.Kind == DateTimeKind.Unspecified )
                    throw new ArgumentException("Unspecified DateTimeKind is not supported!").Store("value", value);

                this.utcNow = value.ToUniversalTime();
            }
        }

        /// <summary>
        /// Suspends the current thread for the specified amount of time.
        /// </summary>
        /// <param name="timeout">The amount of time for which the thread is suspended. If the value of the <paramref name="timeout"/> argument is <see cref="TimeSpan.Zero"/>, the thread relinquishes the remainder of its time slice to any thread of equal priority that is ready to run. If there are no other threads of equal priority that are ready to run, execution of the current thread is not suspended.</param>
        public void Sleep( TimeSpan timeout )
        {
            //// NOTE: the Sleep implementations are based on what dotPeek shows

            // we can not depend on the second Sleep handling negative arguments, because of overflow
            long milliseconds = (long)timeout.TotalMilliseconds;
            if( milliseconds > (long)int.MaxValue
             || (milliseconds < 0 && milliseconds != Timeout.Infinite) )
                throw new ArgumentOutOfRangeException().Store("timeout", timeout);

            this.Sleep((int)milliseconds);
        }

        /// <summary>
        /// Suspends the current thread for the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds for which the thread is suspended. If the value of the <paramref name="millisecondsTimeout"/> argument is zero, the thread relinquishes the remainder of its time slice to any thread of equal priority that is ready to run. If there are no other threads of equal priority that are ready to run, execution of the current thread is not suspended.</param>
        public void Sleep( int millisecondsTimeout )
        {
            if( millisecondsTimeout < 0
             && millisecondsTimeout != Timeout.Infinite )
                throw new ArgumentOutOfRangeException().Store("millisecondsTimeout", millisecondsTimeout);

            this.UtcNow = this.UtcNow.AddMilliseconds(millisecondsTimeout);
        }
    }
}
