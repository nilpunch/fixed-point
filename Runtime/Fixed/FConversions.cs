using System.Runtime.CompilerServices;
using FP32 = Fixed32.FP;
using FP64 = Fixed64.FP;

namespace Fixed
{
	public static class FConversions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP64 To64(this FP32 value)
		{
			if (FP64.FractionalBits - FP32.FractionalBits >= 0)
			{
				return FP64.FromRaw((long)value.RawValue << (FP64.FractionalBits - FP32.FractionalBits));
			}
			else
			{
				return FP64.FromRaw((long)value.RawValue >> (FP32.FractionalBits - FP64.FractionalBits));
			}
		}
	}
}
