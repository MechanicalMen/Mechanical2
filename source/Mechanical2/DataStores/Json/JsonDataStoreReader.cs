using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores.Xml;
using Mechanical.FileFormats;
using Mechanical.IO;

namespace Mechanical.DataStores.Json
{
    //// NOTE: This class is mainly here to allow deserializing json content using data stores,
    ////       whether the json was originally produced by a data store or not. It is important to know, that
    ////       if you deserialize from a non-data store json, and then serialize again, the json produced this
    ////       way will be significantly different from the original! The data store reader will disregard
    ////       these differences only(!), when properly told which json content was produced from a data store
    ////       (and which wasn't).
    ////
    ////       The reason for this, is that data stores lack much of the functionality of json, while they have
    ////       one or two features json doesn't support either (e.g. names for root objects).
    ////
    ////       These are the conversions applied to non- data store produced json content:
    ////         - arrays are converted into objects. Names of it's children are generated based on their index.
    ////         - the root object or array always has a name ("root")
    ////         - null is converted into an empty string
    ////
    ////       Json produced by data stores always has a json object as a root, with - at most - a single item
    ////       in it. This is used (mostly) to store the name of the data store root.

    /// <summary>
    /// A JSON based data store reader.
    /// Among other changes, arrays area converted into objects, therefore the JSON format is not(!) preserved.
    /// </summary>
    public class JsonDataStoreReader : DataStoreReaderBase.Disposable
    {
        #region Private Fields

        private const int IndexUnused = int.MaxValue;

        private struct ParentInfo
        {
            internal readonly bool IsArray;
            internal readonly int NextIndex;

            internal ParentInfo( int nextIndex, bool isArray )
            {
                this.IsArray = isArray;
                this.NextIndex = nextIndex;
            }
        }

        private readonly bool isDataStoreJson;
        private readonly Stack<ParentInfo> parents;
        private JsonReader jsonReader;
        private Mechanical.IO.StringReader textReader;
        private string currentValue;
        private int nextIndex = IndexUnused;

        #endregion

        #region Constructors

        private JsonDataStoreReader( JsonReader jsonReader, bool isDataStoreJson )
            : base()
        {
            Ensure.That(jsonReader).NotNull();

            this.jsonReader = jsonReader;
            this.textReader = new IO.StringReader();
            this.parents = new Stack<ParentInfo>();
            this.isDataStoreJson = isDataStoreJson;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataStoreReader"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to take ownership of.</param>
        /// <param name="isDataStoreJson">Determines whether the JSON content was produced by a data store, or elsewhere.</param>
        public JsonDataStoreReader( Stream stream, bool isDataStoreJson )
            : this(IOWrapper.ToTextReader(stream), isDataStoreJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataStoreReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to take ownership of.</param>
        /// <param name="isDataStoreJson">Determines whether the JSON content was produced by a data store, or elsewhere.</param>
        public JsonDataStoreReader( TextReader textReader, bool isDataStoreJson )
            : this(new JsonReader(IOWrapper.Wrap(textReader)), isDataStoreJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataStoreReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="ITextReader"/> to take ownership of.</param>
        /// <param name="isDataStoreJson">Determines whether the JSON content was produced by a data store, or elsewhere.</param>
        public JsonDataStoreReader( ITextReader textReader, bool isDataStoreJson )
            : this(new JsonReader(textReader), isDataStoreJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataStoreReader"/> class.
        /// </summary>
        /// <param name="json">The JSON content to parse.</param>
        /// <param name="isDataStoreJson">Determines whether the JSON content was produced by a data store, or elsewhere.</param>
        /// <returns>A new instance of the <see cref="JsonDataStoreReader"/> class.</returns>
        public static JsonDataStoreReader FromJson( string json, bool isDataStoreJson )
        {
            var reader = new Mechanical.IO.StringReader();
            reader.Set(json);
            return new JsonDataStoreReader(reader, isDataStoreJson);
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

                if( this.jsonReader.NotNullReference() )
                {
                    this.jsonReader.Dispose();
                    this.jsonReader = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)
            this.currentValue = null;
            this.textReader = null;

            base.OnDispose(disposing);
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
            // determine name
            bool skipReading = false;
            if( this.Token == DataStoreToken.DataStoreStart )
            {
                //// starting to read the json content...

                if( !this.isDataStoreJson )
                {
                    // assign default name to the root json object or array
                    name = XmlDataStoreWriter.RootName;
                }
                else
                {
                    //// "unwrap" the data store representation

                    // read the wrapping json object (don't push onto "parents" stack!)
                    if( !this.jsonReader.Read() )
                        throw new FormatException("Unexpected end of json file reached!").StoreFileLine();
                    if( this.jsonReader.Token != JsonToken.ObjectStart )
                        throw new FormatException("Unexpected json token found!").Store("expectedToken", JsonToken.ObjectStart).Store("actualToken", this.jsonReader.Token);

                    // read the name of the root data store node, if there is one
                    if( !this.jsonReader.Read() )
                        throw new FormatException("Unexpected end of json file reached!").StoreFileLine();
                    if( this.jsonReader.Token == JsonToken.Name )
                    {
                        name = this.jsonReader.RawValue;
                    }
                    else if( this.jsonReader.Token == JsonToken.ObjectEnd )
                    {
                        name = null;
                        return DataStoreToken.DataStoreEnd;
                    }
                    else
                        throw new FormatException("Unexpected json token found! Name or ObjectEnd expected").Store("TokenFound", this.jsonReader.Token);
                }
            }
            else if( this.parents.Peek().IsArray )
            {
                // parent json object is an array: generate name
                name = 'i' + this.nextIndex.ToString("D", CultureInfo.InvariantCulture);
                ++this.nextIndex;
            }
            else
            {
                // parent json object is an object: read name
                if( !this.jsonReader.Read() )
                    throw new FormatException("Unexpected end of json file reached!").StoreFileLine();

                if( this.jsonReader.Token == JsonToken.Name )
                {
                    name = this.jsonReader.RawValue;
                }
                else if( this.jsonReader.Token == JsonToken.ObjectEnd )
                {
                    name = null;
                    skipReading = true;
                }
                else
                    throw new FormatException("Unexpected json token found! Name or ObjectEnd expected").Store("TokenFound", this.jsonReader.Token);
            }

            // read actual content
            if( !skipReading
             && !this.jsonReader.Read() )
                throw new FormatException("Unexpected end of json file reached!").StoreFileLine();

            // determine data store token
            this.currentValue = null;
            switch( this.jsonReader.Token )
            {
            case JsonToken.ObjectStart:
                this.parents.Push(new ParentInfo(this.nextIndex, isArray: false));
                this.nextIndex = IndexUnused;
                return DataStoreToken.ObjectStart;

            case JsonToken.ArrayStart:
                this.parents.Push(new ParentInfo(this.nextIndex, isArray: true));
                this.nextIndex = 0;
                return DataStoreToken.ObjectStart;

            case JsonToken.ObjectEnd:
            case JsonToken.ArrayEnd:
                // NOTE: the json reader already throws, if there are too many closing tokens,
                //       but we may have one extra, if we're reading data store json.
                if( this.isDataStoreJson && this.parents.Count == 0 )
                {
                    return DataStoreToken.DataStoreEnd;
                }
                else
                {
                    name = null; // in case there is a generated index in there
                    var parent = this.parents.Pop();
                    this.nextIndex = parent.NextIndex;
                    return DataStoreToken.ObjectEnd;
                }

            case JsonToken.StringValue:
            case JsonToken.NumberValue:
            case JsonToken.BooleanValue:
                this.currentValue = this.jsonReader.RawValue;
                return DataStoreToken.TextValue;

            case JsonToken.NullValue:
                this.currentValue = string.Empty; // data stores do not support null values
                return DataStoreToken.TextValue;

            default:
                throw new Exception("Invalid json reader state!").Store("Token", this.jsonReader.Token);
            }
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
            this.textReader.Set(Substring.Empty); // only necessary if reader was opened, but never used. Unlikely, but we don't know for sure.
        }

        /// <summary>
        /// Stores debug information about the current state of the reader, into the specified <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">The exception to store the state of the reader in.</param>
        public override void StorePosition( Exception exception )
        {
            base.StorePosition(exception);

            try
            {
                exception.Store("JsonLine", this.jsonReader.LineNumber);
                exception.Store("JsonColumn", this.jsonReader.ColumnNumber);
            }
            catch
            {
            }
        }

        #endregion
    }
}
