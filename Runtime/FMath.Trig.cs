using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		public const int LutPrecision = 16; // Must be <= FractionalPlaces
		public const int PowerSeriesTerms = 5;

		public const int LutShift = FP.FractionalPlaces - LutPrecision;
		public const int LutSize = (int)(FP.HalfPiRaw >> LutShift);

		private static FP[] s_sinLut;
		private static FP[] s_tanLut;

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
				rawRadians = FP.PiRaw - rawRadians; // Map to [0, Pi/2)
			}

			var lutIndex = (int)(rawRadians >> LutShift);
			if (lutIndex >= LutSize)
			{
				lutIndex = LutSize - 1;
			}

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
				rawRadians = rawRadians + FP.HalfPiRaw;
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
				rawRadians = FP.PiRaw - rawRadians; // Map to [0, Pi/2)
			}

			var lutIndex = (int)(rawRadians >> LutShift);
			if (lutIndex >= LutSize)
			{
				lutIndex = LutSize - 1;
			}

			var tanValue = s_tanLut[lutIndex];

			return flipVertical ? SafeNeg(tanValue) : tanValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Asin(FP value)
		{
			throw new NotImplementedException();
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
			var halfPi = FP.HalfPi;

			var lut = new FP[LutSize];
			for (var i = 0; i < LutSize; i++)
			{
				var angle = (FP)i / (LutSize - 1) * halfPi;
				var angleSqr = angle * angle;

				var result = FP.Zero;
				var pow = angle;
				var fact = FP.One;

				for (var j = 0; j < PowerSeriesTerms; ++j)
				{
					result += pow / fact;
					pow *= -angleSqr;
					fact *= 2 * (j + 1) * (2 * (j + 1) + 1);
				}

				lut[i] = result;
			}

			return lut;
		}

		private static FP[] GenerateTanLut()
		{
			var lut = new FP[LutSize];
			var halfPi = FP.HalfPi;

			lut[^1] = FP.MaxValue;

			for (var i = 0; i < LutSize - 1; i++)
			{
				var angle = (FP)i / (LutSize - 1) * halfPi;

				lut[i] = Sin(angle) / Cos(angle);
			}

			return lut;
		}
	}
}
