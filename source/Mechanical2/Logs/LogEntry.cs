using System;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.Logs
{
    /// <summary>
    /// Represents a log message.
    /// </summary>
    public sealed class LogEntry
    {
        #region Private Fields

        private readonly DateTime timestamp;
        private readonly LogLevel level;
        private readonly string message;
        private readonly ExceptionInfo exception;
        private readonly FileLine sourcePos;

        #endregion

        #region Constructors

        private LogEntry(
            DateTime timestamp,
            Enum<LogLevel> level,
            string message,
            ExceptionInfo exceptionInfo,
            FileLine sourcePos )
        {
            if( timestamp.Kind != DateTimeKind.Utc )
                throw new ArgumentException("Only UTC timestamps allowed!").Store("Kind", timestamp.Kind).Store("timestamp", timestamp);

            if( !level.IsDefined )
                throw new ArgumentException("Log level undefined!").Store("level", (LogLevel)level).Store("message", message).Store("exception", exceptionInfo);

            if( message.NullReference() )
                message = string.Empty;

            this.timestamp = timestamp;
            this.level = level;
            this.message = message;
            this.exception = exceptionInfo;
            this.sourcePos = sourcePos;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="level">The severity of a <see cref="LogEntry"/>.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exceptionInfo">The <see cref="ExceptionInfo"/> associated with the <see cref="LogEntry"/>; or <c>null</c>.</param>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public LogEntry(
            LogLevel level,
            string message,
            ExceptionInfo exceptionInfo = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
            : this(DateTime.UtcNow, level, message, exceptionInfo, new FileLine(file, member, line))
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the creation time of the <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The creation time of the <see cref="LogEntry"/>.</value>
        public DateTime Timestamp
        {
            get { return this.timestamp; }
        }

        /// <summary>
        /// Gets the severity of a <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The severity of a <see cref="LogEntry"/>.</value>
        public LogLevel Level
        {
            get { return this.level; }
        }

        /// <summary>
        /// Gets the log message.
        /// </summary>
        /// <value>The log message.</value>
        public string Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Gets the <see cref="ExceptionInfo"/> associated with the <see cref="LogEntry"/>; or <c>null</c>.
        /// </summary>
        /// <value>The <see cref="ExceptionInfo"/> associated with the <see cref="LogEntry"/>; or <c>null</c>.</value>
        public ExceptionInfo Exception
        {
            get { return this.exception; }
        }

        /// <summary>
        /// Gets the source file position of the caller.
        /// </summary>
        /// <value>The source file position of the caller.</value>
        public FileLine SourcePos
        {
            get { return this.sourcePos; }
        }

        #endregion

        #region Serializer

        /// <summary>
        /// The data store serializer of <see cref="LogEntry"/> instances.
        /// </summary>
        public class Serializer : IDataStoreObjectSerializer<LogEntry>,
                                  IDataStoreObjectDeserializer<LogEntry>
        {
            /* NOTE: Normally you'd want to simply depend on the serializers
             *       provided through the default magic bag.
             *       See notes on ExceptionInfo, on why we didn't do so this time.
             */

            /// <summary>
            /// The default instance of the class.
            /// </summary>
            public static readonly Serializer Default = new Serializer();

            private static class Keys
            {
                internal const string Timestamp = "Timestamp";
                internal const string Level = "Level";
                internal const string Message = "Message";
                internal const string HasException = "HasException";
                internal const string Exception = "Exception";
                internal const string FileName = "FileName";
                internal const string MemberName = "MemberName";
                internal const string LineNumber = "LineNumber";
            }

            /// <summary>
            /// Serializes to a data store object.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="writer">The data store writer to use.</param>
            public void Serialize( LogEntry obj, IDataStoreWriter writer )
            {
                if( obj.NullReference() )
                    throw new ArgumentNullException("obj").StoreFileLine();

                if( writer.NullReference() )
                    throw new ArgumentNullException("writer").StoreFileLine();

                writer.Write(Keys.Timestamp, obj.timestamp, BasicSerialization.DateTime.Default);
                writer.Write(Keys.Level, obj.Level.Wrap().ToString(), BasicSerialization.String.Default);
                writer.Write(Keys.Message, obj.Message, BasicSerialization.String.Default);

                bool hasException = obj.Exception.NotNullReference();
                writer.Write(Keys.HasException, hasException, BasicSerialization.Boolean.Default);
                if( hasException )
                    writer.Write(Keys.Exception, obj.Exception, ExceptionInfo.Serializer.Default);

                writer.Write(Keys.FileName, obj.SourcePos.File, BasicSerialization.String.Default);
                writer.Write(Keys.MemberName, obj.SourcePos.Member, BasicSerialization.String.Default);
                writer.Write(Keys.LineNumber, obj.SourcePos.Line, BasicSerialization.Int32.Default);
            }

            /// <summary>
            /// Deserializes a data store object.
            /// </summary>
            /// <param name="reader">The data store reader to use.</param>
            /// <returns>The deserialized object.</returns>
            public LogEntry Deserialize( IDataStoreReader reader )
            {
                if( reader.NullReference() )
                    throw new ArgumentNullException("reader").StoreFileLine();

                var timestamp = reader.Read(BasicSerialization.DateTime.Default, Keys.Timestamp);
                var level = Enum<LogLevel>.Parse(reader.Read(BasicSerialization.String.Default, Keys.Level));
                var message = reader.Read(BasicSerialization.String.Default, Keys.Message);

                var hasException = reader.Read(BasicSerialization.Boolean.Default, Keys.HasException);
                ExceptionInfo info;
                if( hasException )
                    info = reader.Read(ExceptionInfo.Serializer.Default);
                else
                    info = null;

                var file = reader.Read(BasicSerialization.String.Default, Keys.FileName);
                var member = reader.Read(BasicSerialization.String.Default, Keys.MemberName);
                var line = reader.Read(BasicSerialization.Int32.Default, Keys.LineNumber);

                return new LogEntry(timestamp, level, message, info, new FileLine(file, member, line));
            }
        }

        #endregion
    }
}
