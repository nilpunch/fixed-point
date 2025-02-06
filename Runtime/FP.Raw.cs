using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public partial struct FP
	{
		/// <summary>
		/// Performs multiplication without checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Mul(long x, long y)
		{
			var xlo = (ulong)(x & FractionalMask);
			var xhi = x >> FractionalBits;
			var ylo = (ulong)(y & FractionalMask);
			var yhi = y >> FractionalBits;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FractionalBits;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FractionalBits;

			return (long)loResult + midResult1 + midResult2 + hiResult;
		}

		/// <summary>
		/// Performs multiplication without checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Div(long x, long y)
		{
			if (y == 0)
			{
				return x < 0 ? MinValueRaw : MaxValueRaw;
			}

			var remainder = (ulong)(x >= 0 ? x : -x);
			var divider = (ulong)(y >= 0 ? y : -y);
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
				return ((x ^ y) & MinValueRaw) == 0 ? MaxValueRaw : MinValueRaw;
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
				return ((x ^ y) & MinValueRaw) == 0 ? MaxValueRaw : MinValueRaw;
			}

			// Applying the negative sign.
			if (((x ^ y) & MinValueRaw) != 0)
			{
				result = -result;
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LeadingZeroCount(ulong x)
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
