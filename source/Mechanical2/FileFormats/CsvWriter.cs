using System;
using System.IO;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.FileFormats
{
    /// <summary>
    /// A writer outputting (the specified) CSV format.
    /// </summary>
    public class CsvWriter : DisposableObject
    {
        #region Private Fields

        private readonly CsvFormat csvFormat;
        private TextWriter tw;
        private bool firstCell = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to take ownership of.</param>
        /// <param name="format">The <see cref="CsvFormat"/> to use.</param>
        public CsvWriter( TextWriter textWriter, CsvFormat format )
        {
            Ensure.That(textWriter).NotNull();
            Ensure.That(format).NotNull();

            this.csvFormat = format;
            this.tw = textWriter;
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

                if( this.tw != null )
                {
                    this.tw.Dispose();
                    this.tw = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)


            base.OnDispose(disposing);
        }

        #endregion

        #region Private Methods

        private string ToCsvString( string str )
        {
            if( str.Length == 0 )
            {
                return string.Empty;
            }
            else
            {
                bool enclose = false;

                // according to wikipedia: "In some CSV implementations, leading and trailing spaces and tabs are trimmed."
                if( char.IsWhiteSpace(str, 0)
                 || char.IsWhiteSpace(str, str.Length - 1) )
                    enclose = true;

                if( str.IndexOf(this.csvFormat.Separator, StringComparison.Ordinal) != -1 )
                    enclose = true;

                if( str.Contains("\r")
                 || str.Contains("\n") )
                    enclose = true;

                if( str.Contains("\"") )
                {
                    enclose = true;
                    str = str.Replace("\"", "\"\"");
                }

                if( enclose )
                    str = '"' + str + '"';

                return str;
            }
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
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( sbyte cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( byte cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( short cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( ushort cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( int cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( uint cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( long cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( ulong cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( float cell )
        {
            this.Write(cell.ToString("R", this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( double cell )
        {
            this.Write(cell.ToString("R", this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( decimal cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( bool cell )
        {
            this.Write(cell.ToString(this.csvFormat.Culture));
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write.</param>
        public void Write( char cell )
        {
            this.Write(cell.ToString());
        }

        /// <summary>
        /// Writes an empty cell.
        /// </summary>
        public void Write()
        {
            this.Write((string)null);
        }

        /// <summary>
        /// Writes the specified value as the content of a cell.
        /// </summary>
        /// <param name="cell">The cell value to write. <c>null</c> will leave the cell empty.</param>
        public void Write( string cell )
        {
            if( this.IsDisposed )
                throw new ObjectDisposedException(string.Empty).StoreDefault();

            if( !this.firstCell )
                this.tw.Write(this.csvFormat.Separator);
            else
                this.firstCell = false;

            if( cell.NotNullReference() )
                this.tw.Write(this.ToCsvString(cell));
        }

        /// <summary>
        /// Starts a new record in the CSV file.
        /// </summary>
        public void WriteLine()
        {
            this.firstCell = true;
            this.tw.Write(this.csvFormat.NewLine);
        }

        #endregion
    }
}
