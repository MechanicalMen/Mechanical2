using System;
using System.Collections.Generic;
using Mechanical.Conditions;

namespace Mechanical.Collections
{
    /// <content>
    /// An <see cref="ICollection{T}"/> wrapper.
    /// </content>
    public static partial class Collection
    {
        /// <summary>
        /// A base class for implementing <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        public class Wrapper<T> : Base<T>
        {
            #region Protected Fields

            /// <summary>
            /// The underlying collection.
            /// </summary>
            protected readonly ICollection<T> Items;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Wrapper{T}"/> class.
            /// </summary>
            /// <param name="collection">The collection to wrap.</param>
            public Wrapper( ICollection<T> collection )
                : base()
            {
                Ensure.That(collection).NotNull();

                this.Items = collection;
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
            /// <param name="capacity">The number of elements that the underlying collection can initially store.</param>
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
            public sealed override IEnumerator<T> GetEnumerator()
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
            public sealed override bool Contains( T item )
            {
                return this.Items.Contains(item);
            }

            /// <summary>
            /// Adds an item to the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
            public sealed override void Add( T item )
            {
                this.OnAdding(item);
                this.Items.Add(item);
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item"/> is not found in the original <see cref="ICollection{T}"/>.</returns>
            public sealed override bool Remove( T item )
            {
                this.OnRemoving(item);
                return this.Items.Remove(item);
            }

            /// <summary>
            /// Removes all items from the <see cref="ICollection{T}"/>.
            /// </summary>
            public sealed override void Clear()
            {
                foreach( var item in this.Items )
                    this.OnRemoving(item);

                this.Items.Clear();
            }

            #endregion

            #region Protected Virtual Members

            /// <summary>
            /// Called before an item is added to the wrapped collection.
            /// </summary>
            /// <param name="item">The item to add.</param>
            protected virtual void OnAdding( T item )
            {
            }

            /// <summary>
            /// Called before an item is removed from the wrapped collection.
            /// </summary>
            /// <param name="item">The item to remove.</param>
            protected virtual void OnRemoving( T item )
            {
            }

            #endregion
        }
    }
}
