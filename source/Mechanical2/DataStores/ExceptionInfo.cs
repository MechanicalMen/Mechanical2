using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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

        private readonly string type;
        private readonly string message;
        private readonly ListDictionary<string, string> store;
        private readonly ReadOnlyDictionary.Base<string, string> readOnlyStore;
        private readonly string stackTrace;
        private readonly List<ExceptionInfo> innerExceptions;
        private readonly ReadOnlyList.Base<ExceptionInfo> readOnlyInnerExceptions;

        #endregion

        #region Constructors

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

        /// <summary>
        /// Creates an <see cref="ExceptionInfo"/> from an <see cref="Exception"/>.
        /// </summary>
        /// <param name="exception">The exception to create an <see cref="ExceptionInfo"/> from.</param>
        /// <returns>A new <see cref="ExceptionInfo"/> instance.</returns>
        public static ExceptionInfo From( Exception exception )
        {
            if( exception.NullReference() )
                throw new NullReferenceException().StoreDefault();

            var info = new ExceptionInfo(
                SafeString.DebugPrint(exception.GetType()),
                exception.Message,
                exception.StackTrace);

            foreach( var pair in exception.Retrieve() )
                info.store.Add(pair.Key, pair.Value); // the value being null is perfectly fine for us, but not so much for the serializer

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

            if( info.Store.Count != 0 )
            {
                sb.AppendLine();
                sb.Append("Store:"); // no newline here

                foreach( var pair in info.Store )
                {
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
                    throw new ArgumentNullException().StoreDefault();

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
                    throw new ArgumentNullException("obj").StoreDefault();

                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreDefault();

                writer.Write(Keys.Type, obj.Type);
                writer.Write(Keys.Message, obj.Message);
                writer.Write(Keys.StackTrace, obj.StackTrace);

                writer.Write(
                    Keys.Store,
                    obj,
                    ( info, w ) =>
                    {
                        foreach( var pair in info.Store )
                            w.Write(pair.Key, EncodeStoredValue(pair.Value));
                    });

                writer.Write(
                    Keys.InnerExceptions,
                    obj,
                    ( info, w ) =>
                    {
                        for( int i = 0; i < info.InnerExceptions.Count; ++i )
                            w.Write("ie" + i.ToString(CultureInfo.InvariantCulture), info.InnerExceptions[i], Default);
                    });
            }

            /// <summary>
            /// Deserializes a data store object.
            /// </summary>
            /// <param name="name">The name of the serialized object.</param>
            /// <param name="reader">The data store reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public ExceptionInfo Deserialize( string name, IDataStoreReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreDefault();

                var type = reader.ReadString(Keys.Type);
                var message = reader.ReadString(Keys.Message);
                var stackTrace = reader.ReadString(Keys.StackTrace);
                var info = new ExceptionInfo(type, message, stackTrace);

                reader.Read(
                    Keys.Store,
                    r =>
                    {
                        string key, value;
                        while( r.Token != DataStoreToken.ObjectEnd )
                        {
                            value = r.ReadString(out key);
                            value = DecodeStoredValue(value);
                            info.store.Add(key, value);
                        }
                    });

                reader.Read(
                    Keys.InnerExceptions,
                    r =>
                    {
                        ExceptionInfo innerException;
                        while( r.Token != DataStoreToken.ObjectEnd )
                        {
                            innerException = r.Read(Default);
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
