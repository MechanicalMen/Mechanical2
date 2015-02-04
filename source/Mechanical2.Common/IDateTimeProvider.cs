using System;

namespace Mechanical.Common
{
    /// <summary>
    /// Provides date and time related functionality.
    /// </summary>
    public interface IDateTimeProvider
    {
        //// NOTE: this interface is mainly useful for unit testing
        ////       (where instead of the current time one can test for leap years, ... etc.)

        /// <summary>
        /// Gets the current date and time in UTC.
        /// </summary>
        /// <value>The current date and time in UTC.</value>
        DateTime UtcNow { get; }
    }
}
