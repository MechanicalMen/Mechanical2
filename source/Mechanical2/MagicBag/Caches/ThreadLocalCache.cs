using System;
using System.Collections.Generic;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Allocators;

namespace Mechanical.MagicBag.Caches
{
    /// <summary>
    /// Allocates objects once per thread.
    /// </summary>
    /// <typeparam name="T">The type of objects to return.</typeparam>
    public class ThreadLocalCache<T> : IMagicBagCache<T>
    {
        #region Private Fields

        private ThreadLocal<KeyValuePair<bool, T>> instance = new ThreadLocal<KeyValuePair<bool, T>>(() => new KeyValuePair<bool, T>(false, default(T)));

        #endregion

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

            var pair = this.instance.Value;
            if( !pair.Key )
            {
                pair = new KeyValuePair<bool, T>(true, allocator.CreateInstance(magicBag));
                this.instance.Value = pair;
            }

            return pair.Value;
        }

        #endregion
    }
}
