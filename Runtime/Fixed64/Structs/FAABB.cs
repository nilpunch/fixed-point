using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed64
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(FVector3 point)
		{
			return point.X >= LowerBound.X && point.X <= UpperBound.X &&
				point.Y >= LowerBound.Y && point.Y <= UpperBound.Y &&
				point.Z >= LowerBound.Z && point.Z <= UpperBound.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool RayIntersect(FVector3 origin, FVector3 direction)
		{
			return RayIntersect(origin, direction, out _);
		}

		// Adapted from jitterphysics2
		// https://github.com/notgiven688/jitterphysics2/blob/main/src/Jitter2/LinearMath/JBoundingBox.cs
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool RayIntersect(FVector3 origin, FVector3 direction, out FP enter)
		{
			enter = FP.Zero;
			var exit = FP.MaxValue;

			if (!Intersect1D(origin.X, direction.X, LowerBound.X, UpperBound.X, ref enter, ref exit))
			{
				return false;
			}

			if (!Intersect1D(origin.Y, direction.Y, LowerBound.Y, UpperBound.Y, ref enter, ref exit))
			{
				return false;
			}

			if (!Intersect1D(origin.Z, direction.Z, LowerBound.Z, UpperBound.Z, ref enter, ref exit))
			{
				return false;
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Intersect1D(FP start, FP dir, FP min, FP max, ref FP enter, ref FP exit)
		{
			if (dir * dir < FP.CalculationsEpsilonSqr)
			{
				return start >= min && start <= max;
			}

			var t0 = (min - start) / dir;
			var t1 = (max - start) / dir;

			if (t0 > t1)
			{
				(t0, t1) = (t1, t0);
			}

			if (t0 > exit || t1 < enter)
			{
				return false;
			}

			if (t0 > enter)
			{
				enter = t0;
			}
			if (t1 < exit)
			{
				exit = t1;
			}
			return true;
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
