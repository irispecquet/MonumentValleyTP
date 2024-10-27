using System;
using System.Collections.Generic;
using System.Linq;

namespace LuniLib.Extensions
{
    public static class DictionaryExtensions
    {
        #region GET
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : default(TValue);
        }
        #endregion // GET

        #region EDIT
        public static TObject EditValueOrCreateKey<TKey, TObject>(this Dictionary<TKey, TObject> dictionary, TKey key, TObject objectToAdd, Func<TObject, TObject, TObject> editMethod)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = editMethod(dictionary[key], objectToAdd);
            else
                dictionary[key] = objectToAdd;

            return dictionary[key];
        }
        
        public static int AddValueOrCreateKey<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int valueToAdd)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] += valueToAdd;
            else
                dictionary[key] = valueToAdd;

            return dictionary[key];
        }
        
        public static int RemoveValueAtKey<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int valueToRemove, int destroyThreshold)
        {
            int value = dictionary[key] - valueToRemove;

            if (value <= destroyThreshold)
                dictionary.Remove(key);
            else
                dictionary[key] = value;
            
            return value;
        }
        
        public static float AddValueOrCreateKey<TKey>(this Dictionary<TKey, float> dictionary, TKey key, float valueToAdd)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] += valueToAdd;
            else
                dictionary[key] = valueToAdd;

            return dictionary[key];
        }
        
        public static float RemoveValueAtKey<TKey>(this Dictionary<TKey, float> dictionary, TKey key, float valueToRemove, float destroyThreshold)
        {
            float value = dictionary[key] - valueToRemove;

            if (value <= destroyThreshold)
                dictionary.Remove(key);
            else
                dictionary[key] = value;
            
            return value;
        }
        #endregion // EDIT
        
        #region REMOVE
        public static bool TryRemoveAtKey<TKey, TObject>(this Dictionary<TKey, List<TObject>> dictionary, TKey key, TObject objectToRemove)
        {
            return dictionary.ContainsKey(key) && dictionary[key].Remove(objectToRemove);
        }
        #endregion // REMOVE

        #region ADD
        public static void AddAtKey<TKey, TObject>(this Dictionary<TKey, List<TObject>> dictionary, TKey key, TObject objectToAdd)
        {
            dictionary.AddAtKey<TKey, TObject, List<TObject>>(key, objectToAdd);
        }
        
        public static void AddAtKey<TKey, TObject, TListObject>(this Dictionary<TKey, TListObject> dictionary, TKey key, TObject objectToAdd) where TListObject : IList<TObject>, new()
        {
            if (!dictionary.ContainsKey(key) || dictionary[key] == null)
                dictionary[key] = new TListObject() { objectToAdd };
            else
                dictionary[key].Add(objectToAdd);
        }
        
        public static void AddAtKey<TKey, TObject>(this Dictionary<TKey, HashSet<TObject>> dictionary, TKey key, TObject objectToAdd)
        {
            if (!dictionary.ContainsKey(key) || dictionary[key] == null)
                dictionary[key] = new HashSet<TObject>() { objectToAdd };
            else
                dictionary[key].Add(objectToAdd);
        }

        public static Dictionary<int, int> Add(this Dictionary<int, int> dictionary, Dictionary<int, int> toAdd)
        {
            Dictionary<int, int> dicoCopy = dictionary.Copy();

            for (int i = 0; i < toAdd.Count; i++)
            {
                if (dicoCopy.ContainsKey(toAdd.ElementAt(i).Key))
                    dicoCopy[toAdd.ElementAt(i).Key] += toAdd.ElementAt(i).Value;
                else
                    dicoCopy.Add(toAdd.ElementAt(i).Key, toAdd.ElementAt(i).Value);
            }

            return dicoCopy;
        }
        #endregion // ADD

        #region COPY
        public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            Dictionary<TKey, TValue> copy = new();

            foreach (TKey key in dictionary.Keys)
                copy.Add(key, dictionary[key]);

            return copy;
        }
        #endregion // COPY

        #region MERGES
        public static Dictionary<TKey, TValue> MergeOverride<TKey, TValue>(Dictionary<TKey, TValue> initialDictionary, Dictionary<TKey, TValue> additionalDictionary)
        {
            return Merge(initialDictionary, additionalDictionary, (_, replace) => replace);
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(Dictionary<TKey, TValue> initialDictionary, Dictionary<TKey, TValue> additionalDictionary, Func<TValue, TValue, TValue> mergeMethod)
        {
            Dictionary<TKey, TValue> mergedDictionary = initialDictionary ?? new Dictionary<TKey, TValue>();

            foreach ((TKey key, TValue value) in additionalDictionary)
                mergedDictionary[key] = mergeMethod(mergedDictionary.GetValueOrDefault(key), value);

            return mergedDictionary;
        }
        #endregion // MERGES
    }
}