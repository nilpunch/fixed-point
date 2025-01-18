using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static class FMath
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sin(FP value)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Cos(FP value)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Tan(FP value)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Asin(FP value)
		{
			throw new NotImplementedException();
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Acos(FP value)
		{
			throw new NotImplementedException();
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan2(FP y, FP x)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive or 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sign(FP value)
		{
			return FP.FromRaw(1 | (value.RawValue & FP.SignMask));
		}

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SignWithZero(FP value)
		{
			return FP.FromRaw(
				value.RawValue < 0 ? -1 :
				value.RawValue > 0 ? 1 :
				0);
		}

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP CopySign(FP to, FP from)
		{
			return FP.FromRaw(to.RawValue & FP.IntegerFractionalMask | from.RawValue & FP.SignMask);
		}

		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// FastAbs(Fix64.MinValue) is undefined.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Abs(FP value)
		{
			var mask = value.RawValue >> FP.BitsAmountMinusSign;
			return FP.FromRaw((value.RawValue + mask) ^ mask);
		}

		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeAbs(FP value)
		{
			if (value.RawValue == FP.MinValueRaw)
			{
				return FP.MaxValue;
			}

			var mask = value.RawValue >> FP.BitsAmountMinusSign;
			return FP.FromRaw((value.RawValue + mask) ^ mask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Max(FP x, FP y)
		{
			long rawResult = x.RawValue;
			if (y.RawValue > x.RawValue)
			{
				rawResult = y.RawValue;
			}
			return FP.FromRaw(rawResult);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Min(FP x, FP y)
		{
			long rawResult = x.RawValue;
			if (y.RawValue < x.RawValue)
			{
				rawResult = y.RawValue;
			}
			return FP.FromRaw(rawResult);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Lerp(FP start, FP end, FP t)
		{
			if (t.RawValue < 0)
			{
				t.RawValue = 0L;
			}
			if (t.RawValue > FP.OneRaw)
			{
				t.RawValue = FP.OneRaw;
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
			return Abs(target - current) <= maxDelta ? target : current + Sign(target - current) * maxDelta;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Clamp01(FP value)
		{
			if (value.RawValue < 0)
			{
				return FP.Zero;
			}
			if (value.RawValue > FP.OneRaw)
			{
				return FP.One;
			}
			return value;
		}

		/// <summary>
		/// Compares two values with some epsilon and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FP x, FP y)
		{
			return ApproximatelyEqual(x, y, FP.CalculationsEpsilonSqr);
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
			return FP.FromRaw(value.RawValue & FP.IntegerSignMask);
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FloorToInt(FP value)
		{
			return (int)FP.FromRaw(value.RawValue & FP.IntegerSignMask);
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Ceil(FP value)
		{
			var hasFractionalPart = (value.RawValue & FP.FractionalMask) != 0;
			return hasFractionalPart ? Floor(value) + FP.One : value;
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Round(FP value)
		{
			var fractionalPart = value.RawValue & FP.FractionalMask;
			var integralPart = Floor(value);
			if (fractionalPart < FP.HalfRaw)
			{
				return integralPart;
			}

			if (fractionalPart > FP.HalfRaw)
			{
				return integralPart + FP.One;
			}

			// If number is halfway between two values, round to the nearest even number.
			// This is the method used by System.Math.Round().
			return (integralPart.RawValue & FP.OneRaw) == 0
				? integralPart
				: integralPart + FP.One;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeMod(FP x, FP y)
		{
			return FP.FromRaw(x.RawValue == FP.MinValueRaw & y.RawValue == -1 ? 0 : x.RawValue % y.RawValue);
		}

		/// <summary>
		/// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeAdd(in FP x, in FP y)
		{
			var xl = x.RawValue;
			var yl = y.RawValue;
			var sum = xl + yl;
			// If signs of operands are equal and signs of sum and x are different
			if (((~(xl ^ yl) & (xl ^ sum)) & FP.MinValueRaw) != 0)
			{
				sum = xl > 0 ? FP.MaxValueRaw : FP.MinValueRaw;
			}

			return FP.FromRaw(sum);
		}

		/// <summary>
		/// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeSub(FP x, FP y)
		{
			var xl = x.RawValue;
			var yl = y.RawValue;
			var diff = xl - yl;
			// If signs of operands are different and signs of sum and x are different.
			if ((((xl ^ yl) & (xl ^ diff)) & FP.MinValueRaw) != 0)
			{
				diff = xl < 0 ? FP.MinValueRaw : FP.MaxValueRaw;
			}

			return FP.FromRaw(diff);
		}

		/// <summary>
		/// Multiplies x by y. Performs saturating multiplaction, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeMul(FP x, FP y)
		{
			var xl = x.RawValue;
			var yl = y.RawValue;

			var xlo = (ulong)(xl & FP.FractionalMask);
			var xhi = xl >> FP.FractionalPlaces;
			var ylo = (ulong)(yl & FP.FractionalMask);
			var yhi = yl >> FP.FractionalPlaces;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FP.FractionalPlaces;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FP.FractionalPlaces;

			var overflow = false;
			var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
			sum = AddOverflowHelper(sum, midResult2, ref overflow);
			sum = AddOverflowHelper(sum, hiResult, ref overflow);

			var opSignsEqual = ((xl ^ yl) & FP.MinValueRaw) == 0;

			// If signs of operands are equal and sign of result is negative,
			// then multiplication overflowed positively
			// the reverse is also true.
			if (opSignsEqual)
			{
				if (sum < 0 || (overflow && xl > 0))
				{
					return FP.MaxValue;
				}
			}
			else
			{
				if (sum > 0)
				{
					return FP.MinValue;
				}
			}

			// If the integer sign part of hihi (unused in the result) are neither all 0s or 1s,
			// then this means the result overflowed.
			var topCarry = hihi >> FP.FractionalPlaces;
			if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
			{
				return opSignsEqual ? FP.MaxValue : FP.MinValue;
			}

			// If signs differ, both operands' magnitudes are greater than 1,
			// and the result is greater than the negative operand, then there was negative overflow.
			if (!opSignsEqual)
			{
				long posOp, negOp;
				if (xl > yl)
				{
					posOp = xl;
					negOp = yl;
				}
				else
				{
					posOp = yl;
					negOp = xl;
				}

				if (sum > negOp && negOp < -FP.OneRaw && posOp > FP.OneRaw)
				{
					return FP.MinValue;
				}
			}

			return FP.FromRaw(sum);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long AddOverflowHelper(long x, long y, ref bool overflow)
		{
			var sum = x + y;
			// x + y overflows if sign(x) ^ sign(y) != sign(sum).
			overflow |= ((x ^ y ^ sum) & FP.MinValueRaw) != 0;
			return sum;
		}

		/// <summary>
		/// Returns the square root of a specified number.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The argument was negative.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sqrt(FP x)
		{
			var xl = x.RawValue;
			if (xl < 0)
			{
				// We cannot represent infinities like Single and Double, and Sqrt is
				// mathematically undefined for x < 0. So we just throw an exception.
				throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
			}

			var num = (ulong)xl;
			var result = 0UL;

			// Second-to-top bit.
			var bit = 1UL << (FP.BitsAmount - 2);

			while (bit > num)
			{
				bit >>= 2;
			}

			// The main part is executed twice, in order to avoid
			// using 128 bit values in computations.
			for (var i = 0; i < 2; ++i)
			{
				// First we get the top 48 bits of the answer.
				while (bit != 0)
				{
					if (num >= result + bit)
					{
						num -= result + bit;
						result = (result >> 1) + bit;
					}
					else
					{
						result = result >> 1;
					}

					bit >>= 2;
				}

				if (i == 0)
				{
					// Then process it again to get the lowest 16 bits.
					if (num > (1UL << (FP.BitsAmount / 2)) - 1)
					{
						// The remainder 'num' is too large to be shifted left
						// by 32, so we have to add 1 to result manually and
						// adjust 'num' accordingly.
						// num = a - (result + 0.5)^2
						//       = num + result^2 - (result + 0.5)^2
						//       = num - result - 0.5
						num -= result;
						num = (num << (FP.BitsAmount / 2)) - FP.HalfRaw;
						result = (result << (FP.BitsAmount / 2)) + FP.HalfRaw;
					}
					else
					{
						num <<= (FP.BitsAmount / 2);
						result <<= (FP.BitsAmount / 2);
					}

					bit = 1UL << (FP.BitsAmount / 2 - 2);
				}
			}

			// Finally, if next bit would have been 1, round the result upwards.
			if (num > result)
			{
				++result;
			}

			return FP.FromRaw((long)result);
		}

		/// <summary>
		/// Returns 2 raised to the specified power.
		/// Provides at least 6 decimals of accuracy.
		/// </summary>
		internal static FP Pow2(FP x)
		{
			if (x.RawValue == 0)
			{
				return FP.One;
			}

			// Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
			var neg = x.RawValue < 0;
			if (neg)
			{
				x = -x;
			}

			if (x == FP.One)
			{
				return neg ? FP.One / (FP)2 : (FP)2;
			}

			if (x >= FP.Log2Max)
			{
				return neg ? FP.One / FP.MaxValue : FP.MaxValue;
			}

			if (x <= FP.Log2Min)
			{
				return neg ? FP.MaxValue : FP.Zero;
			}

			/* The algorithm is based on the power series for exp(x):
			* http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
			*
			* From term n, we get term n+1 by multiplying with x/n.
			* When the sum term drops to zero, we can stop summing.
			*/

			var integerPart = (int)Floor(x);
			// Take fractional part of exponent
			x = FP.FromRaw(x.RawValue & FP.FractionalMask);

			var result = FP.One;
			var term = FP.One;
			var i = 1;
			while (term.RawValue != 0)
			{
				term = x * term * FP.Ln2 / (FP)i;
				result += term;
				i++;
			}

			result = FP.FromRaw(result.RawValue << integerPart);
			if (neg)
			{
				result = FP.One / result;
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
				throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
			}

			// This implementation is based on Clay. S. Turner's fast binary logarithm
			// algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
			//     Processing Mag., pp. 124,140, Sep. 2010.)

			long b = 1U << (FP.FractionalPlaces - 1);
			long y = 0;

			var rawX = x.RawValue;
			while (rawX < FP.OneRaw)
			{
				rawX <<= 1;
				y -= FP.OneRaw;
			}

			while (rawX >= (FP.OneRaw << 1))
			{
				rawX >>= 1;
				y += FP.OneRaw;
			}

			var z = FP.FromRaw(rawX);

			for (var i = 0; i < FP.FractionalPlaces; i++)
			{
				z = z * z;
				if (z.RawValue >= (FP.OneRaw << 1))
				{
					z = FP.FromRaw(z.RawValue >> 1);
					y += b;
				}

				b >>= 1;
			}

			return FP.FromRaw(y);
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
			return Log2(x) * FP.Ln2;
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
			if (b == FP.One)
			{
				return FP.One;
			}

			if (exp.RawValue == 0)
			{
				return FP.One;
			}

			if (b.RawValue == 0)
			{
				if (exp.RawValue < 0)
				{
					throw new DivideByZeroException();
				}

				return FP.Zero;
			}

			var log2 = Log2(b);
			return Pow2(exp * log2);
		}
	}
}
