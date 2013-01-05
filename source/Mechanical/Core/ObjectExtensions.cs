using System;

namespace Mechanical.Core
{
    /// <content>
    /// Methods extending the <see cref="Object"/> type.
    /// </content>
    public static partial class CoreExtensions
    {
        //// NOTE: we don't want to "litter" intellisense with rarely used extension methods (especially on System.Object!), so think twice, before adding something here

        #region NullReference, NotNullReference

        /// <summary>
        /// Determines whether the object is <c>null</c>.
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns><c>true</c> if the specified object is <c>null</c>; otherwise, <c>false</c>.</returns>
        public static bool NullReference( this object value )
        {
            return object.ReferenceEquals(value, null);
        }

        /// <summary>
        /// Determines whether the object is not <c>null</c>.
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns><c>true</c> if the specified object is not <c>null</c>; otherwise, <c>false</c>.</returns>
        public static bool NotNullReference( this object value )
        {
            return !object.ReferenceEquals(value, null);
        }

        #endregion
    }
}
