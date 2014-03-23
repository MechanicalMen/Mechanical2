using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.FileFormats
{
    /// <summary>
    /// A low-level JSON reader.
    /// </summary>
    public class JsonReader : DisposableObject
    {
        #region Private Fields

        private readonly StringBuilder sb;
        private readonly Stack<JsonToken> parents;
        private readonly char[] hex = new char[4];
        private ITextReader reader;
        private int lineNumber = 1;
        private int columnNumber = 0; // the first Read() increases this to 1 (or more)
        private JsonToken currentToken = (JsonToken)byte.MaxValue;
        private string currentRawValue = null;
        private bool isReading = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="ITextReader"/> to take ownership of.</param>
        public JsonReader( ITextReader textReader )
        {
            Ensure.That(textReader).NotNull();

            this.sb = new StringBuilder();
            this.parents = new Stack<JsonToken>();
            this.reader = textReader;
        }

        #endregion

        #region Private Methods

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void ThrowIfDisposedOrStartOrEnd( [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0 )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine(filePath, memberName, lineNumber);

            if( !this.isReading )
                throw new InvalidOperationException("No call was made to Read() yet, or the underlying stream ended!").StoreFileLine(filePath, memberName, lineNumber);
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool PeekChar( out char ch )
        {
            int charPeeked = this.reader.Peek();
            if( charPeeked != -1 )
            {
                ch = (char)charPeeked;
                return true;
            }
            else
            {
                ch = default(char);
                return false;
            }
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool ReadChar( out char ch )
        {
            int charRead = this.reader.Read();
            if( charRead != -1 )
            {
                ch = (char)charRead;

                if( ch != '\r' )
                    ++this.columnNumber;

                if( ch == '\n' )
                {
                    ++this.lineNumber;
                    this.columnNumber = 0; // the first character of the next line increases it to 1
                }

                return true;
            }
            else
            {
                ch = default(char);
                return false;
            }
        }

        private void ReadUntilColon( out char ch )
        {
            while( this.ReadChar(out ch) )
            {
                if( ch == ':' )
                {
                    return;
                }
                else if( char.IsWhiteSpace(ch) )
                {
                    // continue
                }
                else
                    throw new FormatException("Invalid JSON format: whitespace or colon expected!").Store("InvalidJsonCharacter", ch);
            }

            throw new FormatException("Invalid JSON format: unexpected end of stream!").StoreFileLine();
        }

        private void ReadUntilCommaOrParentEnd( out char ch, bool acceptStreamEnd )
        {
            while( this.PeekChar(out ch) )
            {
                if( ch == ',' )
                {
                    // consume and stop
                    this.ReadChar(out ch);
                    if( this.parents.Peek() == JsonToken.Name )
                        this.parents.Pop();
                    return;
                }
                else if( ch == '}' || ch == ']' )
                {
                    // do not consume character: stop and let the next Read() handle it
                    if( this.parents.Peek() == JsonToken.Name )
                        this.parents.Pop();
                    return;
                }
                else if( char.IsWhiteSpace(ch) )
                {
                    // consume and continue
                    this.ReadChar(out ch);
                }
                else
                    throw new FormatException("Invalid JSON format: whitespace, comma, object- or array end expected!").Store("InvalidJsonCharacter", ch);
            }

            if( !acceptStreamEnd )
                throw new FormatException("Invalid JSON format: unexpected end of stream!").StoreFileLine();
        }

        private void ReadLiteral( string subliteral, out char ch )
        {
            var substr = this.reader.Read(subliteral.Length);
            if( !substr.Equals(subliteral, CompareOptions.OrdinalIgnoreCase) )
                throw new FormatException("Invalid JSON format: literal expected!").Store("JsonExpected", subliteral).Store("JsonFound", substr);

            this.ReadUntilCommaOrParentEnd(out ch, acceptStreamEnd: false);
        }

        private void ReadNumber( ref char ch )
        {
            // NOTE: this method reads number characters without checking for a valid format
            this.sb.Append(ch);

            while( this.PeekChar(out ch) )
            {
                if( ('0' <= ch && ch <= '9')
                 || ch == '.'
                 || ch == 'e'
                 || ch == 'E'
                 || ch == '+'
                 || ch == '-' )
                {
                    // number character: save, consume and continue
                    this.ReadChar(out ch);
                    this.sb.Append(ch);
                }
                else
                {
                    // not part of a number: prepare for next Read()
                    this.ReadUntilCommaOrParentEnd(out ch, acceptStreamEnd: false);
                    return;
                }
            }

            throw new FormatException("Invalid JSON format: unexpected end of stream!").StoreFileLine();
        }

        private void ReadString( out char ch )
        {
            while( this.ReadChar(out ch) )
            {
                if( ch == '"' )
                {
                    // end of string
                    return;
                }
                else if( ch == '\\' )
                {
                    if( !this.ReadChar(out ch) )
                        throw new FormatException("Invalid JSON format: unexpected end of stream!").StoreFileLine();

                    switch( ch )
                    {
                    case '"':
                        this.sb.Append('"');
                        break;

                    case '\\':
                        this.sb.Append('\\');
                        break;

                    case '/':
                        this.sb.Append('/');
                        break;

                    case 'b':
                        this.sb.Append('\b');
                        break;

                    case 'f':
                        this.sb.Append('\f');
                        break;

                    case 'n':
                        this.sb.Append('\n');
                        break;

                    case 'r':
                        this.sb.Append('\r');
                        break;

                    case 't':
                        this.sb.Append('\t');
                        break;

                    case 'u':
                        {
                            // read 4 characters into a string
                            for( int i = 0; i < 4; ++i )
                            {
                                if( this.ReadChar(out ch) )
                                    this.hex[i] = ch;
                                else
                                    throw new FormatException("Invalid JSON format: unexpected end of stream!").StoreFileLine();
                            }
                            var str = new string(this.hex);

                            // parse hex number
                            int u;
                            if( !int.TryParse(str, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out u) )
                                throw new FormatException("Invalid JSON format: unable to parse unicode escape code!").Store("JsonUnicodeEscapeCode", str);

                            // save and continue
                            this.sb.Append((char)u);
                        }
                        break;

                    default:
                        throw new FormatException("Invalid JSON format: unknown escape character!").StoreFileLine();
                    }
                }
                else
                {
                    // normal string character
                    this.sb.Append(ch);
                }
            }

            throw new FormatException("Invalid JSON format: unexpected end of stream!").StoreFileLine();
        }

        private void PopParent( JsonToken token )
        {
            if( this.parents.Count == 0 )
                throw new FormatException("Invalid JSON format: Missing parent (token)!").StoreFileLine();

            var parent = this.parents.Pop();
            if( parent != token )
                throw new FormatException("Invalid JSON format: unexpected parent (token)!").Store("JsonParentExpected", token).Store("JsonParentFound", parent);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the last <see cref="JsonToken"/> read.
        /// </summary>
        /// <value>The last <see cref="JsonToken"/> read.</value>
        public JsonToken Token
        {
            get
            {
                this.ThrowIfDisposedOrStartOrEnd();

                return this.currentToken;
            }
        }

        /// <summary>
        /// Gets the value of the current JSON token.
        /// </summary>
        /// <value>The value of the current JSON token.</value>
        public string RawValue
        {
            get
            {
                this.ThrowIfDisposedOrStartOrEnd();

                return this.currentRawValue;
            }
        }

        /// <summary>
        /// Gets the current column number.
        /// </summary>
        /// <value>The current column number.</value>
        public int ColumnNumber
        {
            get
            {
                this.ThrowIfDisposedOrStartOrEnd();

                return this.columnNumber;
            }
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <value>The current line number.</value>
        public int LineNumber
        {
            get
            {
                this.ThrowIfDisposedOrStartOrEnd();

                return this.lineNumber;
            }
        }

        /// <summary>
        /// Reads the next token from the JSON stream.
        /// </summary>
        /// <returns><c>true</c> if a token could be read; <c>false</c> if the end of the stream was reached.</returns>
        public bool Read()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            try
            {
                char ch;
                while( this.ReadChar(out ch) )
                {
                    switch( ch )
                    {
                    // NOTE: technically 'true', 'false' and 'null' are all lower case, but it doesn't really cost us to be more permissive
                    case 't':
                    case 'T':
                        this.ReadLiteral("rue", out ch);
                        this.currentRawValue = "true";
                        this.currentToken = JsonToken.BooleanValue;
                        return true;

                    case 'f':
                    case 'F':
                        this.ReadLiteral("alse", out ch);
                        this.currentRawValue = "false";
                        this.currentToken = JsonToken.BooleanValue;
                        return true;

                    case 'n':
                    case 'N':
                        this.ReadLiteral("ull", out ch);
                        this.currentRawValue = null;
                        this.currentToken = JsonToken.NullValue;
                        return true;

                    case '{':
                        this.currentToken = JsonToken.ObjectStart;
                        this.currentRawValue = "{";
                        this.parents.Push(JsonToken.ObjectStart);
                        this.isReading = true;
                        return true;

                    case '}':
                        this.currentToken = JsonToken.ObjectEnd;
                        this.currentRawValue = "}";
                        this.PopParent(JsonToken.ObjectStart);  // it is important to "pop", before reaching the comma
                        this.ReadUntilCommaOrParentEnd(out ch, acceptStreamEnd: true);
                        return true;

                    case '[':
                        this.currentToken = JsonToken.ArrayStart;
                        this.currentRawValue = "[";
                        this.parents.Push(JsonToken.ArrayStart);
                        this.isReading = true;
                        return true;

                    case ']':
                        this.currentToken = JsonToken.ArrayEnd;
                        this.currentRawValue = "]";
                        this.PopParent(JsonToken.ArrayStart); // it is important to "pop", before reaching the comma
                        this.ReadUntilCommaOrParentEnd(out ch, acceptStreamEnd: true);
                        return true;

                    default:
                        if( ch == '-'
                         || ch == '+' // technically not part of the format, but eh... (see comments about character casing above)
                         || ('0' <= ch && ch <= '9') )
                        {
                            this.ReadNumber(ref ch);
                            this.currentToken = JsonToken.NumberValue;
                            this.currentRawValue = this.sb.ToString();
                            this.sb.Clear();
                            return true;
                        }
                        else if( ch == '"' )
                        {
                            this.ReadString(out ch);

                            switch( this.parents.Peek() )
                            {
                            case JsonToken.ObjectStart:
                                this.ReadUntilColon(out ch);
                                this.currentToken = JsonToken.Name;
                                this.parents.Push(JsonToken.Name);
                                break;

                            case JsonToken.Name:
                            case JsonToken.ArrayStart:
                                this.ReadUntilCommaOrParentEnd(out ch, acceptStreamEnd: false);
                                this.currentToken = JsonToken.StringValue;
                                break;
                            }

                            this.currentRawValue = this.sb.ToString();
                            this.sb.Clear();
                            return true;
                        }
                        else if( char.IsWhiteSpace(ch) )
                        {
                            // consume and continue
                        }
                        else
                            throw new FormatException("Invalid JSON format: unexpected character!").Store("JsonCharRead", ch);
                        break;
                    }
                }

                this.currentToken = (JsonToken)byte.MaxValue;
                this.currentRawValue = null;
                if( this.parents.Count != 0 )
                    throw new FormatException("Invalid JSON format: not all objects or arrays were closed at the end of the stream!").Store("JsonParentStack", this.parents);
                this.isReading = false;
                return false;
            }
            catch( Exception ex )
            {
                ex.Store("JsonColumnNumber", this.columnNumber);
                ex.Store("JsonLineNumber", this.lineNumber);
                ex.Store("JsonToken", this.Token);
                ex.Store("JsonRawValue", this.currentRawValue);
                throw;
            }
        }

        #endregion
    }
}
