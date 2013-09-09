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
    /// Generates <see cref="IList{T}"/> mappings.
    /// </summary>
    public class IListGenerator : IMappingGenerator
    {
        #region Allocator

        private class Allocator<T> : IMagicBagAllocator<IList<T>>
        {
            private readonly Mapping[] mappings;

            public Allocator( Mapping[] mappings )
            {
                Ensure.Debug(mappings, m => m.NotNullOrSparse());
#if DEBUG
                foreach( var m in mappings )
                {
                    if( m.From != typeof(T) )
                        throw new ArgumentException("Invalid mapping type!").Store("MappingType", m.From).Store("ExpectedType", typeof(T));
                }
#endif

                this.mappings = mappings;
            }

            public IList<T> CreateInstance( IMagicBag magicBag )
            {
                var list = new List<T>(this.mappings.Length);

                for( int i = 0; i < this.mappings.Length; ++i )
                    list.Add((T)this.mappings[i].Get(magicBag));

                return list;
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
            var dictionary = ArrayGenerator.ToDictionary(mappings);
            var cacheDefaultName = Reveal.Name(() => TransientCache<int>.Default);
            var mappingCreateMethod = Reveal.Method(() => Mapping.Create<int, int>(null, null)).GetGenericMethodDefinition();
            var mappingCreateParameters = new object[2];

            var array = new Mapping[dictionary.Count];
            int index = 0;
            foreach( var pair in dictionary )
            {
                var originalMappingFrom = pair.Key;
                var generatedType = typeof(IList<>).MakeGenericType(originalMappingFrom);

                var allocatorType = typeof(Allocator<>).MakeGenericType(originalMappingFrom);
                var cacheType = typeof(TransientCache<>).MakeGenericType(generatedType);

                mappingCreateParameters[0] = Activator.CreateInstance(allocatorType, new object[] { pair.Value.ToArray() });
                mappingCreateParameters[1] = cacheType.GetField(cacheDefaultName).GetValue(null);

                array[index] = (Mapping)mappingCreateMethod.MakeGenericMethod(generatedType, generatedType).Invoke(null, mappingCreateParameters);
                ++index;
            }
            return array;
        }

        #endregion
    }
}
