using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Collections
{
    /// <summary>
    /// Helps working with dictionaries.
    /// </summary>
    /// <content>
    /// An abstract base class for implementing <see cref="IDictionary{TKey, TValue}"/>.
    /// </content>
    public static partial class Dictionary
    {
        //// NOTE: we do not create the key and value collections
        ////       at construction time, since inheriting types
        ////       may wish to use their own implementations.

        //// NOTE: for the same reason we have the GetKeys, GetValues abstract methods,
        ////       which enable inheritors to both override what our Keys, Values properties
        ////       return, as well as create "new" properties, with their own return types,
        ////       to replace them.

        //// NOTE: IReadOnlyCollection is a better fit for the Keys and Values properties.
        ////       This is evident from the implementation of Dictionary<,>. It was not
        ////       available at the creation of Dictionary, but it is now.
        ////       We therefore expect an IReadOnlyCollection from inheritors, and wrap
        ////       it silently for the IDictionary interface.

        /// <summary>
        /// An abstract base class for implementing <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        public abstract class Base<TKey, TValue> : Collection.Base<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
        {
            #region ReadOnlyCollectionWrapper

            private class ReadOnlyCollectionWrapper<T> : ICollection<T>
            {
                #region Internal Fields

                internal readonly IReadOnlyCollection<T> ReadOnlyCollection;

                internal static readonly ReadOnlyCollectionWrapper<T> NullWrapper = new ReadOnlyCollectionWrapper<T>();

                #endregion

                #region Constructors

                private ReadOnlyCollectionWrapper()
                {
                    this.ReadOnlyCollection = null;
                }

                internal ReadOnlyCollectionWrapper( IReadOnlyCollection<T> readOnlyCollection )
                {
                    Ensure.That(readOnlyCollection).NotNull();

                    this.ReadOnlyCollection = readOnlyCollection;
                }

                #endregion

                #region ICollection

                public IEnumerator<T> GetEnumerator()
                {
                    return this.ReadOnlyCollection.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public int Count
                {
                    get { return this.ReadOnlyCollection.Count; }
                }

                public bool Contains( T item )
                {
                    return this.ReadOnlyCollection.Contains(item, EqualityComparer<T>.Default);
                }

                public void Add( T item )
                {
                    throw new NotSupportedException();
                }

                public bool Remove( T item )
                {
                    throw new NotSupportedException();
                }

                public void Clear()
                {
                    throw new NotSupportedException();
                }

                public void CopyTo( T[] array, int arrayIndex )
                {
                    ((IEnumerable<T>)this).CopyTo(array, arrayIndex);
                }

                public bool IsReadOnly
                {
                    get { return true; }
                }

                #endregion
            }

            #endregion

            #region KeyCollection

            /// <summary>
            /// A collection that contains the keys in the dictionary. 
            /// </summary>
            public class KeyCollection : ReadOnlyCollection.Base<TKey>
            {
                #region Private Fields

                private readonly IDictionary<TKey, TValue> dictionary;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="KeyCollection"/> class.
                /// </summary>
                /// <param name="dictionary">The dictionary to wrap.</param>
                public KeyCollection( IDictionary<TKey, TValue> dictionary )
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
            /// A collection that contains the values in the dictionary. 
            /// </summary>
            public class ValueCollection : ReadOnlyCollection.Base<TValue>
            {
                #region Private Fields

                private readonly IDictionary<TKey, TValue> dictionary;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="ValueCollection"/> class.
                /// </summary>
                /// <param name="dictionary">The dictionary to wrap.</param>
                public ValueCollection( IDictionary<TKey, TValue> dictionary )
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

            private ReadOnlyCollectionWrapper<TKey> dictionaryKeys = ReadOnlyCollectionWrapper<TKey>.NullWrapper;
            private ReadOnlyCollectionWrapper<TValue> dictionaryValues = ReadOnlyCollectionWrapper<TValue>.NullWrapper;
            private KeyCollection keys;
            private ValueCollection values;

            #endregion

            #region ICollection

            /// <summary>
            /// Adds an item to the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
            public override void Add( KeyValuePair<TKey, TValue> item )
            {
                this.Add(item.Key, item.Value);
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item"/> is not found in the original <see cref="ICollection{T}"/>.</returns>
            public override bool Remove( KeyValuePair<TKey, TValue> item )
            {
                return this.Remove(item.Key);
            }

            /// <summary>
            /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
            public override bool Contains( KeyValuePair<TKey, TValue> item )
            {
                TValue value;
                if( this.TryGetValue(item.Key, out value) )
                    return EqualityComparer<TValue>.Default.Equals(item.Value, value);
                else
                    return false;
            }

            #endregion

            #region IDictionary

            /// <summary>
            /// Gets a collection that contains the keys in the dictionary.
            /// </summary>
            /// <value>A collection that contains the keys in the dictionary.</value>
            ICollection<TKey> IDictionary<TKey, TValue>.Keys
            {
                get
                {
                    // NOTE: to be performant: we only create a wrapper when we need to
                    var keys = this.GetKeys();
                    var w = this.dictionaryKeys;
                    if( !object.ReferenceEquals(keys, w.ReadOnlyCollection) )
                        Interlocked.CompareExchange(ref this.dictionaryKeys, new ReadOnlyCollectionWrapper<TKey>(keys), comparand: w);

                    return this.dictionaryKeys;
                }
            }

            /// <summary>
            /// Gets a collection that contains the values in the dictionary.
            /// </summary>
            /// <value>A collection that contains the values in the dictionary.</value>
            ICollection<TValue> IDictionary<TKey, TValue>.Values
            {
                get
                {
                    // NOTE: to be performant: we only create a wrapper when we need to
                    var values = this.GetValues();
                    var w = this.dictionaryValues;
                    if( !object.ReferenceEquals(values, w.ReadOnlyCollection) )
                        Interlocked.CompareExchange(ref this.dictionaryValues, new ReadOnlyCollectionWrapper<TValue>(values), comparand: w);

                    return this.dictionaryValues;
                }
            }

            /// <summary>
            /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="key">The object to use as the key of the element to add.</param>
            /// <param name="value">The object to use as the value of the element to add.</param>
            public abstract void Add( TKey key, TValue value );

            /// <summary>
            /// Removes the element with the specified key from the <see cref="IDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="key"/> was not found in the original <see cref="IDictionary{TKey, TValue}"/>.</returns>
            public abstract bool Remove( TKey key );

            /// <summary>
            /// Determines whether the dictionary contains an element that has the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><c>true</c> if the contains an element that has the specified key; otherwise, <c>false</c>.</returns>
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
            /// Gets or sets the value associated with the specified key.
            /// </summary>
            /// <param name="key">The key of the value to get or set.</param>
            /// <returns>The value associated with the specified key.</returns>
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
                set
                {
                    this.Remove(key);
                    this.Add(key, value);
                }
            }

            #endregion

            #region Protected Methods

            /// <summary>
            /// The implementation of IDictionary{TKey, TValue}.Keys.
            /// </summary>
            /// <returns>A collection that contains the keys in the dictionary.</returns>
            protected virtual IReadOnlyCollection<TKey> GetKeys()
            {
                if( this.keys.NullReference() )
                    Interlocked.CompareExchange(ref this.keys, new KeyCollection(this), null);

                return this.keys;
            }

            /// <summary>
            /// The implementation of IDictionary{TKey, TValue}.Values.
            /// </summary>
            /// <returns>A collection that contains the values in the dictionary.</returns>
            protected virtual IReadOnlyCollection<TValue> GetValues()
            {
                if( this.values.NullReference() )
                    Interlocked.CompareExchange(ref this.values, new ValueCollection(this), null);

                return this.values;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets a collection that contains the keys in the dictionary.
            /// </summary>
            /// <value>A collection that contains the keys in the dictionary.</value>
            public IReadOnlyCollection<TKey> Keys
            {
                get { return this.GetKeys(); }
            }

            /// <summary>
            /// Gets a collection that contains the values in the dictionary.
            /// </summary>
            /// <value>A collection that contains the values in the dictionary.</value>
            public IReadOnlyCollection<TValue> Values
            {
                get { return this.GetValues(); }
            }

            #endregion
        }
    }
}
