using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.Conditions
{
    //// NOTE: we want to be as independent of all other namespaces as possible.

    /// <content>
    /// Methods extending the <see cref="Exception"/> type.
    /// </content>
    public static partial class CoreExtensions
    {
        //// NOTE: Store() adds the string representation of the specified object to Exception.Data using a DataStore compatible key.
        ////       Retrieve() the stored data in a type-safe manner.
        ////       (storing strings should keep exceptions serializable)

        //// NOTE: The first call to Store or StoreDefault adds default information as well (e.g. file and line info, ... etc.)

        //// NOTE: We throw exceptions in debug mode, if the specified key is unacceptable;
        ////       but generate unique (DS compatible) ids in release mode: not knowing
        ////       what it's called is still better than not knowing anything at all! 

        #region Store (general use)

        private static bool ContainsKey( Exception e, string key )
        {
            foreach( DictionaryEntry entry in e.Data )
            {
                string str = entry.Key as string;
                if( str.NotNullReference()
                 && DataStore.NameEquals(str, key) )
                    return true;
            }

            return false;
        }

#if !DEBUG
        private static string GenerateKey()
        {
            return '_' + Guid.NewGuid().ToString("N");
        }
#endif

        private static TException Add<TException>( this TException e, string key, object value )
            where TException : Exception
        {
#if DEBUG
            if( e.NullReference() )
                throw new ArgumentNullException("e");

            if( !DataStore.ValidName(key) )
                throw new ArgumentException("Invalid key!");
#else
            if( e.NullReference() )
                return null;

            if( !DataStore.ValidName(key) )
                key = GenerateKey();
#endif
            // we're not allowing overwriting of earlier exception data
            string actualKey = key;
            int i = 2;
            while( ContainsKey(e, actualKey) )
            {
                actualKey = key + i.ToString(CultureInfo.InvariantCulture);
                ++i;
            }

            e.Data.Add(actualKey, SafeString.DebugPrint(value));
            return e;
        }

        /// <summary>
        /// Stores the specified data in the exception.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="e">The exception to store data in.</param>
        /// <param name="key">The data key.</param>
        /// <param name="value">The data value.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException Store<TException>(
            this TException e,
            string key,
            object value,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0 )
            where TException : Exception
        {
            // make sure the default informations are stored
            return e.StoreDefault(filePath, lineNumber)
                    .Add(key, value);
        }

        #endregion

        #region Store (first call)

        /// <summary>
        /// Stores default data in the exception.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="e">The exception to store data in.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException StoreDefault<TException>(
            this TException e,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0 )
            where TException : Exception
        {
#if DEBUG
            if( e.NullReference() )
                throw new ArgumentNullException("e");
#endif

            if( !string.IsNullOrEmpty(filePath)
             && !ContainsKey(e, "SourceFile") )
            {
                // let's not expose the developer's directory structure!
                // (may contain sensitive information, like user names, ... etc.)
                string file;
                try
                {
                    file = Path.GetFileName(filePath);
                }
                catch
                {
                    // invalid character?! null?!
                    // only if this string was not generated by the compiler!
                    file = filePath;
                }

                return e.Add("SourceFile", file)
                        .Add("SourceLine", lineNumber); // add other data here...
            }
            else
            {
                return e;
            }
        }

        /// <summary>
        /// Stores default data in the exception.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T">The type of the object being validated.</typeparam>
        /// <param name="e">The exception to store data in.</param>
        /// <param name="context">The <see cref="IConditionContext{T}"/> instance to store.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException StoreDefault<TException, T>( this TException e, IConditionContext<T> context )
            where TException : Exception
        {
            return e.StoreDefault(context.FilePath, context.LineNumber)
                    .Add("Object", context.Object);
        }

        #endregion

        #region Retrieve

        /// <summary>
        /// Returns an enumeration of only the string-string entries of <see cref="P:Data"/>.
        /// </summary>
        /// <param name="e">The exception to work with.</param>
        /// <returns>An enumeration of string-string pairs.</returns>
        public static IEnumerable<KeyValuePair<string, string>> Retrieve( this Exception e )
        {
#if DEBUG
            if( e.NullReference() )
                throw new ArgumentNullException("e");
#endif

            foreach( DictionaryEntry entry in e.Data )
            {
                string key = entry.Key as string;
                string value = entry.Value as string;
                if( key.NotNullReference()
                 && value.NotNullReference() )
                    yield return new KeyValuePair<string, string>(key, value);
            }
        }

        #endregion
    }
}
