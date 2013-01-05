using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mechanical.Core;

namespace Mechanical.Collections
{
    //// NOTE: we inherit System.Collections.IList for the binding support.

    /// <summary>
    /// Helps working with lists.
    /// </summary>
    /// <content>
    /// A base class for implementing <see cref="IList{T}"/>.
    /// </content>
    public static partial class List
    {
        /// <summary>
        /// A base class for implementing <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        public abstract class Base<T> : Collection.Base<T>, IList<T>, IList
        {
            #region ICollection

            /// <summary>
            /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
            public override bool Contains( T item )
            {
                return this.IndexOf(item) != -1;
            }

            /// <summary>
            /// Adds an item to the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
            public override void Add( T item )
            {
                this.Insert(this.Count, item);
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item"/> is not found in the original <see cref="ICollection{T}"/>.</returns>
            public override bool Remove( T item )
            {
                int index = this.IndexOf(item);
                if( index >= 0 )
                {
                    this.RemoveAt(index);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Removes all items from the <see cref="ICollection{T}"/>.
            /// </summary>
            public override void Clear()
            {
                int count = this.Count;
                while( count != 0 )
                {
                    this.RemoveAt(count - 1);
                    --count;
                }
            }

            #endregion

            #region IList

            /// <summary>
            /// Determines the index of a specific item in the <see cref="IList{T}"/>.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="IList{T}"/>.</param>
            /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, <c>-1</c>.</returns>
            public virtual int IndexOf( T item )
            {
                var comparer = EqualityComparer<T>.Default;

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
            public abstract void Insert( int index, T item );

            /// <summary>
            /// Removes the <see cref="IList{T}"/> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            public abstract void RemoveAt( int index );

            /// <summary>
            /// Gets or sets the element at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the element to get or set.</param>
            /// <returns>The element at the specified index.</returns>
            public abstract T this[int index]
            {
                get;
                set;
            }

            #endregion

            #region System.Collections.IList

            int IList.Add( object value )
            {
                this.Add((T)value);
                return this.Count - 1;
            }

            void IList.Clear()
            {
                this.Clear();
            }

            bool IList.Contains( object value )
            {
                if( value is T
                 || value == (object)default(T) )
                    return this.Contains((T)value);
                else
                    return false;
            }

            int IList.IndexOf( object value )
            {
                if( value is T
                 || value == (object)default(T) )
                    return this.IndexOf((T)value);
                else
                    return -1;
            }

            void IList.Insert( int index, object value )
            {
                this.Insert(index, (T)value);
            }

            bool IList.IsFixedSize
            {
                get { return false; }
            }

            bool IList.IsReadOnly
            {
                get { return false; }
            }

            void IList.Remove( object value )
            {
                if( value is T
                 || value == (object)default(T) )
                    this.Remove((T)value);
            }

            void IList.RemoveAt( int index )
            {
                this.RemoveAt(index);
            }

            object IList.this[int index]
            {
                get { return this[index]; }
                set { this[index] = (T)value; }
            }

            void ICollection.CopyTo( Array array, int index )
            {
                ((IEnumerable<T>)this).CopyTo(array, index);
            }

            int ICollection.Count
            {
                get { return this.Count; }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            private object syncRoot;
            object ICollection.SyncRoot
            {
                get
                {
                    if( this.syncRoot.NullReference() )
                        Interlocked.CompareExchange(ref this.syncRoot, new object(), null);

                    return this.syncRoot;
                }
            }

            #endregion
        }
    }
}
