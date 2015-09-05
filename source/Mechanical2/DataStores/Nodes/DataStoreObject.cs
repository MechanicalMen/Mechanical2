using System;
using System.Collections.Generic;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores.Nodes
{
    /// <summary>
    /// A data store object.
    /// </summary>
    public class DataStoreObject : DataStoreNode, IDataStoreObject
    {
        #region NodesCollection

        internal class NodesCollection : List.Wrapper<IDataStoreNode>
        {
            private void ThrowIfNameTaken( IDataStoreNode item )
            {
                Ensure.That(item).NotNull();
                Ensure.Debug(DataStore.IsValidName(item.Name), v => v.IsTrue(() => new ArgumentException("Invalid data store name!").Store("Name", item.Name)));

                foreach( var node in this )
                {
                    if( DataStore.Comparer.Equals(node.Name, item.Name) )
                        throw new ArgumentException("A child having the specified name has already been added!").Store("Name", item.Name);
                }
            }

            /// <summary>
            /// Called before an item is added to the wrapped list.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
            /// <param name="item">The item to add.</param>
            /// <returns><c>true</c> to indicate that the adding may continue; otherwise, <c>false</c> to silently cancel it.</returns>
            protected override bool OnAdding( int index, IDataStoreNode item )
            {
                this.ThrowIfNameTaken(item);
                return true;
            }

            /// <summary>
            /// Called before an existing item of the wrapped list is replaced with a new one.
            /// </summary>
            /// <param name="index">The zero-based index to assign a new item to.</param>
            /// <param name="oldItem">The old item being overwritten.</param>
            /// <param name="newItem">The new item being set.</param>
            /// <returns><c>true</c> to indicate that the updating may continue; otherwise, <c>false</c> to silently cancel it.</returns>
            protected override bool OnUpdating( int index, IDataStoreNode oldItem, IDataStoreNode newItem )
            {
                if( !DataStore.Comparer.Equals(oldItem.Name, newItem.Name) )
                    this.ThrowIfNameTaken(newItem);

                return true;
            }
        }

        #endregion

        #region Private Fields

        private readonly IList<IDataStoreNode> nodes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreObject"/> class.
        /// </summary>
        /// <param name="name">The name of the data store node.</param>
        /// <param name="nodes">The children of the object, or <c>null</c>.</param>
        public DataStoreObject( string name, IEnumerable<IDataStoreNode> nodes = null )
            : base(name)
        {
            this.nodes = new NodesCollection();

            if( nodes.NotNullReference() )
            {
                foreach( var n in nodes )
                    this.nodes.Add(n);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreObject"/> class.
        /// </summary>
        /// <param name="name">The name of the data store node.</param>
        /// <param name="nodes">The children of the object, or <c>null</c>.</param>
        public DataStoreObject( string name, params IDataStoreNode[] nodes )
            : this(name, (IEnumerable<IDataStoreNode>)nodes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreObject"/> class.
        /// </summary>
        /// <param name="name">The name of the data store node.</param>
        public DataStoreObject( string name )
            : this(name, (IEnumerable<IDataStoreNode>)null)
        {
        }

        #endregion

        #region IDataStoreObject

        /// <summary>
        /// Gets the nodes of this data store object.
        /// </summary>
        /// <value>The nodes of this date store object.</value>
        public IList<IDataStoreNode> Nodes
        {
            get { return this.nodes; }
        }

        #endregion
    }
}
