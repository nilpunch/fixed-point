namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		public const int SinPrecision = 15; // Corelate with lut size. Must be <= FP.FractionalBits.
		public const int SinLutShift = FP.FractionalBits - SinPrecision;
		private const int SinLutSize = FP.HalfPiRaw >> SinLutShift; // [0, HalfPi)

		public const int TanPrecision = 15; // Corelate with lut size. Must be <= FP.FractionalBits.
		public const int TanLutShift = FP.FractionalBits - TanPrecision;
		private const int TanLutSize = FP.HalfPiRaw >> TanLutShift; // [0, HalfPi)

		public const int AsinPrecision = 15; // Corelate with lut size. Must be <= FP.FractionalPlaces.
		public const int AsinLutShift = FP.FractionalBits - AsinPrecision;
		private const int AsinLutSize = FP.OneRaw >> AsinLutShift; // [0, 1)

		public const int SqrtPrecision01 = 15; // Corelate with lut size. Must be <= FP.FractionalPlaces.
		public const int SqrtLutShift01 = FP.FractionalBits - SqrtPrecision01;
		private const int SqrtLutSize01 = FP.OneRaw >> SqrtLutShift01; // [0, 1)

		public readonly static FP[] SinLut;
		public readonly static FP[] TanLut;
		public readonly static FP[] AsinLut;
		public readonly static int[] SqrtLutRaw;
		public readonly static byte[] LogTable256;

		static FMath()
		{
			SinLut = GenerateSinLut();
			TanLut = GenerateTanLut();
			AsinLut = GenerateAsinLut();
			SqrtLutRaw = GenerateSqrtLut();
			LogTable256 = GenerateLZCLut();
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

		private static int[] GenerateSqrtLut()
		{
			var lut = new int[SqrtLutSize01 + 1];
			lut[^1] = FP.OneRaw;

			for (var i = 0; i < SqrtLutSize01; i++)
			{
				var value = i.ToFP() / (SqrtLutSize01 - 1);

				lut[i] = SqrtPrecise(value.RawValue);
			}

			return lut;
		}

		private static byte[] GenerateLZCLut()
		{
			var lut = new byte[256];
			lut[0] = lut[1] = 0;

			for (var i = 2; i < 256; i++)
			{
				lut[i] = (byte)(1 + lut[i / 2]);
			}

			return lut;
		}
	}
}
