namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		public const int SinPrecision = 16; // Corelate with lut size. Must be <= FP.FractionalBits.
		public const int SinLutShift = FP.FractionalBits - SinPrecision;
		private const int SinLutSize = (int)(FP.HalfPiRaw >> SinLutShift); // [0, HalfPi)

		public const int TanPrecision = 18; // Corelate with lut size. Must be <= FP.FractionalBits.
		public const int TanLutShift = FP.FractionalBits - TanPrecision;
		private const int TanLutSize = (int)(FP.HalfPiRaw >> TanLutShift); // [0, HalfPi)

		public const int AsinPrecision = 16; // Corelate with lut size. Must be <= FP.FractionalPlaces.
		public const int AsinLutShift = FP.FractionalBits - AsinPrecision;
		private const int AsinLutSize = (int)(FP.OneRaw >> AsinLutShift); // [0, 1)

		public const int SqrtPrecision01 = 16; // Corelate with lut size. Must be <= FP.FractionalPlaces.
		public const int SqrtLutShift01 = FP.FractionalBits - SqrtPrecision01;
		private const int SqrtLutSize01 = (int)(FP.OneRaw >> SqrtLutShift01); // [0, 1)

		public readonly static FP[] SinLut;
		public readonly static FP[] TanLut;
		public readonly static FP[] AsinLut;
		public readonly static long[] SqrtLutRaw;

		static FMath()
		{
			SinLut = GenerateSinLut();
			TanLut = GenerateTanLut();
			AsinLut = GenerateAsinLut();
			SqrtLutRaw = GenerateSqrtLut();
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

		private static long[] GenerateSqrtLut()
		{
			var lut = new long[SqrtLutSize01 + 1];
			lut[^1] = FP.OneRaw;

			for (var i = 0; i < SqrtLutSize01; i++)
			{
				var value = i.ToFP() / (SqrtLutSize01 - 1);

				lut[i] = SqrtPrecise(value.RawValue);
			}

			return lut;
		}
	}
}
