using System;

namespace Mechanical.Common
{
    internal class DateTimeProvider : IDateTimeProvider
    {
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
