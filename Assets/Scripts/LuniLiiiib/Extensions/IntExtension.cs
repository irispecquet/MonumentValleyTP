using UnityEngine;

namespace LuniLib.Extensions
{
    public static class IntExtension
    {
        #region COMPARE
        public static int CompareToVector2Int(this int value, Vector2Int range, bool minInclusive = true, bool maxInclusive = false)
        {
            int minValue = minInclusive ? range.x : range.x + 1;
            int maxValue = maxInclusive ? range.y : range.y - 1;
            
            if (value < minValue)
                return value - minValue;
            if (value > maxValue)
                return value - maxValue;

            return 0;
        }
        #endregion // COMPARE

        #region RANGE

        public static bool IsInRange(this int value, Vector2Int range, bool minInclusive = true, bool maxInclusive = false) => value.CompareToVector2Int(range, minInclusive, maxInclusive) == 0;

        #endregion // RANGE
    }
}