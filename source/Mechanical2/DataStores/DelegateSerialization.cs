using System;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores
{
    internal class DelegateSerialization<T> : IDataStoreObjectSerializer<T>,
                                              IDataStoreObjectDeserializer<T>
    {
        internal Action<T, IDataStoreWriter> WriteDelegate { get; set; }

        internal Func<string, IDataStoreReader, T> ReadDelegate { get; set; }

        public void Serialize( T obj, IDataStoreWriter writer )
        {
            if( this.WriteDelegate.NullReference() )
                throw new InvalidOperationException("Write delegate not specified!").StoreDefault();

            this.WriteDelegate(obj, writer);
        }

        public T Deserialize( string name, IDataStoreReader reader )
        {
            if( this.ReadDelegate.NullReference() )
                throw new InvalidOperationException("Read delegate not specified!").StoreDefault();

            return this.ReadDelegate(name, reader);
        }

        private static readonly ThreadLocal<DelegateSerialization<T>> Instance = new ThreadLocal<DelegateSerialization<T>>(() => new DelegateSerialization<T>());

        internal static DelegateSerialization<T> Default
        {
            get { return Instance.Value; }
        }
    }
}
