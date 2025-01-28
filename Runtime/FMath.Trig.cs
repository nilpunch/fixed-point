using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		public const int SinPrecision = 16; // Corelate with lut size. Must be <= FractionalPlaces.
		public const int TanPrecision = 18; // Corelate with lut size. Must be <= FractionalPlaces.
		public const int ArcsinPrecision = 16; // Corelate with lut size. Must be <= FractionalPlaces.

		public const int SinIterations = 5; // Taylor series iterations.
		public const int TanIterations = 20; // Continued fraction expansion iterations.
		public const int ArcsinIterations = 30; // Taylor series iterations.

		private const int SinLutShift = FP.FractionalPlaces - SinPrecision;
		private const int SinLutSize = (int)(FP.HalfPiRaw >> SinLutShift); // [0, HalfPi)

		private const int TanLutShift = FP.FractionalPlaces - TanPrecision;
		private const int TanLutSize = (int)(FP.HalfPiRaw >> TanLutShift); // [0, HalfPi)

		private const int ArcsinLutShift = FP.FractionalPlaces - ArcsinPrecision;
		private const int ArcsinLutSize = (int)(FP.OneRaw >> ArcsinLutShift); // [0, 1)

		private static FP[] s_sinLut;
		private static FP[] s_tanLut;
		private static FP[] s_arcsinLut;

		/// <summary>
		/// Must be called once to use all the trig function.
		/// Initializes lut tables.
		/// </summary>
		public static void Init()
		{
			if (s_sinLut != null)
			{
				return;
			}

			s_sinLut = GenerateSinLut();
			s_tanLut = GenerateTanLut();
			s_arcsinLut = GenerateArcsinLut();
		}

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

			var lutIndex = (int)(rawValue >> ArcsinLutShift);

			var arcsinValue = s_arcsinLut[lutIndex];

			return flipVertical ? -arcsinValue : arcsinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Acos(FP value)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Atan2(FP y, FP x)
		{
			throw new NotImplementedException();
		}

		private static FP[] GenerateSinLut()
		{
			var lut = new FP[SinLutSize + 1];
			lut[^1] = FP.One;

			for (var i = 0; i < SinLutSize; i++)
			{
				var angle = i.ToFP() / (SinLutSize - 1) * FP.HalfPi;
				var angleSqr = angle * angle;

				var result = FP.Zero;
				var term = angle;
				var factDenominator = FP.One;

				for (var n = 1; n <= SinIterations; ++n)
				{
					result += term / factDenominator;

					term *= -angleSqr;
					factDenominator *= 2 * n * (2 * n + 1);
				}

				lut[i] = result;
			}

			return lut;
		}

		private static FP[] GenerateTanLut()
		{
			var lut = new FP[TanLutSize + 1];
			lut[^1] = FP.MaxValue;

			for (var i = 0; i < TanLutSize; i++)
			{
				var angle = i.ToFP() / (TanLutSize - 1) * FP.HalfPi;
				var angleSqr = angle * angle;

				var denominator = FP.One;

				for (var n = TanIterations; n > 0; n--)
				{
					denominator = (n * 2 - 1).ToFP() - angleSqr / denominator;
				}

				lut[i] = angle / denominator;
			}

			return lut;
		}

		private static FP[] GenerateArcsinLut()
		{
			var lut = new FP[ArcsinLutSize + 1];
			lut[^1] = FP.HalfPi;

			for (var i = 0; i < ArcsinLutSize; i++)
			{
				var value = i.ToFP() / (ArcsinLutSize - 1);
				var valueSqr = value * value;

				var result = value;
				var factor = value;

				for (var n = 0; n <= ArcsinIterations; n++)
				{
					factor *= (2 * n + 1) * (2 * n + 2);
					factor *= valueSqr * (2 * n + 1);
					factor /= 4 * (n + 1) * (n + 1) * (2 * n + 3);

					result += factor;
				}

				lut[i] = result;
			}

			return lut;
		}
	}
}
