using System;
using System.Globalization;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.FileFormats
{
    /// <summary>
    /// Specifies a CSV format.
    /// </summary>
    public class CsvFormat
    {
        #region Private Fields

        private readonly string separator;
        private readonly CultureInfo culture;
        private readonly string newLine;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormat"/> class.
        /// </summary>
        /// <param name="separator">The <see cref="string"/> separating the values.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> used to format numbers.</param>
        /// <param name="newLine">The <see cref="string"/> separating the records.</param>
        public CsvFormat( string separator, CultureInfo culture, string newLine )
        {
            Ensure.That(separator).NotNullOrEmpty();
            Ensure.That(culture).NotNull();
            Ensure.That(newLine).NotNullOrEmpty();

            this.separator = separator;
            this.culture = culture;
            this.newLine = newLine;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="string"/> separating the values.
        /// </summary>
        /// <value>The <see cref="string"/> separating the values.</value>
        public string Separator
        {
            get { return this.separator; }
        }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> used to format numbers.
        /// </summary>
        /// <value>The <see cref="CultureInfo"/> used to format numbers.</value>
        public CultureInfo Culture
        {
            get { return this.culture; }
        }

        /// <summary>
        /// Gets the <see cref="string"/> separating the records.
        /// </summary>
        /// <value>The <see cref="string"/> separating the records.</value>
        public string NewLine
        {
            get { return this.newLine; }
        }

        #endregion

        #region Public Static Members

        /// <summary>
        /// The &quot;american&quot; format (also more RFC4180 compliant).
        /// </summary>
        public static readonly CsvFormat International = new CsvFormat(separator: ",", culture: CultureInfo.InvariantCulture, newLine: "\r\n");

        /// <summary>
        /// The CSV format more commonly used in europe.
        /// </summary>
        public static readonly CsvFormat Continental = new CsvFormat(separator: ";", culture: new CultureInfo("de-DE"), newLine: "\r\n");

        /// <summary>
        /// Determines the <see cref="CsvFormat"/> based on the specified culture.
        /// </summary>
        /// <param name="culture">The culture to use; or <c>null</c> for the current culture.</param>
        /// <returns>The <see cref="CsvFormat"/> for the culture.</returns>
        public static CsvFormat ChooseFor( CultureInfo culture = null )
        {
            if( culture.NullReference() )
                culture = CultureInfo.CurrentCulture;

            if( string.Equals(culture.NumberFormat.NumberDecimalSeparator, ".", StringComparison.Ordinal) )
                return CsvFormat.International;
            else
                return CsvFormat.Continental;
        }

        #endregion
    }
}
