using System;
using System.Runtime.CompilerServices;

namespace Fixed32
{
	public partial struct FP
	{
		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive or 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sign(FP value)
		{
			return value.RawValue < 0 ? MinusOne : One;
		}

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SignZero(FP value)
		{
			if (value.RawValue < 0)
			{
				return MinusOne;
			}
			if (value.RawValue > 0)
			{
				return One;
			}
			return Zero;
		}

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive or 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SignInt(FP value)
		{
			return value.RawValue < 0 ? -1 : 1;
		}

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SignZeroInt(FP value)
		{
			if (value.RawValue < 0)
			{
				return -1;
			}
			if (value.RawValue > 0)
			{
				return 1;
			}
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP CopySign(FP to, FP from)
		{
			return FromRaw(CopySign(to.RawValue, from.RawValue));
		}

		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Abs(FP value)
		{
			return FromRaw(Abs(value.RawValue));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Max(FP x, FP y)
		{
			if (y.RawValue > x.RawValue)
			{
				x.RawValue = y.RawValue;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Min(FP x, FP y)
		{
			if (y.RawValue < x.RawValue)
			{
				x.RawValue = y.RawValue;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Clamp(FP value, FP min, FP max)
		{
			if (value.RawValue < min.RawValue)
			{
				return min;
			}
			if (value.RawValue > max.RawValue)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Clamp01(FP value)
		{
			if (value.RawValue < 0)
			{
				return Zero;
			}
			if (value.RawValue > OneRaw)
			{
				return One;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Lerp(FP start, FP end, FP t)
		{
			if (t.RawValue < 0)
			{
				t.RawValue = 0;
			}
			if (t.RawValue > OneRaw)
			{
				t.RawValue = OneRaw;
			}
			return start + t * (end - start);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP LerpUnclamped(FP start, FP end, FP t)
		{
			return start + t * (end - start);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP MoveTowards(FP current, FP target, FP maxDelta)
		{
			return Abs(target - current) <= maxDelta ? target : current + SignInt(target - current) * maxDelta;
		}

		/// <summary>
		/// Compares two values with some epsilon and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FP x, FP y)
		{
			return ApproximatelyEqual(x, y, CalculationsEpsilonSqr);
		}

		/// <summary>
		/// Compares two values with some epsilon and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FP x, FP y, FP epsilon)
		{
			var difference = Abs(x - y);
			return difference <= epsilon;
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Floor(FP value)
		{
			return FromRaw(value.RawValue & IntegerSignMask);
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FloorToInt(FP value)
		{
			return value.ToInt();
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Ceil(FP value)
		{
			var hasFractionalPart = (value.RawValue & FractionalMask) != 0;
			return hasFractionalPart ? Floor(value) + One : value;
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CeilToInt(FP value)
		{
			var hasFractionalPart = (value.RawValue & FractionalMask) != 0;
			return hasFractionalPart ? value.ToInt() + 1 : value.ToInt();
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Round(FP value)
		{
			var fractionalPart = value.RawValue & FractionalMask;
			var integralPart = Floor(value);
			if (fractionalPart < HalfRaw)
			{
				return integralPart;
			}

			if (fractionalPart > HalfRaw)
			{
				return integralPart + One;
			}

			// If number is halfway between two values, round to the nearest even number.
			// This is the method used by System.Math.Round().
			return (integralPart.RawValue & OneRaw) == 0
				? integralPart
				: integralPart + One;
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int RoundToInt(FP value)
		{
			return Round(value).ToInt();
		}

		/// <summary>
		/// Calculates the square root of a fixed-point number, using LUT.
		/// Accuracy degrade when operating with huge values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sqrt(FP x)
		{
			return FromRaw(Sqrt(x.RawValue));
		}

		/// <summary>
		/// Calculates the square root of a fixed-point number.
		/// Has absolute precision when <see cref="FP.FractionalBits"/> &lt;= 31. Otherwise
		/// may have some rare minor inaccuracies, that are tied to absolute precision.<br/>
		/// If any, inaccuracies are in range [0, Epsilon * 1000).
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the input is negative.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SqrtPrecise(FP x)
		{
			return FromRaw(SqrtPrecise(x.RawValue));
		}

		/// <summary>
		/// Returns 2 raised to the specified power.
		/// Provides at least 6 decimals of accuracy.
		/// </summary>
		public static FP Pow2(FP x)
		{
			if (x.RawValue == 0)
			{
				return One;
			}

			// Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
			var neg = x.RawValue < 0;
			if (neg)
			{
				x = -x;
			}

			if (x == One)
			{
				return neg ? One / 2 : Two;
			}

			if (x >= Log2Max)
			{
				return neg ? One / MaxValue : MaxValue;
			}

			if (x <= Log2Min)
			{
				return neg ? MaxValue : Zero;
			}

			/* The algorithm is based on the power series for exp(x):
			* http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
			*
			* From term n, we get term n+1 by multiplying with x/n.
			* When the sum term drops to zero, we can stop summing.
			*/

			var integerPart = Floor(x).ToInt();
			// Take fractional part of exponent
			x = FromRaw(x.RawValue & FractionalMask);

			var result = One;
			var term = One;
			var i = 1;
			while (term.RawValue != 0)
			{
				term = x * term * Ln2 / i;
				result += term;
				i++;
			}

			result = FromRaw(result.RawValue << integerPart);
			if (neg)
			{
				result = One / result;
			}

			return result;
		}

		/// <summary>
		/// Returns the base-2 logarithm of a specified number.
		/// Provides at least 9 decimals of accuracy.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The argument was non-positive
		/// </exception>
		internal static FP Log2(FP x)
		{
			if (x.RawValue <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(x), x, "Non-positive value passed to Ln");
			}

			// This implementation is based on Clay. S. Turner's fast binary logarithm
			// algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
			//     Processing Mag., pp. 124,140, Sep. 2010.)

			var b = 1 << (FractionalBits - 1);
			var y = 0;

			var rawX = x.RawValue;
			while (rawX < OneRaw)
			{
				rawX <<= 1;
				y -= OneRaw;
			}

			while (rawX >= (OneRaw << 1))
			{
				rawX >>= 1;
				y += OneRaw;
			}

			var z = FromRaw(rawX);

			for (var i = 0; i < FractionalBits; i++)
			{
				z = z * z;
				if (z.RawValue >= (OneRaw << 1))
				{
					z = FromRaw(z.RawValue >> 1);
					y += b;
				}

				b >>= 1;
			}

			return FromRaw(y);
		}

		/// <summary>
		/// Returns the natural logarithm of a specified number.
		/// Provides at least 7 decimals of accuracy.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The argument was non-positive
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Ln(FP x)
		{
			return Log2(x) * Ln2;
		}

		/// <summary>
		/// Returns a specified number raised to the specified power.
		/// Provides about 5 digits of accuracy for the result.
		/// </summary>
		/// <exception cref="DivideByZeroException">
		/// The base was zero, with a negative exponent
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The base was negative, with a non-zero exponent
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Pow(FP b, FP exp)
		{
			if (b == One)
			{
				return One;
			}

			if (exp.RawValue == 0)
			{
				return One;
			}

			if (b.RawValue == 0)
			{
				if (exp.RawValue < 0)
				{
					throw new DivideByZeroException();
				}

				return Zero;
			}

			var log2 = Log2(b);
			return Pow2(exp * log2);
		}
	}
}
