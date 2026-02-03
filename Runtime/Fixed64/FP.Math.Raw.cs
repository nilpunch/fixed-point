using System;
using System.Runtime.CompilerServices;

namespace Fixed64
{
	public partial struct FP
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long SafeNeg(long x)
		{
			return x == MinValueRaw ? MaxValueRaw : -x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long CopySign(long to, long from)
		{
			var signTo = to >> AllBitsWithoutSign;
			var absTo = (to + signTo) ^ signTo;
			var sign = from >> AllBitsWithoutSign;
			return (absTo ^ sign) - sign;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Floor(long value)
		{
			return value & IntegerSignMask;
		}

		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Abs(long value)
		{
			var mask = value >> AllBitsWithoutSign;
			var t = (value ^ mask) - mask;
			return t + (t >> AllBitsWithoutSign);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Sqrt(long x)
		{
			var sqrtLut = SqrtLutRaw;
			var logTable256 = LogTable256;

			if (x <= OneRaw)
			{
				if (x < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(x), "Negative value passed to Sqrt.");
				}
				return sqrtLut[(int)(x >> SqrtLutShift01)];
			}

			// Math behind the algorithm:
			// x = m x 2^n, where m is in the LUT range [0,1]
			// sqrt(x) = sqrt(m) x 2^(n/2)
			// n = log2(x)
			// m = x / 2^n
			// sqrt(m) ≈ LUT(m)

			// Approximate upper bound of log2(x) using bit length. Essentially ceil(log2(x)).
			// We just want proper scaling to the LUT range, not a precise log2(x).
			var log2 = IntegerBitsWithSign - InlinedLeadingZeroCount(logTable256, (ulong)x);

			// Ensure n is even so that no fraction is lost when dividing n by 2.
			var n = log2 + (log2 & 1);
			var halfN = n >> 1;

			var m = x >> n;
			var sqrtM = sqrtLut[(int)(m >> SqrtLutShift01)];

			return sqrtM << halfN;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static int InlinedLeadingZeroCount(byte[] logTable256, ulong x)
			{
				const ulong MSB32 = 1UL << 32;

				var baseLog = 0;
				ulong t;
				if (x >= MSB32)
				{
					baseLog = 32;
					x >>= 32;
				}

				if ((t = x >> 24) > 0)
				{
					return 40 - (baseLog + logTable256[(int)t]);
				}
				else if ((t = x >> 16) > 0)
				{
					return 48 - (baseLog + logTable256[(int)t]);
				}
				else if ((t = x >> 8) > 0)
				{
					return 56 - (baseLog + logTable256[(int)t]);
				}
				else
				{
					return 64 - (baseLog + logTable256[(int)x]);
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
		public static long SqrtPrecise(long x)
		{
			if (x < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(x), "Negative value passed to Sqrt.");
			}

			var value = (ulong)x;
			var result = 0UL;

			const int correctionForOdd = FractionalBits & 1;

			// Find highest power of 4 <= num.
			var bit = 1UL << (AllBits - 2 + correctionForOdd);
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
			bit = 1UL << ((FractionalBits - 2 + correctionForOdd) & (AllBits - 1));

#pragma warning disable CS0162 // Unreachable code detected
			if (FractionalBits < AllBits / 2) // Faster case for FP.FractionalBits <= 31.
			{
				value <<= FractionalBits;
				result <<= FractionalBits;
			}
			else
			{
				LeftShift128(out var valueHigh, ref value, FractionalBits);
				LeftShift128(out var resultHigh, ref result, FractionalBits);

				var t = result + bit;

				// Exit early if we can continue with a standart 64-bit version.
				while (bit != 0 && (valueHigh != 0 || resultHigh != 0 || t < result))
				{
					AddToNew128(out var tHigh, out t, ref resultHigh, ref result, bit);
					RightShift128(ref resultHigh, ref result, 1);
					if (valueHigh > tHigh || (valueHigh == tHigh && value >= t))
					{
						Sub128(ref valueHigh, ref value, ref tHigh, ref t);
						Add128(ref resultHigh, ref result, bit);
					}
					bit >>= 2;
				}
			}
#pragma warning restore CS0162 // Unreachable code detected

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

			return (long)result;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void LeftShift128(out ulong high, ref ulong low, int shift)
			{
				high = low >> (AllBits - shift);
				low <<= shift;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void RightShift128(ref ulong high, ref ulong low, int shift)
			{
				low = (high << (AllBits - shift)) | (low >> shift);
				high >>= shift;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Add128(ref ulong highA, ref ulong lowA, ulong b)
			{
				var sum = lowA + b;
				if (sum < lowA)
				{
					++highA;
				}
				lowA = sum;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void AddToNew128(out ulong highC, out ulong lowC, ref ulong highA, ref ulong lowA, ulong b)
			{
				lowC = lowA + b;
				highC = highA;
				if (lowC < lowA)
				{
					++highC;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void Sub128(ref ulong highA, ref ulong lowA, ref ulong highB, ref ulong lowB)
			{
				if (lowA < lowB)
				{
					--highA;
				}
				lowA -= lowB;
				highA -= highB;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(ulong x)
		{
			const ulong MSB32 = 1UL << 32;

			var baseLog = 0;
			ulong t;
			if (x >= MSB32)
			{
				baseLog = 32;
				x >>= 32;
			}

			if ((t = x >> 24) > 0)
			{
				return 40 - (baseLog + LogTable256[(int)t]);
			}
			else if ((t = x >> 16) > 0)
			{
				return 48 - (baseLog + LogTable256[(int)t]);
			}
			else if ((t = x >> 8) > 0)
			{
				return 56 - (baseLog + LogTable256[(int)t]);
			}
			else
			{
				return 64 - (baseLog + LogTable256[(int)x]);
			}
		}
	}
}
