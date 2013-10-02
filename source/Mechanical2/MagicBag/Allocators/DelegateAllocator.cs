using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MagicBag.Allocators
{
    /// <summary>
    /// An <see cref="IMagicBagAllocator{T}"/> that uses a delegate to create a new object instance.
    /// </summary>
    /// <typeparam name="T">The type of objects to create instances of.</typeparam>
    public class DelegateAllocator<T> : IMagicBagAllocator<T>
    {
        #region Private Fields

        private readonly Func<IMagicBag, T> method;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateAllocator{T}"/> class.
        /// </summary>
        /// <param name="method">The method returning the object instance.</param>
        public DelegateAllocator( Func<T> method )
            : this(bag => method())
        {
            Ensure.That(method).NotNull();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateAllocator{T}"/> class.
        /// </summary>
        /// <param name="method">The method returning the object instance.</param>
        public DelegateAllocator( Func<IMagicBag, T> method )
        {
            Ensure.That(method).NotNull();

            this.method = method;
        }

        #endregion

        #region IMagicBagAllocator

        /// <summary>
        /// Creates a new object instance.
        /// </summary>
        /// <param name="magicBag">The magic bag to use for dependency injection.</param>
        /// <returns>The new instance created.</returns>
        public T CreateInstance( IMagicBag magicBag )
        {
            if( magicBag.NullReference() )
                throw new ArgumentNullException("magicBag").StoreFileLine();

            return this.method(magicBag);
        }

        #endregion
    }
}
