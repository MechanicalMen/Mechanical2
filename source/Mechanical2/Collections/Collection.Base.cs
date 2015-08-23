using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mechanical.Collections
{
    /// <summary>
    /// Helps working with collections.
    /// </summary>
    /// <content>
    /// An abstract base class for implementing <see cref="ICollection{T}"/>.
    /// </content>
    public static partial class Collection
    {
        //// NOTE: Use a "Base" class, if you are implementing a new (general) data structure.
        ////       Use a "Wrapper" class, if you need to alter the behaviour or interface
        ////       of a specialized collection (e.g. 'Car.Wheels' should not accept more than 4 items)

        /// <summary>
        /// An abstract base class for implementing <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        public abstract class Base<T> : ICollection<T>
        {
            #region ICollection

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
            public abstract IEnumerator<T> GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            /// <summary>
            /// Gets the number of elements in the collection.
            /// </summary>
            /// <value>The number of elements in the collection.</value>
            public virtual int Count
            {
                get
                {
                    //// NOTE: not using Count(), because that tries to cast to an ICollection
                    ////       and would result in a StackOverflowException

                    // NOTE: same code as in ReadOnlyCollection.Base
                    int num = 0;
                    var enumerator = this.GetEnumerator();

                    while( enumerator.MoveNext() )
                        ++num;

                    return num;
                }
            }

            /// <summary>
            /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
            public virtual bool Contains( T item )
            {
                return ((IEnumerable<T>)this).Contains(item, EqualityComparer<T>.Default);
            }

            /// <summary>
            /// Adds an item to the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
            public abstract void Add( T item );

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item"/> is not found in the original <see cref="ICollection{T}"/>.</returns>
            public abstract bool Remove( T item );

            /// <summary>
            /// Removes all items from the <see cref="ICollection{T}"/>.
            /// </summary>
            public virtual void Clear()
            {
                while( this.Count != 0 )
                    this.Remove(this.First());
            }

            /// <summary>
            /// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            public void CopyTo( T[] array, int arrayIndex )
            {
                ((IEnumerable<T>)this).CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
            /// </summary>
            /// <value><c>true</c> if the <see cref="ICollection{T}"/> is read-only; otherwise, <c>false</c>.</value>
            bool ICollection<T>.IsReadOnly
            {
                get { return false; }
            }

            #endregion
        }
    }
}
