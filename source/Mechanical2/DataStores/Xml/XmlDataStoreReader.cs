using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores.Xml
{
    //// NOTE: There are no empty xml documents, only empty xml fragments!
    ////       Therefore we always have a root element, even for empty data stores.

    /// <summary>
    /// An XML based data store reader.
    /// </summary>
    public class XmlDataStoreReader : DataStoreReaderBase.Disposable, IDataStoreReader
    {
        #region Private Fields

        private XmlReader xmlReader;
        private Mechanical.IO.StringReader textReader;

        #endregion

        #region Constructors

        private XmlDataStoreReader( XmlReader xmlReader )
        {
            Ensure.That(xmlReader).NotNull();

            this.xmlReader = xmlReader;
            this.textReader = new IO.StringReader();

            // jump to root
            if( !this.MoveToNextStartElement() )
                throw this.CreateUnexpectedEndException();

            // verify root
            if( !string.Equals(this.xmlReader.Name, XmlDataStoreWriter.RootName, StringComparison.Ordinal) )
                throw this.CreateInvalidRootException();

            // jump past root
            if( !this.MoveToNextStartOrEndElement() )
                throw this.CreateUnexpectedEndException();

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreReader"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to take ownership of.</param>
        public XmlDataStoreReader( Stream stream )
            : this(XmlReader.Create(stream))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to take ownership of.</param>
        public XmlDataStoreReader( TextReader textReader )
            : this(XmlReader.Create(textReader))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreReader"/> class.
        /// </summary>
        /// <param name="xml">The xml content to parse.</param>
        /// <returns>A new instance of the <see cref="XmlDataStoreReader"/> class.</returns>
        public static XmlDataStoreReader FromXml( string xml )
        {
            return new XmlDataStoreReader(new StringReader(xml));
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

                if( this.xmlReader.NotNullReference() )
                {
                    this.xmlReader.Close();
                    this.xmlReader = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)

            this.textReader = null;


            base.OnDispose(disposing);
        }

        #endregion

        #region Private Methods

        private FormatException CreateUnexpectedEndException( [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0 )
        {
            return new FormatException("Unexpected end of xml file reached!").StoreDefault(filePath, memberName, lineNumber);
        }

        private FormatException CreateInvalidRootException( [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0 )
        {
            return new FormatException("Invalid root node!").StoreDefault(filePath, memberName, lineNumber);
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool MoveToNextStartElement()
        {
            while( this.xmlReader.Read() )
            {
                if( this.xmlReader.NodeType == XmlNodeType.Element )
                    return true;
            }

            return false;
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool MoveToNextStartOrEndElement()
        {
            while( this.xmlReader.Read() )
            {
                if( this.xmlReader.NodeType == XmlNodeType.Element
                 || this.xmlReader.NodeType == XmlNodeType.EndElement )
                    return true;
            }

            return false;
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool MoveToNextStartOrEndOrTextElement()
        {
            while( this.xmlReader.Read() )
            {
                if( this.xmlReader.NodeType == XmlNodeType.Element
                 || this.xmlReader.NodeType == XmlNodeType.EndElement
                 || this.xmlReader.NodeType == XmlNodeType.Text )
                    return true;
            }

            return false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Moves the reader to the next <see cref="DataStoreToken"/>.
        /// </summary>
        /// <param name="name">The name of the data store node found; or <c>null</c>.</param>
        /// <returns>The <see cref="DataStoreToken"/> found.</returns>
        protected override DataStoreToken ReadToken( out string name )
        {
            if( this.xmlReader.EOF )
                throw this.CreateUnexpectedEndException(); // this shouldn't happen, because the root node is always present

            name = this.xmlReader.Name;

            DataStoreToken result;
            switch( this.xmlReader.NodeType )
            {
            case XmlNodeType.Element:
                if( this.xmlReader.IsEmptyElement )
                {
                    result = DataStoreToken.Value;
                    //// not moving away: empty elements have no EndElement
                }
                else
                {
                    if( !this.MoveToNextStartOrEndOrTextElement() )
                        throw this.CreateUnexpectedEndException();

                    if( this.xmlReader.NodeType == XmlNodeType.Text )
                        result = DataStoreToken.Value;
                    else
                        result = DataStoreToken.ObjectStart;
                }
                break;

            case XmlNodeType.EndElement:
                if( this.Depth == 0 )
                {
                    if( !string.Equals(this.xmlReader.Name, XmlDataStoreWriter.RootName, StringComparison.Ordinal) )
                        throw this.CreateInvalidRootException();

                    result = DataStoreToken.DataStoreEnd; // TODO: do we need this? doeas it get triggered properly?
                }
                else
                {
                    result = DataStoreToken.ObjectEnd;
                    this.MoveToNextStartOrEndElement();
                }
                break;

            default:
                {
                    var exception = new Exception("Invalid xml reader state!");
                    this.StoreReaderInfo(exception);
                    exception.Store("XmlNodeType", this.xmlReader.NodeType);
                    throw exception;
                }
            }

            return result;
        }

        /// <summary>
        /// Reads the data store value the reader is currently at.
        /// </summary>
        /// <param name="textReader">The <see cref="Mechanical.IO.ITextReader"/> to use; or <c>null</c>.</param>
        /// <param name="binaryReader">The <see cref="Mechanical.IO.IBinaryReader"/> to use; or <c>null</c>.</param>
        protected override void ReadValue( out Mechanical.IO.ITextReader textReader, out Mechanical.IO.IBinaryReader binaryReader )
        {
            if( this.xmlReader.NodeType == XmlNodeType.Text )
            {
                this.textReader.Set(this.xmlReader.Value);
                this.MoveToNextStartOrEndElement(); // moves to EndElement
                this.MoveToNextStartOrEndElement(); // next token
            }
            else
            {
                this.textReader.Set(Substring.Empty);
                this.MoveToNextStartOrEndElement(); // moves to next token (currently at Element; empty elements have no EndElement)
            }

            textReader = this.textReader;
            binaryReader = null;
        }

        /// <summary>
        /// Begins reading the data store object the reader is currently at.
        /// </summary>
        protected override void ReadObjectStart()
        {
        }

        /// <summary>
        /// Ends reading the data store object the reader is currently at.
        /// </summary>
        protected override void ReadObjectEnd()
        {
        }

        /// <summary>
        /// Stores additional information about the state of the reader, on exceptions being thrown.
        /// </summary>
        /// <typeparam name="TException">The type of the exception being thrown.</typeparam>
        /// <param name="exception">The exception being thrown.</param>
        protected override void StoreReaderInfo<TException>( TException exception )
        {
            base.StoreReaderInfo<TException>(exception);

            var lineInfo = this.xmlReader as IXmlLineInfo;
            if( lineInfo.NotNullReference()
             && lineInfo.HasLineInfo() )
            {
                exception.Store("XmlLine", lineInfo.LineNumber);
                exception.Store("XmlColumn", lineInfo.LinePosition);
            }
        }

        #endregion
    }
}
