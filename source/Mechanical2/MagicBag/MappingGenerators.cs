using System;
using Mechanical.MagicBag.Generators;

namespace Mechanical.MagicBag
{
    /// <summary>
    /// Gets the default <see cref="IMappingGenerator"/> instances.
    /// </summary>
    public static class MappingGenerators
    {
        /// <summary>
        /// Generates <see cref="System.Func{T}"/> mappings.
        /// </summary>
        public static readonly IMappingGenerator Func = new FuncGenerator();

        /// <summary>
        /// Generates <see cref="System.Lazy{T}"/> mappings.
        /// </summary>
        public static readonly IMappingGenerator Lazy = new LazyGenerator();

        /// <summary>
        /// Generates <see cref="System.Collections.Generic.IEnumerable{T}"/> mappings.
        /// </summary>
        public static readonly IMappingGenerator IEnumerable = new IEnumerableGenerator();

        /// <summary>
        /// Generates <see cref="System.Collections.Generic.IList{T}"/> mappings.
        /// </summary>
        public static readonly IMappingGenerator IList = new IListGenerator();

        /// <summary>
        /// Generates <c>T[]</c> mappings.
        /// </summary>
        public static readonly IMappingGenerator Array = new ArrayGenerator();

        /// <summary>
        /// Gets all the default mapping generators.
        /// </summary>
        /// <value>The default mapping generators.</value>
        public static IMappingGenerator[] Defaults
        {
            get
            {
                return new IMappingGenerator[]
                {
                    Func,
                    Lazy,
                    IEnumerable,
                    IList,
                    Array
                };
            }
        }
    }
}
