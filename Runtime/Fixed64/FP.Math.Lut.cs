namespace Fixed64
{
	public partial struct FP
	{
		public const int SinPrecision = 16; // Corelate with lut size. Must satisfy the guard.
		public const int SinLutShift = FractionalBits - SinPrecision;
		private const int SinLutSize = (int)(HalfPiRaw >> SinLutShift); // [0, HalfPi)

		private const int SinPrecisionGuard = 1 / (SinPrecision <= FractionalBits ? 1 : 0);

		public const int TanPrecision = 18; // Corelate with lut size. Must satisfy the guard.
		public const int TanLutShift = FractionalBits - TanPrecision;
		private const int TanLutSize = (int)(HalfPiRaw >> TanLutShift); // [0, HalfPi)

		private const int TanPrecisionGuard = 1 / (TanPrecision <= FractionalBits ? 1 : 0);

		public const int AsinPrecision = 16; // Corelate with lut size. Must satisfy the guard.
		public const int AsinLutShift = FractionalBits - AsinPrecision;
		private const int AsinLutSize = (int)(OneRaw >> AsinLutShift); // [0, 1)

		private const int AsinPrecisionGuard = 1 / (AsinPrecision <= FractionalBits ? 1 : 0);

		public const int SqrtPrecision01 = 16; // Corelate with lut size. Must satisfy the guard.
		public const int SqrtLutShift01 = FractionalBits - SqrtPrecision01;
		private const int SqrtLutSize01 = (int)(OneRaw >> SqrtLutShift01); // [0, 1)

		private const int SqrtPrecisionGuard = 1 / (SqrtPrecision01 <= FractionalBits ? 1 : 0);

		public static readonly byte[] LogTable256;
		public static readonly FP[] SinLut;
		public static readonly FP[] TanLut;
		public static readonly FP[] AsinLut;
		public static readonly long[] SqrtLutRaw;

		static FP()
		{
			LogTable256 = GenerateLZCLut();
			SinLut = GenerateSinLut();
			TanLut = GenerateTanLut();
			AsinLut = GenerateAsinLut();
			SqrtLutRaw = GenerateSqrtLut();
		}

		private static byte[] GenerateLZCLut()
		{
			var lut = new byte[256];
			lut[0] = lut[1] = 0;

			for (var i = 2; i < 256; i++)
			{
				lut[i] = (byte)(1 + lut[i / 2]);
			}

			for (var i = 1; i < 256; i++)
			{
				lut[i]++;
			}

			return lut;
		}

		private static FP[] GenerateSinLut()
		{
			var lut = new FP[SinLutSize + 1];
			lut[^1] = One;

			for (var i = 0; i < SinLutSize; i++)
			{
				var angle = i.ToFP() / (SinLutSize - 1) * HalfPi;

				FCordic.SinCosZeroToHalfPi(angle.RawValue, out var sin, out var cos);

				lut[i] = FromRaw(sin);
			}

			return lut;
		}

		private static FP[] GenerateTanLut()
		{
			var lut = new FP[TanLutSize + 1];
			lut[^1] = MaxValue;

			for (var i = 0; i < TanLutSize; i++)
			{
				var angle = i.ToFP() / (TanLutSize - 1) * HalfPi;

				FCordic.SinCosZeroToHalfPi(angle.RawValue, out var sin, out var cos);

				lut[i] = FromRaw(Div(sin, cos));
			}

			return lut;
		}

		private static FP[] GenerateAsinLut()
		{
			var lut = new FP[AsinLutSize + 1];
			lut[^1] = HalfPi;

			for (var i = 0; i < AsinLutSize; i++)
			{
				var sin = i.ToFP() / (AsinLutSize - 1);

				var angle = FCordic.AsinZeroToOne(sin.RawValue);

				lut[i] = FromRaw(angle);
			}

			return lut;
		}

		private static long[] GenerateSqrtLut()
		{
			var lut = new long[SqrtLutSize01 + 1];
			lut[^1] = OneRaw;

			for (var i = 0; i < SqrtLutSize01; i++)
			{
				var value = i.ToFP() / (SqrtLutSize01 - 1);

				lut[i] = SqrtPrecise(value.RawValue);
			}

			return lut;
		}
	}
}
