using System;
using System.Globalization;
using System.Windows.Data;
using Mechanical.Core;

namespace Mechanical.MVVM
{
    /// <summary>
    /// A base class for binding converters.
    /// </summary>
    /// <typeparam name="TSource">The type of the binding source.</typeparam>
    /// <typeparam name="TTarget">The type of the binding target.</typeparam>
    /// <typeparam name="TParameter">The type of the converter parameter.</typeparam>
    public abstract class ConverterBase<TSource, TTarget, TParameter> : IValueConverter
    {
        #region IValueConverter

        object IValueConverter.Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
#if DEBUG
            try
            {
                return this.Convert((TSource)value, (TParameter)parameter, culture);
            }
            catch( Exception e )
            {
                e.NullReference(); // kepps the compiler from nagging abount unused variables
                System.Diagnostics.Debugger.Break();
                throw;
            }
#else
            return this.Convert((TSource)value, (TParameter)parameter, culture);
#endif
        }

        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
#if DEBUG
            try
            {
                return this.ConvertBack((TTarget)value, (TParameter)parameter, culture);
            }
            catch( Exception e )
            {
                e.NullReference(); // kepps the compiler from nagging abount unused variables
                System.Diagnostics.Debugger.Break();
                throw;
            }
#else
            return this.ConvertBack((TTarget)value, (TParameter)parameter, culture);
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public abstract TTarget Convert( TSource value, TParameter parameter, CultureInfo culture );

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public abstract TSource ConvertBack( TTarget value, TParameter parameter, CultureInfo culture );

        #endregion
    }

    /// <summary>
    /// A base class for binding converters.
    /// </summary>
    /// <typeparam name="TSource">The type of the binding source.</typeparam>
    /// <typeparam name="TTarget">The type of the binding target.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The two classes are very short, and are almost the same.")]
    public abstract class ConverterBase<TSource, TTarget> : IValueConverter
    {
        #region IValueConverter

        object IValueConverter.Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
#if DEBUG
            try
            {
                return this.Convert((TSource)value, culture);
            }
            catch( Exception e )
            {
                e.NullReference(); // kepps the compiler from nagging abount unused variables
                System.Diagnostics.Debugger.Break();
                throw;
            }
#else
            return this.Convert((TSource)value, culture);
#endif
        }

        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
#if DEBUG
            try
            {
                return this.ConvertBack((TTarget)value, culture);
            }
            catch( Exception e )
            {
                e.NullReference(); // kepps the compiler from nagging abount unused variables
                System.Diagnostics.Debugger.Break();
                throw;
            }
#else
            return this.ConvertBack((TTarget)value, culture);
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public abstract TTarget Convert( TSource value, CultureInfo culture );

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public abstract TSource ConvertBack( TTarget value, CultureInfo culture );

        #endregion
    }
}
