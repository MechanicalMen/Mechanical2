using System;
using System.Collections.Generic;
using Mechanical.Conditions;

namespace Mechanical.Collections
{
    /// <content>
    /// An <see cref="IList{T}"/> wrapper.
    /// </content>
    public static partial class List
    {
        /// <summary>
        /// A base class for implementing <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        public class Wrapper<T> : Base<T>
        {
            #region Protected Fields

            /// <summary>
            /// The underlying list.
            /// </summary>
            protected readonly IList<T> Items;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Wrapper{T}"/> class.
            /// </summary>
            /// <param name="list">The list to wrap.</param>
            public Wrapper( IList<T> list )
                : base()
            {
                Ensure.That(list).NotNull();

                this.Items = list;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Wrapper{T}"/> class.
            /// </summary>
            public Wrapper()
                : this(new List<T>())
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Wrapper{T}"/> class.
            /// </summary>
            /// <param name="capacity">The number of elements that the underlying list can initially store.</param>
            public Wrapper( int capacity )
                : this(new List<T>(capacity))
            {
            }

            #endregion

            #region ICollection

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
            public override IEnumerator<T> GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            /// <summary>
            /// Gets the number of elements in the collection.
            /// </summary>
            /// <value>The number of elements in the collection.</value>
            public override int Count
            {
                get { return this.Items.Count; }
            }

            /// <summary>
            /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
            public override bool Contains( T item )
            {
                return this.Items.Contains(item);
            }

            /// <summary>
            /// Adds an item to the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
            public override void Add( T item )
            {
                this.Items.Add(item);
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item"/> is not found in the original <see cref="ICollection{T}"/>.</returns>
            public override bool Remove( T item )
            {
                return this.Items.Remove(item);
            }

            /// <summary>
            /// Removes all items from the <see cref="ICollection{T}"/>.
            /// </summary>
            public override void Clear()
            {
                this.Items.Clear();
            }

            #endregion

            #region IList

            /// <summary>
            /// Determines the index of a specific item in the <see cref="IList{T}"/>.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="IList{T}"/>.</param>
            /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, <c>-1</c>.</returns>
            public override int IndexOf( T item )
            {
                return this.Items.IndexOf(item);
            }

            /// <summary>
            /// Inserts an item to the <see cref="IList{T}"/> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
            /// <param name="item">The object to insert into the <see cref="IList{T}"/>.</param>
            public override void Insert( int index, T item )
            {
                this.Items.Insert(index, item);
            }

            /// <summary>
            /// Removes the <see cref="IList{T}"/> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            public override void RemoveAt( int index )
            {
                this.Items.RemoveAt(index);
            }

            /// <summary>
            /// Gets or sets the element at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the element to get or set.</param>
            /// <returns>The element at the specified index.</returns>
            public override T this[int index]
            {
                get { return this.Items[index]; }
                set { this.Items[index] = value; }
            }

            #endregion
        }
    }
}
