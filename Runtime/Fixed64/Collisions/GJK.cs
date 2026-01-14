using System;
using System.Runtime.CompilerServices;

namespace Fixed64
{
	public struct Simplex
	{
		public MinkowskiDifference A;
		public MinkowskiDifference B;
		public MinkowskiDifference C;
		public MinkowskiDifference D;

		public int Stage;
	}

	public static class GJK
	{
		private static FP Tolerance => FP.CalculationsEpsilon;

		public struct Result
		{
			public bool CollisionHappened { get; }

			public Simplex Simplex { get; }

			public int Iterations { get; }

			public FVector3 Direction { get; }

			public Result(bool collisionHappened, Simplex simplex, int iterations, FVector3 direction)
			{
				CollisionHappened = collisionHappened;
				Simplex = simplex;
				Iterations = iterations;
				Direction = direction;
			}
		}

		public static Result Calculate<TA, TB>(TA shapeA, TB shapeB, int maxIterations = 100)
			where TA : ISupportMappable
			where TB : ISupportMappable
		{
			var simplex = new Simplex();

			var direction = NormalizeSafe(shapeB.Center - shapeA.Center, FVector3.Up);

			var colliding = false;
			var iterations = 1;
			while (iterations < maxIterations)
			{
				var supportPoint = MinkowskiDifference.Calculate(shapeA, shapeB, direction);

				simplex.D = simplex.C;
				simplex.C = simplex.B;
				simplex.B = simplex.A;
				simplex.A = supportPoint;

				if (FVector3.Dot(supportPoint.Difference, direction) <= FP.Zero)
				{
					break;
				}

				var encloseResult = TryEncloseOrigin(ref simplex, shapeA, shapeB, direction);

				if (encloseResult.EncloseOrigin)
				{
					colliding = true;
					break;
				}

				direction = encloseResult.NextDirection;
				simplex.Stage += 1;
				iterations += 1;
			}

			if (iterations >= maxIterations)
			{
				throw new Exception();
			}

			return new Result(colliding, simplex, iterations, direction);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static FVector3 NormalizeSafe(FVector3 vector, FVector3 defaultValue)
		{
			var result = FVector3.Normalize(vector);

			if (result.Equals(FVector3.Zero))
			{
				return defaultValue;
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static FVector3 TripleProduct(FVector3 a, FVector3 b, FVector3 c)
		{
			return FVector3.Cross(FVector3.Cross(a, b), c);
		}

		private static (bool EncloseOrigin, FVector3 NextDirection) TryEncloseOrigin<TA, TB>(ref Simplex simplex,
			TA shapeA, TB shapeB, FVector3 direction)
			where TA : ISupportMappable
			where TB : ISupportMappable
		{
			switch (simplex.Stage)
			{
				case 0:
				{
					direction = NormalizeSafe(shapeB.Center - shapeA.Center, FVector3.Up);
					break;
				}
				case 1:
				{
					// Flip the direction
					direction = -direction;
					break;
				}
				case 2:
				{
					// Line ab is the line formed by the first two vertices
					var ab = simplex.B.Difference - simplex.A.Difference;
					// Line a0 is the line from the first vertex to the origin
					var a0 = -simplex.A.Difference;

					if (FVector3.Cross(ab, a0) == FVector3.Zero)
					{
						direction = FVector3.Orthonormal(ab);
					}
					else
					{
						// Use the triple-cross-product to calculate a direction perpendicular
						// To line ab in the direction of the origin
						direction = TripleProduct(ab, a0, ab);
					}
					break;
				}
				case 3:
				{
					var ab = simplex.B.Difference - simplex.A.Difference;
					var ac = simplex.C.Difference - simplex.A.Difference;
					direction = FVector3.Cross(ab, ac);

					// Ensure it points toward the origin
					var a0 = -simplex.A.Difference;
					if (FVector3.Dot(direction, a0) < FP.Zero)
					{
						direction = -direction;
					}
					break;
				}
				case 4:
				{
					// Calculate edges of interest
					var ab = simplex.B.Difference - simplex.A.Difference;
					var ac = simplex.C.Difference - simplex.A.Difference;
					var ad = simplex.D.Difference - simplex.A.Difference;

					var bc = simplex.C.Difference - simplex.B.Difference;
					var bd = simplex.D.Difference - simplex.B.Difference;
					var ba = -ab;

					// ABC
					direction = FVector3.Normalize(FVector3.Cross(ab, ac));
					if (FVector3.Dot(ad, direction) > FP.Zero)
					{
						direction = -direction;
					}
					if (FVector3.Dot(simplex.A.Difference, direction) < -Tolerance)
					{
						// Remove d
						simplex.Stage = 3;
						return (false, direction);
					}

					// ADB
					direction = FVector3.Normalize(FVector3.Cross(ab, ad));
					if (FVector3.Dot(ac, direction) > FP.Zero)
					{
						direction = -direction;
					}
					if (FVector3.Dot(simplex.A.Difference, direction) < -Tolerance)
					{
						// Remove c
						simplex.C = simplex.D;
						simplex.Stage = 3;
						return (false, direction);
					}

					// ACD
					direction = FVector3.Normalize(FVector3.Cross(ac, ad));
					if (FVector3.Dot(ab, direction) > FP.Zero)
					{
						direction = -direction;
					}
					if (FVector3.Dot(simplex.A.Difference, direction) < -Tolerance)
					{
						// Remove b
						simplex.B = simplex.C;
						simplex.C = simplex.D;
						simplex.Stage = 3;
						return (false, direction);
					}

					// BCD
					direction = FVector3.Normalize(FVector3.Cross(bc, bd));
					if (FVector3.Dot(ba, direction) > FP.Zero)
					{
						direction = -direction;
					}
					if (FVector3.Dot(simplex.B.Difference, direction) < -Tolerance)
					{
						// Remove a
						simplex.A = simplex.B;
						simplex.B = simplex.C;
						simplex.C = simplex.D;
						simplex.Stage = 3;
						return (false, direction);
					}

					// origin is in center
					return (true, direction);
				}
			}

			return (false, direction);
		}
	}
}
