using System;
using Mechanical.MagicBag.Allocators;

namespace Mechanical.MagicBag.Caches
{
    /// <summary>
    /// Regulates <see cref="IMagicBagAllocator{T}"/> usage.
    /// </summary>
    /// <typeparam name="T">The type of objects to return.</typeparam>
    public interface IMagicBagCache<T>
    {
        /// <summary>
        /// Returns an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="allocator">The allocator that may be used.</param>
        /// <param name="magicBag">The magic bag to access the <paramref name="allocator"/> with.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T GetInstance( IMagicBagAllocator<T> allocator, IMagicBag magicBag );
    }
}
