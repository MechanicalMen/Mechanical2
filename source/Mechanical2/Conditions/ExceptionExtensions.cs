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
        ////       Retrieve() gets the stored data in a type-safe manner.
        ////       (storing strings should keep exceptions serializable)

        //// NOTE: The first call to Store or StoreDefault adds default information as well (e.g. file and line info, ... etc.)

        //// NOTE: We throw exceptions in debug mode, if the specified key is unacceptable;
        ////       but generate unique (DatStore compatible) IDs in release mode: not knowing
        ////       what it's called is still better than not knowing anything at all. (or at least, it's not worse :) 

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
            foreach( DictionaryEntry entry in e.Data )
            {
                string str = entry.Key as string;
                if( str.NotNullReference()
                 && DataStore.SameNames(str, key) )
                {
                    if( (entry.Value.NullReference() && value.NullReference())
                     || string.Equals(entry.Value as string, value, StringComparison.Ordinal) )
                        return SearchResult.KeyAndValueFound;
                    else
                        return SearchResult.KeyFound;
                }
            }

            return SearchResult.NotFound;
        }

#if !DEBUG
        private static string GenerateKey()
        {
            return "GUID_" + Guid.NewGuid().ToString("N");
        }
#endif

        private static TException Add<TException>( this TException e, string key, object value, bool forceNewKey = false )
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
            var stringValue = value as string;
            if( stringValue.NullReference()
             && value.NotNullReference() )
            {
                if( value is char
                 || value is Substring )
                    stringValue = value.ToString();
                else
                    stringValue = SafeString.DebugPrint(value);
            }

            // we do not allow overwriting of earlier exception data
            // and we also avoid data duplication (though we are very conservative about detecting it)
            string actualKey = key;
            int i = 2;
            bool done = false;
            while( !done )
            {
                switch( Contains(e, actualKey, stringValue) )
                {
                case SearchResult.KeyAndValueFound:
                    // already added both:
                    if( forceNewKey )
                    {
                        actualKey = key + i.ToString(CultureInfo.InvariantCulture);
                        ++i;
                    }
                    else
                    {
                        // skip
                        return e;
                    }
                    break;

                case SearchResult.KeyFound:
                    // same key, different value: try again with another key
                    actualKey = key + i.ToString(CultureInfo.InvariantCulture);
                    ++i;
                    break;

                default:
                    // unused key found
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
            return e.StoreFileLine_OnFirstCall(filePath, memberName, lineNumber)
                    .Add(key, value);
        }

        #endregion

        #region StoreFileLine, Store( IConditionContext )

        internal const string File = "SourceFile";
        internal const string Member = "SourceMember";
        internal const string Line = "SourceLine";

        private static readonly char[] DirectorySeparatorChars = new char[] { '\\', '/' };

        internal static string SanitizeFilePath( [CallerFilePath] string filePath = "" )
        {
            // let's not expose the developer's directory structure!
            // (may contain sensitive information, like user names, ... etc.)
            if( filePath.NotNullReference() )
            {
                // System.IO.Path expects the directory separators 
                // of the platform this code is being run on. But code may
                // have been compiled on a different platform! (e.g. building Android apps on Windows)
                int directorySeparatorAt = filePath.LastIndexOfAny(DirectorySeparatorChars);
                if( directorySeparatorAt != -1 )
                    return filePath.Substring(startIndex: directorySeparatorAt + 1);
            }

            // no directory separator?! null?!
            // only if this string was not generated by the compiler!
            // In that case, let the user deal with whatever this is
            return filePath;
        }

        /// <summary>
        /// Stores the current source file position. If they are already present, then - unlike other Store calls - duplicates will(!) be produced.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="e">The exception to store data in.</param>
        /// <param name="filePath">The full path of the source file that contains the caller.</param>
        /// <param name="memberName">The method or property name of the caller to the method.</param>
        /// <param name="lineNumber">The line number in the source file at which the method is called.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException StoreFileLine<TException>(
            this TException e,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
            where TException : Exception
        {
            return e.Add(File, SanitizeFilePath(filePath), forceNewKey: true)
                    .Add(Member, memberName, forceNewKey: true)
                    .Add(Line, lineNumber, forceNewKey: true);
        }

        private static TException StoreFileLine_OnFirstCall<TException>( this TException e, string filePath, string memberName, int lineNumber )
            where TException : Exception
        {
            if( Contains(e, File, null) == SearchResult.NotFound )
            {
                return e.Add(File, SanitizeFilePath(filePath))
                        .Add(Member, memberName)
                        .Add(Line, lineNumber);
            }
            else
            {
                return e;
            }
        }

        /// <summary>
        /// Stores data from the specified <see cref="IConditionContext{T}"/>.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T">The type of the object being validated.</typeparam>
        /// <param name="e">The exception to store data in.</param>
        /// <param name="context">The <see cref="IConditionContext{T}"/> instance to store.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException Store<TException, T>( this TException e, IConditionContext<T> context )
            where TException : Exception
        {
            return e.StoreFileLine(context.FilePath, context.MemberName, context.LineNumber)
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
