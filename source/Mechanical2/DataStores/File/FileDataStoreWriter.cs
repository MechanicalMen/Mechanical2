using System;
using System.IO;
using System.Text;
using System.Xml;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores.File
{
    //// NOTE: There are no empty xml documents, only empty xml fragments!
    ////       Therefore we always have a root element, even for empty data stores.

    /*/// <summary>
    /// An XML based data store writer.
    /// </summary>
    public class XmlDataStoreWriter : DisposableObject, IDataStoreWriter
    {
        #region StringWriterWithEncoding

        private class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            internal StringWriterWithEncoding( StringBuilder sb, Encoding encoding )
                : base(sb)
            {
                Ensure.That(encoding).NotNull();

                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return this.encoding; }
            }
        }

        #endregion

        #region Private Fields

        internal const string RootName = "root";

        private readonly bool hasRootObject;
        private XmlWriter xmlWriter;
        private Mechanical.IO.StringWriter textWriter;

        #endregion

        #region Constructors

        private XmlDataStoreWriter( XmlWriter xmlWriter, string writeRootObject )
        {
            Ensure.That(xmlWriter).NotNull();

            this.xmlWriter = xmlWriter;
            this.textWriter = new Mechanical.IO.StringWriter();
            this.xmlWriter.WriteStartElement(RootName);

            if( writeRootObject.NotNullReference() )
            {
                if( !DataStore.IsValidName(writeRootObject) )
                    throw new ArgumentException("Invalid data store name!").Store("writeRootObject", writeRootObject);

                this.xmlWriter.WriteStartElement(writeRootObject);
                this.hasRootObject = true;
            }
            else
                this.hasRootObject = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreWriter"/> class.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="indent">Determines whether to indent the xml elements.</param>
        /// <param name="writeRootObject">The name of the object, to use as the root of the data store, or <c>null</c> to have the root determined by a Write call.</param>
        public XmlDataStoreWriter( StringBuilder sb, bool indent = true, string writeRootObject = null )
            : this(XmlWriter.Create(new StringWriterWithEncoding(sb, DataStore.DefaultEncoding) { NewLine = DataStore.DefaultNewLine }, new XmlWriterSettings() { Encoding = DataStore.DefaultEncoding, NewLineChars = DataStore.DefaultNewLine, Indent = indent, CloseOutput = true }), writeRootObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreWriter"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to take ownership of.</param>
        /// <param name="indent">Determines whether to indent the xml elements.</param>
        /// <param name="writeRootObject">The name of the object, to use as the root of the data store, or <c>null</c> to have the root determined by a Write call.</param>
        public XmlDataStoreWriter( Stream stream, bool indent = true, string writeRootObject = null )
            : this(XmlWriter.Create(new StreamWriter(stream, DataStore.DefaultEncoding) { NewLine = DataStore.DefaultNewLine }, new XmlWriterSettings() { Encoding = DataStore.DefaultEncoding, NewLineChars = DataStore.DefaultNewLine, Indent = indent, CloseOutput = true }), writeRootObject)
        {
        }

        #endregion

        #region IDisposableObject

        /// <summary>
        /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
        protected override void OnDispose( bool disposing )
        {
            if( disposing )
            {
                //// dispose-only (i.e. non-finalizable) logic
                //// (managed, disposable resources you own)

                if( this.xmlWriter.NotNullReference() )
                {
                    // close root data store object, if one was specified
                    // in the constructor
                    if( this.hasRootObject )
                        this.xmlWriter.WriteFullEndElement();

                    this.xmlWriter.WriteFullEndElement(); // closing root xml element

                    this.xmlWriter.Flush();
                    this.xmlWriter.Close();
                    this.xmlWriter = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)

            this.textWriter = null;


            base.OnDispose(disposing);
        }

        #endregion

        #region IDataStoreWriter

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The serializer to use.</param>
        public void Write<T>( string name, T obj, IDataStoreValueSerializer<T> serializer )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).Store("name", name).Store("obj", "obj").Store("serializer", serializer);

            if( !DataStore.IsValidName(name) )
                throw new ArgumentException("Invalid data store name!").Store("name", name).Store("obj", "obj").Store("serializer", serializer);

            if( serializer.NullReference() )
                throw new ArgumentNullException("serializer").Store("name", name).Store("obj", "obj").Store("serializer", serializer);

            // serialize
            serializer.Serialize(obj, this.textWriter);
            var value = this.textWriter.ToString();
            this.textWriter.Clear();

            // write node
            if( value.Length != 0 )
            {
                this.xmlWriter.WriteElementString(name, value);
            }
            else
            {
                this.xmlWriter.WriteStartElement(name);
                this.xmlWriter.WriteEndElement(); // not a full end element, that would be an empty object
            }
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The serializer to use.</param>
        public void Write<T>( string name, T obj, IDataStoreObjectSerializer<T> serializer )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).Store("name", name).Store("obj", "obj").Store("serializer", serializer);

            if( !DataStore.IsValidName(name) )
                throw new ArgumentException("Invalid data store name!").Store("name", name).Store("obj", "obj").Store("serializer", serializer);

            if( serializer.NullReference() )
                throw new ArgumentNullException("serializer").Store("name", name).Store("obj", "obj").Store("serializer", serializer);

            // write start element
            this.xmlWriter.WriteStartElement(name);

            // serialize
            serializer.Serialize(obj, this);

            // close the node
            this.xmlWriter.WriteFullEndElement();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Flushes the buffers of the underlying implementation(s).
        /// </summary>
        public void Flush()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreDefault();

            this.xmlWriter.Flush();
        }

        #endregion
    }*/
}
