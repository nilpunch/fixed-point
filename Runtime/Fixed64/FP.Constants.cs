using System.Runtime.CompilerServices;

namespace Fixed64
{
	public partial struct FP
	{
		public const int AllBits = sizeof(long) * 8;

		public const int AllBitsWithoutSign = AllBits - 1;
		public const int IntegerBits = AllBitsWithoutSign - FractionalBits;
		public const int IntegerBitsWithSign = IntegerBits + 1;
		public const long FractionalMask = long.MaxValue >> (AllBitsWithoutSign - FractionalBits);
		public const long IntegerSignMask = long.MaxValue << FractionalBits;
		public const long IntegerFractionalMask = long.MaxValue;
		public const long SignMask = long.MinValue;

		public const long EpsilonRaw = 1L;
		public const long CalculationsEpsilonSqrRaw = EpsilonRaw * CalculationsEpsilonScaling;
		public const long CalculationsEpsilonRaw = CalculationsEpsilonSqrRaw * CalculationsEpsilonScaling;

		public const long MaxValueRaw = long.MaxValue;
		public const long MinValueRaw = long.MinValue;

		public const long OneRaw = 1L << FractionalBits;
		public const long MinusOneRaw = IntegerSignMask;
		public const long TwoRaw = OneRaw * 2;
		public const long ThreeRaw = OneRaw * 3;
		public const long HalfRaw = OneRaw / 2;
		public const long QuarterRaw = OneRaw / 4;

		// 61 is the largest N such that (long)(PI * 2^N) < long.MaxValue.
		public const long PiBase61 = 7244019458077122560L; // (long)(3.141592653589793 * (1L << 61));
		public const long PiRaw = PiBase61 >> (61 - FractionalBits);
		public const long HalfPiRaw = PiRaw / 2;
		public const long TwoPiRaw = PiRaw * 2;

		public const long Deg2RadBase61 = 40244552544872904L; // (long)(0.017453292519943295 * (1L << 61));
		public const long Rad2DegBase57 = 8257192040480628736L; // (long)(57.29577951308232 * (1L << 57));
		public const long Deg2RadRaw = Deg2RadBase61 >> (61 - FractionalBits);
		public const long Rad2DegRaw = Rad2DegBase57 >> (57 - FractionalBits);

		public const long Ln2Base61 = 1598288580650331904L; // (long)(0.6931471805599453 * (1L << 61));
		public const long Ln2Raw = Ln2Base61 >> (61 - FractionalBits);
		public const long Log2MaxRaw = IntegerBits * OneRaw;
		public const long Log2MinRaw = -(IntegerBits + 1) * OneRaw;

		public const long Sqrt2Base61 = 3260954456333195264L; // (long)(1.414213562373095 * (1L << 61));
		public const long Sqrt2Raw = Sqrt2Base61 >> (61 - FractionalBits);

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
			get => FromRaw(0L);
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
