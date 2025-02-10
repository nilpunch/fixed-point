using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Mathematics.Fixed
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	[Il2CppEagerStaticClassConstruction]
	public static partial class FCordic
	{
		public const int Precision = FP.FractionalBits;

		// CORDIC cosine constant 0.60725...
		public const long InvGainBase63 = 5600919740058907648;
		public const long InvGain = InvGainBase63 >> (63 - FP.FractionalBits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long SinRaw(long angle)
		{
			angle = FastModTwoPi(angle);
			//angle %= FP.TwoPiRaw; // Map to [-2*Pi, 2*Pi)

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
			// Works only for angle range of [0, Pi/2]
			CordicCircular16(ref cos, ref sin, ref angle);

			return flipVertical ? -sin : sin;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long FastModTwoPi(long x)
		{
			var xShift = FP.FractionalBits / 2;
			var twoPiShift = FP.FractionalBits - xShift;

			var xDivPi = (x << FP.FractionalBits) / (FP.TwoPiRaw >> 0);
			return FP.TwoPiRaw * (FMath.Floor(xDivPi) >> FP.FractionalBits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SinCosRaw(long angle, out long sin, out long cos)
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
			// Works only for angle range of [0, Pi/2]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SinCosRawBranchless(long angle, out long sin, out long cos)
		{
			// Reduce angle modulo [–TwoPiRaw, TwoPiRaw)
			angle %= FP.TwoPiRaw;
			// If negative, add TwoPiRaw: mask = 0 if angle>=0, -1 if angle<0.
			var negMask = angle >> 63;
			angle += FP.TwoPiRaw & negMask;

			// Determine if we need to flip vertically (angle in [Pi, 2Pi))
			// If angle >= PiRaw then (angle - PiRaw) is non-negative.
			// (angle - PiRaw) >> 63 is 0 if angle>=PiRaw, -1 otherwise; invert it.
			var flipVerticalMask = ~((angle - FP.PiRaw) >> 63);
			// If flipVertical true, subtract PiRaw.
			angle -= FP.PiRaw & flipVerticalMask;

			// Determine if we need to flip horizontally (angle in [HalfPi, Pi))
			var flipHorizontalMask = ~((angle - FP.HalfPiRaw) >> 63);
			// If flipHorizontal true, set angle = PiRaw - angle.
			// This is equivalent to: angle = (flipHorizontal ? FP.PiRaw - angle : angle)
			angle = angle + ((FP.PiRaw - 2 * angle) & flipHorizontalMask);

			// Now angle is in [0, HalfPiRaw] so we can compute sin & cos with CORDIC.
			sin = 0L;
			cos = InvGain;
			CordicCircular16(ref cos, ref sin, ref angle);

			// Branchless sign correction:
			// Flip sin if we did a vertical flip.
			sin = (sin ^ flipVerticalMask) - flipVerticalMask;
			// For cos, flip sign if exactly one of the vertical or horizontal flips occurred.
			var cosSignMask = flipVerticalMask ^ flipHorizontalMask;
			cos = (cos ^ cosSignMask) - cosSignMask;
		}

		/// <summary>
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

			return flipVertical ? FMath.SafeNeg(result) : result;
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
			var tan = FP.Div(y, x);

			if (x > 0)
			{
				return Atan(tan);
			}

			if (x < 0)
			{
				if (y >= 0)
				{
					y = Atan(tan) + FP.HalfPiRaw;
				}
				else
				{
					y = Atan(tan) - FP.HalfPiRaw;
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
		/// See cordit1 from http://www.voidware.com/cordic.htm.
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
		/// See cordit1 from http://www.voidware.com/cordic.htm.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CordicVectoring(ref long xRef, ref long yRef, ref long zRef, long target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			for (int i = 0; i < Precision; i++)
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

			for (int i = 0; i < Precision; i++)
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
