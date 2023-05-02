using System.Numerics;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
    public struct FixedPointWide
    {
        public Vector<long> RawValue;

        /// <summary>
        /// This is the constructor from raw value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointWide(ref Vector<long> vector)
        {
            RawValue = vector;
        }

        /// <summary>
        /// This is the constructor from raw value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointWide(Vector<long> vector)
        {
            RawValue = vector;
        }
    }
}