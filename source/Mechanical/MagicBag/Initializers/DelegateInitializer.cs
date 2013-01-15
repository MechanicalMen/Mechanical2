using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MagicBag.Initializers
{
    /// <summary>
    /// Initializes newly allocated objects using a delegate.
    /// </summary>
    /// <typeparam name="T">The type of objects to initialize.</typeparam>
    public class DelegateInitializer<T> : IMagicBagInitializer<T>
    {
        #region Private Fields

        private readonly Func<T, IMagicBag, T> init;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateInitializer{T}"/> class.
        /// </summary>
        /// <param name="init">The delegate to initialize newly allocated objects with.</param>
        public DelegateInitializer( Func<T, IMagicBag, T> init )
        {
            Ensure.That(init).NotNull();

            this.init = init;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateInitializer{T}"/> class.
        /// </summary>
        /// <param name="init">The delegate to initialize newly allocated objects with.</param>
        public DelegateInitializer( Action<T, IMagicBag> init )
            : this(( newObj, bag ) => { init(newObj, bag); return newObj; })
        {
            Ensure.That(init).NotNull();
        }

        #endregion

        #region IMagicBagInitializer

        /// <summary>
        /// Initializes the newly allocated object.
        /// </summary>
        /// <param name="allocated">The object just allocated.</param>
        /// <param name="magicBag">The magic bag to use.</param>
        /// <returns>The initialized object.</returns>
        public T Initialize( T allocated, IMagicBag magicBag )
        {
            if( Reveal.IsByRef<T>()
             && ((object)allocated).NotNullReference() )
                throw new ArgumentNullException("allocated").StoreDefault();

            if( magicBag.NullReference() )
                throw new ArgumentNullException("magicBag").StoreDefault();

            return this.init(allocated, magicBag);
        }

        #endregion
    }
}
