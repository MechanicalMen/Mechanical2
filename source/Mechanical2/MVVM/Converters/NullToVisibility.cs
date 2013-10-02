using System;
using System.Globalization;
using System.Windows;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM.Converters
{
    /// <summary>
    /// Determines visibility based on the binding target.
    /// </summary>
    public class NullToVisibility : ConverterBase<object, Visibility>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullToVisibility"/> class.
        /// </summary>
        public NullToVisibility()
            : base()
        {
            this.NullValue = Visibility.Collapsed;
            this.NotNullValue = Visibility.Visible;
        }

        /// <summary>
        /// Gets or sets the value returned when the target is <c>null</c>.
        /// </summary>
        /// <value>The value returned when the target is <c>null</c>.</value>
        public Visibility NullValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value returned when the target is not <c>null</c>.
        /// </summary>
        /// <value>The value returned when the target is not <c>null</c>.</value>
        public Visibility NotNullValue
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
        public override Visibility Convert( object value, CultureInfo culture )
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
        public override object ConvertBack( Visibility value, CultureInfo culture )
        {
            throw new InvalidOperationException().StoreFileLine();
        }
    }
}
