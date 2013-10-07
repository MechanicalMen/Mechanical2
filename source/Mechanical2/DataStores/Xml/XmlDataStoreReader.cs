using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.DataStores.Xml
{
    //// NOTE: There are no empty xml documents, only empty xml fragments!
    ////       Therefore we always have a root element, even for empty data stores.

    /// <summary>
    /// An XML based data store reader.
    /// </summary>
    public class XmlDataStoreReader : DataStoreReaderBase.Disposable
    {
        #region Private Fields

        private XmlReader xmlReader;
        private Mechanical.IO.StringReader textReader;
        private string currentValue;
        private bool needToMoveForNextToken;

        #endregion

        #region Constructors

        private XmlDataStoreReader( XmlReader xmlReader )
            : base()
        {
            Ensure.That(xmlReader).NotNull();

            this.xmlReader = xmlReader;
            this.textReader = new IO.StringReader();
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
        /// <param name="textReader">The <see cref="ITextReader"/> to take ownership of.</param>
        public XmlDataStoreReader( ITextReader textReader )
            : this(IOWrapper.Wrap(textReader))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataStoreReader"/> class.
        /// </summary>
        /// <param name="xml">The xml content to parse.</param>
        /// <returns>A new instance of the <see cref="XmlDataStoreReader"/> class.</returns>
        public static XmlDataStoreReader FromXml( string xml )
        {
            return new XmlDataStoreReader(new System.IO.StringReader(xml));
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
            this.currentValue = null;
            this.textReader = null;

            base.OnDispose(disposing);
        }

        #endregion

        #region Private Methods

        private FormatException CreateUnexpectedEndException( [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0 )
        {
            return new FormatException("Unexpected end of xml file reached!").StoreFileLine(filePath, memberName, lineNumber);
        }

        private FormatException CreateInvalidRootException( [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0 )
        {
            return new FormatException("Invalid root node!").StoreFileLine(filePath, memberName, lineNumber);
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

        #region DataStoreReaderBase

        /// <summary>
        /// Reads the next name and token.
        /// </summary>
        /// <param name="name">The name of the data store node found. Ignored for ObjectEnd, DataStoreEnd tokens.</param>
        /// <returns>The <see cref="DataStoreToken"/> found.</returns>
        protected override DataStoreToken ReadToken( out string name )
        {
            if( this.Token == DataStoreToken.DataStoreStart )
            {
                // jump to root
                if( !this.MoveToNextStartElement() )
                    throw this.CreateUnexpectedEndException();

                // verify root
                if( !string.Equals(this.xmlReader.Name, XmlDataStoreWriter.RootName, StringComparison.Ordinal) )
                    throw this.CreateInvalidRootException();

                this.needToMoveForNextToken = true;
            }

            // file end reached?
            if( this.xmlReader.EOF )
                throw this.CreateUnexpectedEndException(); // this shouldn't happen, because the root node is always present

            // move to next element
            if( this.needToMoveForNextToken )
            {
                if( !this.MoveToNextStartOrEndElement() )
                    throw this.CreateUnexpectedEndException(); // this shouldn't happen, because the root node is always present
            }

            name = this.xmlReader.Name;

            DataStoreToken result;
            switch( this.xmlReader.NodeType )
            {
            case XmlNodeType.Element:
                if( this.xmlReader.IsEmptyElement )
                {
                    result = DataStoreToken.TextValue;
                    this.currentValue = string.Empty;
                    this.needToMoveForNextToken = true; // not moving away: empty elements have no EndElement
                }
                else
                {
                    if( !this.MoveToNextStartOrEndOrTextElement() )
                        throw this.CreateUnexpectedEndException();

                    if( this.xmlReader.NodeType == XmlNodeType.Text )
                    {
                        result = DataStoreToken.TextValue;
                        this.currentValue = this.xmlReader.Value;
                        this.MoveToNextStartOrEndElement(); // moves to EndElement
                        this.needToMoveForNextToken = true;
                    }
                    else
                    {
                        result = DataStoreToken.ObjectStart;
                        this.needToMoveForNextToken = false;
                    }
                }
                break;

            case XmlNodeType.EndElement:
                if( this.Depth == 0 )
                {
                    if( !string.Equals(this.xmlReader.Name, XmlDataStoreWriter.RootName, StringComparison.Ordinal) )
                        throw this.CreateInvalidRootException();

                    result = DataStoreToken.DataStoreEnd;
                    //// no need to set 'needToMoveForNextToken' anymore
                }
                else
                {
                    result = DataStoreToken.ObjectEnd;
                    this.needToMoveForNextToken = true;
                }
                break;

            default:
                throw new Exception("Invalid xml reader state!").Store("XmlNodeType", this.xmlReader.NodeType);
            }

            return result;
        }

        /// <summary>
        /// Returns the reader of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The reader of the value.</returns>
        protected override IBinaryReader OpenBinaryReader()
        {
            throw new InvalidOperationException().StoreFileLine();
        }

        /// <summary>
        /// Returns the reader of the value.
        /// If possible, a new reader should not be created.
        /// </summary>
        /// <returns>The reader of the value.</returns>
        protected override ITextReader OpenTextReader()
        {
            this.textReader.Set(this.currentValue);
            return this.textReader;
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="reader">The reader of the value.</param>
        protected override void CloseReader( IBinaryReader reader )
        {
            throw new InvalidOperationException().StoreFileLine();
        }

        /// <summary>
        /// Releases any resources held by an open reader.
        /// </summary>
        /// <param name="reader">The reader of the value.</param>
        protected override void CloseReader( ITextReader reader )
        {
            this.currentValue = null;
            this.textReader.Set(Substring.Empty); // only necessary if reader was opened, but never used. Unlikely, but we don't know for sure.
        }

        /// <summary>
        /// Stores debug information about the current state of the reader, into the specified <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">The exception to store the state of the reader in.</param>
        public override void StorePosition( Exception exception )
        {
            base.StorePosition(exception);

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
