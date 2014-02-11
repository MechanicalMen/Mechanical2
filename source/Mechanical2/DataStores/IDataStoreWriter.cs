using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores.Node;
using Mechanical.IO;
using Mechanical.MagicBag;

namespace Mechanical.DataStores
{
    /// <summary>
    /// A data store writer.
    /// </summary>
    public interface IDataStoreWriter
    {
        /// <summary>
        /// Writes a BinaryValue or TextValue token. The writer implementation chooses which one.
        /// There is exactly one - binary or text - reader for each value.
        /// </summary>
        /// <param name="name">The name of the data store value.</param>
        /// <param name="binaryWriter">The <see cref="IBinaryWriter"/> that can be used to write the value; or <c>null</c>.</param>
        /// <param name="textWriter">The <see cref="ITextWriter"/> that can be used to write the value; or <c>null</c>.</param>
        void WriteValue( string name, out IBinaryWriter binaryWriter, out ITextWriter textWriter );

        /// <summary>
        /// Writes an ObjectStart token.
        /// </summary>
        /// <param name="name">The name of the data store object.</param>
        void WriteObjectStart( string name );

        /// <summary>
        /// Writes an ObjectEnd token.
        /// </summary>
        void WriteObjectEnd();
    }

    /// <content>
    /// <see cref="IDataStoreWriter"/> extension methods.
    /// </content>
    public static partial class DataStoresExtensions
    {
        #region ThrowIfNull

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [Conditional("DEBUG")]
        private static void ThrowIfNull(
            IDataStoreWriter writer,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0 )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException("writer").StoreFileLine(filePath, memberName, lineNumber);
        }

        #endregion

        #region Write( name, obj, serializer )

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The serializer to use.</param>
        public static void Write<T>( this IDataStoreWriter writer, string name, T obj, IDataStoreValueSerializer<T> serializer )
        {
            ThrowIfNull(writer);

            if( serializer.NullReference() )
                throw new ArgumentNullException("serializer").StoreFileLine();

            IBinaryWriter binaryWriter;
            ITextWriter textWriter;
            writer.WriteValue(name, out binaryWriter, out textWriter);
            if( binaryWriter.NotNullReference() )
                serializer.Serialize(obj, binaryWriter);
            else if( textWriter.NotNullReference() )
                serializer.Serialize(obj, textWriter);
            else
                throw new Exception("Both data store value writers are null!").StoreFileLine();
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The serializer to use.</param>
        public static void Write<T>( this IDataStoreWriter writer, string name, T obj, IDataStoreObjectSerializer<T> serializer )
        {
            ThrowIfNull(writer);

            if( serializer.NullReference() )
                throw new ArgumentNullException("serializer").StoreFileLine();

            writer.WriteObjectStart(name);
            serializer.Serialize(obj, writer);
            writer.WriteObjectEnd();
        }

        #endregion

        #region WriteAsValue, WriteAsObject

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void WriteAsValue<T>( this IDataStoreWriter writer, string name, T obj, IMagicBag magicBag = null )
        {
            ThrowIfNull(writer);

            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var serializer = magicBag.Pull<IDataStoreValueSerializer<T>>();
            Write<T>(writer, name, obj, serializer);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void WriteAsObject<T>( this IDataStoreWriter writer, string name, T obj, IMagicBag magicBag = null )
        {
            ThrowIfNull(writer);

            if( magicBag.NullReference() )
                magicBag = Mechanical.MagicBag.MagicBag.Default;

            var serializer = magicBag.Pull<IDataStoreObjectSerializer<T>>();
            Write<T>(writer, name, obj, serializer);
        }

        #endregion

        #region Basic types

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, sbyte obj, IMagicBag magicBag = null )
        {
            WriteAsValue<sbyte>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, byte obj, IMagicBag magicBag = null )
        {
            WriteAsValue<byte>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, short obj, IMagicBag magicBag = null )
        {
            WriteAsValue<short>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, ushort obj, IMagicBag magicBag = null )
        {
            WriteAsValue<ushort>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, int obj, IMagicBag magicBag = null )
        {
            WriteAsValue<int>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, uint obj, IMagicBag magicBag = null )
        {
            WriteAsValue<uint>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, long obj, IMagicBag magicBag = null )
        {
            WriteAsValue<long>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, ulong obj, IMagicBag magicBag = null )
        {
            WriteAsValue<ulong>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, float obj, IMagicBag magicBag = null )
        {
            WriteAsValue<float>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, double obj, IMagicBag magicBag = null )
        {
            WriteAsValue<double>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, decimal obj, IMagicBag magicBag = null )
        {
            WriteAsValue<decimal>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, bool obj, IMagicBag magicBag = null )
        {
            WriteAsValue<bool>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, char obj, IMagicBag magicBag = null )
        {
            WriteAsValue<char>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, string obj, IMagicBag magicBag = null )
        {
            WriteAsValue<string>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, Substring obj, IMagicBag magicBag = null )
        {
            WriteAsValue<Substring>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, DateTime obj, IMagicBag magicBag = null )
        {
            WriteAsValue<DateTime>(writer, name, obj, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, string name, TimeSpan obj, IMagicBag magicBag = null )
        {
            WriteAsValue<TimeSpan>(writer, name, obj, magicBag);
        }

        #endregion

        #region Nodes

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="node">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, IDataStoreValue node, IMagicBag magicBag = null )
        {
            if( node.NullReference() )
                throw new ArgumentNullException().Store("node", node);

            WriteAsValue<IDataStoreValue>(writer, node.Name, node, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="node">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, IDataStoreObject node, IMagicBag magicBag = null )
        {
            if( node.NullReference() )
                throw new ArgumentNullException().Store("node", node);

            WriteAsObject<IDataStoreObject>(writer, node.Name, node, magicBag);
        }

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="node">The object to serialize.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to use for serialization; or <c>null</c> for the default magic bag.</param>
        public static void Write( this IDataStoreWriter writer, IDataStoreNode node, IMagicBag magicBag = null )
        {
            if( node.NullReference() )
                throw new ArgumentNullException().Store("node", node);

            var value = node as IDataStoreValue;
            if( value.NotNullReference() )
            {
                Write(writer, value, magicBag);
            }
            else
            {
                var obj = node as IDataStoreObject;
                if( obj.NotNullReference() )
                    Write(writer, obj, magicBag);
                else
                    throw new ArgumentException("Invalid data store node type!").Store("nodeType", node.GetType());
            }
        }

        #endregion

        #region Delegate

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <typeparam name="T">The type of object to write.</typeparam>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="action">The delegate performing the actual serialization.</param>
        public static void Write<T>( this IDataStoreWriter writer, string name, T obj, Action<T> action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException("action").Store("name", name).Store("obj", obj);

            var serializer = DelegateSerialization<T>.Default;
            serializer.WriteDelegate = action;
            Write<T>(writer, name, obj, serializer);
        }

        private static readonly object Dummy = new object();

        /// <summary>
        /// Writes an object to the data store.
        /// </summary>
        /// <param name="writer">The <see cref="IDataStoreWriter"/> to use.</param>
        /// <param name="name">The name of the serialized object.</param>
        /// <param name="action">The delegate performing the actual serialization.</param>
        public static void Write( this IDataStoreWriter writer, string name, Action action )
        {
            if( action.NullReference() )
                throw new ArgumentNullException("action").Store("name", name);

            var serializer = DelegateSerialization<object>.Default;
            serializer.WriteDelegate = obj => action();
            Write<object>(writer, name, Dummy, serializer);
        }

        #endregion
    }
}
