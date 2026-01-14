using System.Runtime.CompilerServices;

namespace Fixed32
{
	public static class FConversions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP ToFP(this int value)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				return FP.FromRaw(value << FP.FractionalBits);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP ToFP(this long value)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				return FP.FromRaw((int)(value << FP.FractionalBits));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP ToFP(this float value)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				return FP.FromRaw((int)(value * FP.OneRaw));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP ToFP(this double value)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				return FP.FromRaw((int)(value * FP.OneRaw));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt(this FP value)
		{
			return value.RawValue >> FP.FractionalBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToFloat(this FP value)
		{
			return (float)value.RawValue / FP.OneRaw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this FP value)
		{
			return (double)value.RawValue / FP.OneRaw;
		}
	}
}
