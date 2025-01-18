using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	[Serializable]
	public struct FP : IEquatable<FP>, IComparable<FP>, IFormattable
	{
		public const int FractionalPlaces = 32;

		public const int BitsAmount = sizeof(long) * 8;
		public const int BitsAmountMinusSign = BitsAmount - 1;
		public const long FractionalMask = (long)(0xFFFFFFFFFFFFFFFF >> (BitsAmount - FractionalPlaces));
		public const long IntegerSignMask = unchecked((long)(0xFFFFFFFFFFFFFFFF << FractionalPlaces));
		public const long IntegerFractionalMask = (long)(0xFFFFFFFFFFFFFFFF >> 1);
		public const long SignMask = 1L << BitsAmountMinusSign;

		public static FP Epsilon
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(EpsilonRaw);
		}

		public static FP MaxValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(MaxValueRaw);
		}

		public static FP MinValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(MinValueRaw);
		}

		public static FP One
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(OneRaw);
		}

		public static FP Zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(0L);
		}

		public static FP MinusOne
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(MinusOneRaw);
		}

		public static FP Half
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(HalfRaw);
		}

		public static FP Quarter
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(QuarterRaw);
		}

		public static FP Pi
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(PiRaw);
		}

		public static FP HalfPi
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(HalfPiRaw);
		}

		public static FP TwoPi
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(TwoPiRaw);
		}

		public static FP Rad2Deg
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Rad2DegRaw);
		}

		public static FP Deg2Rad
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Deg2RadRaw);
		}

		public static FP Log2Max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Log2MaxRaw);
		}

		public static FP Log2Min
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Log2MinRaw);
		}

		public static FP Ln2
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Ln2Raw);
		}

		public const long EpsilonRaw = 1L;
		public const long MaxValueRaw = long.MaxValue;
		public const long MinValueRaw = long.MinValue;
		public const long OneRaw = 1L << FractionalPlaces;
		public const long MinusOneRaw = IntegerSignMask;
		public const long HalfRaw = FractionalMask / 2;
		public const long QuarterRaw = FractionalMask / 4;

		public const double RealPi = 3.141592653589793;
		public const long PiRaw = (long)(RealPi * OneRaw);
		public const long HalfPiRaw = (long)(0.5 * RealPi * OneRaw);
		public const long TwoPiRaw = (long)(2 * RealPi * OneRaw);

		public const long Deg2RadRaw = (long)(RealPi / 180.0 * OneRaw);
		public const long Rad2DegRaw = (long)(180.0 / RealPi * OneRaw);

		public const long Ln2Raw = 0xB17217F7;
		public const long Log2MaxRaw = 0x1F00000000;
		public const long Log2MinRaw = -0x2000000000;

		public long RawValue;

		/// <summary>
		/// This is the constructor from raw value.
		/// </summary>
		/// <param name="rawValue"></param>
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

		/// <summary>
		/// Performs multiplication without checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(FP x, int scalar)
		{
			return FromRaw(x.RawValue * scalar);
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
			var bitPos = BitsAmount / 2 + 1;

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
				remainder = remainder % divider;
				quotient += div << bitPos;

				// Detect overflow
				if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, int scalar)
		{
			return FromRaw(x.RawValue / scalar);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator -(FP x)
		{
			return x.RawValue == MinValueRaw ? MaxValue : FromRaw(-x.RawValue);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator FP(long value)
		{
			return FromRaw(value * OneRaw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator long(FP value)
		{
			return value.RawValue >> FractionalPlaces;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator FP(float value)
		{
			return FromRaw((long)(value * OneRaw));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float(FP value)
		{
			return (float)value.RawValue / OneRaw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator FP(double value)
		{
			return FromRaw((long)(value * OneRaw));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double(FP value)
		{
			return (double)value.RawValue / OneRaw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator FP(decimal value)
		{
			return FromRaw((long)(value * OneRaw));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator decimal(FP value)
		{
			return (decimal)value.RawValue / OneRaw;
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
			((float)this).ToString(format, formatProvider);

		public string ToString(string format) => ((float)this).ToString(format);

		public string ToString(IFormatProvider provider) => ((float)this).ToString(provider);

		public override string ToString() => ((decimal)this).ToString("0.##########", System.Globalization.CultureInfo.InvariantCulture);

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
