﻿using System;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores
{
    internal class DelegateSerialization<T> : IDataStoreObjectSerializer<T>,
                                              IDataStoreObjectDeserializer<T>
    {
        internal Action<T> WriteDelegate { get; set; }

        internal Func<T> ReadDelegate { get; set; }

        public void Serialize( T obj, IDataStoreWriter writer )
        {
            if( this.WriteDelegate.NullReference() )
                throw new InvalidOperationException("Write delegate not specified!").StoreFileLine();

            this.WriteDelegate(obj);
        }

        public T Deserialize( IDataStoreReader reader )
        {
            if( this.ReadDelegate.NullReference() )
                throw new InvalidOperationException("Read delegate not specified!").StoreFileLine();

            return this.ReadDelegate();
        }

        private static readonly ThreadLocal<DelegateSerialization<T>> Instance = new ThreadLocal<DelegateSerialization<T>>(() => new DelegateSerialization<T>());

        internal static DelegateSerialization<T> Default
        {
            get { return Instance.Value; }
        }
    }
}
