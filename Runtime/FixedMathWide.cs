using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
    public static class FixedMathWide
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(in FixedPointWide a, in FixedPointWide b, out FixedPointWide c)
        {
            c.RawValue = a.RawValue + b.RawValue;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sub(in FixedPointWide a, in FixedPointWide b, out FixedPointWide c)
        {
            c.RawValue = a.RawValue - b.RawValue;
        }
    }
}