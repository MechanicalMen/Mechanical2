﻿using System;

namespace Mechanical.DataStores
{
    /// <summary>
    /// The tokens of a data store.
    /// </summary>
    public enum DataStoreToken
    {
        /// <summary>
        /// The reader is at a value.
        /// </summary>
        Value,

        /// <summary>
        /// The reader is at the start of an object.
        /// </summary>
        ObjectStart,

        /// <summary>
        /// The reader is at the end of an object.
        /// </summary>
        ObjectEnd,

        /// <summary>
        /// The reader is et the end of the data store, there is nothing more to read.
        /// </summary>
        DataStoreEnd,
    }
}
