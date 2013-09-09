using System;
using System.Collections.Generic;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// The interface of data store object nodes.
    /// </summary>
    public interface IDataStoreObject : IDataStoreNode
    {
        /// <summary>
        /// Gets the nodes of this data store object.
        /// </summary>
        /// <value>The nodes of this date store object.</value>
        IList<IDataStoreNode> Nodes { get; }
    }
}
