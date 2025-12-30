using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SafeNeg(int x)
		{
			return x == FP.MinValueRaw ? FP.MaxValueRaw : -x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CopySign(int to, int from)
		{
			var signTo = to >> FP.AllBitsWithoutSign;
			var absTo = (to + signTo) ^ signTo;
			var sign = from >> FP.AllBitsWithoutSign;
			return (absTo ^ sign) - sign;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Floor(int value)
		{
			return value & FP.IntegerSignMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Abs(int value)
		{
			var mask = value >> FP.AllBitsWithoutSign;
			return (value + mask) ^ mask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sqrt(int x)
		{
			var sqrtLut = SqrtLutRaw;
			var logTable256 = LogTable256;
			
			if (x <= FP.OneRaw)
			{
				if (x < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(x), "Negative value passed to Sqrt.");
				}
				return sqrtLut[x >> SqrtLutShift01];
			}

			// Math behind the algorithm:
			// x = m x 2^n, where m is in the LUT range [0,1]
			// sqrt(x) = sqrt(m) x 2^(n/2)
			// n = log2(x)
			// m = x / 2^n
			// sqrt(m) ≈ LUT(m)

			// Approximate upper bound of log2(x) using bit length. Essentially ceil(log2(x)).
			// We just want proper scaling to the LUT range, not a precise log2(x).
			var log2 = FP.IntegerBitsWithSign - InlinedLeadingZeroCount(logTable256, (uint)x);

			// Ensure n is even so that no fraction is lost when dividing n by 2.
			var n = log2 + (log2 & 1);
			var halfN = n >> 1;

			var m = x >> n;
			var sqrtM = sqrtLut[m >> SqrtLutShift01];

			return sqrtM << halfN;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static int InlinedLeadingZeroCount(byte[] logTable256, uint x)
			{
				uint t;

				if ((t = x >> 24) > 0)
				{
					return 7 - logTable256[t];
				}
				else if ((t = x >> 16) > 0)
				{
					return 15 - logTable256[t];
				}
				else if ((t = x >> 8) > 0)
				{
					return 23 - logTable256[t];
				}
				else
				{
					return 31 - logTable256[x];
				}
			}
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
		public static int SqrtPrecise(int x)
		{
			if (x < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(x), "Negative value passed to Sqrt.");
			}

			var value = (ulong)x;
			var result = 0UL;

			const int correctionForOdd = FP.FractionalBits & 1;

			// Find highest power of 4 <= num.
			var bit = 1UL << (FP.AllBits - 2 + correctionForOdd);
			while (bit > value)
			{
				bit >>= 2;
			}

			while (bit != 0)
			{
				var t = result + bit;
				result >>= 1;
				if (value >= t)
				{
					value -= t;
					result += bit;
				}
				bit >>= 2;
			}

			// & (FP.AllBits - 1) is a correction when FractionalBits == 0.
			bit = 1UL << ((FP.FractionalBits - 2 + correctionForOdd) & (FP.AllBits - 1));

			value <<= FP.FractionalBits;
			result <<= FP.FractionalBits;

			while (bit != 0)
			{
				var t = result + bit;
				result >>= 1;
				if (value >= t)
				{
					value -= t;
					result += bit;
				}
				bit >>= 2;
			}

			// Rounding up.
			if (value > result)
			{
				result++;
			}

			return (int)result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(uint x)
		{
			uint t;

			if ((t = x >> 24) > 0)
			{
				return 7 - LogTable256[t];
			}
			else if ((t = x >> 16) > 0)
			{
				return 15 - LogTable256[t];
			}
			else if ((t = x >> 8) > 0)
			{
				return 23 - LogTable256[t];
			}
			else
			{
				return 31 - LogTable256[x];
			}
		}
	}
}
