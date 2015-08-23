using System;
using System.Collections;
using System.Collections.Generic;

namespace Mechanical.Collections
{
    /// <summary>
    /// Helps working with read-only collections.
    /// </summary>
    /// <content>
    /// An abstract base class for implementing <see cref="IReadOnlyCollection{T}"/>.
    /// </content>
    public static partial class ReadOnlyCollection
    {
        /// <summary>
        /// An abstract base class for implementing <see cref="IReadOnlyCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        public abstract class Base<T> : IReadOnlyCollection<T>
        {
            #region IReadOnlyCollection

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
                    // NOTE: same code as in Collection.Base
                    int num = 0;
                    var enumerator = this.GetEnumerator();

                    while( enumerator.MoveNext() )
                        ++num;

                    return num;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
            public virtual bool Contains( T item )
            {
                return ((IEnumerable<T>)this).Contains(item, EqualityComparer<T>.Default);
            }

            #endregion
        }
    }
}
