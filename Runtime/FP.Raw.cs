using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Mathematics.Fixed
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public partial struct FP
	{
		/// <summary>
		/// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Add(in long x, in long y)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Mul(long x, long y)
		{
			var maskX = x >> AllBitsWithoutSign;
			var maskY = y >> AllBitsWithoutSign;
			var absX = (ulong)((x + maskX) ^ maskX);
			var absY = (ulong)((y + maskY) ^ maskY);

			var sign = maskX ^ maskY;

			Mult64To128(absX, absY, out ulong hi, out ulong lo);

			var shiftedHi = hi >> FractionalBits;
			var shiftedLo = hi << (AllBits - FractionalBits) | lo >> FractionalBits;

			if (shiftedLo > MaxValueRaw || shiftedHi > 0)
			{
				return sign < 0 ? MinValueRaw : MaxValueRaw;
			}

			return ((long)shiftedLo ^ sign) - sign;
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

			if (scaledY == 0)
			{
				return sign < 0 ? MinValueRaw : MaxValueRaw;
			}

			var quotient = scaledX / scaledY;

			if (quotient > MaxValueRaw)
			{
				return sign < 0 ? MinValueRaw : MaxValueRaw;
			}

			return ((long)quotient ^ sign) - sign;
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

			if (absY == 0)
			{
				reminder = 0;
				return sign < 0 ? MinValueRaw : MaxValueRaw;
			}

			var xHi = absX >> (AllBits - FractionalBits);
			var xLo = absX << FractionalBits;

			DivMod128By64(xHi, xLo, absY, out var quotient, out var reminderUnsigned);
			reminder = (long)reminderUnsigned; // reminderUnsigned is never bigger then MaxValueRaw.

			if (quotient > MaxValueRaw)
			{
				return sign < 0 ? MinValueRaw : MaxValueRaw;
			}

			return ((long)quotient ^ sign) - sign;
		}

		/// <summary>
		/// All credits goes to this guy:
		/// https://www.codeproject.com/Tips/785014/UInt-Division-Modulus
		/// </summary>
		public static void DivMod128By64(ulong u1, ulong u0, ulong v, out ulong q, out ulong r)
		{
			const ulong b = 1UL << 32;
			ulong un1, un0, vn1, vn0, q1, q0, un32, un21, un10, rhat, left, right;

			var s = LeadingZeroCount(v);
			v <<= s;
			vn1 = v >> 32;
			vn0 = v & 0xFFFFFFFF;

			if (s > 0)
			{
				un32 = (u1 << s) | (u0 >> (64 - s));
				un10 = u0 << s;
			}
			else
			{
				un32 = u1;
				un10 = u0;
			}

			un1 = un10 >> 32;
			un0 = un10 & 0xFFFFFFFF;

			q1 = un32 / vn1;
			rhat = un32 % vn1;

			left = q1 * vn0;
			right = (rhat << 32) + un1;

			again1:
			if (q1 >= b || left > right)
			{
				--q1;
				rhat += vn1;
				if (rhat < b)
				{
					left -= vn0;
					right = (rhat << 32) | un1;
					goto again1;
				}
			}

			un21 = (un32 << 32) + (un1 - (q1 * v));

			q0 = un21 / vn1;
			rhat = un21 % vn1;

			left = q0 * vn0;
			right = (rhat << 32) | un0;

			again2:
			if (q0 >= b || left > right)
			{
				--q0;
				rhat += vn1;
				if (rhat < b)
				{
					left -= vn0;
					right = (rhat << 32) | un0;
					goto again2;
				}
			}

			r = ((un21 << 32) + (un0 - (q0 * v))) >> s;
			q = (q1 << 32) | q0;
		}
		
		/// <summary>
		/// All credits goes to this guy:
		/// https://www.codeproject.com/Tips/618570/UInt-Multiplication-Squaring
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Mult64To128(ulong op1, ulong op2, out ulong hi, out ulong lo)
		{
			var u1 = op1 & 0xFFFFFFFF;
			var v1 = op2 & 0xFFFFFFFF;
			var t = u1 * v1;
			var w3 = t & 0xFFFFFFFF;
			var k = t >> 32;

			op1 >>= 32;
			t = (op1 * v1) + k;
			k = t & 0xFFFFFFFF;
			var w1 = t >> 32;

			op2 >>= 32;
			t = (u1 * op2) + k;
			k = t >> 32;

			hi = (op1 * op2) + w1 + k;
			lo = (t << 32) | w3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LeadingZeroCount(ulong x)
		{
			var hi = (uint)(x >> 32);

			if (hi == 0)
			{
				return 32 + LeadingZeroCount((uint)x);
			}

			return LeadingZeroCount(hi);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LeadingZeroCount(uint x)
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
