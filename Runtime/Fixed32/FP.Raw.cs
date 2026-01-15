using System.Runtime.CompilerServices;

namespace Fixed32
{
	public partial struct FP
	{
		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Add(int x, int y)
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
		public static int Sub(int x, int y)
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
		public static int Neg(int x)
		{
			return x == MinValueRaw ? MaxValueRaw : -x;
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
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

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int DivScalar(int x, int scalar)
		{
			// There is a single edge case.
			return x == MinValueRaw && scalar == -1 ? MaxValueRaw : x / scalar;
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
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

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Mod(int x, int y)
		{
			// There is a single edge case.
			return x == MinValueRaw & y == -1 ? 0 : x % y;
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Div(int x, int y)
		{
			var scaledX = (long)x << FractionalBits;

			if (scaledX == (long)MinValueRaw << FractionalBits && y == -1)
			{
				return MaxValueRaw;
			}

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

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int DivRem(int x, int y, out int remainder)
		{
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
	}
}
