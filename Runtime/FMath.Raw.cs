using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FMath
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long SafeNeg(long x)
		{
			return x == FP.MinValueRaw ? FP.MaxValueRaw : -x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long CopySign(long to, long from)
		{
			var signTo = to >> FP.AllBitsWithoutSign;
			var absTo = (to + signTo) ^ signTo;
			var sign = from >> FP.AllBitsWithoutSign;
			return (absTo ^ sign) - sign;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Floor(long value)
		{
			return value & FP.IntegerSignMask;
		}
	}
}
