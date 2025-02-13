using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public partial struct FP
	{
		/// <summary>
		/// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Add(long x, long y)
		{
			var sum = x + y;
			// Overflow occurs if x and y have the same sign and sum has a different sign.
			if ((~(x ^ y) & (x ^ sum)) < 0)
			{
				sum = x > 0 ? MaxValueRaw : MinValueRaw;
			}

			return sum;
		}

		/// <summary>
		/// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Sub(long x, long y)
		{
			var diff = x - y;
			// Overflow occurs if x and y have different signs and x and diff have different signs.
			if (((x ^ y) & (x ^ diff)) < 0)
			{
				diff = x < 0 ? MinValueRaw : MaxValueRaw;
			}

			return diff;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Negate(long x)
		{
			return x == MinValueRaw ? MaxValueRaw : -x;
		}

		/// <summary>
		/// Performs multiplication with overflow checking.<br/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Mul(long x, long y)
		{
			var maskX = x >> AllBitsWithoutSign;
			var maskY = y >> AllBitsWithoutSign;
			var absX = (ulong)((x + maskX) ^ maskX);
			var absY = (ulong)((y + maskY) ^ maskY);

			var sign = maskX ^ maskY;

			Mul64To128(absX, absY, out ulong hi, out ulong lo);

			var shiftedHi = hi >> FractionalBits;
			var shiftedLo = hi << (AllBits - FractionalBits) | lo >> FractionalBits;

			if (shiftedLo > MaxValueRaw || shiftedHi > 0)
			{
				return sign < 0 ? MinValueRaw : MaxValueRaw;
			}

			return ((long)shiftedLo ^ sign) - sign;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long MulScalar(long x, int scalar)
		{
			var result = x * scalar;
			var expectedSign = x ^ scalar;

			// Overflow occurs if expected and result sign are different.
			if (result != 0 && (expectedSign ^ result) < 0)
			{
				return expectedSign > 0 ? MaxValueRaw : MinValueRaw;
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long DivScalar(long x, int scalar)
		{
			// There is a single edge case.
			return x == MinValueRaw && scalar == -1 ? MaxValueRaw : x / scalar;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Mod(long x, long y)
		{
			// There is a single edge case.
			return x == MinValueRaw & y == -1 ? 0 : x % y;
		}

		/// <summary>
		/// Performs division with overflow checking.<br/>
		/// Precision loss may occur when the numerator and denominator have significantly different magnitudes.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Div(long x, long y)
		{
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long DivRem(long x, long y, out long reminder)
		{
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(ulong x)
		{
			var hi = (uint)(x >> 32);

			if (hi == 0)
			{
				return 32 + LeadingZeroCount((uint)x);
			}
			else
			{
				return LeadingZeroCount(hi);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(uint x)
		{
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;

			x -= x >> 1 & 0x55555555;
			x = (x >> 2 & 0x33333333) + (x & 0x33333333);
			x = (x >> 4) + x & 0x0f0f0f0f;
			x += x >> 8;
			x += x >> 16;

			return 32 - (int)(x & 0x3F);
		}
	}
}
