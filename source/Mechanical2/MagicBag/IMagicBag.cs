using System;

namespace Mechanical.MagicBag
{
    /// <summary>
    /// A basic, immutable IoC container.
    /// </summary>
    public interface IMagicBag
    {
        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>An object of type <typeparamref name="T"/>.</returns>
        T Pull<T>();

        /// <summary>
        /// Determines whether the specified type is registered into the magic bag.
        /// Does not guarantee that the type can be resolved.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns><c>true</c> if type <typeparamref name="T"/> is registered; otherwise <c>false</c>.</returns>
        bool IsRegistered<T>();
    }
}
