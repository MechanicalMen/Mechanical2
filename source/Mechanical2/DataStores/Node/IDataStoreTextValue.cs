using System;
using Mechanical.Core;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// The interface of text-based data store values.
    /// </summary>
    public interface IDataStoreTextValue : IDataStoreValue
    {
        /// <summary>
        /// Gets or sets the content of the data store value.
        /// </summary>
        /// <value>The content of the data store value.</value>
        Substring Content { get; set; }
    }
}
