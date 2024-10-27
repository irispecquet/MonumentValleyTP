namespace LuniLib.Maths
{
    public static class Maths
    {
        /// <summary>Computes the distance between two points.</summary>
        /// <param name="ax">The x value of the first point.</param>
        /// <param name="ay">The y value of the first point.</param>
        /// <param name="bx">The x value of the second point.</param>
        /// <param name="by">The y value of the second point.</param>
        /// <returns>The distance between the two points.</returns>
        public static float Distance(float ax, float ay, float bx, float by)
        {
            return System.MathF.Sqrt(System.MathF.Pow(bx - ax, 2) + System.MathF.Pow(by - ay, 2));
        }
        
        /// <summary>
        /// Computes the greatest common divisor of two integers.
        /// </summary>
        /// <param name="a">First integer.</param>
        /// <param name="b">Second integer.</param>
        /// <returns>Greatest common divisor.</returns>
        public static int ComputeGreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
            {
                int c = a % b;
                a = b;
                b = c;
            }

            return System.Math.Abs(a);
        }
        
        /// <summary>
        /// Computes the solution(s) to a quadratic equation ax²+bx+c=0.
        /// Returns the number of valid solutions.
        /// </summary>
        /// <param name="a">Equation first value. Cannot be equal to 0.</param>
        /// <param name="b">Equation second value.</param>
        /// <param name="c">Equation third value.</param>
        /// <param name="r1">Equation first solution.</param>
        /// <param name="r2">Equation second solution.</param>
        /// <returns>Number of valid solutions.</returns>
        public static int QuadraticEquation(float a, float b, float c, out float r1, out float r2)
        {
            float delta = b * b - 4 * a * c;

            if (delta < 0f)
            {
                r1 = UnityEngine.Mathf.Infinity;
                r2 = -r1;
                return 0;
            }
            
            r1 = (-b + UnityEngine.Mathf.Sqrt(delta)) / (2f * a);
            r2 = (-b - UnityEngine.Mathf.Sqrt(delta)) / (2f * a);

            return delta > 0f ? 2 : 1;
        }
        
        /// <summary>
        /// Custom modulo operating method to handle negative values.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="n">Second operand.</param>
        /// <returns>Modulo result.</returns>
        public static int Mod(int a, int n)
        {
            return (a % n + n) % n;
        }

        #region INVERSE LERP UNCLAMPED
        /// <summary>
        /// Determines where a value lies between two points.
        /// </summary>
        /// <param name="a">The start of the range.</param>
        /// <param name="b">The end of the range.</param>
        /// <param name="t">The point within the range you want to calculate.</param>
        /// <returns>A value between zero and one, representing where the "value" parameter falls within the range defined by a and b.</returns>
        public static float InverseLerpUnclamped(float a, float b, float t)
        {
            return System.MathF.Abs(a - b) > 0.0001f ? (t - a) / (b - a) : 0.0f;
        }
        #endregion // INVERSE LERP UNCLAMPED
        
        #region REMAP
        /// <summary>
        /// Brings any value in a given range to an unclamped custom range.
        /// </summary>
        /// <param name="x">Value to remap.</param>
        /// <param name="r1Min">Source range minimum value.</param>
        /// <param name="r1Max">Source range maximum value.</param>
        /// <param name="r2Min">Target range minimum value.</param>
        /// <param name="r2Max">Target range maximum value.</param>
        /// <returns>Remapped value.</returns>
        public static float Remap(this float x, float r1Min, float r1Max, float r2Min, float r2Max)
        {
            return r2Min + (x - r1Min) * (r2Max - r2Min) / (r1Max - r1Min);
        }

        /// <summary>
        /// Brings any value in a given range to an unclamped custom range.
        /// </summary>
        /// <param name="x">Value to remap.</param>
        /// <param name="r1">Source range.</param>
        /// <param name="r2">Target range.</param>
        /// <returns>Remapped value.</returns>
        public static float Remap(this float x, UnityEngine.Vector2 r1, UnityEngine.Vector2 r2)
        {
            return Remap(x, r1.x, r1.y, r2.x, r2.y);
        }

        /// <summary>
        /// Brings any float value in a given input range to a Vector3 in the output range.
        /// It combines InverseLerp and Lerp to make it easier to use.
        /// </summary>
        /// <param name="inputMin">The minimum input (can be greater than input max).</param>
        /// <param name="inputMax">The maximum input (can be smaller than input min).</param>
        /// <param name="outputMin">The minimum output (can be greater than output max).</param>
        /// <param name="outputMax">The maximum output (can be smaller than output min).</param>
        /// <param name="value">The current value, between inputMin and inputMax.</param>
        /// <returns>A lerp Vector3 between outputMin and outputMax depending on the input.</returns>
        public static UnityEngine.Vector3 Remap(float inputMin, float inputMax, UnityEngine.Vector3 outputMin, UnityEngine.Vector3 outputMax, float value)
        {
            float t = UnityEngine.Mathf.InverseLerp(inputMin, inputMax, value);
            return UnityEngine.Vector3.Lerp(outputMin, outputMax, t);
        }

        /// <summary>
        /// Brings any value in a given range to a clamped custom range.
        /// </summary>
        /// <param name="x">Value to remap.</param>
        /// <param name="r1Min">Source range minimum value.</param>
        /// <param name="r1Max">Source range maximum value.</param>
        /// <param name="r2Min">Target range minimum value.</param>
        /// <param name="r2Max">Target range maximum value.</param>
        /// <returns>Remapped value.</returns>
        public static float RemapClamped(this float x, float r1Min, float r1Max, float r2Min, float r2Max)
        {
            return UnityEngine.Mathf.Clamp(Remap(x, r1Min, r1Max, r2Min, r2Max), r2Min, r2Max);
        }
        
        /// <summary>
        /// Brings any value in a given range to a clamped custom range.
        /// </summary>
        /// <param name="x">Value to remap.</param>
        /// <param name="r1">Source range.</param>
        /// <param name="r2">Target range.</param>
        /// <returns>Remapped value.</returns>
        public static float RemapClamped(this float x, UnityEngine.Vector2 r1, UnityEngine.Vector2 r2)
        {
            return RemapClamped(x, r1.x, r1.y, r2.x, r2.y);
        }
        #endregion // REMAP

        #region NORMALIZATION
        /// <summary>
        /// Brings any value in a given range to the [0,1] unclamped range.
        /// </summary>
        /// <param name="x">Value to normalize.</param>
        /// <param name="rMin">Minimum range.</param>
        /// <param name="rMax">Maximum range.</param>
        /// <returns>Normalized value.</returns>
        public static float Normalize(this float x, float rMin, float rMax)
        {
            return (x - rMin) / (rMax - rMin);
        }

        /// <summary>
        /// Brings any value in a given range to the [0,1] unclamped range.
        /// </summary>
        /// <param name="x">Value to normalize.</param>
        /// <param name="r">Source range.</param>
        /// <returns>Normalized value.</returns>
        public static float Normalize(this float x, UnityEngine.Vector2 r)
        {
            return (x - r.x) / (r.y - r.x);
        }

        /// <summary>
        /// Brings any value in a given range to the [0,1] clamped range.
        /// </summary>
        /// <param name="x">Value to normalize.</param>
        /// <param name="rMin">Minimum range.</param>
        /// <param name="rMax">Maximum range.</param>
        /// <returns>Normalized value.</returns>
        public static float NormalizeClamped(this float x, float rMin, float rMax)
        {
            return UnityEngine.Mathf.Clamp01(Normalize(x, rMin, rMax));
        }

        /// <summary>
        /// Brings any value in a given range to the [0,1] clamped range.
        /// </summary>
        /// <param name="x">Value to normalize.</param>
        /// <param name="r">Source range.</param>
        /// <returns>Normalized value.</returns>
        public static float NormalizeClamped(this float x, UnityEngine.Vector2 r)
        {
            return NormalizeClamped(x, r.x, r.y);
        }
        
        /// <summary>
        /// Computes the normalized value for a given angle.
        /// </summary>
        /// <returns>Angle value between 0 and 360.</returns>
        public static float NormalizeAngle(this float a)
        {
            a %= 360f;
            if (a < 0)
                a += 360f;

            return a;
        }
        #endregion // NORMALIZATION
    }
}