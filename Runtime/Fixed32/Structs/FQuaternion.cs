using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed32
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FQuaternion : IEquatable<FQuaternion>, IFormattable
	{
		public FP X;
		public FP Y;
		public FP Z;
		public FP W;

		/// <summary>
		/// Constructs a unit quaternion from four FP values. Use if you know what you are doing.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FQuaternion(FP x, FP y, FP z, FP w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		/// <summary>
		/// The identity rotation.
		/// </summary>
		public static FQuaternion Identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FQuaternion(FP.Zero, FP.Zero, FP.Zero, FP.One);
		}

		/// <summary>
		/// Returns true if the given quaternion is exactly equal to this quaternion.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object other) => other is FQuaternion softQuaternion && Equals(softQuaternion);

		/// <summary>
		/// Returns true if the given quaternion is exactly equal to this quaternion.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FQuaternion other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		public override string ToString() =>
			ToString("F2", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		public string ToString(string format) =>
			ToString(format, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		public string ToString(IFormatProvider provider) => ToString("F2", provider);

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("({0}, {1}, {2}, {3})", X.ToString(format, formatProvider),
				Y.ToString(format, formatProvider), Z.ToString(format, formatProvider),
				Y.ToString(format, formatProvider));
		}

		public override int GetHashCode() =>
			X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2 ^ W.GetHashCode() >> 1;

		/// <summary>
		/// The componentwise addition.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion operator +(FQuaternion a, FQuaternion b)
		{
			return new FQuaternion(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}

		/// <summary>
		/// The componentwise negotiation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion operator -(FQuaternion a)
		{
			return new FQuaternion(-a.X, -a.Y, -a.Z, -a.W);
		}

		/// <summary>
		/// The componentwise subtraction.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion operator -(FQuaternion a, FQuaternion b)
		{
			return -b + a;
		}

		/// <summary>
		/// The quaternions multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion operator *(FQuaternion a, FQuaternion b)
		{
			return new FQuaternion(
				a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
				a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z,
				a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X,
				a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
		}

		/// <summary>
		/// The componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion operator *(FQuaternion a, FP b)
		{
			return new FQuaternion(a.X * b, a.Y * b, a.Z * b, a.W * b);
		}

		/// <summary>
		/// The componentwise multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion operator *(FP b, FQuaternion a)
		{
			return new FQuaternion(a.X * b, a.Y * b, a.Z * b, a.W * b);
		}

		/// <summary>
		/// Rotate vector by the quaternion. Valid only for normalized quaternions.
		/// </summary>
		public static FVector3 operator *(FQuaternion unitQuaternion, FVector3 vector)
		{
			var twoX = unitQuaternion.X * 2;
			var twoY = unitQuaternion.Y * 2;
			var twoZ = unitQuaternion.Z * 2;
			var xx2 = unitQuaternion.X * twoX;
			var yy2 = unitQuaternion.Y * twoY;
			var zz2 = unitQuaternion.Z * twoZ;
			var xy2 = unitQuaternion.X * twoY;
			var xz2 = unitQuaternion.X * twoZ;
			var yz2 = unitQuaternion.Y * twoZ;
			var wx2 = unitQuaternion.W * twoX;
			var wy2 = unitQuaternion.W * twoY;
			var wz2 = unitQuaternion.W * twoZ;
			var result = new FVector3(
				(FP.One - (yy2 + zz2)) * vector.X + (xy2 - wz2) * vector.Y + (xz2 + wy2) * vector.Z,
				(xy2 + wz2) * vector.X + (FP.One - (xx2 + zz2)) * vector.Y + (yz2 - wx2) * vector.Z,
				(xz2 - wy2) * vector.X + (yz2 + wx2) * vector.Y + (FP.One - (xx2 + yy2)) * vector.Z);
			return result;
		}

		/// <summary>
		/// Returns the vector transformed by the quaternion, including scale and rotation.
		/// Also known as sandwich product: q * vec * conj(q)
		/// </summary>
		public static FVector3 Sandwich(FQuaternion quaternion, FVector3 vector)
		{
			var twoX = quaternion.X * 2;
			var twoY = quaternion.Y * 2;
			var twoZ = quaternion.Z * 2;
			var xx = quaternion.X * quaternion.X;
			var yy = quaternion.Y * quaternion.Y;
			var zz = quaternion.Z * quaternion.Z;
			var ww = quaternion.W * quaternion.W;
			var xy2 = quaternion.X * twoY;
			var xz2 = quaternion.X * twoZ;
			var yz2 = quaternion.Y * twoZ;
			var wx2 = quaternion.W * twoX;
			var wy2 = quaternion.W * twoY;
			var wz2 = quaternion.W * twoZ;
			var result = new FVector3(
				(ww + xx - yy - zz) * vector.X + (xy2 - wz2) * vector.Y + (xz2 + wy2) * vector.Z,
				(xy2 + wz2) * vector.X + (ww - xx + yy - zz) * vector.Y + (yz2 - wx2) * vector.Z,
				(xz2 - wy2) * vector.X + (yz2 + wx2) * vector.Y + (ww - xx - yy + zz) * vector.Z);
			return result;
		}

		/// <summary>
		/// The quaternions division.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion operator /(FQuaternion a, FQuaternion b)
		{
			return a * Inverse(b);
		}

		/// <summary>
		/// The componentwise division.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion operator /(FQuaternion a, FP b)
		{
			var invB = FP.One / b;
			return new FQuaternion(a.X * invB, a.Y * invB, a.Z * invB, a.W * invB);
		}

		/// <summary>
		/// Returns true if quaternions are approximately equal, false otherwise.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FQuaternion a, FQuaternion b) => ApproximatelyEqual(a, b);

		/// <summary>
		/// Returns true if quaternions are not approximately equal, false otherwise.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FQuaternion a, FQuaternion b) => !(a == b);

		/// <summary>
		/// The dot product between two rotations.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Dot(FQuaternion a, FQuaternion b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
		}

		/// <summary>
		/// The length of a quaternion.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Length(FQuaternion a)
		{
			return FP.Sqrt(LengthSqr(a));
		}

		/// <summary>
		/// The squared length of a quaternion.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP LengthSqr(FQuaternion a)
		{
			return Dot(a, a);
		}

		/// <summary>
		/// The inverse of a quaternion.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion Inverse(FQuaternion a)
		{
			return new FQuaternion(-a.X, -a.Y, -a.Z, a.W);
		}

		/// <summary>
		/// Returns a spherical interpolation between two quaternions.
		/// Non-commutative, torque-minimal, constant velocity.
		/// </summary>
		public static FQuaternion Slerp(FQuaternion a, FQuaternion b, FP t, bool intPath = false)
		{
			// Calculate angle between them.
			var cosHalfTheta = Dot(a, b);

			if (intPath)
			{
				if (cosHalfTheta > FP.Zero)
				{
					b = -b;
					cosHalfTheta = -cosHalfTheta;
				}
			}
			else
			{
				if (cosHalfTheta < FP.Zero)
				{
					b = -b;
					cosHalfTheta = -cosHalfTheta;
				}
			}

			// If a = b or a = b then theta = 0 then we can return interpolation between a and b.
			if (FP.Abs(cosHalfTheta) > FP.One - FP.CalculationsEpsilon)
			{
				return Nlerp(a, b, t, intPath);
			}

			var halfTheta = FP.Acos(cosHalfTheta);
			var sinHalfTheta = FP.Sin(halfTheta);

			var influenceA = FP.Sin((FP.One - t) * halfTheta) / sinHalfTheta;
			var influenceB = FP.Sin(t * halfTheta) / sinHalfTheta;

			return EnsureNormalization(a * influenceA + b * influenceB);
		}

		/// <summary>
		/// Returns a normalized componentwise interpolation between two quaternions.
		/// Commutative, torque-minimal, non-constant velocity.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion Nlerp(FQuaternion a, FQuaternion b, FP t, bool intPath = false)
		{
			return NormalizeSafe(Lerp(a, b, t, intPath));
		}

		/// <summary>
		/// Returns a componentwise interpolation between two quaternions.
		/// </summary>
		public static FQuaternion Lerp(FQuaternion a, FQuaternion b, FP t, bool intPath = false)
		{
			var dot = Dot(a, b);

			if (intPath)
			{
				if (dot > FP.Zero)
				{
					b = -b;
				}
			}
			else
			{
				if (dot < FP.Zero)
				{
					b = -b;
				}
			}

			return a * (FP.One - t) + b * t;
		}

		/// <summary>
		/// Returns a rotation with the specified forward and up directions.
		/// If inputs are zero length or collinear or have some other weirdness,
		/// then rotation result will be some mix of <see cref="FVector3.Forward"/> and <see cref="FVector3.Up"/> vectors.
		/// </summary>
		public static FQuaternion LookRotation(FVector3 forward, FVector3 up)
		{
			// Third matrix column
			var lookAt = FVector3.NormalizeSafe(forward, FVector3.Forward);
			// First matrix column
			var sideAxis = FVector3.NormalizeSafe(FVector3.Cross(up, lookAt), FVector3.Orthonormal(lookAt));
			// Second matrix column
			var rotatedUp = FVector3.Cross(lookAt, sideAxis);

			// Sums of matrix main diagonal elements
			var trace1 = FP.One + sideAxis.X - rotatedUp.Y - lookAt.Z;
			var trace2 = FP.One - sideAxis.X + rotatedUp.Y - lookAt.Z;
			var trace3 = FP.One - sideAxis.X - rotatedUp.Y + lookAt.Z;

			// If orthonormal vectors forms identity matrix, then return identity rotation
			if (trace1 + trace2 + trace3 < FP.CalculationsEpsilon)
			{
				return Identity;
			}

			// Choose largest diagonal
			if (trace1 + FP.CalculationsEpsilon > trace2 && trace1 + FP.CalculationsEpsilon > trace3)
			{
				var s = FP.Sqrt(trace1) * 2;
				var invS = FP.One / s;
				return new FQuaternion(
					FP.Quarter * s,
					(rotatedUp.X + sideAxis.Y) * invS,
					(lookAt.X + sideAxis.Z) * invS,
					(rotatedUp.Z - lookAt.Y) * invS);
			}
			else if (trace2 + FP.CalculationsEpsilon > trace1 && trace2 + FP.CalculationsEpsilon > trace3)
			{
				var s = FP.Sqrt(trace2) * 2;
				var invS = FP.One / s;
				return new FQuaternion(
					(rotatedUp.X + sideAxis.Y) * invS,
					FP.Quarter * s,
					(lookAt.Y + rotatedUp.Z) * invS,
					(lookAt.X - sideAxis.Z) * invS);
			}
			else
			{
				var s = FP.Sqrt(trace3) * 2;
				var invS = FP.One / s;
				return new FQuaternion(
					(lookAt.X + sideAxis.Z) * invS,
					(lookAt.Y + rotatedUp.Z) * invS,
					FP.Quarter * s,
					(sideAxis.Y - rotatedUp.X) * invS);
			}
		}

		/// <summary>
		/// Returns a quaternion representing a rotation around a unit axis by an angle in radians.
		/// The rotation direction is clockwise when looking aint the rotation axis towards the origin.
		/// If input vector is zero length then rotation will be around forward axis.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion AxisAngleRadians(FVector3 axis, FP angle)
		{
			axis = FVector3.NormalizeSafe(axis, FVector3.Forward);
			var sin = FP.Sin(FP.Half * angle);
			var cos = FP.Cos(FP.Half * angle);
			return new FQuaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
		}

		/// <summary>
		/// Returns a quaternion representing a euler angle in radians.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion EulerRadians(FVector3 angle)
		{
			var cr = FP.Cos(angle.X * FP.Half);
			var sr = FP.Sin(angle.X * FP.Half);
			var cp = FP.Cos(angle.Y * FP.Half);
			var sp = FP.Sin(angle.Y * FP.Half);
			var cy = FP.Cos(angle.Z * FP.Half);
			var sy = FP.Sin(angle.Z * FP.Half);

			return new FQuaternion(
				sr * cp * cy - cr * sp * sy,
				cr * sp * cy + sr * cp * sy,
				cr * cp * sy - sr * sp * cy,
				cr * cp * cy + sr * sp * sy);
		}

		/// <summary>
		/// Returns a quaternion representing a rotation around a unit axis by an angle in degrees.
		/// The rotation direction is clockwise when looking aint the rotation axis towards the origin.
		/// /// If input vector is zero length then rotation will be around forward axis.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion AxisAngleDegrees(FVector3 axis, FP angle)
		{
			axis = FVector3.NormalizeSafe(axis, FVector3.Forward);
			var sin = FP.Sin(FP.Half * angle * FP.Deg2Rad);
			var cos = FP.Cos(FP.Half * angle * FP.Deg2Rad);
			return new FQuaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
		}

		/// <summary>
		/// Compares two quaternions with <see cref="FP.CalculationsEpsilonSqr"/> and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FQuaternion a, FQuaternion b)
		{
			return ApproximatelyEqual(a, b, FP.CalculationsEpsilonSqr);
		}

		/// <summary>
		/// Compares two quaternions with some epsilon and returns true if they are similar.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyEqual(FQuaternion a, FQuaternion b, FP epsilon)
		{
			return FP.Abs(Dot(a, b)) > FP.One - epsilon;
		}

		/// <summary>
		/// Returns a normalized version of a quaternion.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion Normalize(FQuaternion a)
		{
			var length = Length(a);
			return a / length;
		}

		/// <summary>
		/// Returns a safe normalized version of a quaternion.
		/// Returns the <see cref="Identity"/> when quaternion length close to zero.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion NormalizeSafe(FQuaternion a)
		{
			return NormalizeSafe(a, Identity);
		}

		/// <summary>
		/// Returns a safe normalized version of a quaternion.
		/// Returns the given default value when quaternion length close to zero.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion NormalizeSafe(FQuaternion a, FQuaternion defaultValue)
		{
			var sqrLength = LengthSqr(a);
			if (sqrLength < FP.CalculationsEpsilonSqr)
			{
				return defaultValue;
			}
			return a / FP.Sqrt(sqrLength);
		}

		/// <summary>
		/// Check quaternion for normalization precision error and re-normalize it if needed.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion EnsureNormalization(FQuaternion a)
		{
			var lengthSqr = LengthSqr(a);
			if (FP.Abs(FP.One - lengthSqr) > FP.CalculationsEpsilonSqr)
			{
				return a / FP.Sqrt(lengthSqr);
			}
			return a;
		}
	}
}
