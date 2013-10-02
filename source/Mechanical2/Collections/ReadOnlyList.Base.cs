using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Collections
{
    //// NOTE: we inherit System.Collections.IList for the binding support.

    /// <summary>
    /// Helps working with read-only lists.
    /// </summary>
    /// <content>
    /// A base class for implementing <see cref="IReadOnlyList{T}"/>.
    /// </content>
    public static partial class ReadOnlyList
    {
        /// <summary>
        /// A base class for implementing <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        public abstract class Base<T> : ReadOnlyCollection.Base<T>, IReadOnlyList<T>, IList
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

            #endregion

            #region IReadOnlyList

            /// <summary>
            /// Gets the element at the specified index in the read-only list.
            /// </summary>
            /// <param name="index">The zero-based index of the element to get.</param>
            /// <returns>The element at the specified index in the read-only list.</returns>
            public virtual T this[int index]
            {
                get { return ((IEnumerable<T>)this).ElementAt(index); }
            }

            #endregion

            #region Public Methods

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

            #endregion

            #region System.Collections.IList

            int IList.Add( object value )
            {
                throw new NotSupportedException().StoreFileLine();
            }

            void IList.Clear()
            {
                throw new NotSupportedException().StoreFileLine();
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
                throw new NotSupportedException().StoreFileLine();
            }

            bool IList.IsFixedSize
            {
                get { return false; }
            }

            bool IList.IsReadOnly
            {
                get { return true; }
            }

            void IList.Remove( object value )
            {
                throw new NotSupportedException().StoreFileLine();
            }

            void IList.RemoveAt( int index )
            {
                throw new NotSupportedException().StoreFileLine();
            }

            object IList.this[int index]
            {
                get { return this[index]; }
                set { throw new NotSupportedException().StoreFileLine(); }
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
