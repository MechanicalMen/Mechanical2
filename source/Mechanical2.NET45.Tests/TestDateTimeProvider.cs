using System;
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
    }
}
