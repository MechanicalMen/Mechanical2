using System;
using System.Collections.Generic;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Allocators;
using Mechanical.MagicBag.Caches;

namespace Mechanical.MagicBag.Generators
{
    /// <summary>
    /// Generates <see cref="Lazy{T}"/> mappings.
    /// </summary>
    public class LazyGenerator : IMappingGenerator
    {
        #region Allocator

        private class Allocator<T> : IMagicBagAllocator<Lazy<T>>
        {
            private readonly Mapping mapping;

            public Allocator( Mapping mapping )
            {
                Ensure.Debug(mapping, m => m.NotNull());
                Ensure.Debug(mapping.From, from => from.Equal(typeof(T), EqualityComparer<Type>.Default));

                this.mapping = mapping;
            }

            public Lazy<T> CreateInstance( IMagicBag magicBag )
            {
                return new Lazy<T>(() => (T)this.mapping.Get(magicBag));
            }
        }

        #endregion

        #region IMappingGenerator

        /// <summary>
        /// Generates new mappings from the ones specified.
        /// </summary>
        /// <param name="mappings">The mappings to use.</param>
        /// <returns>The new mappings generated.</returns>
        public Mapping[] Generate( Mapping[] mappings )
        {
            Ensure.That(mappings).NotNullOrSparse();

            var cacheDefaultName = Reveal.Name(() => TransientCache<int>.Default);
            var mappingCreateMethod = Reveal.Method(() => Mapping.Create<int, int>(null, null)).GetGenericMethodDefinition();
            var mappingCreateParameters = new object[2];

            var array = new Mapping[mappings.Length];
            for( int i = 0; i < array.Length; ++i )
            {
                var originalMapping = mappings[i];
                var generatedType = typeof(Lazy<>).MakeGenericType(originalMapping.From);

                var allocatorType = typeof(Allocator<>).MakeGenericType(originalMapping.From);
                var cacheType = typeof(TransientCache<>).MakeGenericType(generatedType);

                mappingCreateParameters[0] = Activator.CreateInstance(allocatorType, originalMapping);
                mappingCreateParameters[1] = cacheType.GetField(cacheDefaultName).GetValue(null);

                array[i] = (Mapping)mappingCreateMethod.MakeGenericMethod(generatedType, generatedType).Invoke(null, mappingCreateParameters);
            }
            return array;
        }

        #endregion
    }
}
