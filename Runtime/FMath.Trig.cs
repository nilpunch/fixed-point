using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		public const int LutPower = 16;

		public const int LutSize = 1 << LutPower;
		public const int LutShift = FP.FractionalPlaces - LutPower;
		
		private static long[] SinLut;

		public static void Init()
		{
			if (SinLut != null)
			{
				return;
			}

			SinLut = GenerateSinLut(FP.FractionalPlaces, LutSize);
		}
		
		private static long[] GenerateSinLut(int fractionalPlaces, int lutSize)
		{
			const double piOverTwo = Math.PI / 2.0;
			var oneRaw = 1L << fractionalPlaces;

			var lut = new long[lutSize];
			for (int i = 0; i < lutSize; i++)
			{
				double angle = (i / (double)(lutSize - 1)) * piOverTwo;

				double sineValue = Math.Sin(angle);

				lut[i] = (long)(sineValue * oneRaw);
			}

			return lut;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sin(FP angle)
		{
			// Clamp angle to [0, 2*Pi)
			long rawAngle = angle.RawValue % FP.TwoPiRaw;
			if (rawAngle < 0)
			{
				rawAngle += FP.TwoPiRaw;
			}

			bool flipVertical = rawAngle >= FP.PiRaw;
			if (flipVertical)
			{
				rawAngle -= FP.PiRaw; // Map to [0, Pi)
			}

			bool flipHorizontal = rawAngle >= FP.HalfPiRaw;
			if (flipHorizontal)
			{
				rawAngle = FP.PiRaw - rawAngle; // Map to [0, Pi/2)
			}

			// Scale raw angle to LUT index
			int lutIndex = (int)(rawAngle * LutSize / FP.HalfPiRaw);
			if (lutIndex >= LutSize)
			{
				lutIndex = LutSize - 1;
			}

			long sinValue = SinLut[lutIndex];

			return FP.FromRaw(flipVertical ? -sinValue : sinValue);
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
