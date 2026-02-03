using System;
using System.Runtime.CompilerServices;

namespace Fixed64
{
	public partial struct FP
	{
		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Add(long x, long y)
		{
			var sum = unchecked(x + y);

			if ((~(x ^ y) & (x ^ sum)) < 0)
			{
				sum = x > 0 ? MaxValueRaw : MinValueRaw;
			}

			return sum;
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Sub(long x, long y)
		{
			var diff = unchecked(x - y);

			if (((x ^ y) & (x ^ diff)) < 0)
			{
				diff = x < 0 ? MinValueRaw : MaxValueRaw;
			}

			return diff;
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Neg(long x)
		{
			return x == MinValueRaw ? MaxValueRaw : -x;
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Mul(long x, long y)
		{
			unchecked
			{
				var maskX = x >> AllBitsWithoutSign;
				var maskY = y >> AllBitsWithoutSign;
				var absX = (ulong)((x + maskX) ^ maskX);
				var absY = (ulong)((y + maskY) ^ maskY);

				var sign = maskX ^ maskY;

				// Mul64To128
				var op1 = absX;
				var op2 = absY;
				var u1 = op1 & 0xFFFFFFFF;
				var v1 = op2 & 0xFFFFFFFF;
				var t = u1 * v1;
				var w3 = t & 0xFFFFFFFF;
				var k = t >> 32;

				op1 >>= 32;
				t = op1 * v1 + k;
				k = t & 0xFFFFFFFF;
				var w1 = t >> 32;

				op2 >>= 32;
				t = u1 * op2 + k;
				k = t >> 32;

				var hi = op1 * op2 + w1 + k;
				var lo = (t << 32) | w3;

				var shiftedHi = hi >> FractionalBits;
				var shiftedLo = hi << (AllBits - FractionalBits) | lo >> FractionalBits;

				if (shiftedLo > MaxValueRaw || shiftedHi > 0)
				{
					return sign < 0 ? MinValueRaw : MaxValueRaw;
				}

				return ((long)shiftedLo ^ sign) - sign;
			}
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long MulScalar(long x, int scalar)
		{
			unchecked
			{
				var result = x * scalar;

				if (x == 0 || scalar == 0)
				{
					return result;
				}

				if (result / scalar != x)
				{
					return (x ^ scalar) < 0 ? MinValueRaw : MaxValueRaw;
				}

				return result;
			}
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long DivScalar(long x, int scalar)
		{
			// There is a single edge case.
			return x == MinValueRaw && scalar == -1 ? MaxValueRaw : x / scalar;
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Mod(long x, long y)
		{
			// There is a single edge case.
			return x == MinValueRaw & y == -1 ? 0 : x % y;
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.<br/>
		/// Precision loss may occur when the numerator and denominator have significantly different magnitudes.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Div(long x, long y)
		{
			unchecked
			{
				if (y == 0)
				{
					throw new DivideByZeroException();
				}

				if (x == 0)
				{
					return 0;
				}

				var maskX = x >> AllBitsWithoutSign;
				var maskY = y >> AllBitsWithoutSign;
				var absX = (ulong)((x + maskX) ^ maskX);
				var absY = (ulong)((y + maskY) ^ maskY);
				var sign = maskX ^ maskY;

				var xLzc = LeadingZeroCount(absX);
				var xShift = xLzc > FractionalBits ? FractionalBits : xLzc;
				var yShift = FractionalBits - xShift;

				var scaledX = absX << xShift;
				var scaledY = absY >> yShift;

				var nonZeroY = scaledY == 0 ? 1 : scaledY;

				var quotient = scaledX / nonZeroY;

				var overflowValue = sign < 0 ? MinValueRaw : MaxValueRaw;
				var signedQuotient = ((long)quotient ^ sign) - sign;
				var result = quotient > MaxValueRaw ? overflowValue : signedQuotient;

				var finalResult = scaledY == 0 ? overflowValue : result;

				return finalResult;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long DivRem(long x, long y, out long reminder)
		{
			unchecked
			{
				if (y == 0)
				{
					throw new DivideByZeroException();
				}

				if (x == 0)
				{
					reminder = y;
					return 0;
				}

				var maskX = x >> AllBitsWithoutSign;
				var maskY = y >> AllBitsWithoutSign;
				var absX = (ulong)((x + maskX) ^ maskX);
				var absY = (ulong)((y + maskY) ^ maskY);
				var sign = maskX ^ maskY;

				var xLzc = LeadingZeroCount(absX);
				var xShift = xLzc > FractionalBits ? FractionalBits : xLzc;
				var yShift = FractionalBits - xShift;

				var scaledX = absX << xShift;
				var scaledY = absY >> yShift;

				if (scaledY == 0)
				{
					reminder = 0;
					return sign < 0 ? MinValueRaw : MaxValueRaw;
				}

				var quotient = scaledX / scaledY;

				if (quotient > MaxValueRaw)
				{
					reminder = 0;
					return sign < 0 ? MinValueRaw : MaxValueRaw;
				}

				reminder = ((long)(scaledX - quotient * scaledY) ^ sign) - sign;
				return ((long)quotient ^ sign) - sign;
			}
		}

		/// <summary>
		/// All credits goes to this guy:
		/// https://www.codeproject.com/Tips/618570/UInt-Multiplication-Squaring
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Mul64To128(ulong op1, ulong op2, out ulong hi, out ulong lo)
		{
			var u1 = op1 & 0xFFFFFFFF;
			var v1 = op2 & 0xFFFFFFFF;
			var t = u1 * v1;
			var w3 = t & 0xFFFFFFFF;
			var k = t >> 32;

			op1 >>= 32;
			t = op1 * v1 + k;
			k = t & 0xFFFFFFFF;
			var w1 = t >> 32;

			op2 >>= 32;
			t = u1 * op2 + k;
			k = t >> 32;

			hi = op1 * op2 + w1 + k;
			lo = (t << 32) | w3;
		}
	}
}
