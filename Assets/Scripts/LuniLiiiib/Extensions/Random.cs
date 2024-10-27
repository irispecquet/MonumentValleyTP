using System.Collections.Generic;
using System.Linq;

namespace LuniLib.Extensions
{
    public static class Random
    {
        private static int seed;

        private static readonly Dictionary<string, System.Random> RANDOM_PER_CALLER = new();

        private static void SetSeed(int newSeed)
        {
            seed = newSeed;
        }

        public static void Init(int newSeed)
        {
            RANDOM_PER_CALLER.Clear();
            SetSeed(newSeed);
        }

        public static void InitRandomForCaller(string caller, int? specificSeed = null)
        {
            specificSeed ??= seed + caller.Length;
            caller ??= IDGenerator.CONSTANT_ID_CONTAINER.ToString();

            System.Random random = new(specificSeed.Value);
            RANDOM_PER_CALLER[caller] = random;
        }

        private static System.Random GetRandomForCaller(string caller)
        {
            caller ??= IDGenerator.CONSTANT_ID_CONTAINER.ToString();

            if (!RANDOM_PER_CALLER.ContainsKey(caller))
                InitRandomForCaller(caller);

            return RANDOM_PER_CALLER[caller];
        }

        #region RANDOM BOOL

        public static bool Bool() => Bool(IDGenerator.CONSTANT_ID_CONTAINER);

        public static bool Bool(string caller) => RandomFloat(GetRandomForCaller(caller), 0f, 1f) > 0.5f;
        public static bool Bool(object caller) => Bool(caller.GetType().Name);
        public static bool Bool(IDContainer caller) => Bool(caller.ToString());

        #endregion // RANDOM BOOL

        #region RANDOM INT

        public static int Int() => Int(IDGenerator.CONSTANT_ID_CONTAINER);
        public static int Int(int min) => Int(IDGenerator.CONSTANT_ID_CONTAINER, min);
        public static int Int(int min, int max) => Int(IDGenerator.CONSTANT_ID_CONTAINER, min, max);

        // core
        private static int RandomInt(System.Random random) => random.Next();
        private static int RandomInt(System.Random random, int min) => random.Next(min);
        private static int RandomInt(System.Random random, int min, int max) => random.Next(min, max);

        // string based
        public static int Int(string caller) => RandomInt(GetRandomForCaller(caller));
        public static int Int(string caller, int min) => RandomInt(GetRandomForCaller(caller), min);
        public static int Int(string caller, int min, int max) => RandomInt(GetRandomForCaller(caller), min, max);

        // object
        public static int Int(object caller) => Int(caller.GetType().Name);
        public static int Int(object caller, int min) => Int(caller.GetType().Name, min);
        public static int Int(object caller, int min, int max) => Int(caller.GetType().Name, min, max);

        // uid container
        public static int Int(IDContainer caller) => Int(caller.ToString());
        public static int Int(IDContainer caller, int min) => Int(caller.ToString(), min);
        public static int Int(IDContainer caller, int min, int max) => Int(caller.ToString(), min, max);

        #endregion // RANDOM INT

        #region RANDOM FLOAT

        public static float Float() => Float(IDGenerator.CONSTANT_ID_CONTAINER);
        public static float Float(float min) => Float(IDGenerator.CONSTANT_ID_CONTAINER, min);
        public static float Float(float min, float max) => Float(IDGenerator.CONSTANT_ID_CONTAINER, min, max);

        // core
        private static float RandomFloat(System.Random random) => (float)random.NextDouble();
        private static float RandomFloat(System.Random random, float min) => Float(random, min, float.MaxValue);
        private static float RandomFloat(System.Random random, float min, float max) => (float)random.NextDouble() * (max - min) + min;

        // string
        public static float Float(string caller) => RandomFloat(GetRandomForCaller(caller));
        public static float Float(string caller, float min) => RandomFloat(GetRandomForCaller(caller), min);
        public static float Float(string caller, float min, float max) => RandomFloat(GetRandomForCaller(caller), min, max);

        // object
        public static float Float(object caller) => Float(caller.GetType().Name);
        public static float Float(object caller, float min) => Float(caller.GetType().Name, min);
        public static float Float(object caller, float min, float max) => Float(caller.GetType().Name, min, max);

        // uid container
        public static float Float(IDContainer caller) => Float(caller.ToString());
        public static float Float(IDContainer caller, float min) => Float(caller.ToString(), min);
        public static float Float(IDContainer caller, float min, float max) => Float(caller.ToString(), min, max);

        #endregion // RANDOM FLOAT

        #region RANDOM ELEMENT

        public static T RandomElement<T>(System.Random random, IEnumerable<T> enumerable, int minIndex = 0, int maxIndex = -1)
        {
            T[] array = enumerable as T[] ?? enumerable.ToArray();

            if (array.Length == 0)
                return default(T);

            int n = maxIndex >= 0 ? maxIndex + 1 : array.Length;
            return array[RandomInt(random, minIndex, n)];
        }

        public static T RandomElement<T>(IEnumerable<T> enumerable, int minIndex = 0, int maxIndex = -1) => RandomElement(IDGenerator.CONSTANT_ID_CONTAINER, enumerable, minIndex, maxIndex);
        public static T RandomElement<T>(string caller, IEnumerable<T> enumerable, int minIndex = 0, int maxIndex = -1) => RandomElement(GetRandomForCaller(caller), enumerable, minIndex, maxIndex);
        public static T RandomElement<T>(object caller, IEnumerable<T> enumerable, int minIndex = 0, int maxIndex = -1) => RandomElement(caller.GetType().Name, enumerable, minIndex, maxIndex);
        public static T RandomElement<T>(IDContainer caller, IEnumerable<T> enumerable, int minIndex = 0, int maxIndex = -1) => RandomElement(caller.ToString(), enumerable, minIndex, maxIndex);

        #endregion // RANDOM ELEMENT

        #region SHUFFLE

        /// <summary>Shuffle the enumerable using the specified random. Start and end indices can be specified to shuffle a part of the enumerable only.</summary>
        /// <param name="random">The random to use.</param>
        /// <param name="enumerable">The enumerable to shuffle.</param>
        /// <param name="startIndex">The index of the first item to shuffle. Inclusive.</param>
        /// <param name="endIndex">The index of the last item to shuffle. Inclusive.</param>
        /// <typeparam name="T">The item Type.</typeparam>
        /// <returns>The shuffled enumerable.</returns>
        public static IEnumerable<T> Shuffle<T>(System.Random random, IEnumerable<T> enumerable, int startIndex = 0, int endIndex = -1)
        {
            T[] array = enumerable as T[] ?? enumerable.ToArray();
            int n = endIndex >= 0 ? endIndex + 1 : array.Length;
            int limit = startIndex + 1;

            while (n > limit)
            {
                n--;
                int k = RandomInt(random, startIndex, n + 1);
                (array[k], array[n]) = (array[n], array[k]);
            }

            return array;
        }

        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> enumerable, int startIndex = 0, int endIndex = -1) => Shuffle(IDGenerator.CONSTANT_ID_CONTAINER, enumerable, startIndex, endIndex);
        public static IEnumerable<T> Shuffle<T>(string caller, IEnumerable<T> enumerable, int startIndex = 0, int endIndex = -1) => Shuffle(GetRandomForCaller(caller), enumerable, startIndex, endIndex);
        public static IEnumerable<T> Shuffle<T>(object caller, IEnumerable<T> enumerable, int startIndex = 0, int endIndex = -1) => Shuffle(caller.GetType().Name, enumerable, startIndex, endIndex);
        public static IEnumerable<T> Shuffle<T>(IDContainer caller, IEnumerable<T> enumerable, int startIndex = 0, int endIndex = -1) => Shuffle(caller.ToString(), enumerable, startIndex, endIndex);

        /// <summary>Shuffle the list using the specified random. Start and end indices can be specified to shuffle a part of the list only. Directly modify the list!</summary>
        /// <param name="random">The random to use.</param>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="startIndex">The index of the first item to shuffle. Inclusive.</param>
        /// <param name="endIndex">The index of the last item to shuffle. Inclusive.</param>
        /// <typeparam name="T">The item Type.</typeparam>
        /// <returns>The shuffled list.</returns>
        public static void Shuffle<T>(System.Random random, IList<T> list, int startIndex = 0, int endIndex = -1)
        {
            int n = endIndex >= 0 ? endIndex + 1 : list.Count;
            int limit = startIndex + 1;

            while (n > limit)
            {
                n--;
                int k = RandomInt(random, startIndex, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static void Shuffle<T>(IList<T> list, int startIndex = 0, int endIndex = -1) => Shuffle(IDGenerator.CONSTANT_ID_CONTAINER, list, startIndex, endIndex);
        public static void Shuffle<T>(string caller, IList<T> list, int startIndex = 0, int endIndex = -1) => Shuffle(GetRandomForCaller(caller), list, startIndex, endIndex);
        public static void Shuffle<T>(object caller, IList<T> list, int startIndex = 0, int endIndex = -1) => Shuffle(caller.GetType().Name, list, startIndex, endIndex);
        public static void Shuffle<T>(IDContainer caller, IList<T> list, int startIndex = 0, int endIndex = -1) => Shuffle(caller.ToString(), list, startIndex, endIndex);

        #endregion // SHUFFLE
    }

    public class IDContainer
    {
        private readonly string _id;

        public IDContainer(string id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return _id;
        }

        public override bool Equals(object obj)
        {
            if (obj is IDContainer other)
                return _id == other._id;
            return false;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }

    public static class IDGenerator
    {
        public static readonly IDContainer CONSTANT_ID_CONTAINER = new IDContainer("CONSTANT");

        public static IDContainer GenerateUniqueID()
        {
            return new IDContainer(System.Guid.NewGuid().ToString());
        }
    }
}