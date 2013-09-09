using System;
using System.Collections.Generic;
using Mechanical.Core;

namespace Mechanical.Collections
{
    /// <summary>
    /// A list-based dictionary. Preserves the order of it's items.
    /// May be smaller and faster than hash-based implementations, for very small collections.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class ListDictionary<TKey, TValue> : Dictionary.Base<TKey, TValue>
    {
        #region Private Fields

        private readonly IEqualityComparer<TKey> keyComparer;
        private readonly List<KeyValuePair<TKey, TValue>> items;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="keyComparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        public ListDictionary( IEqualityComparer<TKey> keyComparer = null )
        {
            if( keyComparer.NullReference() )
                keyComparer = EqualityComparer<TKey>.Default;

            this.keyComparer = keyComparer;
            this.items = new List<KeyValuePair<TKey, TValue>>();
        }

        #endregion

        #region Private Methods

        private int IndexOf( TKey key )
        {
            for( int i = 0; i < this.items.Count; ++i )
            {
                if( this.keyComparer.Equals(this.items[i].Key, key) )
                    return i;
            }

            return -1;
        }

        #endregion

        #region ICollection

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <value>The number of elements in the collection.</value>
        public override int Count
        {
            get { return this.items.Count; }
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        public override void Clear()
        {
            this.items.Clear();
        }

        #endregion

        #region IDictionary

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public override void Add( TKey key, TValue value )
        {
            this.items.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="key"/> was not found in the original <see cref="IDictionary{TKey, TValue}"/>.</returns>
        public override bool Remove( TKey key )
        {
            int index = this.IndexOf(key);
            if( index == -1 )
                return false;

            this.items.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">The value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns>When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</returns>
        public override bool TryGetValue( TKey key, out TValue value )
        {
            int index = this.IndexOf(key);
            if( index == -1 )
            {
                value = default(TValue);
                return false;
            }
            else
            {
                value = this.items[index].Value;
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public override TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                int index = this.IndexOf(key);
                if( index == -1 )
                    this.Add(key, value);
                else
                    this.items[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        #endregion
    }
}
