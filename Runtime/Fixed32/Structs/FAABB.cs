using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed32
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FAABB : IEquatable<FAABB>
	{
		public FVector3 LowerBound;
		public FVector3 UpperBound;

		public FAABB(FVector3 lowerBound, FVector3 upperBound)
		{
			LowerBound = lowerBound;
			UpperBound = upperBound;
		}

		public FVector3 Center
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (LowerBound + UpperBound) * FP.Half;
		}

		public FVector3 Extents
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (UpperBound - LowerBound) * FP.Half;
		}

		public FP SurfaceArea
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var size = UpperBound - LowerBound;
				return 2 * (size.X * size.Y + size.Y * size.Z + size.Z * size.X);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB FromCenterAndExtents(FVector3 center, FVector3 extents)
		{
			return new FAABB(
				center - extents,
				center + extents
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(FVector3 point)
		{
			return point.X >= LowerBound.X && point.X <= UpperBound.X &&
				point.Y >= LowerBound.Y && point.Y <= UpperBound.Y &&
				point.Z >= LowerBound.Z && point.Z <= UpperBound.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(FAABB a, FAABB b)
		{
			return a.LowerBound.X <= b.UpperBound.X && a.UpperBound.X >= b.LowerBound.X &&
				a.LowerBound.Y <= b.UpperBound.Y && a.UpperBound.Y >= b.LowerBound.Y &&
				a.LowerBound.Z <= b.UpperBound.Z && a.UpperBound.Z >= b.LowerBound.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB Union(FAABB a, FAABB b)
		{
			return new FAABB(
				FVector3.MinComponents(a.LowerBound, b.LowerBound),
				FVector3.MaxComponents(a.UpperBound, b.UpperBound)
			);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FAABB other)
		{
			return LowerBound.Equals(other.LowerBound) && UpperBound.Equals(other.UpperBound);
		}

		public override bool Equals(object obj)
		{
			return obj is FAABB other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(LowerBound, UpperBound);
		}
	}
}
