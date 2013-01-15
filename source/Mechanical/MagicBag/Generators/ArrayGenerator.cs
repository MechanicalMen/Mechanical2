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
    /// Generates <c>T[]</c> mappings.
    /// </summary>
    public class ArrayGenerator : IMappingGenerator
    {
        #region Allocator

        private class Allocator<T> : IMagicBagAllocator<T[]>
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

            public T[] CreateInstance( IMagicBag magicBag )
            {
                var array = new T[this.mappings.Length];

                for( int i = 0; i < this.mappings.Length; ++i )
                    array[i] = (T)this.mappings[i].Get(magicBag);

                return array;
            }
        }

        #endregion

        #region Private and Internal Members

        private static Type GetArrayType<T>()
        {
            // NOTE: unfortunately arrays do not have an open generic type, so we need to be tricky...
            return typeof(T[]);
        }

        internal static Dictionary<Type, List<Mapping>> ToDictionary( Mapping[] mappings )
        {
            Ensure.That(mappings).NotNullOrSparse();

            var dictionary = new Dictionary<Type, List<Mapping>>();
            foreach( var m in mappings )
            {
                List<Mapping> list;
                if( dictionary.ContainsKey(m.From) )
                {
                    list = dictionary[m.From];
                }
                else
                {
                    list = new List<Mapping>();
                    dictionary.Add(m.From, list);
                }

                list.Add(m);
            }
            return dictionary;
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
            var dictionary = ToDictionary(mappings);
            var cacheDefaultName = Reveal.Name(() => TransientCache<int>.Default);
            var mappingCreateMethod = Reveal.Method(() => Mapping.Create<int, int>(null, null)).GetGenericMethodDefinition();
            var mappingCreateParameters = new object[2];
            var getArrayTypeMethod = Reveal.Method(() => GetArrayType<int>()).GetGenericMethodDefinition();

            var array = new Mapping[dictionary.Count];
            int index = 0;
            foreach( var pair in dictionary )
            {
                var originalMappingFrom = pair.Key;
                var generatedType = (Type)getArrayTypeMethod.MakeGenericMethod(originalMappingFrom).Invoke(null, null);

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
