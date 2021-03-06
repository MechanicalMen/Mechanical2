﻿using System;
using System.Collections.Generic;
using Mechanical.Conditions;

namespace Mechanical.Collections
{
    /// <content>
    /// An <see cref="IReadOnlyCollection{T}"/> wrapper.
    /// </content>
    public static partial class ReadOnlyCollection
    {
        /// <summary>
        /// A base class for implementing <see cref="IReadOnlyCollection{T}"/>.
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

            #region IReadOnlyCollection

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

            #endregion

            #region Public Methods

            /// <summary>
            /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
            public override bool Contains( T item )
            {
                return this.Items.Contains(item);
            }

            #endregion
        }
    }
}
