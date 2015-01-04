using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores
{
    /// <summary>
    /// An immutable and always loadable representation of an <see cref="Exception"/> (even when the exception's assembly is not loaded).
    /// </summary>
    public class ExceptionInfo
    {
        #region Private Fields

        // NOTE: we are making the pattern a bit more accepting than strictly necessary,
        //       in case a platform uses a slightly different format. The pattern matches:
        //       " in " + filePath + (newLine OR ":line " + digits + newLine)
#if !SILVERLIGHT
        private static readonly Regex StackTraceLineRegex = new Regex(@"(\s+in\s+)(.*)(($)|(\s*\:\s*line\s+\d+$))", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
#else
        private static readonly Regex StackTraceLineRegex = new Regex(@"(\s+in\s+)(.*)(($)|(\s*\:\s*line\s+\d+$))", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
#endif

        private readonly string type;
        private readonly string message;
        private readonly ListDictionary<string, string> store;
        private readonly ReadOnlyDictionary.Base<string, string> readOnlyStore;
        private readonly string stackTrace;
        private readonly List<ExceptionInfo> innerExceptions;
        private readonly ReadOnlyList.Base<ExceptionInfo> readOnlyInnerExceptions;

        #endregion

        #region Constructors

        private ExceptionInfo( Exception exception )
            : this(SafeString.DebugPrint(exception.GetType()), exception.Message, SanitizeStackTrace(exception.StackTrace))
        {
        }

        private ExceptionInfo( string type, string message, string stackTrace )
        {
            this.type = type.NullReference() ? string.Empty : type;
            this.message = message.NullReference() ? string.Empty : message;
            this.store = new ListDictionary<string, string>();
            this.readOnlyStore = new ReadOnlyDictionary.Wrapper<string, string>(this.store);
            this.stackTrace = stackTrace.NullReference() ? string.Empty : stackTrace;
            this.innerExceptions = new List<ExceptionInfo>();
            this.readOnlyInnerExceptions = new ReadOnlyList.Wrapper<ExceptionInfo>(this.innerExceptions);
        }

        private static string SanitizeStackTrace( string stackTrace )
        {
            //// NOTE: since we sanitize our partial stack traces already, this makes sense as well

            if( stackTrace.NullOrWhiteSpace() )
                return stackTrace;

            return StackTraceLineRegex.Replace(stackTrace, StackTraceMatchEvaluator);
        }

        private static string StackTraceMatchEvaluator( Match match )
        {
            try
            {
                return match.Groups[1].Value + ConditionsExtensions.SanitizeFilePath(match.Groups[2].Value) + match.Groups[3].Value;
            }
            catch
            {
                // something went wrong: keep everything
                // (we don't want to accidentally loose data)
                return match.ToString();
            }
        }

        /// <summary>
        /// Creates an <see cref="ExceptionInfo"/> from an <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">The exception to create an <see cref="ExceptionInfo"/> from.</param>
        /// <returns>A new <see cref="ExceptionInfo"/> instance.</returns>
        public static ExceptionInfo From( Exception exception )
        {
            if( exception.NullReference() )
                throw new NullReferenceException().StoreFileLine();

            var info = new ExceptionInfo(exception); // sanitize stack trace

            // add contents of Data, except for partial stack trace entries
            var partialStackTrace = new List<Tuple<int, string>>();
            foreach( var pair in exception.Retrieve() )
            {
                if( pair.Key.IndexOf(ConditionsExtensions.PartialStackTrace, StringComparison.Ordinal) == 0 )
                {
                    // do not add to Store
                    try
                    {
                        int index;
                        if( pair.Key.Length == ConditionsExtensions.PartialStackTrace.Length )
                            index = 0;
                        else
                            index = int.Parse(pair.Key.Substring(startIndex: ConditionsExtensions.PartialStackTrace.Length), NumberStyles.None, CultureInfo.InvariantCulture);
                        partialStackTrace.Add(Tuple.Create(index, pair.Value));
                    }
                    catch
                    {
                        //// bad name format? perhaps the user tried to add their own info?
                    }
                }
                else
                {
                    // not a partial stack trace
                    info.store.Add(pair.Key, pair.Value); // the value being null is perfectly fine for us, but not so much for the serializer
                }
            }

            // compile and add partial stack trace
            if( partialStackTrace.Count != 0 )
            {
                var sb = new StringBuilder();
                partialStackTrace.Sort(( x, y ) => x.Item1.CompareTo(y.Item1));
                for( int i = 0; ; ++i )
                {
                    if( i != partialStackTrace.Count - 1 )
                    {
                        sb.AppendLine(partialStackTrace[i].Item2);
                    }
                    else
                    {
                        sb.Append(partialStackTrace[i].Item2);
                        break;
                    }
                }
                info.store.Add(ConditionsExtensions.PartialStackTrace, sb.ToString());
            }

            if( exception.InnerException.NotNullReference() )
            {
                var aggregateException = exception as AggregateException;
                if( aggregateException.NotNullReference() )
                {
                    for( int i = 0; i < aggregateException.InnerExceptions.Count; ++i )
                        info.innerExceptions.Add(From(aggregateException.InnerExceptions[i]));
                }
                else
                {
                    info.innerExceptions.Add(From(exception.InnerException));
                }
            }

            return info;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the exception type.
        /// </summary>
        /// <value>The exception type.</value>
        public string Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        /// <value>The exception message.</value>
        public string Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Gets the string-string entries from the exception Data.
        /// </summary>
        /// <value>The string-string entries from the exception Data.</value>
        public ReadOnlyDictionary.Base<string, string> Store
        {
            get { return this.readOnlyStore; }
        }

        /// <summary>
        /// Gets the exception's stack trace.
        /// </summary>
        /// <value>The exception's stack trace.</value>
        public string StackTrace
        {
            get { return this.stackTrace; }
        }

        /// <summary>
        /// Gets the exception's partial stack trace (added manually using StoreFileLine).
        /// </summary>
        /// <value>The exception's partial stack trace.</value>
        public string PartialStackTrace
        {
            get
            {
                string value;
                if( this.Store.TryGetValue(ConditionsExtensions.PartialStackTrace, out value) )
                    return value;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="ExceptionInfo"/> that caused the current exception.
        /// </summary>
        /// <value>The <see cref="ExceptionInfo"/> that caused the current exception.</value>
        public ExceptionInfo InnerException
        {
            get
            {
                if( this.InnerExceptions.Count == 0 )
                    return null;
                else
                    return this.InnerExceptions[0]; // same as AggregateException
            }
        }

        /// <summary>
        /// Gets the inner exceptions of <see cref="AggregateException"/> instances.
        /// </summary>
        /// <value>The inner exceptions of <see cref="AggregateException"/> instances.</value>
        public ReadOnlyList.Base<ExceptionInfo> InnerExceptions
        {
            get { return this.readOnlyInnerExceptions; }
        }

        #endregion

        #region Printing

        private static void Append( StringBuilder sb, ExceptionInfo info )
        {
            sb.Append("Type: ");
            sb.Append(info.Type);

            if( !info.Message.NullOrWhiteSpace() )
            {
                sb.AppendLine();
                sb.Append("Message: ");
                sb.Append(info.Message);
            }

            int minStoreCount = info.Store.ContainsKey(ConditionsExtensions.PartialStackTrace) ? 1 : 0;
            if( info.Store.Count > minStoreCount )
            {
                sb.AppendLine();
                sb.Append("Store:"); // no newline here

                foreach( var pair in info.Store )
                {
                    if( string.Equals(pair.Key, ConditionsExtensions.PartialStackTrace, StringComparison.Ordinal) )
                        continue;

                    sb.AppendLine(); // newline here
                    sb.Append(' ', 2);
                    sb.Append(pair.Key); // valid data store name
                    sb.Append(" = ");
                    sb.Append(SafeString.DebugPrint(pair.Value));
                    //// no newline here
                }
            }

            if( !info.StackTrace.NullOrWhiteSpace() ) // this can actually happen
            {
                sb.AppendLine();
                sb.AppendLine();

                sb.AppendLine("StackTrace:");
                sb.Append(info.StackTrace);
            }

            var partialStackTrace = info.PartialStackTrace;
            if( !partialStackTrace.NullReference() )
            {
                sb.AppendLine();
                sb.AppendLine();

                sb.AppendLine(ConditionsExtensions.PartialStackTrace + ':');
                sb.Append(partialStackTrace);
            }

            if( info.InnerExceptions.Count != 0 )
            {
                if( info.InnerExceptions.Count == 1 )
                {
                    sb.AppendLine();
                    sb.AppendLine();

                    sb.AppendLine("InnerException:");
                    Append(sb, info.InnerExceptions[0]);
                }
                else
                {
                    for( int i = 0; i < info.InnerExceptions.Count; ++i )
                    {
                        sb.AppendLine();
                        sb.AppendLine();

                        sb.Append("InnerExceptions[");
                        sb.Append(i.ToString(CultureInfo.InvariantCulture));
                        sb.AppendLine("]:");
                        Append(sb, info.InnerExceptions[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the string representation of the exception.
        /// </summary>
        /// <returns>The string representation of the exception.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            Append(sb, this);
            return sb.ToString();
        }

        #endregion

        #region Serializer

        /// <summary>
        /// The data store serializer of <see cref="ExceptionInfo"/> instances.
        /// </summary>
        public class Serializer : IDataStoreObjectSerializer<ExceptionInfo>,
                                  IDataStoreObjectDeserializer<ExceptionInfo>
        {
            /* NOTE: Normally you'd want to simply depend on the serializers
             *       provided through the default magic bag.
             *       This time however we will explicitly pass each on ourselves.
             *       There are two main reasons for this:
             *         - We want it to be possible, to log errors from
             *       setting up the magic bag itself.
             *         - We want to format to be uniform across all applications,
             *       platforms, cultures, ... etc.
             */

            /// <summary>
            /// The default instance of the class.
            /// </summary>
            public static readonly Serializer Default = new Serializer();

            private static class Keys
            {
                internal const string Type = "Type";
                internal const string Message = "Message";
                internal const string Store = "Store";
                internal const string StackTrace = "StackTrace";
                internal const string InnerExceptions = "InnerExceptions";
            }

            private static string EncodeStoredValue( string str )
            {
                if( str.NullReference() )
                    return string.Empty;
                else
                    return '_' + str;
            }

            private static string DecodeStoredValue( string str )
            {
                if( str.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                if( str.Length == 0 )
                    return null;
                else
                    return str.Substring(startIndex: 1);
            }

            /// <summary>
            /// Serializes to a data store object.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The data store writer to use.</param>
            public void Serialize( ExceptionInfo obj, IDataStoreWriter writer )
            {
                if( obj.NullReference() )
                    throw new ArgumentNullException("obj").StoreFileLine();

                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(Keys.Type, obj.Type, BasicSerialization.Default);
                writer.Write(Keys.Message, obj.Message, BasicSerialization.Default);
                writer.Write(Keys.StackTrace, obj.StackTrace, BasicSerialization.Default);

                writer.Write(
                    Keys.Store,
                    obj,
                    info =>
                    {
                        foreach( var pair in info.Store )
                            writer.Write(pair.Key, EncodeStoredValue(pair.Value), BasicSerialization.Default);
                    });

                writer.Write(
                    Keys.InnerExceptions,
                    obj,
                    info =>
                    {
                        for( int i = 0; i < info.InnerExceptions.Count; ++i )
                            writer.Write("ie" + i.ToString(CultureInfo.InvariantCulture), info.InnerExceptions[i], Default);
                    });
            }

            /// <summary>
            /// Deserializes a data store object.
            /// </summary>
            /// <param name="reader">The data store reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public ExceptionInfo Deserialize( IDataStoreReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                // NOTE: normally you'd use reader.ReadString(...)
                var type = reader.Read((IDataStoreValueDeserializer<string>)BasicSerialization.Default, Keys.Type);
                var message = reader.Read((IDataStoreValueDeserializer<string>)BasicSerialization.Default, Keys.Message);
                var stackTrace = reader.Read((IDataStoreValueDeserializer<string>)BasicSerialization.Default, Keys.StackTrace);
                var info = new ExceptionInfo(type, message, stackTrace); // do not sanitize stack trace again

                reader.Read(
                    Keys.Store,
                    () =>
                    {
                        string key, value;
                        while( reader.Read()
                            && reader.IsValue() )
                        {
                            // NOTE: normally you'd use reader.DeserializeAsValue<string>(...)
                            value = reader.Deserialize((IDataStoreValueDeserializer<string>)BasicSerialization.Default, out key);
                            value = DecodeStoredValue(value);
                            info.store.Add(key, value);
                        }
                    });

                reader.Read(
                    Keys.InnerExceptions,
                    () =>
                    {
                        ExceptionInfo innerException;
                        while( reader.Read()
                            && reader.Token == DataStoreToken.ObjectStart )
                        {
                            innerException = reader.Deserialize(Default);
                            info.innerExceptions.Add(innerException);
                        }
                    });

                return info;
            }
        }

        #endregion
    }

    /// <content>
    /// <see cref="ExceptionInfo"/> extension methods.
    /// </content>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "It seems cleaner to put these extension methods next to this class.")]
    public static partial class DataStoresExtensions
    {
        /// <summary>
        /// Creates an <see cref="ExceptionInfo"/> from an <see cref="Exception"/>./// 
        /// </summary>
        /// <param name="exception">The exception to create an <see cref="ExceptionInfo"/> from.</param>
        /// <returns>A new <see cref="ExceptionInfo"/> instance.</returns>
        public static ExceptionInfo ToInfo( this Exception exception )
        {
            return ExceptionInfo.From(exception);
        }
    }
}
