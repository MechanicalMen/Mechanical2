using System;
using System.Collections.Generic;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// A data store object.
    /// </summary>
    public class DataStoreObject : DataStoreNode, IDataStoreObject
    {
        #region NodesCollection

        private class NodesCollection : List.Wrapper<IDataStoreNode>
        {
            /// <summary>
            /// Inserts an item to the <see cref="IList{T}"/> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
            /// <param name="item">The object to insert into the <see cref="IList{T}"/>.</param>
            public override void Insert( int index, IDataStoreNode item )
            {
                Ensure.That(item).NotNull();
                Ensure.Debug(DataStore.IsValidName(item.Name), v => v.IsTrue(() => new ArgumentException("Invalid data store name!").Store("Name", item.Name)));

                foreach( var node in this )
                {
                    if( DataStore.SameNames(node.Name, item.Name) )
                        throw new ArgumentException("A child having the specified name has already been added!").Store("Name", item.Name);
                }

                base.Insert(index, item);
            }

            /// <summary>
            /// Gets or sets the element at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the element to get or set.</param>
            /// <returns>The element at the specified index.</returns>
            public override IDataStoreNode this[int index]
            {
                get
                {
                    return this.Items[index];
                }
                set
                {
                    Ensure.That(index).InRange(0, this.Count);

                    this.Insert(index + 1, value);
                    this.RemoveAt(index);
                }
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
