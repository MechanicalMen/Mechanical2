using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// A data store reader which reads from an <see cref="IDataStoreNode"/>.
    /// </summary>
    public class DataStoreNodeReader : DataStoreReaderBase, ISeekableDataStoreReader
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

        private static readonly char[] PathSeparator = new char[] { DataStore.PathSeparator };

        private readonly BinaryArrayReaderLE binaryReader = new BinaryArrayReaderLE();
        private readonly StringReader textReader = new StringReader();
        private readonly IDataStoreNode rootNode;
        private readonly List<NodeInfo> parents = new List<NodeInfo>();
        private IDataStoreNode currentNode;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreNodeReader"/> class.
        /// </summary>
        /// <param name="rootNode">The node representing the data store root; or <c>null</c> for empty data stores.</param>
        public DataStoreNodeReader( IDataStoreNode rootNode )
            : base()
        {
            this.rootNode = rootNode;
        }

        #endregion

        #region Private Methods

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void PushObject()
        {
            var info = new NodeInfo((IDataStoreObject)this.currentNode);
            this.parents.Add(info);
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void PopObject()
        {
            if( this.parents.Count != 0 )
                this.parents.RemoveAt(this.parents.Count - 1);
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
            if( this.Token == DataStoreToken.DataStoreStart )
            {
                if( this.rootNode.NullReference() )
                {
                    this.currentNode = null;
                    name = null;
                    return DataStoreToken.DataStoreEnd;
                }
                else
                    this.currentNode = this.rootNode;
            }
            else
            {
                if( this.parents.Count == 0 )
                {
                    if( this.rootNode is IDataStoreObject )
                    {
                        this.currentNode = this.rootNode;
                        name = this.currentNode.Name;
                        this.PopObject();
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
                    //// not at root level

                    int infoAt = this.parents.Count - 1;
                    var info = this.parents[infoAt];
                    if( info.Index == info.Object.Nodes.Count )
                    {
                        this.currentNode = info.Object;
                        name = this.currentNode.Name;
                        this.PopObject();
                        return DataStoreToken.ObjectEnd;
                    }
                    else
                    {
                        this.currentNode = info.Object.Nodes[info.Index];
                        ++info.Index;
                        this.parents[infoAt] = info;
                    }
                }
            }

            // proces current node
            name = this.currentNode.Name;
            if( this.currentNode is IDataStoreObject )
            {
                this.PushObject();
                return DataStoreToken.ObjectStart;
            }
            else if( this.currentNode is IDataStoreTextValue )
                return DataStoreToken.TextValue;
            else if( this.currentNode is IDataStoreBinaryValue )
                return DataStoreToken.BinaryValue;
            else
                throw new Exception("Invalid data store node type!").Store("nodeType", this.currentNode.GetType());
        }

        /// <summary>
        /// Returns the reader of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The reader of the value.</returns>
        protected override IBinaryReader OpenBinaryReader()
        {
            var binaryValue = (IDataStoreBinaryValue)this.currentNode;
            this.binaryReader.Bytes = binaryValue.Content;
            return this.binaryReader;
        }

        /// <summary>
        /// Returns the reader of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The reader of the value.</returns>
        protected override ITextReader OpenTextReader()
        {
            var textValue = (IDataStoreTextValue)this.currentNode;
            this.textReader.Set(textValue.Content);
            return this.textReader;
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="reader">The reader of the value.</param>
        protected override void CloseReader( IBinaryReader reader )
        {
            this.binaryReader.Clear();
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="reader">The reader of the value.</param>
        protected override void CloseReader( ITextReader reader )
        {
            this.textReader.Set(Substring.Empty); // only necessary if reader was opened, but never used. Unlikely, but we don't know for sure.
        }

        #endregion

        #region ISeekableDataStoreReader

        /// <summary>
        /// Positions the reader at the start of the data store.
        /// </summary>
        public void SeekStart()
        {
            this.SetPosition(DataStoreToken.DataStoreStart, string.Empty);

            this.currentNode = null;
            this.parents.Clear();
        }

        /// <summary>
        /// Positions the reader at the end of the data store.
        /// </summary>
        public void SeekEnd()
        {
            this.SetPosition(DataStoreToken.DataStoreStart, string.Empty);

            this.currentNode = null;
            this.parents.Clear();
        }

        /// <summary>
        /// Seeks the specified data store path.
        /// Positions the reader at the specified ObjectStart or value token if the path is found, or at DataStoreEnd otherwise.
        /// Throws an exception, if the path does not exist.
        /// </summary>
        /// <param name="absolutePath">The absolute data store path to seek.</param>
        public void Seek( string absolutePath )
        {
            if( !this.TrySeek(absolutePath) )
                throw new KeyNotFoundException().Store("absolutePath", absolutePath);
        }

        /// <summary>
        /// Tries to seek the specified data store path.
        /// Positions the reader at the specified ObjectStart or value token if the path is found, or at DataStoreEnd otherwise.
        /// </summary>
        /// <param name="absolutePath">The absolute data store path to seek.</param>
        /// <returns><c>true</c> if the path was found; otherwise, <c>false</c>.</returns>
        public bool TrySeek( string absolutePath )
        {
            this.currentNode = null;
            this.parents.Clear();


            Substring remainingPath = absolutePath;
            Substring name;
            do
            {
                name = Substring.SplitFirst(ref remainingPath, PathSeparator, StringSplitOptions.None);
                if( !DataStore.IsValidName(name) )
                    goto error;

                if( this.currentNode.NullReference() )
                {
                    // first path segment
                    if( !DataStore.Comparer.Equals(this.rootNode.Name, name) )
                        goto error;

                    this.currentNode = this.rootNode;
                }
                else
                {
                    // in the middle of the path
                    var parent = this.currentNode as IDataStoreObject;
                    if( parent.NullReference() )
                        goto error;

                    bool found = false;
                    for( int i = 0; i < parent.Nodes.Count; ++i )
                    {
                        if( DataStore.Comparer.Equals(parent.Nodes[i].Name, name) )
                        {
                            this.PushObject();
                            this.currentNode = parent.Nodes[i];
                            found = true;
                            break;
                        }
                    }
                    if( !found )
                        goto error;
                }
            }
            while( !remainingPath.NullOrEmpty );


            // update base class
            DataStoreToken token;
            if( this.currentNode is IDataStoreObject )
                token = DataStoreToken.ObjectStart;
            else if( this.currentNode is IDataStoreTextValue )
                token = DataStoreToken.TextValue;
            else if( this.currentNode is IDataStoreBinaryValue )
                token = DataStoreToken.BinaryValue;
            else
                throw new Exception("Invalid data store node type!").Store("nodeType", this.currentNode.GetType());

            this.SetPosition(token, absolutePath);
            return true;

        error:
            this.SetPosition(DataStoreToken.DataStoreEnd, null);
            return false;
        }

        #endregion
    }
}
