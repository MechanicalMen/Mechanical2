using System;
using System.Globalization;
using System.Windows;

namespace Mechanical.MVVM.Converters
{
    /// <summary>
    /// Determines visibility based on the binding target.
    /// </summary>
    public class BoolToVisibility : ConverterBase<bool, Visibility>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToVisibility"/> class.
        /// </summary>
        public BoolToVisibility()
            : base()
        {
            this.TrueValue = Visibility.Visible;
            this.FalseValue = Visibility.Collapsed;
        }

        /// <summary>
        /// Gets or sets the value returned on <c>true</c>.
        /// </summary>
        /// <value>The value returned on <c>true</c>.</value>
        public Visibility TrueValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value returned on <c>false</c>.
        /// </summary>
        /// <value>The value returned on <c>false</c>.</value>
        public Visibility FalseValue
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
        public override Visibility Convert( bool value, CultureInfo culture )
        {
            if( value )
                return this.TrueValue;
            else
                return this.FalseValue;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public override bool ConvertBack( Visibility value, CultureInfo culture )
        {
            if( value == this.TrueValue )
                return true;
            else
                return false;
        }
    }
}
