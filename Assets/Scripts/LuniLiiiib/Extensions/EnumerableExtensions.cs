using System.Collections.Generic;

namespace LuniLib.Extensions
{
    public static class EnumerableExtensions
    {
        #region RANDOM
        public static T RandomElement<T>(this IEnumerable<T> enumerable, int minIndex = 0, int maxIndex = -1) => Random.RandomElement(enumerable, minIndex, maxIndex);
        public static T RandomElement<T>(this IEnumerable<T> enumerable, object caller, int minIndex = 0, int maxIndex = -1) => Random.RandomElement(caller, enumerable, minIndex, maxIndex);
        public static T RandomElement<T>(this IEnumerable<T> enumerable, string caller, int minIndex = 0, int maxIndex = -1) => Random.RandomElement(caller, enumerable, minIndex, maxIndex);
        #endregion // RANDOM
    }
}