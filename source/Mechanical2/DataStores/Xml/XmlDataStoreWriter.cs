using System;
using System.IO;
using System.Text;
using System.Xml;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.Xml
{
    //// NOTE: There are no empty xml documents, only empty xml fragments!
    ////       Therefore we always have a root element, even for empty data stores.

    /// <summary>
    /// An XML based data store writer.
    /// </summary>
    public class XmlDataStoreWriter : DataStoreWriterBase.Disposable
    {
        #region StringWriterWithEncoding

        private class StringWriterWithEncoding : System.IO.StringWriter
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

        private XmlWriter xmlWriter;
        private Mechanical.IO.StringWriter textWriter;

        #endregion

        #region Constructors

        private XmlDataStoreWriter( XmlWriter xmlWriter )
        {
            Ensure.That(xmlWriter).NotNull();

            this.xmlWriter = xmlWriter;
            this.textWriter = new Mechanical.IO.StringWriter();
            this.xmlWriter.WriteStartElement(RootName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreWriter"/> class.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="indent">Determines whether to indent the xml elements.</param>
        public XmlDataStoreWriter( StringBuilder sb, bool indent = true )
            : this(XmlWriter.Create(new StringWriterWithEncoding(sb, DataStore.DefaultEncoding) { NewLine = DataStore.DefaultNewLine }, new XmlWriterSettings() { Encoding = DataStore.DefaultEncoding, NewLineChars = DataStore.DefaultNewLine, Indent = indent, CloseOutput = true }))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreWriter"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to take ownership of.</param>
        /// <param name="indent">Determines whether to indent the xml elements.</param>
        public XmlDataStoreWriter( Stream stream, bool indent = true )
            : this(XmlWriter.Create(new StreamWriter(stream, DataStore.DefaultEncoding) { NewLine = DataStore.DefaultNewLine }, new XmlWriterSettings() { Encoding = DataStore.DefaultEncoding, NewLineChars = DataStore.DefaultNewLine, Indent = indent, CloseOutput = true }))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The <see cref="ITextWriter"/> to take ownership of.</param>
        /// <param name="indent">Determines whether to indent the xml elements.</param>
        public XmlDataStoreWriter( ITextWriter textWriter, bool indent = true )
            : this(XmlWriter.Create(IOWrapper.Wrap(textWriter, DataStore.DefaultEncoding), new XmlWriterSettings() { Encoding = DataStore.DefaultEncoding, NewLineChars = DataStore.DefaultNewLine, Indent = indent, CloseOutput = true }))
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

        #region DataStoreWriterBase

        /// <summary>
        /// Writes an ObjectStart, ObjectEnd, or a value.
        /// </summary>
        /// <param name="name">The data store name to use; or <c>null</c> for the ObjectEnd token.</param>
        /// <param name="isObjectStart"><c>true</c> if an ObjectStart token is to be written, <c>false</c> for ObjectEnd, and <c>null</c> for a binary or text value.</param>
        /// <returns>A value determining whether the value to be written is binary or not. <c>null</c> if an object was written.</returns>
        protected override bool? Write( string name, bool? isObjectStart )
        {
            try
            {
                if( isObjectStart != false
                 && !DataStore.IsValidName(name) )
                    throw new ArgumentException("Invalid data store name!").StoreFileLine();

                bool? isBinary;
                if( isObjectStart.HasValue )
                {
                    if( isObjectStart.Value )
                        this.xmlWriter.WriteStartElement(name);
                    else
                        this.xmlWriter.WriteFullEndElement();

                    isBinary = null;
                }
                else
                {
                    this.xmlWriter.WriteStartElement(name);
                    isBinary = false; // value writing is finished in CloseWriter
                }

                return isBinary;
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("name", name);
                ex.Store("isObjectStart", isObjectStart);
                throw;
            }
        }

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected override IBinaryWriter OpenBinaryWriter()
        {
            throw new NotSupportedException().StoreFileLine();
        }

        /// <summary>
        /// Returns the writer of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The writer of the value.</returns>
        protected override ITextWriter OpenTextWriter()
        {
            return this.textWriter;
        }

        /// <summary>
        /// Releases any resources held by an open writer.
        /// </summary>
        /// <param name="writer">The writer of the value.</param>
        protected override void CloseWriter( IBinaryWriter writer )
        {
            throw new NotSupportedException().StoreFileLine();
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="writer">The writer of the value.</param>
        protected override void CloseWriter( ITextWriter writer )
        {
            var value = this.textWriter.ToString();
            this.textWriter.Clear();

            if( value.Length != 0 )
            {
                this.xmlWriter.WriteString(value);
                this.xmlWriter.WriteFullEndElement();
            }
            else
            {
                this.xmlWriter.WriteEndElement(); // not a full end element, that would be equivalent to an empty data store object
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Flushes the buffers of the underlying implementation(s).
        /// </summary>
        public void Flush()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            this.xmlWriter.Flush();
        }

        #endregion
    }
}
