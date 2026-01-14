using System.Runtime.CompilerServices;

namespace Fixed32
{
	public partial struct FP
	{
		public const int AllBits = sizeof(int) * 8;

		public const int AllBitsWithoutSign = AllBits - 1;
		public const int IntegerBits = AllBitsWithoutSign - FractionalBits;
		public const int IntegerBitsWithSign = IntegerBits + 1;
		public const int FractionalMask = int.MaxValue >> (AllBitsWithoutSign - FractionalBits);
		public const int IntegerSignMask = int.MaxValue << FractionalBits;
		public const int IntegerFractionalMask = int.MaxValue;
		public const int SignMask = int.MinValue;

		public const int EpsilonRaw = 1;
		public const int CalculationsEpsilonSqrRaw = EpsilonRaw * CalculationsEpsilonScaling;
		public const int CalculationsEpsilonRaw = CalculationsEpsilonSqrRaw * CalculationsEpsilonScaling;

		public const int MaxValueRaw = int.MaxValue;
		public const int MinValueRaw = int.MinValue;

		public const int OneRaw = 1 << FractionalBits;
		public const int MinusOneRaw = IntegerSignMask;
		public const int TwoRaw = OneRaw * 2;
		public const int ThreeRaw = OneRaw * 3;
		public const int HalfRaw = OneRaw / 2;
		public const int QuarterRaw = OneRaw / 4;

		// 29 is the largest N such that (int)(PI * 2^N) < int.MaxValue.
		public const int PiBase29 = 1686629713; // (int)(3.141592653589793 * (1 << 29));
		public const int PiRaw = PiBase29 >> (29 - FractionalBits);
		public const int HalfPiRaw = PiRaw / 2;
		public const int TwoPiRaw = PiRaw * 2;

		public const int Deg2RadBase30 = 18740330; // (int)(0.017453292519943295 * (1 << 30));
		public const int Rad2DegBase25 = 1922527337; // (int)(57.29577951308232 * (1 << 25));
		public const int Deg2RadRaw = Deg2RadBase30 >> (30 - FractionalBits);
		public const int Rad2DegRaw = Rad2DegBase25 >> (25 - FractionalBits);

		public const int Ln2Base30 = 744261117; // (int)(0.6931471805599453 * (1 << 30));
		public const int Ln2Raw = Ln2Base30 >> (30 - FractionalBits);
		public const int Log2MaxRaw = IntegerBits * OneRaw;
		public const int Log2MinRaw = -(IntegerBits + 1) * OneRaw;

		public const int Sqrt2Base30 = 1518500249; // (int)(1.414213562373095 * (1 << 30));
		public const int Sqrt2Raw = Sqrt2Base30 >> (30 - FractionalBits);

		public static FP Epsilon
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(EpsilonRaw);
		}

		/// <summary>
		/// Epsilon for linear operations.
		/// </summary>
		public static FP CalculationsEpsilon
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(CalculationsEpsilonRaw);
		}

		/// <summary>
		/// More precise epsilon for operations with squares involved.
		/// </summary>
		public static FP CalculationsEpsilonSqr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(CalculationsEpsilonSqrRaw);
		}

		public static FP MaxValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(MaxValueRaw);
		}

		public static FP MinValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(MinValueRaw);
		}

		public static FP One
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(OneRaw);
		}

		public static FP Two
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(TwoRaw);
		}

		public static FP Three
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(TwoRaw);
		}

		public static FP Zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(0);
		}

		public static FP MinusOne
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(MinusOneRaw);
		}

		public static FP Half
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(HalfRaw);
		}

		public static FP Quarter
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(QuarterRaw);
		}

		public static FP Pi
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(PiRaw);
		}

		public static FP HalfPi
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(HalfPiRaw);
		}

		public static FP TwoPi
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(TwoPiRaw);
		}

		public static FP Rad2Deg
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Rad2DegRaw);
		}

		public static FP Deg2Rad
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Deg2RadRaw);
		}

		/// <summary>
		/// Represents the maximum number for which a logarithmic operation in base 2 is valid or meaningful in this system.
		/// </summary>
		public static FP Log2Max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Log2MaxRaw);
		}

		/// <summary>
		/// Represents the minimum valid value (or negative logarithm) in base 2 for the system.
		/// </summary>
		public static FP Log2Min
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Log2MinRaw);
		}

		public static FP Ln2
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FromRaw(Ln2Raw);
		}
	}
}
