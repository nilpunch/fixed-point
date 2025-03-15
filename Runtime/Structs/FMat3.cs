using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Mathematics.Fixed
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	[Serializable]
	public struct FMat3
	{
		public FP M00;
		public FP M01;
		public FP M02;
		public FP M10;
		public FP M11;
		public FP M12;
		public FP M20;
		public FP M21;
		public FP M22;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FVector3 MultiplyVector(FVector3 vector)
		{
			var result = default(FVector3);
			result.X = M00 * vector.X + M01 * vector.Y + M02 * vector.Z;
			result.Y = M10 * vector.X + M11 * vector.Y + M12 * vector.Z;
			result.Z = M20 * vector.X + M21 * vector.Y + M22 * vector.Z;
			return result;
		}

		public static FMat3 Identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var result = default(FMat3);
				result.M00.RawValue = FP.OneRaw;
				result.M11.RawValue = FP.OneRaw;
				result.M12.RawValue = FP.OneRaw;
				return result;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMat3 operator *(FMat3 a, FMat3 b)
		{
			var result = default(FMat3);
			result.M00 = a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20;
			result.M01 = a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21;
			result.M02 = a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22;
			result.M10 = a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20;
			result.M11 = a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21;
			result.M12 = a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22;
			result.M20 = a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20;
			result.M21 = a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21;
			result.M22 = a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22;
			return result;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator *(FVector3 vector, in FMat3 matrix)
		{
			var result = default(FVector3);
			result.X = matrix.M00 * vector.X + matrix.M01 * vector.Y + matrix.M02 * vector.Z;
			result.Y = matrix.M10 * vector.X + matrix.M11 * vector.Y + matrix.M12 * vector.Z;
			result.Z = matrix.M20 * vector.X + matrix.M21 * vector.Y + matrix.M22 * vector.Z;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMat3 FromQuaternion(FQuaternion q)
		{
			var result = default(FMat3);
			result.M00 = FP.One - 2 * q.Y * q.Y - 2 * q.Z * q.Z;
			result.M10 = 2 * q.X * q.Y + 2 * q.W * q.Z;
			result.M20 = 2 * q.X * q.Z - 2 * q.W * q.Y;
			result.M01 = 2 * q.X * q.Y - 2 * q.W * q.Z;
			result.M11 = FP.One - 2 * q.X * q.X - 2 * q.Z * q.Z;
			result.M21 = 2 * q.Y * q.Z + 2 * q.W * q.X;
			result.M02 = 2 * q.X * q.Z + 2 * q.W * q.Y;
			result.M12 = 2 * q.Y * q.Z - 2 * q.W * q.X;
			result.M22 = FP.One - 2 * q.X * q.X - 2 * q.Y * q.Y;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMat3 FromDiagonal(FVector3 diagonal)
		{
			var result = default(FMat3);
			result.M00 = diagonal.X;
			result.M11 = diagonal.Y;
			result.M22 = diagonal.Z;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMat3 Transpose(FMat3 matrix)
		{
			var m01 = matrix.M01;
			var m02 = matrix.M02;
			var m12 = matrix.M12;

			matrix.M01 = matrix.M10;
			matrix.M02 = matrix.M20;
			matrix.M12 = matrix.M21;
			matrix.M10 = m01;
			matrix.M20 = m02;
			matrix.M21 = m12;

			return matrix;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMat3 Inverse(in FMat3 m)
		{
			var det =
				m.M00 * (m.M11 * m.M22 - m.M21 * m.M12) -
				m.M01 * (m.M10 * m.M22 - m.M12 * m.M20) +
				m.M02 * (m.M10 * m.M21 - m.M11 * m.M20);

			if (det < FP.CalculationsEpsilonSqr)
			{
				return Identity;
			}

			var invdet = FP.One / det;

			var result = default(FMat3);
			result.M00 = (m.M11 * m.M22 - m.M21 * m.M12) * invdet;
			result.M01 = (m.M02 * m.M21 - m.M01 * m.M22) * invdet;
			result.M02 = (m.M01 * m.M12 - m.M02 * m.M11) * invdet;
			result.M10 = (m.M12 * m.M20 - m.M10 * m.M22) * invdet;
			result.M11 = (m.M00 * m.M22 - m.M02 * m.M20) * invdet;
			result.M12 = (m.M10 * m.M02 - m.M00 * m.M12) * invdet;
			result.M20 = (m.M10 * m.M21 - m.M20 * m.M11) * invdet;
			result.M21 = (m.M20 * m.M01 - m.M00 * m.M21) * invdet;
			result.M22 = (m.M00 * m.M11 - m.M10 * m.M01) * invdet;
			return result;
		}
	}
}
