using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		public const int LutPrecision = 16;
		public const int PowerSeriesTerms = 5;

		public const int LutShift = FP.FractionalPlaces - LutPrecision;
		public const int LutSize = (int)(FP.HalfPiRaw >> LutShift);

		private static FP[] s_sinLut;

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
		}

		private static FP[] GenerateSinLut()
		{
			var halfPi = FP.HalfPi;

			var lut = new FP[LutSize];
			for (var i = 0; i < LutSize; i++)
			{
				var angle = (FP)i / (LutSize - 1) * halfPi;

				var result = FP.Zero;
				var pow = angle;
				var fact = FP.One;

				for (int j = 0; j < PowerSeriesTerms; ++j)
				{
					result += pow / fact;
					pow *= -1 * angle * angle;
					fact *= 2 * (j + 1) * (2 * (j + 1) + 1);
				}

				lut[i] = result;
			}

			return lut;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Tan(FP value)
		{
			throw new NotImplementedException();
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
	}
}
