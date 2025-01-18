using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public struct Rotation2D
	{
		public FP Sin;
		public FP OneMinusCos;

		public Rotation2D(FAngle angle)
		{
			Sin = FMath.Sin(angle.Radians);
			OneMinusCos = FP.One - FMath.Cos(angle.Radians);
		}

		private Rotation2D(FP sin, FP oneMinusCos)
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
			get => FAngle.FromRadians(FMath.Atan2(Sin, Cos));
		}

		public FAngle ClockwiseAngle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => -CounterclockwiseAngle;
		}

		public static Rotation2D Identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Rotation2D();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator *(Rotation2D rotation2D, FVector2 vector)
		{
			FP sin = rotation2D.Sin;
			FP cos = rotation2D.Cos;
			return new FVector2(
				vector.X * cos - vector.Y * sin,
				vector.X * sin + vector.Y * cos
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Rotation2D operator *(Rotation2D a, Rotation2D b)
		{
			FP sinA = a.Sin;
			FP cosA = a.Cos;
			FP sinB = b.Sin;
			FP cosB = b.Cos;

			FP cos = cosA * cosB - sinA * sinB;
			FP sin = sinA * cosB + cosA * sinB;

			return new Rotation2D(sin, FP.One - cos);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Rotation2D Inverse(Rotation2D rotation2D)
		{
			return new Rotation2D(-rotation2D.Sin, rotation2D.OneMinusCos);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Rotation2D FromToRotation(FVector2 fromDirection, FVector2 toDirection)
		{
			FP angleRadians = FMath.Atan2(toDirection.Y, toDirection.X) - FMath.Atan2(fromDirection.Y, fromDirection.X);
			return new Rotation2D(FAngle.FromRadians(angleRadians));
		}
	}
}
