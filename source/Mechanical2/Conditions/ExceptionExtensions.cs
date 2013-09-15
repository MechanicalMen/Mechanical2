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
    public static partial class ConditionsExtensions
    {
        //// NOTE: Store() adds the string representation of the specified object to Exception.Data using a DataStore compatible key.
        ////       Retrieve() the stored data in a type-safe manner.
        ////       (storing strings should keep exceptions serializable)

        //// NOTE: The first call to Store or StoreDefault adds default information as well (e.g. file and line info, ... etc.)

        //// NOTE: We throw exceptions in debug mode, if the specified key is unacceptable;
        ////       but generate unique (DS compatible) ids in release mode: not knowing
        ////       what it's called is still better than not knowing anything at all! 

        #region Store (general use)

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "The enumeration is private, and self explanatory.")]
        private enum SearchResult
        {
            KeyAndValueFound,
            KeyFound,
            NotFound
        }

        private static SearchResult Contains( Exception e, string key, string value )
        {
            bool keyFound = false;
            foreach( DictionaryEntry entry in e.Data )
            {
                string str = entry.Key as string;
                if( str.NotNullReference()
                 && DataStore.SameNames(str, key) )
                {
                    keyFound = true;

                    if( (entry.Value.NullReference() && value.NullReference())
                     || string.Equals(entry.Value as string, value, StringComparison.Ordinal) )
                        return SearchResult.KeyAndValueFound;
                }
            }

            return keyFound ? SearchResult.KeyFound : SearchResult.NotFound;
        }

        private static bool ContainsKey( Exception e, string key )
        {
            foreach( DictionaryEntry entry in e.Data )
            {
                string str = entry.Key as string;
                if( str.NotNullReference()
                 && DataStore.SameNames(str, key) )
                    return true;
            }

            return false;
        }

#if !DEBUG
        private static string GenerateKey()
        {
            return "GUID_" + Guid.NewGuid().ToString("N");
        }
#endif

        private static TException Add<TException>( this TException e, string key, object value )
            where TException : Exception
        {
            if( e.NullReference() )
                throw new ArgumentNullException("e");
#if DEBUG
            if( !DataStore.IsValidName(key) )
                throw new ArgumentException("Invalid key!");
#else
            if( !DataStore.IsValidName(key) )
            {
                var newKey = GenerateKey();
                Add(e, newKey + "_OriginalKey", key);
                key = newKey;
            }
#endif
            // convert 'value' to string
            // no quotes for now
            string stringValue;
            if( value is string )
                stringValue = (string)value;
            else if( value is char
                  || value is Substring )
                stringValue = value.ToString();
            else
                stringValue = SafeString.DebugPrint(value);

            // we do not allow overwriting of earlier exception data
            // and we also avoid data duplication (though we are very conservative about it)
            string actualKey = key;
            int i = 2;
            bool done = false;
            while( !done )
            {
                switch( Contains(e, actualKey, stringValue) )
                {
                case SearchResult.KeyAndValueFound:
                    // already added both: skip
                    return e;

                case SearchResult.KeyFound:
                    // same key, different value: try again with another key
                    actualKey = key + i.ToString(CultureInfo.InvariantCulture);
                    ++i;
                    break;

                default:
                    // key not found: add
                    done = true;
                    break;
                }
            }

            e.Data.Add(actualKey, stringValue); // value being null is perfectly valid
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
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException Store<TException>(
            this TException e,
            string key,
            object value,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
            where TException : Exception
        {
            // make sure the default informations are stored
            return e.StoreDefault(filePath, memberName, lineNumber)
                    .Add(key, value);
        }

        #endregion

        #region Store (first call)

        internal static string SanitizeFilePath( [CallerFilePath] string filePath = "" )
        {
            // let's not expose the developer's directory structure!
            // (may contain sensitive information, like user names, ... etc.)
            try
            {
                return Path.GetFileName(filePath);
            }
            catch
            {
                // invalid character?! null?!
                // only if this string was not generated by the compiler!
                return filePath;
            }
        }

        /// <summary>
        /// Stores default data in the exception.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="e">The exception to store data in.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException StoreDefault<TException>(
            this TException e,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
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
                //// NOTE: Instead of using ContainsKey, we could simply always 'Store' values
                ////       since duplicates are not saved.
                ////       We don't do this, in case at a later time, we want to add some more resource
                ////       intensive (default) data here (e.g. user or network data; ... etc.)

                return e.Add("SourceFile", SanitizeFilePath(filePath))
                        .Add("SourceMember", memberName)
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
            return e.StoreDefault(context.FilePath, context.MemberName, context.LineNumber)
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
                if( key.NotNullReference()
                 && (entry.Value.NullReference() || (entry.Value is string)) )
                    yield return new KeyValuePair<string, string>(key, (string)entry.Value);
            }
        }

        #endregion
    }
}
