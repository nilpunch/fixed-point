using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Fixed64
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static partial class FCordic
	{
		public const int Precision = FP.FractionalBits;

		/// <summary>
		/// CORDIC cosine constant 0.60725...<br/>
		/// Calculated using <see cref="FCordic.CalculateRawGain"/>.
		/// </summary>
		public const long InvGainBase63 = 5600919740058907648;
		public const long InvGain = InvGainBase63 >> (63 - FP.FractionalBits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Sin(long angle)
		{
			angle %= FP.TwoPiRaw; // Map to [-2*Pi, 2*Pi)

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

			var sin = 0L;
			var cos = InvGain;
			CordicCircular16(ref cos, ref sin, ref angle);

			return flipVertical ? -sin : sin;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SinCos(long angle, out long sin, out long cos)
		{
			angle %= FP.TwoPiRaw; // Map to [-2*Pi, 2*Pi)

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
			CordicCircular16(ref cos, ref sin, ref angle);

			if (flipVertical)
			{
				sin = -sin;
			}

			if (flipVertical != flipHorizontal)
			{
				cos = -cos;
			}
		}

		/// <summary>
		/// Precise cordic.
		/// Angle is [0, HalfPi].
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SinCosZeroToHalfPi(long angle, out long sin, out long cos)
		{
			sin = 0L;
			cos = InvGain;
			CordicCircular(ref cos, ref sin, ref angle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Tan(long angle)
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
			CordicCircular16(ref cos, ref sin, ref angle);

			var result = FP.Div(sin, cos);

			return flipVertical ? FP.SafeNeg(result) : result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Atan(long a)
		{
			var x = FP.OneRaw;
			var z = 0L;
			CordicVectoring16(ref x, ref a, ref z, 0);

			return z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Atan2(long y, long x)
		{
			if (x == 0)
			{
				if (y > 0)
				{
					return FP.HalfPiRaw;
				}

				if (y < 0)
				{
					return -FP.HalfPiRaw;
				}

				return 0;
			}

			var absY = FP.Abs(y);
			var absX = FP.Abs(x);

			if (absY <= absX)
			{
				var angle = Atan(FP.Div(y, x));

				if (x > 0)
				{
					return angle;
				}

				if (y >= 0)
				{
					return angle + FP.PiRaw;
				}

				return angle - FP.PiRaw;
			}
			else
			{
				var angle = FP.HalfPiRaw - Atan(FP.Div(x, y));

				if (y > 0)
				{
					return angle;
				}

				return angle - FP.PiRaw;
			}
		}

		/// <summary>
		/// Precise cordic [-1, 1].
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Asin(long sin)
		{
			if (sin < -FP.OneRaw || sin > FP.OneRaw)
			{
				throw new ArgumentOutOfRangeException(nameof(sin));
			}

			var flipVertical = sin < 0;
			if (flipVertical)
			{
				sin = -sin; // Map to [0, 1]
			}

			var result = AsinZeroToOne(sin);

			return flipVertical ? -result : result;
		}

		/// <summary>
		/// Precise cordic [-1, 1].
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long AsinZeroToOne(long sin)
		{
			var x = FP.OneRaw;
			var y = 0L;
			var z = 0L;
			CordicVectoringDoubleIteration(ref x, ref y, ref z, sin);

			return z;
		}

		/// <summary>
		/// See cordit1 from http://www.voidware.com/cordic.htm.<br/>
		/// z is [0, Pi/2].
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CordicCircular(ref long xRef, ref long yRef, ref long zRef)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			for (var i = 0; i < Precision; ++i)
			{
				if (z >= 0)
				{
					var xNext = x - (y >> i);
					y = y + (x >> i);
					x = xNext;
					z -= RawAtans[i];
				}
				else
				{
					var xNext = x + (y >> i);
					y = y - (x >> i);
					x = xNext;
					z += RawAtans[i];
				}
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}

		/// <summary>
		/// See cordit1 from http://www.voidware.com/cordic.htm.<br/>
		/// z is [0, Pi/2].
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CordicVectoring(ref long xRef, ref long yRef, ref long zRef, long target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			for (var i = 0; i < Precision; i++)
			{
				if (y < target)
				{
					var xNext = x - (y >> i);
					y = y + (x >> i);
					x = xNext;
					z -= RawAtans[i];
				}
				else
				{
					var xNext = x + (y >> i);
					y = y - (x >> i);
					x = xNext;
					z += RawAtans[i];
				}
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}

		/// <summary>
		/// See cordit1 from http://www.voidware.com/cordic.htm.<br/>
		/// Double iteration method https://stackoverflow.com/questions/25976656/cordic-arcsine-implementation-fails.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CordicVectoringDoubleIteration(ref long xRef, ref long yRef, ref long zRef, long target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			for (var i = 0; i < Precision; i++)
			{
				if (y < target == x >= 0)
				{
					var xNext = x - (y >> i);
					var yNext = y + (x >> i);
					x = xNext - (yNext >> i);
					y = yNext + (xNext >> i);

					var atan = RawAtans[i];
					z += atan + atan;
					target += target >> (i + i);
				}
				else
				{
					var xNext = x + (y >> i);
					var yNext = y - (x >> i);
					x = xNext + (yNext >> i);
					y = yNext - (xNext >> i);

					var atan = RawAtans[i];
					z -= atan + atan;
					target += target >> (i + i);
				}
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}
	}
}
