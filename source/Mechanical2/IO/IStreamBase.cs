using System;

namespace Mechanical.IO
{
    /// <summary>
    /// An abstract stream. It is not known whether it is readable, writable or both.
    /// Neither are acceptable data types made explicit.
    /// </summary>
    public interface IStreamBase
    {
        /// <summary>
        /// Closes the abstract stream.
        /// Calling it implies that this instances will not be used anymore.
        /// </summary>
        void Close();
    }
}
