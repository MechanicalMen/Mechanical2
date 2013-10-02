using System;
using System.Globalization;
using System.Windows;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM.Converters
{
    /// <summary>
    /// Converts the binding target to <see cref="Boolean"/>.
    /// </summary>
    public class NullToBool : ConverterBase<object, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullToBool"/> class.
        /// </summary>
        public NullToBool()
            : base()
        {
            this.NullValue = false;
            this.NotNullValue = true;
        }

        /// <summary>
        /// Gets or sets the value returned when the target is <c>null</c>.
        /// </summary>
        /// <value>The value returned when the target is <c>null</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "In this case, the boolean return type is not used to indicate something.")]
        public bool NullValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value returned when the target is not <c>null</c>.
        /// </summary>
        /// <value>The value returned when the target is not <c>null</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "In this case, the boolean return type is not used to indicate something.")]
        public bool NotNullValue
        {
            get;
            set;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public override bool Convert( object value, CultureInfo culture )
        {
            if( value.NullReference() )
                return this.NullValue;
            else
                return this.NotNullValue;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public override object ConvertBack( bool value, CultureInfo culture )
        {
            throw new InvalidOperationException().StoreFileLine();
        }
    }
}
