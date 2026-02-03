using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed64
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FVector2 : IEquatable<FVector2>, IFormattable
	{
		public FP X;
		public FP Y;

		/// <summary>
		/// Constructs a vector from two FP values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FVector2(FP x, FP y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Shorthand for writing FVector2(0, 0).
		/// </summary>
		public static FVector2 Zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector2(FP.Zero, FP.Zero);
		}

		/// <summary>
		/// Shorthand for writing FVector2(1, 1).
		/// </summary>
		public static FVector2 One
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector2(FP.One, FP.One);
		}

		/// <summary>
		/// Shorthand for writing FVector2(1, 0).
		/// </summary>
		public static FVector2 Right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector2(FP.One, FP.Zero);
		}

		/// <summary>
		/// Shorthand for writing FVector2(-1, 0).
		/// </summary>
		public static FVector2 Left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector2(FP.MinusOne, FP.Zero);
		}

		/// <summary>
		/// Shorthand for writing FVector2(0, 1).
		/// </summary>
		public static FVector2 Up
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector2(FP.Zero, FP.One);
		}

		/// <summary>
		/// Shorthand for writing FVector2(0, -1).
		/// </summary>
		public static FVector2 Down
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector2(FP.Zero, FP.MinusOne);
		}

		/// <summary>
		/// Returns true if the given vector is exactly equal to this vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object other) => other is FVector2 otherVector && Equals(otherVector);

		/// <summary>
		/// Returns true if the given vector is exactly equal to this vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FVector2 other)
		{
			return X == other.X && Y == other.Y;
		}

		public override string ToString() =>
			ToString("F2", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		public string ToString(string format) =>
			ToString(format, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		public string ToString(IFormatProvider provider) => ToString("F2", provider);

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("({0}, {1})", X.ToString(format, formatProvider),
				Y.ToString(format, formatProvider));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() << 2;

		/// <summary>
		/// Returns the componentwise addition.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator +(FVector2 a, FVector2 b)
		{
			return new FVector2(a.X + b.X, a.Y + b.Y);
		}

		/// <summary>
		/// Returns the componentwise addition.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator +(FVector2 a, FP b)
		{
			return new FVector2(a.X + b, a.Y + b);
		}

		/// <summary>
		/// Returns the componentwise addition.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator +(FP a, FVector2 b)
		{
			return b + a;
		}

		/// <summary>
		/// Returns the componentwise negotiation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator -(FVector2 a)
		{
			return new FVector2(-a.X, -a.Y);
		}

		/// <summary>
		/// Returns the componentwise subtraction.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator -(FVector2 a, FVector2 b)
		{
			return -b + a;
		}

		/// <summary>
		/// Returns the componentwise subtraction.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator -(FVector2 a, FP b)
		{
			return -b + a;
		}

		/// <summary>
		/// Returns the componentwise subtraction.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator -(FP a, FVector2 b)
		{
			return -b + a;
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator *(FVector2 a, FVector2 b)
		{
			return new FVector2(a.X * b.X, a.Y * b.Y);
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator *(FVector2 a, FP b)
		{
			return new FVector2(a.X * b, a.Y * b);
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator *(FP a, FVector2 b)
		{
			return b * a;
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator *(FVector2 a, int b)
		{
			return new FVector2(a.X * b, a.Y * b);
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator *(int b, FVector2 a)
		{
			return a * b;
		}

		/// <summary>
		/// Returns the componentwise division.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator /(FVector2 a, FVector2 b)
		{
			return new FVector2(a.X / b.X, a.Y / b.Y);
		}

		/// <summary>
		/// Returns the componentwise division.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator /(FVector2 a, FP b)
		{
			var invB = FP.One / b;
			return new FVector2(a.X * invB, a.Y * invB);
		}

		/// <summary>
		/// Returns the componentwise division.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 operator /(FVector2 a, int b)
		{
			return new FVector2(a.X / b, a.Y / b);
		}

		/// <summary>
		/// Returns true if vectors are approximately equal, false otherwise.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FVector2 a, FVector2 b) => ApproximatelyEqual(a, b);

		/// <summary>
		/// Returns true if vectors are not approximately equal, false otherwise.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FVector2 a, FVector2 b) => !(a == b);

		/// <summary>
		/// Returns the dot product of two vectors.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Dot(FVector2 a, FVector2 b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		/// <summary>
		/// Returns the length of 3D vector from cross product of two vectors.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Cross(FVector2 a, FVector2 b)
		{
			return a.X * b.Y - a.Y * b.X;
		}

		/// <summary>
		/// Returns the length of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Length(FVector2 a)
		{
			return FP.Sqrt(LengthSqr(a));
		}

		/// <summary>
		/// Returns the squared length of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP LengthSqr(FVector2 a)
		{
			return Dot(a, a);
		}

		/// <summary>
		/// Returns the distance between a and b.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Distance(FVector2 a, FVector2 b)
		{
			return FP.Sqrt(DistanceSqr(a, b));
		}

		/// <summary>
		/// Returns the squared distance between a and b.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP DistanceSqr(FVector2 a, FVector2 b)
		{
			var deltaX = a.X - b.X;
			var deltaY = a.Y - b.Y;
			return deltaX * deltaX + deltaY * deltaY;
		}

		/// <summary>
		/// Returns a normalized version of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 Normalize(FVector2 a)
		{
			var length = Length(a);
			return a / length;
		}

		/// <summary>
		/// Returns a safe normalized version of a vector.
		/// Returns the given default value when vector length close to zero.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 NormalizeSafe(FVector2 a, FVector2 defaultValue = new FVector2())
		{
			var length = Length(a);
			if (length == FP.Zero)
			{
				return defaultValue;
			}
			return a / length;
		}

		/// <summary>
		/// Returns non-normalized perpendicular vector to a given one. For normalized see <see cref="Orthonormal"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 Orthogonal(FVector2 a)
		{
			return new FVector2(-a.Y, a.X);
		}

		/// <summary>
		/// Returns orthogonal basis vector to a given one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 Orthonormal(FVector2 a)
		{
			var orthogonal = Orthogonal(a);
			var length = Length(orthogonal);
			if (length == FP.Zero)
			{
				return Zero;
			}
			return orthogonal / length;
		}

		/// <summary>
		/// Returns a vector that is made from the largest components of two vectors.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 MaxComponents(FVector2 a, FVector2 b)
		{
			return new FVector2(FP.Max(a.X, b.X), FP.Max(a.Y, b.Y));
		}

		/// <summary>
		/// Returns a vector that is made from the smallest components of two vectors.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 MinComponents(FVector2 a, FVector2 b)
		{
			return new FVector2(FP.Min(a.X, b.X), FP.Min(a.Y, b.Y));
		}

		/// <summary>
		/// Returns the componentwise absolute value of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 AbsComponents(FVector2 a)
		{
			return new FVector2(FP.Abs(a.X), FP.Abs(a.Y));
		}

		/// <summary>
		/// Returns the componentwise signes of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 SignComponents(FVector2 a)
		{
			return new FVector2(FP.Sign(a.X), FP.Sign(a.Y));
		}

		/// <summary>
		/// Compares two vectors with <see cref="FP.CalculationsEpsilonSqr"/> and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FVector2 a, FVector2 b)
		{
			return ApproximatelyEqual(a, b, FP.CalculationsEpsilonSqr);
		}

		/// <summary>
		/// Compares two vectors with some epsilon and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FVector2 a, FVector2 b, FP epsilon)
		{
			return DistanceSqr(a, b) < epsilon;
		}
	}
}
