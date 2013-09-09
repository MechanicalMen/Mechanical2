using System;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// The interface of binary-based data store values.
    /// </summary>
    public interface IDataStoreBinaryValue : IDataStoreValue
    {
        /// <summary>
        /// Gets or sets the content of the data store value.
        /// </summary>
        /// <value>The content of the data store value.</value>
        ArraySegment<byte> Content { get; set; }
    }
}
