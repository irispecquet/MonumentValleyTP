namespace LuniLib.Maths
{
	using static System.Math;

	/// <summary>
	/// Methods representing some easing curves where t is clamped between 0 and 1. Should be used to tweak the t parameter of a lerp function.
	/// Warning : some of the methods below will return a result below 0 or above 1, that will require an unclamped lerp.
	/// Curves visualizations : https://easings.net/fr.
	/// </summary>
	public static class Easing
	{
		public enum Curve
		{
			LINEAR,
			IN_BACK,
			IN_BOUNCE,
			IN_CIRC,
			IN_CUBIC,
			IN_ELASTIC,
			IN_EXPO,
			IN_QUAD,
			IN_QUART,
			IN_QUINT,
			IN_SINE,
			OUT_BACK,
			OUT_BOUNCE,
			OUT_CIRC,
			OUT_CUBIC,
			OUT_ELASTIC,
			OUT_EXPO,
			OUT_QUAD,
			OUT_QUART,
			OUT_QUINT,
			OUT_SINE,
			IN_OUT_BACK,
			IN_OUT_BOUNCE,
			IN_OUT_CIRC,
			IN_OUT_CUBIC,
			IN_OUT_ELASTIC,
			IN_OUT_EXPO,
			IN_OUT_QUAD,
			IN_OUT_QUART,
			IN_OUT_QUINT,
			IN_OUT_SINE
		}
		
		public static float Ease(this float t, Curve curve)
		{
			return curve switch
			{
				Curve.IN_BACK => InBack(t),
				Curve.IN_BOUNCE => InBounce(t),
				Curve.IN_CIRC => InCirc(t),
				Curve.IN_CUBIC => InCubic(t),
				Curve.IN_ELASTIC => InElastic(t),
				Curve.IN_EXPO => InExpo(t),
				Curve.IN_QUAD => InQuad(t),
				Curve.IN_QUART => InQuart(t),
				Curve.IN_QUINT => InQuint(t),
				Curve.IN_SINE => InSine(t),
				Curve.OUT_BACK => OutBack(t),
				Curve.OUT_BOUNCE => OutBounce(t),
				Curve.OUT_CIRC => OutCirc(t),
				Curve.OUT_CUBIC => OutCubic(t),
				Curve.OUT_ELASTIC => OutElastic(t),
				Curve.OUT_EXPO => OutExpo(t),
				Curve.OUT_QUAD => OutQuad(t),
				Curve.OUT_QUART => OutQuart(t),
				Curve.OUT_QUINT => OutQuint(t),
				Curve.OUT_SINE => OutSine(t),
				Curve.IN_OUT_BACK => InOutBack(t),
				Curve.IN_OUT_BOUNCE => InOutBounce(t),
				Curve.IN_OUT_CIRC => InOutCirc(t),
				Curve.IN_OUT_CUBIC => InOutCubic(t),
				Curve.IN_OUT_ELASTIC => InOutElastic(t),
				Curve.IN_OUT_EXPO => InOutExpo(t),
				Curve.IN_OUT_QUAD => InOutQuad(t),
				Curve.IN_OUT_QUART => InOutQuart(t),
				Curve.IN_OUT_QUINT => InOutQuint(t),
				Curve.IN_OUT_SINE => InOutSine(t),
				Curve.LINEAR => t,
				_ => t
			};
		}

		#region EASING FUNCTIONS
		private static float InBack(this float t)
		{
			const float s = 1.70158f;
			return t * t * ((s + 1f) * t - s);
		}

		private static float OutBack(this float t)
		{
			const float s = 1.70158f;
			return 1f + (t - 1f) * (t - 1f) * ((s + 1f) * (t - 1f) + s);
		}

		private static float InOutBack(this float t)
		{
			const float s = 1.70158f * 1.525f;
			return t < 0.5f
				   ? 0.5f * (4f * t * t * ((s + 1f) * (t * 2f) - s))
				   : 0.5f * ((t * 2f - 2f) * (t * 2f - 2f) * ((s + 1f) * (t * 2f - 2f) + s) + 2f);
		}

		private static float InBounce(this float t)
		{
			return 1 - OutBounce(1 - t);
		}

		private static float OutBounce(this float t)
		{
			if (t < 1f / 2.75f)
				return 7.5625f * t * t;

			if (t < 2f / 2.75f)
				return 7.5625f * (t - 1.5f / 2.75f) * (t - 1.5f / 2.75f) + 0.75f;

			if (t < 2.5f / 2.75f)
				return 7.5625f * (t - 2.25f / 2.75f) * (t - 2.25f / 2.75f) + 0.9375f;

			return 7.5625f * (t - 2.625f / 2.75f) * (t - 2.625f / 2.75f) + 0.984375f;
		}

		private static float InOutBounce(this float t)
		{
			return t < 0.5f
				   ? InBounce(t * 2f) * 0.5f
				   : OutBounce(t * 2f - 1f) * 0.5f + 0.5f;
		}

		private static float InCirc(this float t)
		{
			return -(float)(Sqrt(1f - t * t) - 1f);
		}

		private static float OutCirc(this float t)
		{
			return (float)Sqrt(1f - (t * t - 2f * t + 1f));
		}

		private static float InOutCirc(this float t)
		{
			return t < 0.5f
				   ? -0.5f * ((float)Sqrt(1f - 4f * t * t) - 1f)
				   : 0.5f * ((float)Sqrt(1f - (4f * t * t - 8f * t + 4f)) + 1f);
		}

		private static float InCubic(this float t)
		{
			return t * t * t;
		}

		private static float OutCubic(this float t)
		{
			return --t * t * t + 1f;
		}

		private static float InOutCubic(this float t)
		{
			return t < 0.5f
				   ? 4f * t * t * t
				   : (t - 1f) * (4f * t * t - 8f * t + 4f) + 1f;
		}

		private static float InElastic(this float t)
		{
			if (t == 0f || t == 1f)
				return t;

			const float f = 0.3f;
			const float s = f * 0.25f;

			return -(float)(Pow(2f, 10f * (t -= 1f)) * Sin((t * 1f - s) * (2f * PI) / f));
		}

		private static float OutElastic(this float t)
		{
			if (t == 0f || t == 1f)
				return t;

			const float f = 0.3f;
			const float s = f * 0.25f;

			return (float)(Pow(2f, -10f * t) * Sin((t - s) * 2f * PI / f) + 1f);
		}

		private static float InOutElastic(this float t)
		{
			if (t == 0f || (t /= 0.5f) == 2f)
				return t;

			const float f = 0.3f;
			const float s = f * 0.25f;
			
			return t < 1f
				   ? -0.5f * (float)(Pow(2f, 10f * --t) * Sin((t - s) * 2f * PI / f))
				   : (float)(Pow(2f, -10f * --t) * Sin((t - s) * 2f * PI / f) * 0.5f + 1f);
		}

		private static float InExpo(this float t)
		{
			return (float)Pow(2f, 10f * (t - 1f));
		}

		private static float OutExpo(this float t)
		{
			return -(float)Pow(2f, -10f * t) + 1f;
		}

		private static float InOutExpo(this float t)
		{
			return t < 0.5f
				   ? 0.5f * (float)Pow(2f, 10f * (t * 2f - 1f))
				   : 0.5f * (-(float)Pow(2f, -10f * (t * 2f - 1f)) + 2f);
		}

		private static float InQuad(this float t)
		{
			return t * t;
		}

		private static float OutQuad(this float t)
		{
			return t * (2f - t);
		}

		private static float InOutQuad(this float t)
		{
			return t < 0.5f
				   ? 2f * t * t
				   : (4f - 2f * t) * t - 1f;
		}

		private static float InQuart(this float t)
		{
			return t * t * t * t;
		}

		private static float OutQuart(this float t)
		{
			return 1f - (--t) * t * t * t;
		}

		private static float InOutQuart(this float t)
		{
			return t < 0.5f
				   ? 8f * t * t * t * t
				   : 1f - 8f * --t * t * t * t;
		}

		private static float InQuint(this float t)
		{
			return t * t * t * t * t;
		}

		private static float OutQuint(this float t)
		{
			return --t * t * t * t * t + 1f;
		}

		private static float InOutQuint(this float t)
		{
			return t < 0.5f
				   ? 16f * t * t * t * t * t
				   : 16f * --t * t * t * t * t + 1f;
		}

		private static float InSine(this float t)
		{
			return -(float)Cos(t * (PI / 2f)) + 1f;
		}

		private static float OutSine(this float t)
		{
			return (float)Sin(t * (PI / 2f));
		}

		private static float InOutSine(this float t)
		{
			return -0.5f * (float)(Cos(PI * t) - 1f);
		}
		#endregion // EASING FUNCTIONS
	}
}