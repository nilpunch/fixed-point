using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long SafeNeg(long x)
		{
			return x == FP.MinValueRaw ? FP.MaxValueRaw : -x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long CopySign(long to, long from)
		{
			var signTo = to >> FP.AllBitsWithoutSign;
			var absTo = (to + signTo) ^ signTo;
			var sign = from >> FP.AllBitsWithoutSign;
			return (absTo ^ sign) - sign;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Floor(long value)
		{
			return value & FP.IntegerSignMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Abs(long value)
		{
			var mask = value >> FP.AllBitsWithoutSign;
			return (value + mask) ^ mask;
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
		public static long Sqrt(long x)
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

#pragma warning disable CS0162 // Unreachable code detected
			if (FP.FractionalBits < FP.AllBits / 2) // Faster case for FP.FractionalBits <= 31.
			{
				value <<= FP.FractionalBits;
				result <<= FP.FractionalBits;
			}
			else
			{
				LeftShift128(out var valueHigh, ref value, FP.FractionalBits);
				LeftShift128(out var resultHigh, ref result, FP.FractionalBits);

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
				high = low >> (FP.AllBits - shift);
				low <<= shift;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void RightShift128(ref ulong high, ref ulong low, int shift)
			{
				low = (high << (FP.AllBits - shift)) | (low >> shift);
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
	}
}
