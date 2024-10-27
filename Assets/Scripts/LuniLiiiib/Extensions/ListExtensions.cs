using System;
using System.Collections.Generic;
using System.Linq;

namespace LuniLib.Extensions
{
    public static class ListExtensions
    {
        #region SHUFFLE

        public static void Shuffle<T>(this IList<T> list, int firstIndex = 0, int lastIndex = -1) => Random.Shuffle(list, firstIndex, lastIndex);
        public static void Shuffle<T>(this IList<T> list, string caller, int firstIndex = 0, int lastIndex = -1) => Random.Shuffle(caller, list, firstIndex, lastIndex);
        public static void Shuffle<T>(this IList<T> list, object caller, int firstIndex = 0, int lastIndex = -1) => Random.Shuffle(caller, list, firstIndex, lastIndex);

        #endregion // SHUFFLE

        #region FIND
        public static bool TryFind<T>(this List<T> list, Predicate<T> predicate, out T value)
        {
            T foundValue = list.Find(predicate);

            if (EqualityComparer<T>.Default.Equals(foundValue, default(T)))
            {
                value = default(T);
                return false;
            }

            value = foundValue;
            return true;
        }

        public static bool TryFind<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, out T value)
        {
            T foundValue = enumerable.FirstOrDefault(predicate);

            if (EqualityComparer<T>.Default.Equals(foundValue, default(T)))
            {
                value = default(T);
                return false;
            }

            value = foundValue;
            return true;
        }

        public static bool TryFindRandom<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, out T value)
        {
            List<T> clone = new(enumerable);
        
            while (clone.Count > 0)
            {
                T element = clone.PopAtRandom();
        
                if (!predicate(element))
                    continue;
        
                value = element;
                return true;
            }
        
            value = default(T);
            return false;
        }
        #endregion // FIND

        #region GET
        public static bool TryGetAtIndex<T>(this List<T> list, int index, out T value)
        {
            if (list.Count >= index)
            {
                value = list[index];
                return true;
            }

            value = default(T);
            return false;
        }
        #endregion // GET

        #region POP
        public static T PopAt<T>(this IList<T> list, int index)
        {
            T element = list[index];
            list.RemoveAt(index);
            return element;
        }
        
        public static T PopAtRandom<T>(this IList<T> list, int minIndex = 0, int maxIndex = -1) => list.PopAt(Random.Int(minIndex, maxIndex >= 0 ? maxIndex : list.Count));
        #endregion // POP

        #region CONTAINS
        public static bool Contains<T>(this HashSet<T> hashSet, Func<T, bool> func)
        {
            for (int index = hashSet.Count - 1; index >= 0; --index)
            {
                if (func(hashSet.ElementAt(index)))
                    return true;
            }

            return false;
        }
        #endregion // CONTAINS

        #region SPLIT
        /// <summary>
        /// Split the list in two groups. The first part of the list contains the elements verifying the splitCondition.
        /// </summary>
        /// <param name="list"> The list to split in two. </param>
        /// <param name="splitCondition"> The condition that will be tested for each element. Those who return true will be inserted at the beginning of the list. </param>
        /// <param name="startIndex"> The index to which we start splitting. /!\ Split items will still go to the start of the list! </param>
        /// <param name="maxIndex"> The index to which we stop splitting. </param>
        /// <typeparam name="T"> Any type. </typeparam>
        /// <returns> The number of elements verifying the splitCondition (the length of the first part). </returns>
        public static int Split<T>(this List<T> list, Func<T, bool> splitCondition, int startIndex = 0, int maxIndex = -1)
        {
            if (maxIndex <= 0 || maxIndex >= list.Count)
            {
                maxIndex = list.Count - 1;
            }

            int splitItemsCount = 0;
            int currentIndex = startIndex;
                
            while (currentIndex <= maxIndex)
            {
                T currentElement = list[currentIndex];
                
                if (splitCondition(currentElement))
                {
                    list.RemoveAt(currentIndex);
                    list.Insert(0, currentElement);
                    splitItemsCount++;
                }
                
                currentIndex++;
            }

            return splitItemsCount;
        }
        #endregion // SPLIT
    }
}