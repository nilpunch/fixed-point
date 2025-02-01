using System;
using System.Linq;
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

			var sum = (long)loResult + midResult1 + midResult2 + hiResult;
			return FromRaw(sum);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, FP y)
		{
			var xRaw = x.RawValue;
			var yRaw = y.RawValue;

			if (yRaw == 0)
			{
				throw new DivideByZeroException();
			}

			var remainder = (ulong)(xRaw >= 0 ? xRaw : -xRaw);
			var divider = (ulong)(yRaw >= 0 ? yRaw : -yRaw);
			var quotient = 0UL;
			var bit = 1UL << FractionalBits;

			// The algorithm requires D >= R.
			while (divider < remainder)
			{
				divider <<= 1;
				bit <<= 1;
			}

			// Overflow.
			if (bit == 0)
			{
				return ((xRaw ^ yRaw) & MinValueRaw) == 0 ? MaxValue : MinValue;
			}

			if ((divider & (1UL << (AllBits - 1))) != 0)
			{
				// Perform one step manually to avoid overflows later.
				// We know that divider's bottom bit is 0 here.
				if (remainder >= divider)
				{
					quotient |= bit;
					remainder -= divider;
				}
				divider >>= 1;
				bit >>= 1;
			}

			while (bit != 0 && remainder != 0)
			{
				if (remainder >= divider)
				{
					quotient |= bit;
					remainder -= divider;
				}

				remainder <<= 1;
				bit >>= 1;
			}

			// Rounding.
			if (remainder >= divider && quotient < ulong.MaxValue)
			{
				quotient++;
			}

			var result = (long)quotient;

			if (result < 0)
			{
				return ((xRaw ^ yRaw) & MinValueRaw) == 0 ? MaxValue : MinValue;
			}

			// Applying the negative sign.
			if (((xRaw ^ yRaw) & MinValueRaw) != 0)
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

		public override string ToString() => this.ToDouble().ToString("G", System.Globalization.CultureInfo.InvariantCulture);
	}
}
