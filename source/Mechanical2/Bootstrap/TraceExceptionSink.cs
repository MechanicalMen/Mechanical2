using System;
using System.Diagnostics;
using System.Text;
using Mechanical.Core;

namespace Mechanical.Bootstrap
{
#if !SILVERLIGHT
    /// <summary>
    /// Uses <see cref="Trace"/> to log unhandled exceptions.
    /// </summary>
#else
    /// <summary>
    /// Uses <see cref="Debug"/> to log unhandled exceptions.
    /// </summary>
#endif
    public sealed class TraceExceptionSink : IExceptionSink
    {
        /// <summary>
        /// Processes unhandled exceptions.
        /// </summary>
        /// <param name="exception">The unhandled exception to process.</param>
        public void Handle( Exception exception )
        {
            if( exception.NullReference() )
                return;

            // NOTE: SafeString should be fool-proof, but in case we encounter a bug in it
            //       at the worst moment, let's report it as well
            string str;
            try
            {
                str = "Unhandled exception caught:" + Environment.NewLine + SafeString.DebugPrint(exception);
            }
            catch( Exception safeStringException )
            {
                var sb = new StringBuilder();
                sb.AppendLine("Unhandled exception caught:");
                sb.AppendLine(exception.ToString());
                sb.AppendLine();
                sb.AppendLine("Error encountered while logging the exception:");
                sb.AppendLine(safeStringException.ToString());
                str = sb.ToString();
            }

#if !SILVERLIGHT
            // Unlike Debug, Trace remains in Release builds
            Trace.WriteLine(str);
            Trace.Flush();
#else
            Debug.WriteLine(str);
#endif
        }
    }
}
