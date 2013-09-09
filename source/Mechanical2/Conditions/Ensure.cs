using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mechanical.Conditions
{
    //// NOTE: we want to be as independent of all other namespaces as possible.

    /// <summary>
    /// Ensures that conditions are met, before the next statement is reached.
    /// </summary>
    public static class Ensure
    {
        #region That, Debug

        /// <summary>
        /// Prepares the object for testing.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to test.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        /// <returns>Information about the object being tested.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> That<T>(
            T obj,
            [CallerFilePath] string filePath = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            return new ConditionContext<T>(obj, filePath, lineNumber);
        }

        /// <summary>
        /// Prepares the object for testing (only runs in DEBUG builds).
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to test.</param>
        /// <param name="tests">The validations to perform.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void Debug<T>(
            T obj,
            Action<IConditionContext<T>> tests,
            [CallerFilePath] string filePath = "",
            [CallerLineNumberAttribute] int lineNumber = 0 )
        {
            if( object.ReferenceEquals(tests, null) )
                throw new ArgumentNullException("tests");

            tests(That(obj, filePath, lineNumber));
        }

        #endregion
    }
}
