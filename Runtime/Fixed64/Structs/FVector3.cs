using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed64
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FVector3 : IEquatable<FVector3>, IFormattable
	{
		public FP X;
		public FP Y;
		public FP Z;

		/// <summary>
		/// Constructs a vector from three FP values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FVector3(FP x, FP y, FP z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public FP this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)] get
			{
				switch (index)
				{
					case 0:
						return X;
					case 1:
						return Y;
					case 2:
						return Z;
					default:
						throw new IndexOutOfRangeException();
				}
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)] set
			{
				switch (index)
				{
					case 0:
						X = value;
						break;
					case 1:
						Y = value;
						break;
					case 2:
						Z = value;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// Shorthand for writing FVector3(0, 0, 0).
		/// </summary>
		public static FVector3 Zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector3(FP.Zero, FP.Zero, FP.Zero);
		}

		/// <summary>
		/// Shorthand for writing FVector3(1, 1, 1).
		/// </summary>
		public static FVector3 One
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector3(FP.One, FP.One, FP.One);
		}

		/// <summary>
		/// Shorthand for writing FVector3(1, 0, 0).
		/// </summary>
		public static FVector3 Right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector3(FP.One, FP.Zero, FP.Zero);
		}

		/// <summary>
		/// Shorthand for writing FVector3(-1, 0, 0).
		/// </summary>
		public static FVector3 Left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector3(FP.MinusOne, FP.Zero, FP.Zero);
		}

		/// <summary>
		/// Shorthand for writing FVector3(0, 1, 0).
		/// </summary>
		public static FVector3 Up
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector3(FP.Zero, FP.One, FP.Zero);
		}

		/// <summary>
		/// Shorthand for writing FVector3(0, -1, 0).
		/// </summary>
		public static FVector3 Down
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector3(FP.Zero, FP.MinusOne, FP.Zero);
		}

		/// <summary>
		/// Shorthand for writing FVector3(0, 0, 1).
		/// </summary>
		public static FVector3 Forward
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector3(FP.Zero, FP.Zero, FP.One);
		}

		/// <summary>
		/// Shorthand for writing FVector3(0, 0, -1).
		/// </summary>
		public static FVector3 Backward
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FVector3(FP.Zero, FP.Zero, FP.MinusOne);
		}

		/// <summary>
		/// Returns true if the given vector is exactly equal to this vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object other) => other is FVector3 otherVector && Equals(otherVector);

		/// <summary>
		/// Returns true if the given vector is exactly equal to this vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FVector3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override string ToString() =>
			ToString("F2", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		public string ToString(string format) =>
			ToString(format, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		public string ToString(IFormatProvider provider) => ToString("F2", provider);

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("({0}, {1}, {2})", X.ToString(format, formatProvider),
				Y.ToString(format, formatProvider), Z.ToString(format, formatProvider));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2;

		/// <summary>
		/// Returns the componentwise addition.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator +(FVector3 a, FVector3 b)
		{
			return new FVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		/// <summary>
		/// Returns the componentwise addition.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator +(FVector3 a, FP b)
		{
			return new FVector3(a.X + b, a.Y + b, a.Z + b);
		}

		/// <summary>
		/// Returns the componentwise addition.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator +(FP a, FVector3 b)
		{
			return b + a;
		}

		/// <summary>
		/// Returns the componentwise negotiation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator -(FVector3 a)
		{
			return new FVector3(-a.X, -a.Y, -a.Z);
		}

		/// <summary>
		/// Returns the componentwise subtraction.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator -(FVector3 a, FVector3 b)
		{
			return -b + a;
		}

		/// <summary>
		/// Returns the componentwise subtraction.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator -(FVector3 a, FP b)
		{
			return -b + a;
		}

		/// <summary>
		/// Returns the componentwise subtraction.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator -(FP a, FVector3 b)
		{
			return -b + a;
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator *(FVector3 a, FVector3 b)
		{
			return new FVector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator *(FVector3 a, FP b)
		{
			return new FVector3(a.X * b, a.Y * b, a.Z * b);
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator *(FP a, FVector3 b)
		{
			return b * a;
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator *(FVector3 a, int b)
		{
			return new FVector3(a.X * b, a.Y * b, a.Z * b);
		}

		/// <summary>
		/// Returns the componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator *(int b, FVector3 a)
		{
			return a * b;
		}

		/// <summary>
		/// Returns the componentwise division.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator /(FVector3 a, FVector3 b)
		{
			return new FVector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		}

		/// <summary>
		/// Returns the componentwise division.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator /(FVector3 a, FP b)
		{
			var invB = FP.One / b;
			return new FVector3(a.X * invB, a.Y * invB, a.Z * invB);
		}

		/// <summary>
		/// Returns the componentwise division.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator /(FVector3 a, int b)
		{
			return new FVector3(a.X / b, a.Y / b, a.Z / b);
		}

		/// <summary>
		/// Returns true if vectors are approximately equal, false otherwise.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FVector3 a, FVector3 b) => ApproximatelyEqual(a, b);

		/// <summary>
		/// Returns true if vectors are not approximately equal, false otherwise.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FVector3 a, FVector3 b) => !(a == b);

		/// <summary>
		/// Returns the dot product of two vectors.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Dot(FVector3 a, FVector3 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		/// <summary>
		/// Returns the cross product of two vectors.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 Cross(FVector3 a, FVector3 b)
		{
			return new FVector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 MoveTowards(FVector3 current, FVector3 target, FP maxDistanceDelta)
		{
			var direction = target - current;

			var sqrLength = LengthSqr(direction);

			if (sqrLength == FP.Zero || maxDistanceDelta >= FP.Zero && sqrLength <= maxDistanceDelta * maxDistanceDelta)
				return target;

			var distance = FP.Sqrt(sqrLength);

			return current + direction / distance * maxDistanceDelta;
		}

		/// <summary>
		/// Returns the length of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Length(FVector3 a)
		{
			return FP.Sqrt(LengthSqr(a));
		}

		/// <summary>
		/// Returns the squared length of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP LengthSqr(FVector3 a)
		{
			return Dot(a, a);
		}

		/// <summary>
		/// Returns the distance between a and b.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Distance(FVector3 a, FVector3 b)
		{
			return FP.Sqrt(DistanceSqr(a, b));
		}

		/// <summary>
		/// Returns the squared distance between a and b.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP DistanceSqr(FVector3 a, FVector3 b)
		{
			var deltaX = a.X - b.X;
			var deltaY = a.Y - b.Y;
			var deltaZ = a.Z - b.Z;
			return deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
		}

		/// <summary>
		/// Returns a normalized version of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 Normalize(FVector3 a)
		{
			var length = Length(a);
			return a / length;
		}

		/// <summary>
		/// Returns a safe normalized version of a vector.
		/// Returns the given default value when vector length close to zero.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 NormalizeSafe(FVector3 a, FVector3 defaultValue = new FVector3())
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
		public static FVector3 Orthogonal(FVector3 a)
		{
			return new FVector3(
				FP.CopySign(a.Z, a.X),
				FP.CopySign(a.Z, a.Y),
				-FP.CopySign(a.X, a.Z) - FP.CopySign(a.Y, a.Z));
		}

		/// <summary>
		/// Returns orthogonal basis vector to a given one.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 Orthonormal(FVector3 a)
		{
			var length = Length(a);
			var s = FP.CopySign(length, a.Z);
			var h = a.Z + s;
			return new FVector3(s * h - a.X * a.X, -a.X * a.Y, -a.X * h);
		}

		/// <summary>
		/// Returns a vector that is made from the largest components of two vectors.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 MaxComponents(FVector3 a, FVector3 b)
		{
			return new FVector3(FP.Max(a.X, b.X), FP.Max(a.Y, b.Y), FP.Max(a.Z, b.Z));
		}

		/// <summary>
		/// Returns a vector that is made from the smallest components of two vectors.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 MinComponents(FVector3 a, FVector3 b)
		{
			return new FVector3(FP.Min(a.X, b.X), FP.Min(a.Y, b.Y), FP.Min(a.Z, b.Z));
		}

		/// <summary>
		/// Returns the componentwise absolute value of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 AbsComponents(FVector3 a)
		{
			return new FVector3(FP.Abs(a.X), FP.Abs(a.Y), FP.Abs(a.Z));
		}

		/// <summary>
		/// Returns the componentwise signes of a vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 SignComponents(FVector3 a)
		{
			return new FVector3(FP.Sign(a.X), FP.Sign(a.Y), FP.Sign(a.Z));
		}

		/// <summary>
		/// Compares two vectors with <see cref="FP.CalculationsEpsilonSqr"/> and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FVector3 a, FVector3 b)
		{
			return ApproximatelyEqual(a, b, FP.CalculationsEpsilonSqr);
		}

		/// <summary>
		/// Compares two vectors with some epsilon and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FVector3 a, FVector3 b, FP epsilon)
		{
			return DistanceSqr(a, b) < epsilon;
		}
	}
}
