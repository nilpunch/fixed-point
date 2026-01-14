using System;
using System.Runtime.CompilerServices;

namespace Fixed64
{
	public readonly struct MinkowskiDifference : IEquatable<MinkowskiDifference>
	{
		public readonly FVector3 SupportA;
		public readonly FVector3 SupportB;
		public readonly FVector3 Difference;

		private MinkowskiDifference(FVector3 supportA, FVector3 supportB, FVector3 difference)
		{
			SupportA = supportA;
			SupportB = supportB;
			Difference = difference;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MinkowskiDifference Calculate<TA, TB>(TA shapeA, TB shapeB, FVector3 direction)
			where TA : ISupportMappable
			where TB : ISupportMappable
		{
			var supportA = shapeA.SupportPoint(direction);
			var supportB = shapeB.SupportPoint(-direction);
			var difference = supportA - supportB;

			return new MinkowskiDifference(supportA, supportB, difference);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(MinkowskiDifference other)
		{
			return Difference.Equals(other.Difference);
		}

		public override bool Equals(object obj)
		{
			return obj is MinkowskiDifference other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Difference.GetHashCode();
		}
	}
}
