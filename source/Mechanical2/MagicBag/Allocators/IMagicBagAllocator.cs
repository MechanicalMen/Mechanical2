using System;

namespace Mechanical.MagicBag.Allocators
{
    /// <summary>
    /// Creates a new object instance, to be returned by a magic bag.
    /// </summary>
    /// <typeparam name="T">The type of objects to create instances of.</typeparam>
    public interface IMagicBagAllocator<out T>
    {
        /// <summary>
        /// Creates a new object instance.
        /// </summary>
        /// <param name="magicBag">The magic bag to use for dependency injection.</param>
        /// <returns>The new instance created.</returns>
        T CreateInstance( IMagicBag magicBag );
    }
}
