using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;
using Mechanical.DataStores.Node;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// A file system that stores it's contents as data store nodes.
    /// </summary>
    public class DataStoreObjectFileSystem : IFileSystem
    {
        #region DataStoreTextValueWriter

        private class DataStoreTextValueWriter : Mechanical.IO.StringWriter, IDisposableObject
        {
            #region Private Fields

            private readonly IDataStoreTextValue textValue;

            #endregion

            #region Constructors

            internal DataStoreTextValueWriter( IDataStoreTextValue value )
                : base()
            {
                Ensure.Debug(value, v => v.NotNull());

                this.textValue = value;
            }

            #endregion

            #region IDisposableObject

            private readonly object disposeLock = new object();
            private bool isDisposed = false;

            /// <summary>
            /// Gets a value indicating whether this object has been disposed of.
            /// </summary>
            /// <value><c>true</c> if this object has been disposed of; otherwise, <c>false</c>.</value>
            public bool IsDisposed
            {
                get { return this.isDisposed; }
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="DataStoreTextValueWriter"/> class.
            /// </summary>
            /// <remarks>
            /// Releases unmanaged resources and performs other cleanup operations before the
            /// object is reclaimed by garbage collection.
            /// </remarks>
            ~DataStoreTextValueWriter()
            {
                // call Dispose with false. Since we're in the destructor call,
                // the managed resources will be disposed of anyways.
                this.Dispose(false);
            }

            /// <summary>
            /// Performs tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // dispose of the managed and unmanaged resources
                this.Dispose(true);

                // tell the GC that Finalize no longer needs
                // to be run for this object.
                System.GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Called before the object is disposed of. Inheritors must call base.OnDisposing to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, the method was called by Dispose; otherwise by the destructor.</param>
            protected virtual void OnDisposing( bool disposing )
            {
            }

            /// <summary>
            /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            protected virtual void OnDispose( bool disposing )
            {
                if( disposing )
                {
                    //// dispose-only (i.e. non-finalizable) logic
                    //// (managed, disposable resources you own)

                    this.textValue.Content = this.ToString();
                }

                //// shared cleanup logic
                //// (unmanaged resources)
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
            private void Dispose( bool disposing )
            {
                // don't lock unless we have to
                if( !this.isDisposed )
                {
                    // not only do we want only one Dispose to run at a time,
                    // we also want to halt other callers until Dispose finishes.
                    lock( this.disposeLock )
                    {
                        // necessary if there are multiple concurrent calls
                        if( !this.isDisposed )
                        {
                            this.OnDisposing(disposing);
                            this.OnDispose(disposing);
                            this.isDisposed = true; // if an exception interrupts the process, we may not have been properly disposed of! (and isDisposed correctly stores false).
                        }
                    }
                }
            }

            #endregion
        }

        #endregion

        #region DataStoreBinaryValueWriter

        private class DataStoreBinaryValueWriter : IOWrapper.StreamAsIBinaryWriter
        {
            #region Private Fields

            private IDataStoreBinaryValue binaryValue;

            #endregion

            #region Constructors

            internal DataStoreBinaryValueWriter( IDataStoreBinaryValue value )
                : base(new MemoryStream())
            {
                Ensure.Debug(value, v => v.NotNull());

                this.binaryValue = value;
            }

            #endregion

            #region IDisposableObject

            /// <summary>
            /// Called before the object is disposed of. Inheritors must call base.OnDisposing to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, the method was called by Dispose; otherwise by the destructor.</param>
            protected override void OnDisposing( bool disposing )
            {
                if( this.binaryValue.NotNullReference() )
                {
                    this.binaryValue.Content = new ArraySegment<byte>(((MemoryStream)this.BaseStream).ToArray());
                    this.binaryValue = null;
                }

                base.OnDisposing(disposing);
            }

            #endregion
        }

        #endregion

        #region DataStoreBinaryValueStream

        private class DataStoreBinaryValueStream : IOWrapper.StreamAsIBinaryStream
        {
            #region Private Fields

            private IDataStoreBinaryValue binaryValue;

            #endregion

            #region Constructors

            internal DataStoreBinaryValueStream( IDataStoreBinaryValue value )
                : base(new MemoryStream())
            {
                Ensure.Debug(value, v => v.NotNull());

                this.binaryValue = value;

                if( this.binaryValue.Content.Count != 0 )
                {
                    this.BaseStream.Write(this.binaryValue.Content.Array, this.binaryValue.Content.Offset, this.binaryValue.Content.Count);
                    this.BaseStream.Position = 0;
                }
            }

            #endregion

            #region IDisposableObject

            /// <summary>
            /// Called before the object is disposed of. Inheritors must call base.OnDisposing to be properly disposed.
            /// </summary>
            /// <param name="disposing">If set to <c>true</c>, the method was called by Dispose; otherwise by the destructor.</param>
            protected override void OnDisposing( bool disposing )
            {
                if( this.binaryValue.NotNullReference() )
                {
                    this.binaryValue.Content = new ArraySegment<byte>(((MemoryStream)this.BaseStream).ToArray());
                    this.binaryValue = null;
                }

                base.OnDisposing(disposing);
            }

            #endregion
        }

        #endregion


        #region Private Fields

        private readonly DataStoreObject.NodesCollection rootNodes;
        private readonly bool escapeFileNames;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreObjectFileSystem"/> class
        /// </summary>
        /// <param name="escapeFileNames">Indicates whether the original file names should be escaped. Sets the appropriate property, but otherwise ignored.</param>
        public DataStoreObjectFileSystem( bool escapeFileNames = false )
        {
            this.rootNodes = new DataStoreObject.NodesCollection();
            this.escapeFileNames = escapeFileNames;
        }

        #endregion

        #region Private Methods

        private IDataStoreNode GetNodeAt( Substring dataStorePath )
        {
            if( dataStorePath.NullOrEmpty )
                return null;

            Substring remainingPath = dataStorePath;
            Substring name;
            IDataStoreNode currentNode = null;
            IList<IDataStoreNode> nodes;
            do
            {
                // get the next name from the path
                name = Substring.SplitFirst(ref remainingPath, DataStore.PathSeparatorArray, StringSplitOptions.None);
                if( !DataStore.IsValidName(name) )
                    return null;

                // search among root nodes, or child nodes
                if( currentNode.NullReference() )
                {
                    nodes = this.rootNodes;
                }
                else
                {
                    // searching for a subdirectory inside a file?
                    var asObjectNode = currentNode as IDataStoreObject;
                    if( asObjectNode.NullReference() )
                        return null;
                    else
                        nodes = asObjectNode.Nodes;
                }

                // search for the name among the nodes
                currentNode = null;
                for( int i = 0; i < nodes.Count; ++i )
                {
                    if( DataStore.Comparer.Equals(name, nodes[i].Name) )
                    {
                        currentNode = nodes[i];
                        break;
                    }
                }

                // name not found: exit; otherise, continue
                if( currentNode.NullReference() )
                    return null;
            }
            while( !remainingPath.NullOrEmpty );

            return currentNode;
        }

        private string[] GetEntries( string dataStorePath, bool getFiles )
        {
            try
            {
                if( dataStorePath.NullOrEmpty() )
                    return this.rootNodes.Select(n => n.Name).ToArray();

                var node = this.GetNodeAt(dataStorePath);
                if( node.NullReference() )
                    throw new FileNotFoundException("File system entry not found!").StoreFileLine();

                var objectNode = node as IDataStoreObject;
                if( objectNode.NullReference() )
                    throw new FileNotFoundException("File system entry is not a directory!").StoreFileLine();

                var results = new List<string>();
                for( int i = 0; i < objectNode.Nodes.Count; ++i )
                {
                    if( (getFiles && (objectNode.Nodes[i] is IDataStoreValue))
                     || (!getFiles && (objectNode.Nodes[i] is IDataStoreObject)) )
                        results.Add(objectNode.Nodes[i].Name);
                }

                return results.ToArray();
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        private IDataStoreObject CreateDirectoryRecursively( string dataStorePath )
        {
            var node = this.GetNodeAt(dataStorePath);
            if( node.NotNullReference() )
            {
                var objectNode = node as IDataStoreObject;
                if( objectNode.NullReference() )
                    throw new FileNotFoundException("File system entry already exists, but isn't a directory!").Store("filePath", dataStorePath);
                else
                    return objectNode;
            }

            IList<IDataStoreNode> parentNodes;
            var parentPath = DataStore.GetParentPath(dataStorePath);
            if( parentPath.NullOrEmpty() )
                parentNodes = this.rootNodes;
            else
                parentNodes = this.CreateDirectoryRecursively(parentPath).Nodes;

            // we know the parent does not have a child with the same name
            var newObject = new DataStoreObject(DataStore.GetNodeName(dataStorePath));
            parentNodes.Add(newObject);
            return newObject;
        }

        private void DeleteNode( string dataStorePath, bool expectValue )
        {
            if( dataStorePath.NullOrEmpty()
             || !DataStore.IsValidPath(dataStorePath) )
                throw new ArgumentException("Invalid data store path!").StoreFileLine();

            var parentPath = DataStore.GetParentPath(dataStorePath);
            IList<IDataStoreNode> parentNodes;
            if( parentPath.NullOrEmpty() )
            {
                parentNodes = this.rootNodes;
            }
            else
            {
                var parent = this.GetNodeAt(parentPath) as IDataStoreObject;
                if( parent.NullReference() )
                    return;
                else
                    parentNodes = parent.Nodes;
            }

            var name = DataStore.GetNodeName(dataStorePath);
            for( int i = 0; i < parentNodes.Count; ++i )
            {
                if( DataStore.Comparer.Equals(name, parentNodes[i].Name) )
                {
                    if( (expectValue && parentNodes[i] is IDataStoreValue)
                     || (!expectValue && parentNodes[i] is IDataStoreObject) )
                        parentNodes.RemoveAt(i);
                    return;
                }
            }
        }

        private IList<IDataStoreNode> GetOrCreateParentNodes( string dataStorePath )
        {
            var parentPath = DataStore.GetParentPath(dataStorePath);
            if( !parentPath.NullOrEmpty() )
                return this.CreateDirectoryRecursively(dataStorePath).Nodes;
            else
                return this.rootNodes;
        }

        private void SetValueForExistingParent( IList<IDataStoreNode> parentNodes, IDataStoreValue value )
        {
            // remove previous value with the same name, if it exists
            for( int i = 0; i < parentNodes.Count; ++i )
            {
                if( DataStore.Equals(value.Name, parentNodes[i].Name) )
                {
                    if( parentNodes[i] is IDataStoreValue )
                        parentNodes.RemoveAt(i);
                    else
                        throw new FileNotFoundException("File system entry already exists, but is a directory!").StoreFileLine();
                    break;
                }
            }

            // add the value
            // there are no name conflicts at this point
            parentNodes.Add(value);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the root data store nodes of the file system.
        /// </summary>
        /// <value>The root data store nodes of the file system.</value>
        public IList<IDataStoreNode> Nodes
        {
            get { return this.rootNodes; }
        }

        #endregion

        #region IFileSystemBase

        //// NOTE: We could actually support host paths (since they are just data store paths),
        ////       but we don't want them to be mixed up with actual host paths accidentally.

        /// <summary>
        /// Gets a value indicating whether the names of files and directories are escaped.
        /// If <c>false</c>, the data store path maps directly to the file path; otherwise escaping needs to be used, both by the implementation, as well as the calling code.
        /// Setting it to <c>true</c> is the only way to influence file names, but then even valid data store names may need to be escaped (underscores!).
        /// </summary>
        /// <value>Indicates whether the names of files and directories are escaped.</value>
        public bool EscapesNames
        {
            get { return this.escapeFileNames; }
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostFilePath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostFilePath
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The host file path.</returns>
        public string ToHostFilePath( string dataStorePath )
        {
            throw new NotSupportedException().StoreFileLine();
        }


        /// <summary>
        /// Gets a value indicating whether the ToHostDirectoryPath method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsToHostDirectoryPath
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the string the underlying system uses to represent the specified directory.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory.</param>
        /// <returns>The host directory path.</returns>
        public string ToHostDirectoryPath( string dataStorePath )
        {
            throw new NotSupportedException().StoreFileLine();
        }

        #endregion

        #region IFileSystemReader

        /// <summary>
        /// Gets the names of the files found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for files.</param>
        /// <returns>The names of the files found.</returns>
        public string[] GetFileNames( string dataStorePath = "" )
        {
            return this.GetEntries(dataStorePath, getFiles: true);
        }

        /// <summary>
        /// Gets the names of the directories found directly in the specified directory.
        /// Subdirectories are not searched.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to search for directories.</param>
        /// <returns>The names of the directories found.</returns>
        public string[] GetDirectoryNames( string dataStorePath = "" )
        {
            return this.GetEntries(dataStorePath, getFiles: false);
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextReader"/> representing the file opened.</returns>
        public ITextReader ReadText( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var node = this.GetNodeAt(dataStorePath);
                if( node.NullReference() )
                    throw new FileNotFoundException("File system entry not found!").StoreFileLine();
                if( !(node is IDataStoreValue) )
                    throw new FileNotFoundException("File system entry is not a file!").StoreFileLine();

                var textValue = node as IDataStoreTextValue;
                if( textValue.NotNullReference() )
                {
                    var reader = new StringReader();
                    reader.Set(textValue.Content);
                    return reader;
                }

                var binaryValue = node as IDataStoreBinaryValue;
                if( binaryValue.NotNullReference() )
                {
                    var ms = new MemoryStream(capacity: binaryValue.Content.Count);
                    ms.Write(binaryValue.Content.Array, binaryValue.Content.Offset, binaryValue.Content.Count);
                    ms.Position = 0;
                    return IOWrapper.ToTextReader(ms);
                }

                throw new Exception("Data store node value is neither text, nor binary based!").Store("nodeType", node.GetType());
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryReader"/> representing the file opened.</returns>
        public IBinaryReader ReadBinary( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var node = this.GetNodeAt(dataStorePath);
                if( node.NullReference() )
                    throw new FileNotFoundException("File system entry not found!").StoreFileLine();
                if( !(node is IDataStoreValue) )
                    throw new FileNotFoundException("File system entry is not a file!").StoreFileLine();

                var binaryValue = node as IDataStoreBinaryValue;
                if( binaryValue.NotNullReference() )
                {
                    return new BinaryArrayReaderLE(binaryValue.Content);
                }

                var textValue = node as IDataStoreTextValue;
                if( textValue.NotNullReference() )
                {
                    var str = textValue.Content.ToString();
                    var ms = new MemoryStream(DataStore.DefaultEncoding.GetBytes(str));
                    ms.Position = 0;
                    return IOWrapper.ToBinaryReader(ms);
                }

                throw new Exception("Data store node value is neither text, nor binary based!").Store("nodeType", node.GetType());
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }


        /// <summary>
        /// Gets a value indicating whether the GetFileSize method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsGetFileSize
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file.</param>
        /// <returns>The size of the specified file in bytes.</returns>
        public long GetFileSize( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                var node = this.GetNodeAt(dataStorePath);
                if( node.NullReference() )
                    throw new FileNotFoundException("File system entry not found!").StoreFileLine();
                if( !(node is IDataStoreValue) )
                    throw new FileNotFoundException("File system entry is not a file!").StoreFileLine();

                var binaryValue = node as IDataStoreBinaryValue;
                if( binaryValue.NotNullReference() )
                    return binaryValue.Content.Count;

                var textValue = node as IDataStoreTextValue;
                if( textValue.NotNullReference() )
                    return DataStore.DefaultEncoding.GetByteCount(textValue.Content.ToString());

                throw new Exception("Data store node value is neither text, nor binary based!").Store("nodeType", node.GetType());
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        #endregion

        #region IFileSystemWriter

        /// <summary>
        /// Creates the specified directory (and any directories along the path) should it not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to create.</param>
        public void CreateDirectory( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                this.CreateDirectoryRecursively(dataStorePath);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Deletes the specified file. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to delete.</param>
        public void DeleteFile( string dataStorePath )
        {
            try
            {
                this.DeleteNode(dataStorePath, expectValue: true);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Deletes the specified directory. Does nothing if it does not exist.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the directory to delete.</param>
        public void DeleteDirectory( string dataStorePath )
        {
            try
            {
                this.DeleteNode(dataStorePath, expectValue: false);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Always creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="ITextWriter"/> representing the file opened.</returns>
        public ITextWriter CreateNewText( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                // create parent
                var parentNodes = this.GetOrCreateParentNodes(dataStorePath);

                // get or create value
                var textValue = this.GetNodeAt(dataStorePath) as IDataStoreTextValue;
                if( textValue.NullReference() )
                {
                    textValue = new DataStoreTextValue(DataStore.GetNodeName(dataStorePath), Substring.Empty);
                    this.SetValueForExistingParent(parentNodes, textValue);
                }

                return new DataStoreTextValueWriter(textValue);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        /// <summary>
        /// Always creates a new empty file, and opens it for writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateNewBinary( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                // create parent
                var parentNodes = this.GetOrCreateParentNodes(dataStorePath);

                // get or create value
                var binaryValue = this.GetNodeAt(dataStorePath) as IDataStoreBinaryValue;
                if( binaryValue.NullReference() )
                {
                    binaryValue = new DataStoreBinaryValue(DataStore.GetNodeName(dataStorePath));
                    this.SetValueForExistingParent(parentNodes, binaryValue);
                }

                return new DataStoreBinaryValueWriter(binaryValue);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }


        /// <summary>
        /// Gets a value indicating whether the CreateWriteThroughBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsCreateWriteThroughBinary
        {
            get { return false; }
        }

        /// <summary>
        /// Always creates a new empty file, and opens it for writing.
        /// No intermediate buffers are kept: all operations access the file directly.
        /// This hurts performance, but is important for log files (less is lost in case of a crash).
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryWriter"/> representing the file opened.</returns>
        public IBinaryWriter CreateWriteThroughBinary( string dataStorePath )
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IFileSystem

        /// <summary>
        /// Gets a value indicating whether the ReadWriteBinary method is supported.
        /// </summary>
        /// <value><c>true</c> if the method is supported; otherwise, <c>false</c>.</value>
        public bool SupportsReadWriteBinary
        {
            get { return true; }
        }

        /// <summary>
        /// Opens an existing file, or creates a new one, for both reading and writing.
        /// </summary>
        /// <param name="dataStorePath">The data store path specifying the file to open.</param>
        /// <returns>An <see cref="IBinaryStream"/> representing the file opened.</returns>
        public IBinaryStream ReadWriteBinary( string dataStorePath )
        {
            try
            {
                if( dataStorePath.NullOrEmpty()
                 || !DataStore.IsValidPath(dataStorePath) )
                    throw new ArgumentException("Invalid data store path!").StoreFileLine();

                // create parent
                var parentNodes = this.GetOrCreateParentNodes(dataStorePath);

                // get node
                var nodeName = DataStore.GetNodeName(dataStorePath);
                var node = parentNodes.FirstOrDefault(n => DataStore.Comparer.Equals(n.Name, nodeName));
                if( node.NotNullReference() && !(node is IDataStoreValue) )
                    throw new FileNotFoundException("File system entry is not a file!").StoreFileLine();

                // convert text to binary node
                var textValue = node as IDataStoreTextValue;
                if( textValue.NotNullReference() )
                {
                    var str = textValue.Content.ToString();
                    var bytes = DataStore.DefaultEncoding.GetBytes(str);

                    var value = new DataStoreBinaryValue(DataStore.GetNodeName(dataStorePath), bytes);
                    this.SetValueForExistingParent(parentNodes, value);
                    node = value;
                }

                var binaryValue = node as IDataStoreBinaryValue;
                if( binaryValue.NullReference() )
                {
                    binaryValue = new DataStoreBinaryValue(DataStore.GetNodeName(dataStorePath));
                    this.SetValueForExistingParent(parentNodes, binaryValue);
                }

                return new DataStoreBinaryValueStream(binaryValue);
            }
            catch( Exception ex )
            {
                ex.Store("dataStorePath", dataStorePath);
                throw;
            }
        }

        #endregion
    }
}
