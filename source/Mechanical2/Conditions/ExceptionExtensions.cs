using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
                 && DataStore.Comparer.Equals(str, key) )
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
                var newKey = DataStore.GenerateHash();
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
                        actualKey = key + i.ToString("D", CultureInfo.InvariantCulture);
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
                    actualKey = key + i.ToString("D", CultureInfo.InvariantCulture);
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
        /// <param name="key">The data key. Must be DataStore compatible.</param>
        /// <param name="value">The data value.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException Store<TException>(
            this TException e,
            string key,
            object value,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
            where TException : Exception
        {
            return e.StoreFileLine_OnFirstCall(file, member, line)
                    .Add(key, value);
        }

        #endregion

        #region StoreFileLine, Store( IConditionContext )

        internal const string PartialStackTrace = "PartialStackTrace";

        /// <summary>
        /// Stores the specified source file position. If they are already present, then - unlike other Store calls - duplicates will(!) be produced.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="e">The exception to store data in.</param>
        /// <param name="sourcePos">The source file position to add.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException StoreFileLine<TException>( this TException e, FileLine sourcePos )
            where TException : Exception
        {
            return e.Add(PartialStackTrace, sourcePos.ToString(), forceNewKey: true);
        }

        /// <summary>
        /// Stores the current source file position. If they are already present, then - unlike other Store calls - duplicates will(!) be produced.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="e">The exception to store data in.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        /// <returns>The exception data was stored in.</returns>
        public static TException StoreFileLine<TException>(
            this TException e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
            where TException : Exception
        {
            return StoreFileLine(e, new FileLine(file, member, line));
        }

        private static TException StoreFileLine_OnFirstCall<TException>( this TException e, string file, string member, int line )
            where TException : Exception
        {
            if( Contains(e, PartialStackTrace, null) == SearchResult.NotFound )
            {
                return e.Add(PartialStackTrace, new FileLine(file, member, line).ToString());
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
            return StoreFileLine(e, context.SourcePos)
                    .Add(context.ObjectName, context.Object);
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
