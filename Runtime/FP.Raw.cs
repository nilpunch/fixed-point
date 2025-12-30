using System;
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
		public static int Add(int x, int y)
		{
			var sum = x + y;

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
		public static int Sub(int x, int y)
		{
			var diff = x - y;

			if (((x ^ y) & (x ^ diff)) < 0)
			{
				diff = x < 0 ? MinValueRaw : MaxValueRaw;
			}

			return diff;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Negate(int x)
		{
			return x == MinValueRaw ? MaxValueRaw : -x;
		}

		/// <summary>
		/// Performs multiplication with overflow checking.<br/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Mul(int x, int y)
		{
			var result = ((long)x * y) >> FractionalBits;

			if (result > MaxValueRaw)
			{
				return MaxValueRaw;
			}

			if (result < MinValueRaw)
			{
				return MinValueRaw;
			}

			return (int)result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MulScalar(int x, int scalar)
		{
			var result = (long)x * scalar;

			if (result > MaxValueRaw)
			{
				return MaxValueRaw;
			}

			if (result < MinValueRaw)
			{
				return MinValueRaw;
			}

			return (int)result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int DivScalar(int x, int scalar)
		{
			return x == MinValueRaw && scalar == -1 ? MaxValueRaw : x / scalar;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Mod(int x, int y)
		{
			return x == MinValueRaw & y == -1 ? 0 : x % y;
		}

		/// <summary>
		/// Performs division with overflow checking.<br/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Div(int x, int y)
		{
			if (y == 0)
			{
				throw new DivideByZeroException();
			}

			var scaledX = (long)x << FractionalBits;

			if (scaledX == (long)MinValueRaw << FractionalBits && y == -1)
				return MaxValueRaw;

			var result = scaledX / y;

			if (result > MaxValueRaw)
			{
				return MaxValueRaw;
			}

			if (result < MinValueRaw)
			{
				return MinValueRaw;
			}

			return (int)result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int DivRem(int x, int y, out int remainder)
		{
			if (y == 0)
			{
				throw new DivideByZeroException();
			}

			var scaledX = (long)x << FractionalBits;

			if (scaledX == (long)MinValueRaw << FractionalBits && y == -1)
			{
				remainder = 0;
				return MaxValueRaw;
			}

			var quotient = scaledX / y;

			if (quotient > MaxValueRaw)
			{
				remainder = 0;
				return MaxValueRaw;
			}

			if (quotient < MinValueRaw)
			{
				remainder = 0;
				return MinValueRaw;
			}

			remainder = (int)(scaledX - quotient * y);
			return (int)quotient;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(ulong x)
		{
			var hi = (uint)(x >> 32);

			if (hi == 0)
			{
				return 32 + FMath.LeadingZeroCount((uint)x);
			}
			else
			{
				return FMath.LeadingZeroCount(hi);
			}
		}
	}
}
