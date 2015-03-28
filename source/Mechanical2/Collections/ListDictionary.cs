using System;
using System.Collections.Generic;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Collections
{
    /// <summary>
    /// A list-based dictionary. Preserves the order of it's items.
    /// May be smaller and faster than hash-based implementations, for very small collections.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class ListDictionary<TKey, TValue> : Dictionary.Base<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>
    {
        #region KeyComparerAsPairComparer

        private class KeyComparerAsPairComparer : IComparer<KeyValuePair<TKey, TValue>>
        {
            private readonly IComparer<TKey> keyComparer;

            internal KeyComparerAsPairComparer( IComparer<TKey> keyComparer )
            {
                if( keyComparer.NullReference() )
                    throw new ArgumentNullException("keyComparer").StoreFileLine();

                this.keyComparer = keyComparer;
            }

            public int Compare( KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y )
            {
                return this.keyComparer.Compare(x.Key, y.Key);
            }
        }

        #endregion

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
        public sealed override void Add( TKey key, TValue value )
        {
            this.Insert(this.Count, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="key"/> was not found in the original <see cref="IDictionary{TKey, TValue}"/>.</returns>
        public sealed override bool Remove( TKey key )
        {
            int index = this.IndexOf(key);
            if( index == -1 )
                return false;

            this.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">The value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns>When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</returns>
        public sealed override bool TryGetValue( TKey key, out TValue value )
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
        public sealed override TValue this[TKey key]
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
                    this[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        #endregion

        #region IList

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList{T}"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, <c>-1</c>.</returns>
        int IList<KeyValuePair<TKey, TValue>>.IndexOf( KeyValuePair<TKey, TValue> item )
        {
            // NOTE: I believe this is the correct algorithm for a generic list, and therefore should produce the expected results when handled as a list.
            //       However, it does not take into account our own 'keyComparer', which might lead to confusion, so we implement this implicitly.
            var comparer = EqualityComparer<KeyValuePair<TKey, TValue>>.Default;

            for( int i = 0; i < this.Count; ++i )
            {
                if( comparer.Equals(this[i], item) )
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="IList{T}"/>.</param>
        public void Insert( int index, KeyValuePair<TKey, TValue> item )
        {
            this.items.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="IList{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt( int index )
        {
            this.items.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public KeyValuePair<TKey, TValue> this[int index]
        {
            get { return this.items[index]; }
            set { this.items[index] = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines the index of a specific key in the dictionary.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>The index of <paramref name="key"/> if found in the dictionary; otherwise, <c>-1</c>.</returns>
        public int IndexOf( TKey key )
        {
            for( int i = 0; i < this.items.Count; ++i )
            {
                if( this.keyComparer.Equals(this.items[i].Key, key) )
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Sorts the elements in the entire dictionary using the specified comparer.
        /// </summary>
        /// <param name="pairComparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements, or <c>null</c> to use the default comparer <see cref="Comparer{T}.Default"/>.</param>
        public void Sort( IComparer<KeyValuePair<TKey, TValue>> pairComparer = null )
        {
            if( pairComparer.NullReference() )
                pairComparer = Comparer<KeyValuePair<TKey, TValue>>.Default;

            this.items.Sort(pairComparer);
        }

        /// <summary>
        /// Sorts the elements in the entire dictionary using the specified <see cref="System.Comparison{T}"/>.
        /// </summary>
        /// <param name="pairComparison">The <see cref="System.Comparison{T}"/> to use when comparing elements.</param>
        public void Sort( Comparison<KeyValuePair<TKey, TValue>> pairComparison )
        {
            if( pairComparison.NullReference() )
                throw new ArgumentNullException("pairComparison").StoreFileLine();

            this.items.Sort(pairComparison);
        }

        /// <summary>
        /// Sorts the elements in the entire dictionary using the specified comparer.
        /// </summary>
        /// <param name="keyComparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements.</param>
        public void Sort( IComparer<TKey> keyComparer )
        {
            // NOTE: we throw on 'null', so that there is no confusion whether we use the default comparer, or are trying to cast the 'keyComparer'
            if( keyComparer.NullReference() )
                throw new ArgumentNullException("keyComparer").StoreFileLine();

            this.Sort(new KeyComparerAsPairComparer(keyComparer));
        }

        /// <summary>
        /// Sorts the elements in the entire dictionary using the specified <see cref="System.Comparison{T}"/>.
        /// </summary>
        /// <param name="keyComparison">The <see cref="System.Comparison{T}"/> to use when comparing elements.</param>
        public void Sort( Comparison<TKey> keyComparison )
        {
            if( keyComparison.NullReference() )
                throw new ArgumentNullException("keyComparison").StoreFileLine();

            this.Sort(( KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y ) => keyComparison(x.Key, y.Key));
        }

        #endregion
    }
}
