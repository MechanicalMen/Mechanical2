using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Allocators;

namespace Mechanical.MagicBag.Caches
{
    /// <summary>
    /// Allocates objects for each call.
    /// </summary>
    /// <typeparam name="T">The type of objects to return.</typeparam>
    public class TransientCache<T> : IMagicBagCache<T>
    {
        #region IMagicBagCache

        /// <summary>
        /// Returns an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="allocator">The allocator that may be used.</param>
        /// <param name="magicBag">The magic bag to access the <paramref name="allocator"/> with.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public T GetInstance( IMagicBagAllocator<T> allocator, IMagicBag magicBag )
        {
            if( allocator.NullReference() )
                throw new ArgumentNullException("allocator").StoreFileLine();

            if( magicBag.NullReference() )
                throw new ArgumentNullException("magicBag").StoreFileLine();

            return allocator.CreateInstance(magicBag);
        }

        #endregion

        /// <summary>
        /// The default instance of the type.
        /// </summary>
        public static readonly TransientCache<T> Default = new TransientCache<T>();
    }
}
