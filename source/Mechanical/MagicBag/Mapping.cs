using System;
using Mechanical.Conditions;
using Mechanical.MagicBag.Allocators;
using Mechanical.MagicBag.Caches;

namespace Mechanical.MagicBag
{
    //// NOTE: refactoring this into an interface and and a class would be nicer
    ////       (and it would make writing generators a bit easier)
    ////       But doing this breaks code like 'new MagicBag(Map.From<T>.ToDefault<U>())'
    ////       since Map.Builder can not be implicitly converted to interfaces
    ////       and the compiler does not see that the parameter would fit  after  an implicit conversion...

    /// <summary>
    /// A mapping used to resolve a type.
    /// </summary>
    public class Mapping
    {
        #region Private Fields

        private readonly Type from;
        private readonly Func<IMagicBag, object> get;

        #endregion

        #region Constructors

        private Mapping( Type from, Func<IMagicBag, object> get )
        {
            Ensure.Debug(from, f => f.NotNull());
            Ensure.Debug(get, g => g.NotNull());

            this.from = from;
            this.get = get;
        }

        /// <summary>
        /// Returns a new <see cref="Mapping"/> instance.
        /// </summary>
        /// <typeparam name="TFrom">The type of the mapping source.</typeparam>
        /// <typeparam name="TTo">The type of the mapping target.</typeparam>
        /// <param name="allocator">The allocator to use.</param>
        /// <param name="cache">The cache to use.</param>
        /// <returns>The new <see cref="Mapping"/> instance.</returns>
        public static Mapping Create<TFrom, TTo>( IMagicBagAllocator<TTo> allocator, IMagicBagCache<TTo> cache )
        {
            Ensure.That(allocator).NotNull();
            Ensure.That(cache).NotNull();

            return new Mapping(typeof(TFrom), magicBag => cache.GetInstance(allocator, magicBag));
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the mapping source.
        /// </summary>
        /// <value>The mapping source.</value>
        public Type From
        {
            get { return this.from; }
        }

        /// <summary>
        /// Returns an instance of the mapping target
        /// </summary>
        /// <param name="magicBag">The magic bag to use.</param>
        /// <returns>An instance of the mapping target.</returns>
        public object Get( IMagicBag magicBag )
        {
            return this.get(magicBag);
        }

        #endregion
    }
}
