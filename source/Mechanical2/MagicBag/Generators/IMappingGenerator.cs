using System;

namespace Mechanical.MagicBag.Generators
{
    /// <summary>
    /// Takes a set of mappings, and generates new mappings from them.
    /// </summary>
    public interface IMappingGenerator
    {
        /// <summary>
        /// Generates new mappings from the ones specified.
        /// </summary>
        /// <param name="mappings">The mappings to use.</param>
        /// <returns>The new mappings generated.</returns>
        Mapping[] Generate( Mapping[] mappings );
    }
}
