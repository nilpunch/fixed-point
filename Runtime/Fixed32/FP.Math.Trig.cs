using System;
using System.Runtime.CompilerServices;

namespace Fixed32
{
	public partial struct FP
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
			var rawRadians = radians.RawValue % TwoPiRaw; // Map to [-2*Pi, 2*Pi)

			if (rawRadians < 0)
			{
				rawRadians += TwoPiRaw; // Map to [0, 2*Pi)
			}

			var flipVertical = rawRadians >= PiRaw;
			if (flipVertical)
			{
				rawRadians -= PiRaw; // Map to [0, Pi)
			}

			var flipHorizontal = rawRadians >= HalfPiRaw;
			if (flipHorizontal)
			{
				rawRadians = PiRaw - rawRadians; // Map to [0, Pi/2]
			}

			var lutIndex = rawRadians >> SinLutShift;

			var sinValue = SinLut[lutIndex];

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
				rawRadians += -PiRaw - HalfPiRaw;
			}
			else
			{
				rawRadians += HalfPiRaw;
			}

			return Sin(FromRaw(rawRadians));
		}

		/// <summary>
		/// Tan of the angle.
		/// Accuracy degrades when operating with huge values, and when the result is big itself.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Tan(FAngle angle)
		{
			return Tan(angle.Radians);
		}

		/// <summary>
		/// Tan of the angle in radians.
		/// Accuracy degrades when operating with huge values, and when the result is big itself.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Tan(FP radians)
		{
			var rawRadians = radians.RawValue % PiRaw; // Map to [-Pi, Pi)

			if (rawRadians < 0)
			{
				rawRadians += PiRaw; // Map to [0, Pi)
			}

			var flipVertical = rawRadians >= HalfPiRaw;
			if (flipVertical)
			{
				rawRadians = PiRaw - rawRadians; // Map to [0, Pi/2]
			}

			var lutIndex = rawRadians >> TanLutShift;

			var tanValue = TanLut[lutIndex];

			return flipVertical ? -tanValue : tanValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Asin(FP sin)
		{
			var rawValue = sin.RawValue;

			if (rawValue < -OneRaw || rawValue > OneRaw)
			{
				throw new ArgumentOutOfRangeException(nameof(sin));
			}

			var flipVertical = rawValue < 0;
			if (flipVertical)
			{
				rawValue = -rawValue; // Map to [0, 1]
			}

			var lutIndex = rawValue >> AsinLutShift;

			var asinValue = AsinLut[lutIndex];

			return flipVertical ? -asinValue : asinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Acos(FP cos)
		{
			var asin = Asin(cos).RawValue;
			return FromRaw(HalfPiRaw - asin);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan(FP tan)
		{
			return FromRaw(FCordic.Atan(tan.RawValue));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan2(FP y, FP x)
		{
			return FromRaw(FCordic.Atan2(y.RawValue, x.RawValue));
		}
	}
}
