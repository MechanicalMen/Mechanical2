using System;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Allocators;

namespace Mechanical.MagicBag.Caches
{
    /// <summary>
    /// Allocates objects once per application (technically once per <see cref="AppDomain"/>).
    /// </summary>
    /// <typeparam name="T">The type of objects to return.</typeparam>
    public class SingletonCache<T> : IMagicBagCache<T>
    {
        #region Private Fields

        private Tuple<bool, T> state = new Tuple<bool, T>(false, default(T));

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

            var oldState = this.state;
            if( !oldState.Item1 )
            {
                var newState = Tuple.Create(true, allocator.CreateInstance(magicBag));
                Interlocked.CompareExchange(ref this.state, newState, comparand: oldState);
            }

            return this.state.Item2;
        }

        #endregion
    }
}
