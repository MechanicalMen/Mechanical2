using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.IO;

namespace Mechanical.FileFormats
{
    /// <summary>
    /// A reader that parses a CSV stream.
    /// </summary>
    public class CsvReader : DisposableObject
    {
        #region Private Fields

        private readonly CsvFormat csvFormat;
        private readonly StringBuilder sb;
        private readonly List<Substring> record;
        private readonly ReadOnlyList.Wrapper<Substring> readOnlyRecord;
        private ITextReader tr;
        private int lineNumber = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="ITextReader"/> to take ownership of.</param>
        /// <param name="format">The <see cref="CsvFormat"/> to use.</param>
        public CsvReader( ITextReader textReader, CsvFormat format )
        {
            Ensure.That(textReader).NotNull();
            Ensure.That(format).NotNull();

            this.csvFormat = format;
            this.sb = new StringBuilder();
            this.record = new List<Substring>();
            this.readOnlyRecord = new ReadOnlyList.Wrapper<Substring>(this.record);
            this.tr = textReader;
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

                if( this.tr != null )
                {
                    this.tr.Close();
                    this.tr = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)


            base.OnDispose(disposing);
        }

        #endregion

        #region Private Methods

        private Substring FromCsvString( Substring substr )
        {
            if( substr.Length == 0
             || substr.Equals("\"\"", CompareOptions.Ordinal, CultureInfo.InvariantCulture) )
                return Substring.Empty;

            if( substr[0] == '"'
             && substr[substr.Length - 1] == '"' )
                return substr.Substr(1, substr.Length - 2).ToString().Replace("\"\"", "\"");
            else
                return substr;
        }

        private static int NumDoubleQuotes( Substring str )
        {
            return NumDoubleQuotes(str, 0, str.Length);
        }

        private static int NumDoubleQuotes( Substring str, int startIndex, int count )
        {
            int num = 0;
            count += startIndex;
            for( int i = startIndex; i < count; ++i )
            {
                if( str[i] == '"' )
                    ++num;
            }
            return num;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the <see cref="CsvFormat"/> used.
        /// </summary>
        /// <value>The <see cref="CsvFormat"/> used.</value>
        public CsvFormat Format
        {
            get
            {
                Ensure.That(this).NotDisposed();

                return this.csvFormat;
            }
        }

        /// <summary>
        /// Reads the next record from the CSV stream.
        /// </summary>
        /// <returns><c>true</c> if a record could be read; <c>false</c> if the end of the stream was reached.</returns>
        public bool Read()
        {
            Substring lastLineRead;
            this.tr.ReadLine(out lastLineRead);
            if( lastLineRead.Origin.NullReference() )
            {
                return false;
            }
            else
            {
                ++this.lineNumber;
                this.record.Clear();
            }

            // multiline record?
            if( NumDoubleQuotes(lastLineRead) % 2 == 1 )
            {
                do
                {
                    // append previous line
                    if( this.sb.Length != 0 )
                        this.sb.Append(this.csvFormat.NewLine);
                    this.sb.Append(lastLineRead.Origin, lastLineRead.StartIndex, lastLineRead.Length);

                    // try to read new line
                    this.tr.ReadLine(out lastLineRead);
                    if( lastLineRead.NullReference() )
                        throw new FormatException("Unexpected end of stream: a quote was still open!").Store("LineNumber", this.LineNumber);
                    else
                        ++this.lineNumber;
                }
                while( NumDoubleQuotes(lastLineRead) % 2 == 0 );

                // append closing line
                this.sb.Append(this.csvFormat.NewLine);
                this.sb.Append(lastLineRead.Origin, lastLineRead.StartIndex, lastLineRead.Length);

                // create record
                lastLineRead = this.sb.ToString();
                this.sb.Clear();
            }

            // parse our line
            int startIndex = 0;
            int nextSeparatorAt;
            while( startIndex < lastLineRead.Length
                && (nextSeparatorAt = lastLineRead.IndexOf(this.csvFormat.Separator, startIndex, StringComparison.Ordinal)) != -1 )
            {
                while( NumDoubleQuotes(lastLineRead, startIndex, nextSeparatorAt - startIndex) % 2 == 1 )
                {
                    // this separator is inside quotes: skip it
                    nextSeparatorAt = lastLineRead.IndexOf(this.csvFormat.Separator, nextSeparatorAt + 1, StringComparison.Ordinal);
                    if( nextSeparatorAt == -1 )
                        break;
                }
                if( nextSeparatorAt == -1 )
                    break;

                this.record.Add(this.FromCsvString(lastLineRead.Substr(startIndex, nextSeparatorAt - startIndex)));
                startIndex = nextSeparatorAt + 1;
            }

            this.record.Add(this.FromCsvString(lastLineRead.Substr(startIndex)));
            return true;
        }

        /// <summary>
        /// Gets the zero-based line the reader is currently at.
        /// </summary>
        /// <value>The zero-based line the reader is currently at.</value>
        public int LineNumber
        {
            get { return this.lineNumber; }
        }

        /// <summary>
        /// Gets the record that was read.
        /// </summary>
        /// <value>The record that was read.</value>
        public IReadOnlyList<Substring> Record
        {
            get { return this.readOnlyRecord; }
        }

        #endregion
    }
}
