using System;
using System.Collections.Generic;
using System.Linq;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.Collections
{
    /// <summary>
    /// Extension methods for the Mechanical.Collections namespace.
    /// </summary>
    public static partial class CollectionsExtensions
    {
        #region CopyTo

        /// <summary>
        /// Copy the items of this sequence to an array, starting at the specified array index.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="sequence">The enumeration to work on.</param>
        /// <param name="array">The (destination) array to copy to.</param>
        /// <param name="arrayIndex">The index in array at which copying begins.</param>
        public static void CopyTo<T>( this IEnumerable<T> sequence, T[] array, int arrayIndex = 0 )
        {
            Ensure.Debug(sequence, arg => arg.NotNull());
            Ensure.That(array).NotNull();
            Ensure.That(arrayIndex).IsIndex(array);

            int cnt = sequence.Count();
            Ensure.That(arrayIndex + cnt).LessOrEqual(array.Length);

            if( cnt == 0 )
                return;

            int index = arrayIndex;
            int numCopied = 0;

            foreach( T item in sequence )
            {
                if( numCopied == cnt )
                    break;

                array[index] = item;
                ++index;
                ++numCopied;
            }
        }

        /// <summary>
        /// Copy the items of this collection to an array, starting at the specified array index.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="enumerable">The enumeration to work on.</param>
        /// <param name="array">The (destination) array to copy to.</param>
        /// <param name="arrayIndex">The index in array at which copying begins.</param>
        public static void CopyTo<T>( this IEnumerable<T> enumerable, Array array, int arrayIndex = 0 )
        {
            Ensure.Debug(enumerable, arg => arg.NotNull());
            Ensure.That(array).NotNull();
            Ensure.That(array.Rank).Equal(1);

            int lowerBound = array.GetLowerBound(0);
            int upperBound = array.GetUpperBound(0);
            Ensure.That(arrayIndex).InRange(lowerBound, upperBound + 1);

            int cnt = enumerable.Count();
            int length = upperBound + 1 - lowerBound;
            Ensure.That(arrayIndex + cnt).LessOrEqual(length);

            if( cnt == 0 )
                return;

            int index = arrayIndex;
            int numCopied = 0;

            foreach( T item in enumerable )
            {
                if( numCopied == cnt )
                    break;

                array.SetValue(item, index);
                ++index;
                ++numCopied;
            }
        }

        #endregion

        #region Contains

        /// <summary>
        /// Determines whether the sequence contains a specific value.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="sequence">The sequence to work on.</param>
        /// <param name="item">The object to locate in the <see cref="IEnumerable{T}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="IEnumerable{T}"/>; otherwise, <c>false</c>.</returns>
        public static bool Contains<T>( this IEnumerable<T> sequence, T item, IEqualityComparer<T> comparer )
        {
            Ensure.Debug(sequence, arg => arg.NotNull());
            Ensure.That(comparer).NotNull();

            foreach( T i in sequence )
            {
                if( comparer.Equals(item, i) )
                    return true;
            }

            return false;
        }

        #endregion


        //// NOTE: for these three we have to use IEnumerable<T>
        ////       because overloading can easily lead to ambiguity.

        #region NullOrEmpty

        /// <summary>
        /// Determines whether the specified sequence is null, or empty.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="sequence">The object to test.</param>
        /// <returns><c>true</c> if the argument is null or empty; otherwise <c>false</c>.</returns>
        public static bool NullOrEmpty<T>( this IEnumerable<T> sequence )
        {
            if( sequence.NullReference() )
                return true;

            var collection = sequence as ICollection<T>;
            if( collection.NotNullReference() )
                return collection.Count == 0;

            var readOnlyCollection = sequence as IReadOnlyCollection<T>;
            if( readOnlyCollection.NotNullReference() )
                return readOnlyCollection.Count == 0;

            var enumerator = sequence.GetEnumerator();
            return !enumerator.MoveNext();
        }

        #endregion

        #region NullEmptyOrSparse

        /// <summary>
        /// Determines whether the specified sequence is null, empty, or whether it contains any <c>null</c> references.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="sequence">The object to test.</param>
        /// <returns><c>true</c> if the argument is null, empty, or contains any <c>null</c> references; otherwise <c>false</c>.</returns>
        public static bool NullEmptyOrSparse<T>( this IEnumerable<T> sequence )
        {
            if( sequence.NullReference() )
                return true;

            var list = sequence as IList<T>;
            if( list.NotNullReference() )
            {
                if( list.Count == 0 )
                    return true;

                int cnt = list.Count;
                for( int i = 0; i < cnt; ++i )
                {
                    if( list[i].NullReference() )
                        return true;
                }

                return false;
            }

            var readOnlyList = sequence as IReadOnlyList<T>;
            if( readOnlyList.NotNullReference() )
            {
                if( readOnlyList.Count == 0 )
                    return true;

                int cnt = readOnlyList.Count;
                for( int i = 0; i < cnt; ++i )
                {
                    if( readOnlyList[i].NullReference() )
                        return true;
                }

                return false;
            }

            var enumerator = sequence.GetEnumerator();
            if( !enumerator.MoveNext() )
            {
                return true;
            }
            else
            {
                do
                {
                    if( enumerator.Current.NullReference() )
                        return true;
                }
                while( enumerator.MoveNext() );

                return false;
            }
        }

        #endregion

        #region NullOrSparse

        /// <summary>
        /// Determines whether the specified sequence is null, or whether it contains any <c>null</c> references.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="sequence">The object to test.</param>
        /// <returns><c>true</c> if the argument is null, or contains any <c>null</c> references; otherwise <c>false</c>.</returns>
        public static bool NullOrSparse<T>( this IEnumerable<T> sequence )
        {
            if( sequence.NullReference() )
                return true;

            var list = sequence as IList<T>;
            if( list.NotNullReference() )
            {
                int cnt = list.Count;
                for( int i = 0; i < cnt; ++i )
                {
                    if( list[i].NullReference() )
                        return true;
                }

                return false;
            }

            var readOnlyList = sequence as IReadOnlyList<T>;
            if( readOnlyList.NotNullReference() )
            {
                int cnt = readOnlyList.Count;
                for( int i = 0; i < cnt; ++i )
                {
                    if( readOnlyList[i].NullReference() )
                        return true;
                }

                return false;
            }

            var enumerator = sequence.GetEnumerator();
            while( enumerator.MoveNext() )
            {
                if( enumerator.Current.NullReference() )
                    return true;
            }

            return false;
        }

        #endregion

        #region FirstOrNullable

        /// <summary>
        /// Returns the first element of a sequence, or <c>null</c> if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="sequence">The sequence to return the first element of.</param>
        /// <returns>The first element of the sequence; or <c>null</c> if it is empty.</returns>
        public static T? FirstOrNullable<T>( this IEnumerable<T> sequence )
            where T : struct
        {
            if( sequence.NullReference() )
                throw new ArgumentNullException("sequence").StoreFileLine(); // same as FirstOrDefault

            var list = sequence as IList<T>;
            if( list.NotNullReference() )
            {
                if( list.Count > 0 )
                    return list[0];
                else
                    return null;
            }

            var readOnlyList = sequence as IReadOnlyList<T>;
            if( readOnlyList.NotNullReference() )
            {
                if( readOnlyList.Count > 0 )
                    return readOnlyList[0];
                else
                    return null;
            }

            var enumerator = sequence.GetEnumerator();
            if( enumerator.MoveNext() )
                return enumerator.Current;
            else
                return null;
        }

        #endregion
    }
}
