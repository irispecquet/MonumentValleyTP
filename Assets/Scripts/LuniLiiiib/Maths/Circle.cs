using UnityEngine;

namespace LuniLib.Maths
{
    public static class Circle
    {
        public static int GetLatticePointsCountWithin(float radius)
        {
            return 1 // self
                   + 4 * (int)radius // cardinal points
                   + 4 * GetDiagonalLatticePointsCount(radius); // other points
        }

        public static int GetLatticePointsCountWithin(float radius, float minRadius)
        {
            if (minRadius < 1f)
                return GetLatticePointsCountWithin(radius);
            
            if (minRadius > radius)
                return 0;
            
            minRadius -= 0.000001f; // min radius is inclusive. Unfortunately, we can't use float.Epsilon, which would be the most logical thing to do here. But you know, C# ^^

            return (minRadius > 0 ? 0 : 1) // self
                   + 4 * (int)radius - 4 * (int)minRadius // cardinal points
                   + 4 * (GetDiagonalLatticePointsCount(radius) - GetDiagonalLatticePointsCount(minRadius)); // other points
        }

        public static int GetDiagonalLatticePointsCount(float radius)
        {
            int sum = 0;
            int maxIndex = (int)radius;
            
            for (int i = 1; i <= maxIndex; i++)
                sum += (int)(Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(i, 2f)));

            return sum;
        }
    }
}