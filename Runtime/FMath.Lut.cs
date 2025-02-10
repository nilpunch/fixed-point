namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		public const int SinPrecision = 16; // Corelate with lut size. Must be <= FP.FractionalBits.
		public const int SinIterations = 6; // Taylor series iterations.
		private const int SinLutShift = FP.FractionalBits - SinPrecision;
		private const int SinLutSize = (int)(FP.HalfPiRaw >> SinLutShift); // [0, HalfPi)

		public const int TanPrecision = 18; // Corelate with lut size. Must be <= FP.FractionalBits.
		private const int TanLutShift = FP.FractionalBits - TanPrecision;
		private const int TanLutSize = (int)(FP.HalfPiRaw >> TanLutShift); // [0, HalfPi)

		public const int AsinPrecision = 16; // Corelate with lut size. Must be <= FP.FractionalPlaces.
		private const int AsinLutShift = FP.FractionalBits - AsinPrecision;
		private const int AsinLutSize = (int)(FP.OneRaw >> AsinLutShift); // [0, 1)

		public const int AtanPrecision = 20; // Corelate with lut size. Must be <= FP.AllBits.
		public const int AtanIterations = 20; // Taylor series iterations.
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

				FCordic.SinCosZeroToHalfPi(angle.RawValue, out var sin, out var cos);

				lut[i] = FP.FromRaw(sin);
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

				FCordic.SinCosZeroToHalfPi(angle.RawValue, out var sin, out var cos);

				lut[i] = FP.FromRaw(FP.Div(sin, cos));
			}

			return lut;
		}

		private static FP[] GenerateAsinLut()
		{
			var lut = new FP[AsinLutSize + 1];
			lut[^1] = FP.HalfPi;

			for (var i = 0; i < AsinLutSize; i++)
			{
				var sin = i.ToFP() / (AsinLutSize - 1);

				var angle = FCordic.AsinZeroToOne(sin.RawValue);

				lut[i] = FP.FromRaw(angle);
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

		public static FP AtanSeries(FP value)
		{
			var isNegative = value.RawValue < 0;
			if (isNegative)
			{
				value = FP.Negate(value);
			}

			const long eightTenth = FP.OneRaw * 7 / 10;
			const long fefteenTenth = FP.OneRaw * 15 / 10;

			// Increase accuracy near 1.
			var iterations = AtanIterations;
			if (value > FP.FromRaw(eightTenth) && value < FP.FromRaw(fefteenTenth))
			{
				iterations += 100;
			}

			var result = value > FP.One
				? FP.HalfPi - AtanSeries01(FP.One / value, iterations)
				: AtanSeries01(value, iterations);

			return isNegative ? -result : result;
		}

		private static FP AtanSeries01(FP value, int iterations = AtanIterations)
		{
			var valueSqr = value * value;

			var result = FP.Zero;
			var term = value;
			var denominator = FP.One;

			for (var n = 1; n <= iterations; ++n)
			{
				result += term / denominator;
				term *= -valueSqr;
				denominator += FP.Two;
			}

			return result;
		}
	}
}
