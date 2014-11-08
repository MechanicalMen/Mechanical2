using System;
using System.Windows;
using Mechanical.Bootstrap;
using Mechanical.Core;
using Mechanical.MVVM;

namespace Mechanical.Common.Bootstrap
{
    /// <summary>
    /// Uses a simple message box to inform the user that an unhandled exception occurred.
    /// </summary>
    public class MessageBoxExceptionSink : IExceptionSink
    {
        /// <summary>
        /// Processes unhandled exceptions.
        /// </summary>
        /// <param name="exception">The unhandled exception to process.</param>
        public void Handle( Exception exception )
        {
            if( exception.NullReference() )
                return;

            UI.Invoke(() =>
            {
                MessageBox.Show(
                    SafeString.DebugFormat("An unhandled exception of type '{0}' was caught and logged!", exception.GetType()),
                    "Unhandled exception!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            });
        }
    }
}
