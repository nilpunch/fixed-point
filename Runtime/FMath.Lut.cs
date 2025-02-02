﻿namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		public const int SinPrecision = 16; // Corelate with lut size. Must be <= FP.FractionalBits.
		public const int TanPrecision = 18; // Corelate with lut size. Must be <= FP.FractionalBits.
		public const int AsinPrecision = 16; // Corelate with lut size. Must be <= FP.FractionalPlaces.
		public const int AtanPrecision = 20; // Corelate with lut size. Must be <= FP.AllBits.

		public const int SinIterations = 6; // Taylor series iterations.
		public const int AtanIterations = 30; // Taylor series iterations.

		private const int SinLutShift = FP.FractionalBits - SinPrecision;
		private const int SinLutSize = (int)(FP.HalfPiRaw >> SinLutShift); // [0, HalfPi)

		private const int TanLutShift = FP.FractionalBits - TanPrecision;
		private const int TanLutSize = (int)(FP.HalfPiRaw >> TanLutShift); // [0, HalfPi)

		private const int AsinLutShift = FP.FractionalBits - AsinPrecision;
		private const int AsinLutSize = (int)(FP.OneRaw >> AsinLutShift); // [0, 1)

		private const int AtanLutShift = FP.AllBits - AtanPrecision;
		private const int AtanLutSize = (int)(FP.MaxValueRaw >> AtanLutShift); // [0, MaxValue)

		private static FP[] s_sinLut;
		private static FP[] s_tanLut;
		private static FP[] s_asinLut;
		private static FP[] s_atanLut;

		/// <summary>
		/// Must be called once to use all the trig function.
		/// Initializes lut tables.
		/// </summary>
		public static void Init()
		{
			if (s_sinLut != null && s_tanLut != null && s_asinLut != null)
			{
				return;
			}

			s_sinLut = GenerateSinLut();
			s_tanLut = GenerateTanLut();
			s_asinLut = GenerateAsinLut();
		}

		private static FP[] GenerateSinLut()
		{
			var lut = new FP[SinLutSize + 1];
			lut[^1] = FP.One;

			for (var i = 0; i < SinLutSize; i++)
			{
				var angle = i.ToFP() / (SinLutSize - 1) * FP.HalfPi;

				lut[i] = SinSeries(angle);
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

				var sin = SinSeries(angle);
				var cos = SinSeries(FP.HalfPi - angle);

				lut[i] = sin / cos;
			}

			return lut;
		}

		private static FP[] GenerateAsinLut()
		{
			var lut = new FP[AsinLutSize + 1];
			lut[^1] = FP.HalfPi;

			for (var i = 0; i < AsinLutSize; i++)
			{
				var value = i.ToFP() / (AsinLutSize - 1);

				var angle = AtanSeries(value / Sqrt(FP.One - value * value));

				lut[i] = angle;
			}

			return lut;
		}

		private static FP SinSeries(FP angle)
		{
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

			return result;
		}

		public static FP AtanSeries(FP angle)
		{
			if (angle > FP.One || angle < -FP.One)
			{
				return FP.HalfPi - AtanSeries01(FP.One / angle);
			}

			return AtanSeries01(angle);
		}
		
		private static FP AtanSeries01(FP angle)
		{
			var angleSqr = angle * angle;

			var result = FP.Zero;
			var term = angle;
			var denominator = FP.One;

			for (var n = 1; n <= AtanIterations; ++n)
			{
				result += term / denominator;
				term *= -angleSqr;
				denominator += FP.Two;
			}

			return result;
		}
	}
}
