using System;
using UnityEngine;

namespace LuniLib.Helpers
{
    [Serializable]
    public struct DecimalInt : IEquatable<DecimalInt>, IComparable<DecimalInt>, IComparable
    {
        public const int PRECISION_DEFAULT = 100;
        public static readonly DecimalInt ZERO = new(0);

        public DecimalInt(int value, int precision = PRECISION_DEFAULT)
        {
            this.Precision = precision;
            this.value = value * this.Precision;
        }

        public DecimalInt(float value, int precision = PRECISION_DEFAULT)
        {
            this.Precision = precision;
            this.value = (int)(value * this.Precision);
        }
        
        public DecimalInt(double value, int precision = PRECISION_DEFAULT)
        {
            this.Precision = precision;
            this.value = (int)(value * this.Precision);
        }
        
        [SerializeField]
        private int value;

        [field: SerializeField]
        public int Precision { get; private set; }
        public float PrecisionFloat => this.Precision;
        public double PrecisionDouble => this.Precision;

        public int GetCorrectedValue(int precision)
        {
            return precision == this.Precision ? this.value : (int)((float)this * precision);
        }

        #region OPERATORS
        private static DecimalInt ApplyOperator(DecimalInt a, DecimalInt b, Func<int, int, int, int> op)
        {
            int biggestPrecision = Mathf.Max(1, a.Precision, b.Precision);
            int leftValue = a.GetCorrectedValue(biggestPrecision);
            int rightValue = b.GetCorrectedValue(biggestPrecision);

            return new DecimalInt
            {
                value = op(leftValue, rightValue, biggestPrecision),
                Precision = biggestPrecision,
            };
        }
        
        public static DecimalInt operator +(DecimalInt a, DecimalInt b) => ApplyOperator(a, b, (left, right, _) => left + right);

        public static DecimalInt operator -(DecimalInt a, DecimalInt b) => ApplyOperator(a, b, (left, right, _) => left - right);

        public static DecimalInt operator *(DecimalInt a, DecimalInt b) => ApplyOperator(a, b, (left, right, precision) => left * right / precision);

        public static DecimalInt operator /(DecimalInt a, DecimalInt b) => ApplyOperator(a, b, (left, right, precision) => left * precision / right);

        public static DecimalInt operator %(DecimalInt a, DecimalInt b) => ApplyOperator(a, b, (left, right, precision) => left % right * precision);

        public static bool operator ==(DecimalInt a, DecimalInt b) => a.CompareTo(b) == 0;

        public static bool operator !=(DecimalInt a, DecimalInt b) => a.CompareTo(b) != 0;

        public static bool operator <=(DecimalInt a, DecimalInt b) => a.CompareTo(b) <= 0;

        public static bool operator >=(DecimalInt a, DecimalInt b) => a.CompareTo(b) >= 0;

        public static bool operator <(DecimalInt a, DecimalInt b) => a.CompareTo(b) < 0;

        public static bool operator >(DecimalInt a, DecimalInt b) => a.CompareTo(b) > 0;
        #endregion // OPERATORS

        #region CASTS
        public static implicit operator DecimalInt(float f) => new(f);

        public static implicit operator DecimalInt(double d) => new(d);

        public static implicit operator DecimalInt(int i) => new(i);

        public static implicit operator float(DecimalInt f) => f.value / f.PrecisionFloat;

        public static implicit operator double(DecimalInt d) => d.value / d.PrecisionDouble;

        public static implicit operator int(DecimalInt i) => (int)(i.value / i.PrecisionDouble);
        #endregion // CASTS
        
        #region VALUE MANIPULATION
        // static computation
        public static DecimalInt Ceil(DecimalInt decimalInt) => new(Mathf.CeilToInt((float)decimalInt), decimalInt.Precision);
        public static DecimalInt Floor(DecimalInt decimalInt) => new(Mathf.FloorToInt((float)decimalInt), decimalInt.Precision);
        public static DecimalInt Round(DecimalInt decimalInt) => new(Mathf.RoundToInt((float)decimalInt), decimalInt.Precision);

        // value modification
        public void Ceil()
        {
            this.value = Mathf.CeilToInt((float)this) * this.Precision;
        }
        
        public void Floor()
        {
            this.value = Mathf.FloorToInt((float)this) * this.Precision;
        }

        public void Round()
        {
            this.value = Mathf.RoundToInt((float)this) * this.Precision;
        }
        #endregion // VALUE MANIPULATION

        public override string ToString() => ((float)this).ToString("0.##");

        public bool Equals(DecimalInt other) => this.value == other.value && this.Precision == other.Precision;

        public override bool Equals(object obj) => obj is DecimalInt other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(this.value, this.Precision);

        #region COMPARISON
        public int CompareTo(DecimalInt other)
        {
            int biggestPrecision = Math.Max(this.Precision, other.Precision);
            return this.GetCorrectedValue(biggestPrecision).CompareTo(other.GetCorrectedValue(biggestPrecision));
        }

        public int CompareTo(object other) => this.CompareTo((DecimalInt)other);
        #endregion // COMPARISON
    }
}