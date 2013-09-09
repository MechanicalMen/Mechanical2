using System;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.MagicBag.Initializers;

namespace Mechanical.MagicBag.Allocators
{
    /// <summary>
    /// Wraps an allocator, and executes initializers on the instance it returns.
    /// </summary>
    /// <typeparam name="T">The type of objects to create instances of.</typeparam>
    public class InitializingAllocatorWrapper<T> : IMagicBagAllocator<T>
    {
        #region Private Fields

        private readonly IMagicBagAllocator<T> allocator;
        private readonly IMagicBagInitializer<T>[] initializers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializingAllocatorWrapper{T}"/> class.
        /// </summary>
        /// <param name="allocator">The allocator to wrap.</param>
        /// <param name="initializers">The initializers to invoke in the specified order.</param>
        public InitializingAllocatorWrapper( IMagicBagAllocator<T> allocator, params IMagicBagInitializer<T>[] initializers )
        {
            Ensure.That(allocator).NotNull();
            Ensure.That(initializers).NotNullOrSparse();

            this.allocator = allocator;
            this.initializers = initializers;
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
            var instance = this.allocator.CreateInstance(magicBag);

            for( int i = 0; i < this.initializers.Length; ++i )
                instance = this.initializers[i].Initialize(instance, magicBag);

            return instance;
        }

        #endregion
    }
}
