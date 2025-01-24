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
		private static FP s_sinLutInterval;

		public static void Init()
		{
			if (s_sinLut != null)
			{
				return;
			}

			s_sinLut = GenerateSinLut();
			s_sinLutInterval = (FP)(LutSize - 1) / FP.HalfPi;
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
		/// The less the angle value, the more accuracy it gets.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sin(FP angle)
		{
			// Precise gradual modulo for big numbers
			// long rawAngle = angle.RawValue;
			// for (int i = 0; i < FP.IntegerPlaces - 2; ++i)
			// {
			// 	rawAngle %= (FP.PiBase61 >> i);
			// }

			// Fast modulo
			var rawAngle = angle.RawValue % FP.TwoPiRaw; // Map to [-2*Pi, 2*Pi)

			if (rawAngle < 0)
			{
				rawAngle += FP.TwoPiRaw; // Map to [0, 2*Pi)
			}

			var flipVertical = rawAngle >= FP.PiRaw;
			if (flipVertical)
			{
				rawAngle -= FP.PiRaw; // Map to [0, Pi)
			}

			var flipHorizontal = rawAngle >= FP.HalfPiRaw;
			if (flipHorizontal)
			{
				rawAngle = FP.PiRaw - rawAngle; // Map to [0, Pi/2)
			}

			var lutIndex = (int)(rawAngle >> LutShift);
			if (lutIndex >= LutSize)
			{
				lutIndex = LutSize - 1;
			}

			var sinValue = s_sinLut[lutIndex];

			return flipVertical ? -sinValue : sinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SinAcc(FP angle)
		{
			// Fast modulo
			var rawAngle = angle.RawValue % FP.TwoPiRaw; // Map to [-2*Pi, 2*Pi)

			if (rawAngle < 0)
			{
				rawAngle += FP.TwoPiRaw; // Map to [0, 2*Pi)
			}

			var flipVertical = rawAngle >= FP.PiRaw;
			if (flipVertical)
			{
				rawAngle -= FP.PiRaw; // Map to [0, Pi)
			}

			var flipHorizontal = rawAngle >= FP.HalfPiRaw;
			if (flipHorizontal)
			{
				rawAngle = FP.PiRaw - rawAngle; // Map to [0, Pi/2)
			}

			var clampedAngle = FP.FromRaw(rawAngle);

			var rawIndex = clampedAngle * s_sinLutInterval;
			var roundedIndex = Round(rawIndex);
			var indexError = rawIndex - roundedIndex;

			var nearestValue = s_sinLut[(int)roundedIndex];
			var secondNearestValue = s_sinLut[(int)(roundedIndex + SignWithZero(indexError))];

			var delta = (indexError * Abs(nearestValue - secondNearestValue)).RawValue;
			var interpolatedValue = nearestValue.RawValue + delta;
			return FP.FromRaw(flipVertical ? -interpolatedValue : interpolatedValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Cos(FP value)
		{
			throw new NotImplementedException();
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
