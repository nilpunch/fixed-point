using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		/// <summary>
		/// Sin of the angle.
		/// Accuracy degrade when operating with huge values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sin(FAngle angle)
		{
			return Sin(angle.Radians);
		}

		/// <summary>
		/// Sin of the angle in radians.
		/// Accuracy degrade when operating with huge values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sin(FP radians)
		{
			// Fast modulo
			var rawRadians = radians.RawValue % FP.TwoPiRaw; // Map to [-2*Pi, 2*Pi)

			if (rawRadians < 0)
			{
				rawRadians += FP.TwoPiRaw; // Map to [0, 2*Pi)
			}

			var flipVertical = rawRadians >= FP.PiRaw;
			if (flipVertical)
			{
				rawRadians -= FP.PiRaw; // Map to [0, Pi)
			}

			var flipHorizontal = rawRadians >= FP.HalfPiRaw;
			if (flipHorizontal)
			{
				rawRadians = FP.PiRaw - rawRadians; // Map to [0, Pi/2]
			}

			var lutIndex = (int)(rawRadians >> SinLutShift);

			var sinValue = s_sinLut[lutIndex];

			return flipVertical ? -sinValue : sinValue;
		}

		/// <summary>
		/// Cos of the angle.
		/// Accuracy degrade when operating with huge values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Cos(FAngle angle)
		{
			return Cos(angle.Radians);
		}

		/// <summary>
		/// Cos of the angle in radians.
		/// Accuracy degrade when operating with huge values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Cos(FP radians)
		{
			var rawRadians = radians.RawValue;

			if (rawRadians > 0)
			{
				rawRadians += -FP.PiRaw - FP.HalfPiRaw;
			}
			else
			{
				rawRadians += FP.HalfPiRaw;
			}

			return Sin(FP.FromRaw(rawRadians));
		}

		/// <summary>
		/// Tan of the angle in radians.
		/// Accuracy degrades when operating with huge values, and when the result is big itself.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Tan(FP radians)
		{
			// Fast modulo
			var rawRadians = radians.RawValue % FP.PiRaw; // Map to [-Pi, Pi)

			if (rawRadians < 0)
			{
				rawRadians += FP.PiRaw; // Map to [0, Pi)
			}

			var flipVertical = rawRadians >= FP.HalfPiRaw;
			if (flipVertical)
			{
				rawRadians = FP.PiRaw - rawRadians; // Map to [0, Pi/2]
			}

			var lutIndex = (int)(rawRadians >> TanLutShift);

			var tanValue = s_tanLut[lutIndex];

			return flipVertical ? SafeNeg(tanValue) : tanValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Asin(FP value)
		{
			var rawValue = value.RawValue;

			if (rawValue < -FP.OneRaw || rawValue > FP.OneRaw)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			var flipVertical = rawValue < 0;
			if (flipVertical)
			{
				rawValue = -rawValue; // Map to [0, 1]
			}

			var lutIndex = (int)(rawValue >> AsinLutShift);

			var AsinValue = s_asinLut[lutIndex];

			return flipVertical ? -AsinValue : AsinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Acos(FP value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the arctan of of the specified number, calculated using Euler series
		/// This function has at least 7 decimals of accuracy.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan(FP z)
		{
			if (z.RawValue == 0)
				return FP.Zero;

			// Force positive values for argument
			// Atan(-z) = -Atan(z).
			var neg = z.RawValue < 0;
			if (neg)
			{
				z = SafeNeg(z);
			}

			var invert = z > FP.One;
			if (invert) z = FP.One / z;

			var result = FP.One;
			var term = FP.One;

			var zSq = z * z;
			var zSq2 = zSq * FP.Two;
			var zSqPlusOne = zSq + FP.One;
			var zSq12 = zSqPlusOne * FP.Two;
			var dividend = zSq2;
			var divisor = zSqPlusOne * FP.Three;

			for (var i = 2; i < 30; ++i)
			{
				term *= dividend / divisor;
				result += term;

				dividend += zSq2;
				divisor += zSq12;

				if (term.RawValue == 0)
				{
					break;
				}
			}

			result = result * z / zSqPlusOne;

			if (invert)
			{
				result = FP.HalfPi - result;
			}

			if (neg)
			{
				result = -result;
			}

			return result;
		}

		/// <summary>
		/// Atan2 aproximation.<br/>
		/// Has fixed precision below 0.005 rad (0.2864789 deg).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan2(FP y, FP x)
		{
			// Just some magic number for approximation.
			const long consantRawBase61 = 645636042579834306L; // (long)(0.28M * (1L << 61));
			const long constantRaw = consantRawBase61 >> (61 - FP.FractionalBits);

			var yRaw = y.RawValue;
			var xRaw = x.RawValue;
			if (xRaw == 0)
			{
				if (yRaw > 0)
				{
					return FP.HalfPi;
				}
				if (yRaw == 0)
				{
					return FP.Zero;
				}
				return -FP.HalfPi;
			}

			FP angle;

			var z = y / x;
			var constant = FP.FromRaw(constantRaw);
			var denominator = SafeAdd(FP.One, SafeMul(constant * z, z));

			// Deal with overflow
			if (denominator == FP.MaxValue)
			{
				return yRaw < 0 ? -FP.HalfPi : FP.HalfPi;
			}

			if (SafeAbs(z) < FP.One)
			{
				angle = z / denominator;
				if (xRaw < 0)
				{
					if (yRaw < 0)
					{
						return angle - FP.Pi;
					}
					return angle + FP.Pi;
				}
			}
			else
			{
				angle = FP.HalfPi - z / (z * z + constant);
				if (yRaw < 0)
				{
					return angle - FP.Pi;
				}
			}
			return angle;
		}

		/// <summary>
		/// The accuracy of this is 0.01 rad in general. Ups to 0.2 with a huge numbers.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan2Fast(FP y, FP x)
		{
			const long a3 = FP.PiRaw / 16;
			const long a1 = 5 * FP.PiRaw / 16;
			const long a0 = FP.PiRaw / 4;
			const long a30 = 3 * FP.PiRaw / 4;

			var yRaw = y.RawValue;
			var xRaw = x.RawValue;

			if (xRaw == 0)
			{
				if (yRaw > 0)
				{
					return FP.HalfPi;
				}
				if (yRaw == 0)
				{
					return FP.Zero;
				}
				return -FP.HalfPi;
			}

			FP angle;

			// Absolute value of y
			var absY = SafeAbs(y);

			if (xRaw >= 0)
			{
				var r = SafeSub(x, absY) / SafeAdd(x, absY);
				var r3 = r * r * r;
				angle = FP.FromRaw(a3) * r3 - FP.FromRaw(a1) * r + FP.FromRaw(a0);
			}
			else
			{
				var r = SafeAdd(x, absY) / SafeSub(absY, x);
				var r3 = r * r * r;
				angle = FP.FromRaw(a3) * r3 - FP.FromRaw(a1) * r + FP.FromRaw(a30);
			}

			if (yRaw < 0)
			{
				angle = -angle;
			}

			return angle;
		}
	}
}
