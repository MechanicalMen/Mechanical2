using System;
using System.IO;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.FileFormats;
using Mechanical.IO;

namespace Mechanical.DataStores.Json
{
    //// NOTE: Empty data stores, or data stores with a single value as root are valid data store concepts
    ////       (the latter because every subsection of a data store, should be a valid data store in itself).
    ////       However, these don't have valid JSON equivalents, therefore we wrap our JSON content in a "root" object.

    /// <summary>
    /// A JSON based data store writer.
    /// </summary>
    public class JsonDataStoreWriter : DataStoreWriterBase.Disposable
    {
        #region Private Fields

        private JsonWriter jsonWriter;
        private Mechanical.IO.StringWriter textWriter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataStoreWriter"/> class.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to use.</param>
        public JsonDataStoreWriter( JsonWriter jsonWriter )
        {
            Ensure.That(jsonWriter).NotNull();

            this.jsonWriter = jsonWriter;
            this.textWriter = new Mechanical.IO.StringWriter();
            this.jsonWriter.WriteObjectStart();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataStoreWriter"/> class.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="indent">Determines whether to indent the JSON elements.</param>
        public JsonDataStoreWriter( StringBuilder sb, bool indent = true )
            : this(new JsonWriter(new Mechanical.IO.StringWriter(sb), indent: indent))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataStoreWriter"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to take ownership of.</param>
        /// <param name="indent">Determines whether to indent the JSON elements.</param>
        public JsonDataStoreWriter( Stream stream, bool indent = true )
            : this(new JsonWriter(IOWrapper.ToTextWriter(stream, DataStore.DefaultEncoding, DataStore.DefaultNewLine), indent: indent))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataStoreWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The <see cref="ITextWriter"/> to take ownership of.</param>
        /// <param name="indent">Determines whether to indent the JSON elements.</param>
        public JsonDataStoreWriter( ITextWriter textWriter, bool indent = true )
            : this(new JsonWriter(textWriter, indent: indent))
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

                if( this.jsonWriter.NotNullReference() )
                {
                    this.jsonWriter.WriteObjectEnd(); // closing root json object
                    this.jsonWriter.Dispose();
                    this.jsonWriter = null;
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
                bool? isBinary;
                if( isObjectStart != false )
                {
                    if( !DataStore.IsValidName(name) )
                        throw new ArgumentException("Invalid data store name!").StoreFileLine();

                    this.jsonWriter.WriteName(name);

                    if( isObjectStart.HasValue )
                    {
                        // isObjectStart == true
                        this.jsonWriter.WriteObjectStart();
                        isBinary = null;
                    }
                    else
                    {
                        // isObjectStart == null
                        // other than writing the name, there is nothing else to do for values, at this point
                        isBinary = false;
                    }
                }
                else
                {
                    // isObjectStart == false
                    this.jsonWriter.WriteObjectEnd();
                    isBinary = null;
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

            this.jsonWriter.WriteUnknownValue(value);
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

            this.jsonWriter.Flush();
        }

        #endregion
    }
}
