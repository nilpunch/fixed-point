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

			return flipVertical ? -tanValue : tanValue;
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

			var asinValue = s_asinLut[lutIndex];

			return flipVertical ? -asinValue : asinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Acos(FP value)
		{
			var asin = Asin(value).RawValue;
			return FP.FromRaw(FP.HalfPiRaw - asin);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan(FP z)
		{
			return FP.FromRaw(FCordic.Atan(z.RawValue));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan2(FP y, FP x)
		{
			return FP.FromRaw(FCordic.Atan2(y.RawValue, x.RawValue));
		}
	}
}
