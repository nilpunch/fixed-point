using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed32
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FRotation2D
	{
		public FP Sin;
		public FP OneMinusCos;

		public FRotation2D(FAngle angle)
		{
			Sin = FP.Sin(angle.Radians);
			OneMinusCos = FP.One - FP.Cos(angle.Radians);
		}

		private FRotation2D(FP sin, FP oneMinusCos)
		{
			Sin = sin;
			OneMinusCos = oneMinusCos;
		}

		public FP Cos
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FP.One - OneMinusCos;
		}

		public FAngle CounterclockwiseAngle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FAngle.FromRadians(FP.Atan2(Sin, Cos));
		}

		public FAngle ClockwiseAngle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => -CounterclockwiseAngle;
		}

		public static FRotation2D Identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FRotation2D(FP.Zero, FP.Zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator *(FRotation2D rotation2D, FVector2 vector)
		{
			var sin = rotation2D.Sin;
			var cos = rotation2D.Cos;
			return new FVector2(
				vector.X * cos - vector.Y * sin,
				vector.X * sin + vector.Y * cos
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FRotation2D operator *(FRotation2D a, FRotation2D b)
		{
			var sinA = a.Sin;
			var cosA = a.Cos;
			var sinB = b.Sin;
			var cosB = b.Cos;

			var cos = cosA * cosB - sinA * sinB;
			var sin = sinA * cosB + cosA * sinB;

			return new FRotation2D(sin, FP.One - cos);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FRotation2D Inverse(FRotation2D rotation2D)
		{
			return new FRotation2D(-rotation2D.Sin, rotation2D.OneMinusCos);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FRotation2D FromToRotation(FVector2 fromDirection, FVector2 toDirection)
		{
			var angleRadians = FP.Atan2(toDirection.Y, toDirection.X) - FP.Atan2(fromDirection.Y, fromDirection.X);
			return new FRotation2D(FAngle.FromRadians(angleRadians));
		}
	}
}
