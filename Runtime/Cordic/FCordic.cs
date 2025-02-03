using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Mathematics.Fixed
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class FCordic
	{
		public const int Precision = 16;

		// CORDIC cosine constant 0.60725...
		public const long InvGainBase63 = 5600919740058907648;
		public const long InvGain = InvGainBase63 >> (63 - FP.FractionalBits);

		private static long[] s_tableRaw;

		public static void Init()
		{
			var rawTableBase63 = FCordicLut.RawAtanBase63;

			s_tableRaw = new long[rawTableBase63.Length];
			for (var i = 0; i < rawTableBase63.Length; i++)
			{
				s_tableRaw[i] = rawTableBase63[i] >> (63 - FP.FractionalBits);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SinCosRaw(long angle, out long sin, out long cos)
		{
			angle = angle % FP.TwoPiRaw; // Map to [-2*Pi, 2*Pi)

			if (angle < 0)
			{
				angle += FP.TwoPiRaw; // Map to [0, 2*Pi)
			}

			var flipVertical = angle >= FP.PiRaw;
			if (flipVertical)
			{
				angle -= FP.PiRaw; // Map to [0, Pi)
			}

			var flipHorizontal = angle >= FP.HalfPiRaw;
			if (flipHorizontal)
			{
				angle = FP.PiRaw - angle; // Map to [0, Pi/2]
			}

			sin = 0L;
			cos = InvGain;

			CordicCircular(ref cos, ref sin, ref angle);

			if (flipVertical)
			{
				sin = -sin;
			}

			if (flipVertical != flipHorizontal)
			{
				cos = -cos;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long TanRaw(long angle)
		{
			angle = angle % FP.PiRaw; // Map to [-Pi, Pi)

			if (angle < 0)
			{
				angle += FP.PiRaw; // Map to [0, Pi)
			}

			var flipVertical = angle >= FP.HalfPiRaw;
			if (flipVertical)
			{
				angle = FP.PiRaw - angle; // Map to [0, Pi/2]
			}

			var sin = 0L;
			var cos = InvGain;

			CordicCircular(ref cos, ref sin, ref angle);

			var result = FP.DivRaw(sin, cos);

			return flipVertical ? FP.SafNegRaw(result) : result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long AtanRaw(long a)
		{
			var x = FP.OneRaw;
			var z = 0L;

			CordicCircularVecmode(ref x, ref a, ref z, 0);

			return z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Atan2Raw(long y, long x)
		{
			if (x > 0)
			{
				return AtanRaw(y / x);
			}

			if (x < 0)
			{
				if (y >= 0)
				{
					y = AtanRaw(y / x) + FP.HalfPiRaw;
				}
				else
				{
					y = AtanRaw(y / x) - FP.HalfPiRaw;
				}
				return y;
			}

			if (y > 0)
			{
				return FP.HalfPiRaw;
			}

			if (y == 0)
			{
				return 0;
			}

			return -FP.HalfPiRaw;
		}

		/// <summary>
		/// See cordit1 from http://www.voidware.com/cordic.htm.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void CordicCircular(ref long xRef, ref long yRef, ref long zRef)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			for (var i = 0; i < Precision; ++i)
			{
				long xNext;
				if (z >= 0)
				{
					xNext = x - (y >> i);
					y = y + (x >> i);
					z -= s_tableRaw[i];
				}
				else
				{
					xNext = x + (y >> i);
					y = y - (x >> i);
					z += s_tableRaw[i];
				}
				x = xNext;
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}

		/// <summary>
		/// See cordit1 from http://www.voidware.com/cordic.htm.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void CordicCircularVecmode(ref long xRef, ref long yRef, ref long zRef, long vecmode)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			for (var i = 0; i < Precision; ++i)
			{
				long xNext;
				if (y < vecmode)
				{
					xNext = x - (y >> i);
					y = y + (x >> i);
					z -= s_tableRaw[i];
				}
				else
				{
					xNext = x + (y >> i);
					y = y - (x >> i);
					z += s_tableRaw[i];
				}
				x = xNext;
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}
	}
}
