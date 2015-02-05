using System;

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
    }
}
