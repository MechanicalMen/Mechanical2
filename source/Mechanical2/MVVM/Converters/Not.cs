using System;
using System.Globalization;

namespace Mechanical.MVVM.Converters
{
    /// <summary>
    /// Negates a <see cref="Boolean"/> value.
    /// </summary>
    public class Not : ConverterBase<bool, bool>
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public override bool Convert( bool value, CultureInfo culture )
        {
            return !value;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public override bool ConvertBack( bool value, CultureInfo culture )
        {
            return !value;
        }
    }
}
