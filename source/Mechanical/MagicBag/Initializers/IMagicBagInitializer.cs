using System;

namespace Mechanical.MagicBag.Initializers
{
    //// NOTE: the signature looks like this to allow value types to be initialized as well.

    /// <summary>
    /// Initializes an objects allocated for a magic bag.
    /// </summary>
    /// <typeparam name="T">The type of objects to initialize.</typeparam>
    public interface IMagicBagInitializer<T>
    {
        /// <summary>
        /// Initializes the newly allocated object.
        /// </summary>
        /// <param name="allocated">The object just allocated.</param>
        /// <param name="magicBag">The magic bag to use.</param>
        /// <returns>The initialized object.</returns>
        T Initialize( T allocated, IMagicBag magicBag );
    }
}
