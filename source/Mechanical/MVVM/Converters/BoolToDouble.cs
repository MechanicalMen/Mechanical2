using System;
using System.Globalization;

namespace Mechanical.MVVM.Converters
{
    /// <summary>
    /// Converts a <see cref="Boolean"/> value into a <see cref="Double"/>.
    /// </summary>
    public class BoolToDouble : ConverterBase<bool, double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToDouble"/> class.
        /// </summary>
        public BoolToDouble()
            : base()
        {
            this.TrueValue = 1d;
            this.FalseValue = 0d;
        }

        /// <summary>
        /// Gets or sets the value returned on <c>true</c>.
        /// </summary>
        /// <value>The value returned on <c>true</c>.</value>
        public double TrueValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value returned on <c>false</c>.
        /// </summary>
        /// <value>The value returned on <c>false</c>.</value>
        public double FalseValue
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
        public override double Convert( bool value, CultureInfo culture )
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
        public override bool ConvertBack( double value, CultureInfo culture )
        {
            var trueDist = Math.Abs(this.TrueValue - value);
            var falseDist = Math.Abs(this.FalseValue - value);

            if( trueDist > falseDist )
                return true;
            else
                return false;
        }
    }
}
