using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	[Serializable]
	public partial struct FP : IEquatable<FP>, IComparable<FP>, IFormattable
	{
		public const int FractionalPlaces = 31;
		public const int CalculationsEpsilonScaling = 10;

		public long RawValue;

		public FP(long rawValue)
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
			return FromRaw(x.RawValue + y.RawValue);
		}

		/// <summary>
		/// Subtracts y from x without performing overflow checking.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator -(FP x, FP y)
		{
			return FromRaw(x.RawValue - y.RawValue);
		}

		/// <summary>
		/// Performs multiplication without checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(FP x, FP y)
		{
			var xl = x.RawValue;
			var yl = y.RawValue;

			var xlo = (ulong)(xl & FractionalMask);
			var xhi = xl >> FractionalPlaces;
			var ylo = (ulong)(yl & FractionalMask);
			var yhi = yl >> FractionalPlaces;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FractionalPlaces;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FractionalPlaces;

			var sum = (long)loResult + midResult1 + midResult2 + hiResult;
			return FromRaw(sum);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, FP y)
		{
			var xl = x.RawValue;
			var yl = y.RawValue;

			if (yl == 0)
			{
				throw new DivideByZeroException();
			}

			var remainder = (ulong)(xl >= 0 ? xl : -xl);
			var divider = (ulong)(yl >= 0 ? yl : -yl);
			var quotient = 0UL;
			var bitPos = FractionalPlaces + 1;

			// If the divider is divisible by 2^n, take advantage of it.
			while ((divider & 0xF) == 0 && bitPos >= 4)
			{
				divider >>= 4;
				bitPos -= 4;
			}

			while (remainder != 0 && bitPos >= 0)
			{
				var shift = CountLeadingZeroes(remainder);
				if (shift > bitPos)
				{
					shift = bitPos;
				}

				remainder <<= shift;
				bitPos -= shift;

				var div = remainder / divider;
				remainder %= divider;
				quotient += div << bitPos;

				// Detect overflow.
				if ((div & ~(~0UL >> bitPos)) != 0)
				{
					return ((xl ^ yl) & MinValueRaw) == 0 ? MaxValue : MinValue;
				}

				remainder <<= 1;
				--bitPos;
			}

			// Rounding.
			++quotient;
			var result = (long)(quotient >> 1);
			if (((xl ^ yl) & MinValueRaw) != 0)
			{
				result = -result;
			}

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

		public override string ToString() => this.ToDouble().ToString("0.##########", System.Globalization.CultureInfo.InvariantCulture);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CountLeadingZeroes(ulong x)
		{
			var result = 0;
			while ((x & 0xF000000000000000) == 0)
			{
				result += 4;
				x <<= 4;
			}

			while ((x & 0x8000000000000000) == 0)
			{
				result += 1;
				x <<= 1;
			}

			return result;
		}
	}
}
