using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	[Serializable]
	public partial struct FP : IEquatable<FP>, IComparable<FP>, IFormattable
	{
		public const int FractionalBits = 31;
		public const int CalculationsEpsilonScaling = 10;

		public long RawValue;

		private FP(long rawValue)
		{
			RawValue = rawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe FP FromRaw(long rawValue)
		{
			return *(FP*)(&rawValue);
		}

		/// <summary>
		/// Adds x and y without performing overflow checking.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator +(FP x, FP y)
		{
			var result = x.RawValue + y.RawValue;
			return FromRaw(result);
		}

		/// <summary>
		/// Subtracts y from x without performing overflow checking.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator -(FP x, FP y)
		{
			var result = x.RawValue - y.RawValue;
			return FromRaw(result);
		}

		/// <summary>
		/// Performs multiplication without checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(FP x, FP y)
		{
			var result = Mul(x.RawValue, y.RawValue);
			return FromRaw(result);
		}

		/// <summary>
		/// Division by 0 returns min or max value, depending on the sign of the numerator.
		/// It has some rare minor inaccuracies, and they are tied to absolute precision.<br/>
		/// Inaccuracies are in range [0, Epsilon * 2000).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, FP y)
		{
			var result = Div(x.RawValue, y.RawValue);
			return FromRaw(result);
		}

		/// <summary>
		/// Performs modulo as fast as possible. Throws if x == MinValue and y == -1.
		/// Use the <see cref="FMath.SafeMod"/> for a more reliable but slower modulo.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator %(FP x, FP y)
		{
			return FromRaw(x.RawValue % y.RawValue);
		}

		/// <summary>
		/// Negate x without performing overflow checking.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator -(FP x)
		{
			return FromRaw(-x.RawValue);
		}

		/// <summary>
		/// Performs fast multiplication without checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(FP x, int scalar)
		{
			return FromRaw(x.RawValue * scalar);
		}

		/// <summary>
		/// Performs fast multiplication without checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(int scalar, FP x)
		{
			return FromRaw(x.RawValue * scalar);
		}

		/// <summary>
		/// Performs fast division without checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, int scalar)
		{
			return FromRaw(x.RawValue / scalar);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FP x, FP y)
		{
			return x.RawValue == y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FP x, FP y)
		{
			return x.RawValue != y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(FP x, FP y)
		{
			return x.RawValue > y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(FP x, FP y)
		{
			return x.RawValue < y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(FP x, FP y)
		{
			return x.RawValue >= y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(FP x, FP y)
		{
			return x.RawValue <= y.RawValue;
		}

		public override bool Equals(object obj)
		{
			return obj is FP fp && fp.RawValue == RawValue;
		}

		public override int GetHashCode()
		{
			return RawValue.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FP other)
		{
			return RawValue == other.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(FP other)
		{
			return RawValue.CompareTo(other.RawValue);
		}

		public string ToString(string format, IFormatProvider formatProvider) =>
			this.ToDouble().ToString(format, formatProvider);

		public string ToString(string format) => this.ToDouble().ToString(format);

		public string ToString(IFormatProvider provider) => this.ToDouble().ToString(provider);

		public override string ToString() => this.ToDouble().ToString("G", System.Globalization.CultureInfo.InvariantCulture);

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
			if ((~(xl ^ yl) & (xl ^ sum) & MinValueRaw) != 0)
			{
				sum = xl > 0 ? MaxValueRaw : MinValueRaw;
			}

			return FromRaw(sum);
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
			if (((xl ^ yl) & (xl ^ diff) & MinValueRaw) != 0)
			{
				diff = xl < 0 ? MinValueRaw : MaxValueRaw;
			}

			return FromRaw(diff);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeNeg(FP x)
		{
			return x.RawValue == MinValueRaw ? MaxValue : FromRaw(-x.RawValue);
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

			var xlo = (ulong)(xl & FractionalMask);
			var xhi = xl >> FractionalBits;
			var ylo = (ulong)(yl & FractionalMask);
			var yhi = yl >> FractionalBits;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FractionalBits;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FractionalBits;

			var overflow = false;
			var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
			sum = AddOverflowHelper(sum, midResult2, ref overflow);
			sum = AddOverflowHelper(sum, hiResult, ref overflow);

			var opSignsEqual = ((xl ^ yl) & MinValueRaw) == 0;

			// If signs of operands are equal and sign of result is negative,
			// then multiplication overflowed positively
			// the reverse is also true.
			if (opSignsEqual)
			{
				if (sum < 0 || (overflow && xl > 0))
				{
					return MaxValue;
				}
			}
			else
			{
				if (sum > 0)
				{
					return MinValue;
				}
			}

			// If the integer sign part of hihi (unused in the result) are neither all 0s or 1s,
			// then this means the result overflowed.
			var topCarry = hihi >> FractionalBits;
			if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
			{
				return opSignsEqual ? MaxValue : MinValue;
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

				if (sum > negOp && negOp < -OneRaw && posOp > OneRaw)
				{
					return MinValue;
				}
			}

			return FromRaw(sum);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long AddOverflowHelper(long x, long y, ref bool overflow)
		{
			var sum = x + y;
			// x + y overflows if sign(x) ^ sign(y) != sign(sum).
			overflow |= ((x ^ y ^ sum) & MinValueRaw) != 0;
			return sum;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeMod(FP x, FP y)
		{
			return FromRaw(x.RawValue == MinValueRaw & y.RawValue == -1 ? 0 : x.RawValue % y.RawValue);
		}
	}
}
