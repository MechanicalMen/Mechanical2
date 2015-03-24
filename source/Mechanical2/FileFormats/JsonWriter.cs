using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.FileFormats
{
    /// <summary>
    /// A low level JSON writer. Does not enforce correct syntax.
    /// </summary>
    public class JsonWriter : DisposableObject
    {
        #region Private Fields

        private readonly Stack<JsonToken> parents;
        private ITextWriter textWriter;
        private bool indent;
        private bool produceAscii;
        private JsonToken prevToken = (JsonToken)byte.MaxValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The <see cref="ITextWriter"/> to take ownership of.</param>
        /// <param name="indent"><c>true</c> to indent the output; otherwise, <c>false</c>.</param>
        /// <param name="produceAscii"><c>true</c> to write only ASCII characters (and encode others using unicode escape codes); otherwise, <c>false</c>. Non printable ASCII characters are always escaped, unless handled by the JSON standard.</param>
        public JsonWriter( ITextWriter textWriter, bool indent = true, bool produceAscii = false )
        {
            Ensure.That(textWriter).NotNull();

            this.textWriter = textWriter;
            this.indent = indent;
            this.produceAscii = produceAscii;

            this.parents = new Stack<JsonToken>();
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

                if( this.textWriter.NotNullReference() )
                {
                    this.textWriter.Close();
                    this.textWriter = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)


            base.OnDispose(disposing);
        }

        #endregion

        #region Private Methods

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void WriteCommaIfRequired()
        {
            if( this.prevToken != JsonToken.ObjectStart
             && this.prevToken != JsonToken.ArrayStart
             && this.prevToken != JsonToken.Name )
                this.textWriter.Write(',');

            this.WriteNewLine();
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void WriteNewLine()
        {
            if( this.indent )
            {
                this.textWriter.WriteLine();
                for( int i = 0; i < this.Depth; ++i )
                    this.textWriter.Write("  ");
            }
        }

        private int Depth
        {
#if !MECHANICAL_NET4
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get { return this.parents.Count; }
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void PrintCharacterEscaped( char ch )
        {
            this.textWriter.Write("\\u");
            this.textWriter.Write(((int)ch).ToString("X4", CultureInfo.InvariantCulture));
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void WriteStringLiteral( Substring str )
        {
            if( str.Origin.NullReference() )
            {
                this.textWriter.Write("null");
            }
            else
            {
                this.textWriter.Write('"');

                char ch;
                for( int i = 0; i < str.Length; ++i )
                {
                    ch = str[i];
                    switch( ch )
                    {
                    case '"':
                        this.textWriter.Write("\\\"");
                        break;

                    case '\\':
                        this.textWriter.Write("\\\\");
                        break;

                    case '/':
                        this.textWriter.Write("\\/");
                        break;

                    case '\b':
                        this.textWriter.Write("\\b");
                        break;

                    case '\f':
                        this.textWriter.Write("\\f");
                        break;

                    case '\n':
                        this.textWriter.Write("\\n");
                        break;

                    case '\r':
                        this.textWriter.Write("\\r");
                        break;

                    case '\t':
                        this.textWriter.Write("\\t");
                        break;

                    default:
                        if( !this.produceAscii )
                        {
                            // it's OK to output unicode characters
                            if( (0 <= (int)ch && (int)ch <= 31)
                             || ch == 127 )
                                this.PrintCharacterEscaped(ch); // non-printable ASCII character we didn't handle above
                            else
                                this.textWriter.Write(ch); // printable ASCII, or a non-ASCII character (that we assume can be displayed to the user).
                        }
                        else
                        {
                            // escape unicode characters
                            if( 32 <= (int)ch && (int)ch <= 126 )
                                this.textWriter.Write(ch); // a printable ASCII character we didn't handle above
                            else
                                this.PrintCharacterEscaped(ch); // (unhandled) non-printable ASCII, or non-ASCII character
                        }
                        break;
                    }
                }

                this.textWriter.Write('"');
            }
        }

        private bool IsNumber( Substring substr )
        {
            if( substr.NullOrEmpty )
                return false;

            // check for optional leading sign
            char ch = substr[0];
            if( ch == '-' ) // there is no leading plus sign for json numbers
                substr = substr.Substr(startIndex: 1);

            // check for leading zeroes
            // NOTE: leading zeroes are not allowed at the start of the number, but they are okay at the end of the fractional digits, or the start of the exponential digits
            if( substr.Length > 1 // at least two characters: since if there is only one, it may be any digit, and will be consumed below
             && substr[0] == '0'
             && char.IsDigit(substr[1]) )
            {
                // if there is a leading zero, then there better not be another digit next to it
                return false;
            }

            // consume digits
            if( !this.TryConsumeStartingDigits(ref substr) )
                return false;

            // is there an optional fractional part?
            if( substr.Length != 0
             && substr[0] == '.' )
            {
                // consume decimal point
                substr = substr.Substr(startIndex: 1);

                // consume fractional digits
                if( !this.TryConsumeStartingDigits(ref substr) )
                    return false;
            }

            // is there an optional exponential part?
            if( substr.Length != 0 )
            {
                ch = substr[0];
                if( ch == 'e'
                 || ch == 'E' )
                {
                    if( substr.Length == 1 )
                        return false; // just an 'e' or 'E' is not enough

                    // consume 'e' character and optional sign
                    ch = substr[1];
                    if( ch == '+'
                     || ch == '-' )
                        substr = substr.Substr(startIndex: 2);
                    else
                        substr = substr.Substr(startIndex: 1);

                    // consume exponential digits
                    if( !this.TryConsumeStartingDigits(ref substr) )
                        return false;
                }
            }

            // if there is anything left, this is not a number!
            return substr.Length == 0;
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool TryConsumeStartingDigits( ref Substring substr )
        {
            // count number of digits
            int digitCount = substr.Length;
            for( int i = 0; i < substr.Length; ++i )
            {
                if( !char.IsDigit(substr.Origin, substr.StartIndex + i) )
                {
                    digitCount = i;
                    break;
                }
            }

            // is there anything to consume
            if( digitCount == 0 )
            {
                // the very first character was not a digit, or the substring was empty
                return false;
            }
            else
            {
                // at least some characters were digits
                substr = substr.Substr(startIndex: digitCount);
                return true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the opening curly bracket of a JSON object.
        /// </summary>
        public void WriteObjectStart()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.Depth != 0
             && this.prevToken != JsonToken.Name )
                this.WriteCommaIfRequired();

            this.textWriter.Write('{');
            this.prevToken = JsonToken.ObjectStart;
            this.parents.Push(JsonToken.ObjectStart);
        }

        /// <summary>
        /// Writes the closing curly bracket of a JSON object.
        /// </summary>
        public void WriteObjectEnd()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            var parent = this.parents.Pop();
            if( parent != JsonToken.ObjectStart )
                throw new InvalidOperationException("The container being closed is not an object!").Store("parent", parent);

            if( this.prevToken != JsonToken.ObjectStart )
                this.WriteNewLine();

            this.textWriter.Write('}');
            this.prevToken = JsonToken.ObjectEnd;
        }

        /// <summary>
        /// Writes the opening bracket of a JSON array.
        /// </summary>
        public void WriteArrayStart()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.Depth != 0
             && this.prevToken != JsonToken.Name )
                this.WriteCommaIfRequired();

            this.textWriter.Write('[');
            this.prevToken = JsonToken.ArrayStart;
            this.parents.Push(JsonToken.ArrayStart);
        }

        /// <summary>
        /// Writes the closing bracket of a JSON array.
        /// </summary>
        public void WriteArrayEnd()
        {
            var parent = this.parents.Pop();
            if( parent != JsonToken.ArrayStart )
                throw new InvalidOperationException("The container being closed is not an array!").Store("parent", parent);

            if( this.prevToken != JsonToken.ArrayStart )
                this.WriteNewLine();

            this.textWriter.Write(']');
            this.prevToken = JsonToken.ArrayEnd;
        }

        /// <summary>
        /// Writes the name of an item of a JSON object.
        /// </summary>
        /// <param name="name">The name of the value written next.</param>
        public void WriteName( Substring name )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( name.Origin.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            this.WriteCommaIfRequired();

            this.WriteStringLiteral(name);
            if( this.indent )
                this.textWriter.Write(" : ");
            else
                this.textWriter.Write(':');
            this.prevToken = JsonToken.Name;
        }

        /// <summary>
        /// Writes a <c>null</c> value.
        /// </summary>
        public void WriteNullValue()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.parents.Peek() == JsonToken.ArrayStart )
                this.WriteCommaIfRequired();

            this.textWriter.Write("null");
            this.prevToken = JsonToken.NullValue;
        }

        /// <summary>
        /// Writes a <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteValue( bool value )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.parents.Peek() == JsonToken.ArrayStart )
                this.WriteCommaIfRequired();

            if( value )
                this.textWriter.Write("true");
            else
                this.textWriter.Write("false");
            this.prevToken = JsonToken.BooleanValue;
        }

        /// <summary>
        /// Writes an <see cref="Int64"/> value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteValue( long value )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.parents.Peek() == JsonToken.ArrayStart )
                this.WriteCommaIfRequired();

            this.textWriter.Write(value.ToString("D", CultureInfo.InvariantCulture));
            this.prevToken = JsonToken.NumberValue;
        }

        /// <summary>
        /// Writes a <see cref="Double"/> value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteValue( double value )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.parents.Peek() == JsonToken.ArrayStart )
                this.WriteCommaIfRequired();

            this.textWriter.Write(value.ToString("R", CultureInfo.InvariantCulture));
            this.prevToken = JsonToken.NumberValue;
        }

        /// <summary>
        /// Writes a <see cref="Decimal"/> value.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteValue( decimal value )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.parents.Peek() == JsonToken.ArrayStart )
                this.WriteCommaIfRequired();

            this.textWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            this.prevToken = JsonToken.NumberValue;
        }

        /// <summary>
        /// Writes a string value.
        /// </summary>
        /// <param name="str">The string to write.</param>
        public void WriteValue( Substring str )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.parents.Peek() == JsonToken.ArrayStart )
                this.WriteCommaIfRequired();

            this.WriteStringLiteral(str);
            this.prevToken = JsonToken.StringValue;
        }

        /// <summary>
        /// Writes a non-null value, whose token is to be determined. If not a number or boolean, then string is assumed.
        /// </summary>
        /// <param name="str">The string representation of some kind of JSON value.</param>
        public void WriteUnknownValue( Substring str )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            if( this.parents.Peek() == JsonToken.ArrayStart )
                this.WriteCommaIfRequired();

            if( str.Equals("true", CompareOptions.Ordinal)
             || str.Equals("false", CompareOptions.Ordinal) )
            {
                this.textWriter.Write(str);
                this.prevToken = JsonToken.BooleanValue;
            }
            else
            {
                if( this.IsNumber(str) )
                {
                    this.textWriter.Write(str);
                    this.prevToken = JsonToken.NumberValue;
                }
                else
                {
                    this.WriteStringLiteral(str);
                    this.prevToken = JsonToken.StringValue;
                }
            }
        }

        /// <summary>
        /// Flushes buffered data to the underlying stream.
        /// </summary>
        public void Flush()
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreFileLine();

            this.textWriter.Flush();
        }

        #endregion
    }
}
