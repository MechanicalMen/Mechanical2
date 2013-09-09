using System;
using System.Collections.Generic;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Collections
{
    /// <summary>
    /// Helps working with read-only dictionaries.
    /// </summary>
    /// <content>
    /// A base class for implementing <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
    /// </content>
    public static partial class ReadOnlyDictionary
    {
        //// NOTE: we do not create the key and value collections
        ////       at construction time, since inheriting types
        ////       may wish to use their own implementations.

        //// NOTE: for the same reason we have the GetKeys, GetValues methods,
        ////       which enable us to declare Keys, Values properties using our
        ////       own types.

        /// <summary>
        /// A base class for implementing <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
        public abstract class Base<TKey, TValue> : ReadOnlyCollection.Base<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>
        {
            #region KeyCollection

            /// <summary>
            /// A read-only collection that contains the keys in the read-only dictionary. 
            /// </summary>
            public class KeyCollection : ReadOnlyCollection.Base<TKey>
            {
                #region Private Fields

                private readonly IReadOnlyDictionary<TKey, TValue> dictionary;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="KeyCollection"/> class.
                /// </summary>
                /// <param name="dictionary">The dictionary to wrap.</param>
                public KeyCollection( IReadOnlyDictionary<TKey, TValue> dictionary )
                {
                    Ensure.That(dictionary).NotNull();

                    this.dictionary = dictionary;
                }

                #endregion

                #region ReadOnlyCollection.Base

                /// <summary>
                /// Returns an enumerator that iterates through the collection.
                /// </summary>
                /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
                public override IEnumerator<TKey> GetEnumerator()
                {
                    foreach( var pair in this.dictionary )
                        yield return pair.Key;
                }

                /// <summary>
                /// Gets the number of elements in the collection.
                /// </summary>
                /// <value>The number of elements in the collection.</value>
                public override int Count
                {
                    get { return this.dictionary.Count; }
                }

                /// <summary>
                /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
                /// </summary>
                /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
                /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
                public override bool Contains( TKey item )
                {
                    return this.dictionary.ContainsKey(item);
                }

                #endregion
            }

            #endregion

            #region ValueCollection

            /// <summary>
            /// A read-only collection that contains the values in the read-only dictionary. 
            /// </summary>
            public class ValueCollection : ReadOnlyCollection.Base<TValue>
            {
                #region Private Fields

                private readonly IReadOnlyDictionary<TKey, TValue> dictionary;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="ValueCollection"/> class.
                /// </summary>
                /// <param name="dictionary">The dictionary to wrap.</param>
                public ValueCollection( IReadOnlyDictionary<TKey, TValue> dictionary )
                {
                    Ensure.That(dictionary).NotNull();

                    this.dictionary = dictionary;
                }

                #endregion

                #region ReadOnlyCollection.Base

                /// <summary>
                /// Returns an enumerator that iterates through the collection.
                /// </summary>
                /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
                public override IEnumerator<TValue> GetEnumerator()
                {
                    foreach( var pair in this.dictionary )
                        yield return pair.Value;
                }

                /// <summary>
                /// Gets the number of elements in the collection.
                /// </summary>
                /// <value>The number of elements in the collection.</value>
                public override int Count
                {
                    get { return this.dictionary.Count; }
                }

                #endregion
            }

            #endregion


            #region Private Fields

            private KeyCollection keys;
            private ValueCollection values;

            #endregion

            #region IReadOnlyDictionary

            /// <summary>
            /// Gets an enumerable collection that contains the keys in the read-only dictionary.
            /// </summary>
            /// <value>An enumerable collection that contains the keys in the read-only dictionary.</value>
            public IEnumerable<TKey> Keys
            {
                get { return this.GetKeys(); }
            }

            /// <summary>
            /// Gets an enumerable collection that contains the values in the read-only dictionary.
            /// </summary>
            /// <value>An enumerable collection that contains the values in the read-only dictionary.</value>
            public IEnumerable<TValue> Values
            {
                get { return this.GetValues(); }
            }

            /// <summary>
            /// Determines whether the read-only dictionary contains an element that has the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><c>true</c> if the read-only dictionary contains an element that has the specified key; otherwise, <c>false</c>.</returns>
            public virtual bool ContainsKey( TKey key )
            {
                TValue value;
                return this.TryGetValue(key, out value);
            }

            /// <summary>
            /// Gets the value that is associated with the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <param name="value">The value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
            /// <returns>When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</returns>
            public virtual bool TryGetValue( TKey key, out TValue value )
            {
                var comparer = EqualityComparer<TKey>.Default;
                foreach( var pair in this )
                {
                    if( comparer.Equals(pair.Key, key) )
                    {
                        value = pair.Value;
                        return true;
                    }
                }

                value = default(TValue);
                return false;
            }

            /// <summary>
            /// Gets the element that has the specified key in the read-only dictionary.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>The element that has the specified key in the read-only dictionary.</returns>
            public virtual TValue this[TKey key]
            {
                get
                {
                    TValue value;
                    if( this.TryGetValue(key, out value) )
                        return value;
                    else
                        throw new KeyNotFoundException().Store("key", key);
                }
            }

            #endregion

            #region Protected Methods

            /// <summary>
            /// The implementation of IReadOnlyDictionary{TKey, TValue}.Keys.
            /// </summary>
            /// <returns>An enumerable collection that contains the keys in the read-only dictionary.</returns>
            protected virtual IReadOnlyCollection<TKey> GetKeys()
            {
                if( this.keys.NullReference() )
                    Interlocked.CompareExchange(ref this.keys, new KeyCollection(this), null);

                return this.keys;
            }

            /// <summary>
            /// The implementation of IReadOnlyDictionary{TKey, TValue}.Values.
            /// </summary>
            /// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
            protected virtual IReadOnlyCollection<TValue> GetValues()
            {
                if( this.values.NullReference() )
                    Interlocked.CompareExchange(ref this.values, new ValueCollection(this), null);

                return this.values;
            }

            #endregion
        }
    }
}
