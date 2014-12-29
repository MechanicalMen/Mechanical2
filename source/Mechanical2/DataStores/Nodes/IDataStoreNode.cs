using System;

namespace Mechanical.DataStores.Nodes
{
    /// <summary>
    /// The interface of data store nodes.
    /// </summary>
    public interface IDataStoreNode : IEquatable<IDataStoreNode>
    {
        /// <summary>
        /// Gets the name of the data store node.
        /// </summary>
        /// <value>The name of the data store node.</value>
        string Name { get; }
    }
}
