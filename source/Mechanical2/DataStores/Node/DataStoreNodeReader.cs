using System;
using System.Collections.Generic;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// A data store reader which reads from an <see cref="IDataStoreNode"/>.
    /// </summary>
    public class DataStoreNodeReader : DataStoreReaderBase
    {
        #region Private Fields

        private struct NodeInfo
        {
            internal readonly IDataStoreObject Object;
            internal int Index;

            internal NodeInfo( IDataStoreObject obj )
            {
                this.Object = obj;
                this.Index = 0;
            }
        }

        private readonly BinaryArrayReaderLE binaryReader = new BinaryArrayReaderLE();
        private readonly StringReader textReader = new StringReader();
        private readonly IDataStoreNode rootNode;
        private readonly List<NodeInfo> parents = new List<NodeInfo>();
        private bool rootStartRead = false;
        private bool rootEndRead = false;
        private IDataStoreNode currentNode;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreNodeReader"/> class.
        /// </summary>
        /// <param name="rootNode">The node representing the data store root; or <c>null</c> for empty data stores.</param>
        public DataStoreNodeReader( IDataStoreNode rootNode )
        {
            this.rootNode = rootNode;
            this.Initialize();
        }

        #endregion

        #region DataStoreReaderBase

        /// <summary>
        /// Moves the reader to the next <see cref="DataStoreToken"/>.
        /// </summary>
        /// <param name="name">The name of the data store node found; or <c>null</c>.</param>
        /// <returns>The <see cref="DataStoreToken"/> found.</returns>
        protected override DataStoreToken ReadToken( out string name )
        {
            if( this.parents.Count == 0 )
            {
                //// we are at root level

                if( this.rootNode.NullReference()
                 || this.rootEndRead )
                {
                    this.currentNode = null;
                    name = null;
                    return DataStoreToken.DataStoreEnd;
                }
                else if( this.rootStartRead )
                {
                    this.rootEndRead = true;
                    if( this.rootNode is IDataStoreObject )
                    {
                        this.currentNode = this.rootNode;
                        name = this.currentNode.Name;
                        return DataStoreToken.ObjectEnd;
                    }
                    else
                    {
                        this.currentNode = null;
                        name = null;
                        return DataStoreToken.DataStoreEnd;
                    }
                }
                else
                {
                    this.currentNode = this.rootNode;
                    this.rootStartRead = true;
                }
            }
            else
            {
                //// we may or may not be at root level

                int infoAt = this.parents.Count - 1;
                var info = this.parents[infoAt];
                if( info.Index == info.Object.Nodes.Count )
                {
                    this.currentNode = info.Object;
                    name = this.currentNode.Name;
                    return DataStoreToken.ObjectEnd;
                }
                else
                {
                    this.currentNode = info.Object.Nodes[info.Index];
                    ++info.Index;
                    this.parents[infoAt] = info;
                }
            }

            name = this.currentNode.Name;
            if( this.currentNode is IDataStoreObject )
                return DataStoreToken.ObjectStart;
            else
                return DataStoreToken.Value;
        }

        /// <summary>
        /// Reads the data store value the reader is currently at.
        /// </summary>
        /// <param name="textReader">The <see cref="ITextReader"/> to use; or <c>null</c>.</param>
        /// <param name="binaryReader">The <see cref="IBinaryReader"/> to use; or <c>null</c>.</param>
        protected override void ReadValue( out ITextReader textReader, out IBinaryReader binaryReader )
        {
            var textValue = this.currentNode as IDataStoreTextValue;
            if( textValue.NotNullReference() )
            {
                this.textReader.Set(textValue.Content);
                textReader = this.textReader;
                binaryReader = null;
            }
            else
            {
                var binaryValue = this.currentNode as IDataStoreBinaryValue;
                if( binaryValue.NullReference() )
                {
                    var exception = new Exception("Invalid data store node!").Store("nodeType", this.currentNode.GetType());
                    this.StoreReaderInfo(exception);
                    throw exception;
                }

                this.binaryReader.Bytes = binaryValue.Content;
                textReader = null;
                binaryReader = this.binaryReader;
            }
        }

        /// <summary>
        /// Begins reading the data store object the reader is currently at.
        /// </summary>
        protected override void ReadObjectStart()
        {
            var info = new NodeInfo((IDataStoreObject)this.currentNode);
            this.parents.Add(info);
        }

        /// <summary>
        /// Ends reading the data store object the reader is currently at.
        /// </summary>
        protected override void ReadObjectEnd()
        {
            if( this.parents.Count != 0 )
                this.parents.RemoveAt(this.parents.Count - 1);
        }

        #endregion
    }
}
