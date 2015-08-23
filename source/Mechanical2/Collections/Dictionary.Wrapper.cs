using System;
using System.Collections.Generic;
using Mechanical.Conditions;

namespace Mechanical.Collections
{
    /// <content>
    /// An <see cref="IDictionary{TKey, TValue}"/> wrapper.
    /// </content>
    public static partial class Dictionary
    {
        /// <summary>
        /// A base class for implementing <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        public class Wrapper<TKey, TValue> : Base<TKey, TValue>
        {
            #region Protected Fields

            /// <summary>
            /// The underlying dictionary.
            /// </summary>
            protected readonly IDictionary<TKey, TValue> Items;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Wrapper{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary to wrap.</param>
            public Wrapper( IDictionary<TKey, TValue> dictionary )
                : base()
            {
                Ensure.That(dictionary).NotNull();

                this.Items = dictionary;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Wrapper{TKey, TValue}"/> class.
            /// </summary>
            public Wrapper()
                : this(new Dictionary<TKey, TValue>())
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Wrapper{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="capacity">The number of elements that the underlying dictionary can initially store.</param>
            public Wrapper( int capacity )
                : this(new Dictionary<TKey, TValue>(capacity))
            {
            }

            #endregion

            #region ICollection

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
            public sealed override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            /// <summary>
            /// Gets the number of elements in the collection.
            /// </summary>
            /// <value>The number of elements in the collection.</value>
            public sealed override int Count
            {
                get { return this.Items.Count; }
            }

            /// <summary>
            /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
            public sealed override bool Contains( KeyValuePair<TKey, TValue> item )
            {
                return this.Items.Contains(item);
            }

            /// <summary>
            /// Adds an item to the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
            public sealed override void Add( KeyValuePair<TKey, TValue> item )
            {
                base.Add(item); // Dictionary.Base implementation is fine, we just don't want to allow overriding it
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item"/> is not found in the original <see cref="ICollection{T}"/>.</returns>
            public sealed override bool Remove( KeyValuePair<TKey, TValue> item )
            {
                return base.Remove(item); // Dictionary.Base implementation is fine, we just don't want to allow overriding it
            }

            /// <summary>
            /// Removes all items from the <see cref="ICollection{T}"/>.
            /// </summary>
            public sealed override void Clear()
            {
                foreach( var pair in this.Items )
                    this.OnRemoving(pair.Key, pair.Value);

                this.Items.Clear();
            }

            #endregion

            #region IDictionary

            /// <summary>
            /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="key">The object to use as the key of the element to add.</param>
            /// <param name="value">The object to use as the value of the element to add.</param>
            public sealed override void Add( TKey key, TValue value )
            {
                this.OnAdding(key, value);
                this.Items.Add(key, value);
            }

            /// <summary>
            /// Removes the element with the specified key from the <see cref="IDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="key"/> was not found in the original <see cref="IDictionary{TKey, TValue}"/>.</returns>
            public sealed override bool Remove( TKey key )
            {
                if( !this.Items.ContainsKey(key) )
                    throw new KeyNotFoundException().Store("key", key);

                this.OnRemoving(key, this.Items[key]);
                return this.Items.Remove(key);
            }

            /// <summary>
            /// Determines whether the read-only dictionary contains an element that has the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><c>true</c> if the read-only dictionary contains an element that has the specified key; otherwise, <c>false</c>.</returns>
            public sealed override bool ContainsKey( TKey key )
            {
                return this.Items.ContainsKey(key);
            }

            /// <summary>
            /// Gets the value that is associated with the specified key.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <param name="value">The value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
            /// <returns>When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</returns>
            public sealed override bool TryGetValue( TKey key, out TValue value )
            {
                return this.Items.TryGetValue(key, out value);
            }

            /// <summary>
            /// Gets or sets the value associated with the specified key.
            /// </summary>
            /// <param name="key">The key of the value to get or set.</param>
            /// <returns>The value associated with the specified key.</returns>
            public sealed override TValue this[TKey key]
            {
                get
                {
                    return this.Items[key];
                }
                set
                {
                    // report item being overwritten as removed
                    TValue oldValue;
                    if( this.Items.TryGetValue(key, out oldValue) )
                        this.OnRemoving(key, oldValue);

                    // report new value as added
                    this.OnAdding(key, value);

                    // actually overwrite
                    this.Items[key] = value;
                }
            }

            #endregion

            #region Protected Virtual Members

            //// NOTE: the GetKeys, GetValues virtual methods are still overridable as well (see notes on Dictionary.Base).

            /// <summary>
            /// Called before a key-value pair is added to the wrapped dictionary.
            /// </summary>
            /// <param name="key">The key of the element to add.</param>
            /// <param name="value">The value of the element to add.</param>
            protected virtual void OnAdding( TKey key, TValue value )
            {
            }

            /// <summary>
            /// Called before a key-value pair is removed from the wrapped dictionary.
            /// </summary>
            /// <param name="key">The key of the element to remove.</param>
            /// <param name="value">The value of the element to remove.</param>
            protected virtual void OnRemoving( TKey key, TValue value )
            {
            }

            #endregion
        }
    }
}
