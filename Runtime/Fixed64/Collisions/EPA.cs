using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fixed64
{
	public readonly struct Collision
	{
		public ContactPoint ContactFirst { get; }

		public ContactPoint ContactSecond { get; }

		public FVector3 PenetrationNormal { get; }

		public FP PenetrationDepth { get; }

		public Collision(ContactPoint contactFirst, ContactPoint contactSecond, FVector3 penetrationNormal, FP penetrationDepth)
		{
			ContactFirst = contactFirst;
			PenetrationNormal = penetrationNormal;
			PenetrationDepth = penetrationDepth;
			ContactSecond = contactSecond;
		}
	}

	public struct ContactPoint
	{
		public FVector3 Position { get; }

		public ContactPoint(FVector3 position)
		{
			Position = position;
		}
	}

	public static class EPA
	{
		private static FP Tolerance => FP.CalculationsEpsilon;
		private static FP NormalBias => FP.CalculationsEpsilonSqr;

		public static List<MinkowskiDifference> Vertices { get; } = new List<MinkowskiDifference>();
		public static List<PolytopeFace> Faces { get; } = new List<PolytopeFace>();
		private static List<PolytopeEdge> LooseEdges { get; } = new List<PolytopeEdge>();

		public static FVector3 Barycentric(FVector3 a, FVector3 b, FVector3 c, FVector3 point, bool clamp = false)
		{
			var v0 = b - a;
			var v1 = c - a;
			var v2 = point - a;
			var d00 = FVector3.Dot(v0, v0);
			var d01 = FVector3.Dot(v0, v1);
			var d11 = FVector3.Dot(v1, v1);
			var d20 = FVector3.Dot(v2, v0);
			var d21 = FVector3.Dot(v2, v1);
			var denominator = d00 * d11 - d01 * d01;
			var v = (d11 * d20 - d01 * d21) / denominator;
			var w = (d00 * d21 - d01 * d20) / denominator;
			var u = FP.One - v - w;

			return new FVector3(u, v, w);
		}

		public struct PolytopeFace
		{
			public int A;
			public int B;
			public int C;
			public FVector3 Normal;

			public PolytopeFace(int a, int b, int c, FVector3 normal)
			{
				A = a;
				B = b;
				C = c;
				Normal = normal;
			}
		}

		public struct PolytopeEdge
		{
			public int A;
			public int B;

			public PolytopeEdge(int a, int b)
			{
				A = a;
				B = b;
			}

			public void Deconstruct(out int a, out int b)
			{
				a = A;
				b = B;
			}
		}

		public struct ClosestFace
		{
			public FP Distance;
			public PolytopeFace Face;

			public ClosestFace(FP distance, PolytopeFace face)
			{
				Distance = distance;
				Face = face;
			}
		}

		public static Collision Calculate<TA, TB>(Simplex simplex, TA shapeA,
			TB shapeB, int maxIterations = 100)
			where TA : ISupportMappable
			where TB : ISupportMappable
		{
			Faces.Clear();
			Vertices.Clear();

			Vertices.Add(simplex.A);
			Vertices.Add(simplex.B);
			Vertices.Add(simplex.C);
			Vertices.Add(simplex.D);

			Faces.Add(new PolytopeFace(0, 1, 2, CalculateFaceNormal(simplex.A.Difference, simplex.B.Difference, simplex.C.Difference)));
			Faces.Add(new PolytopeFace(0, 2, 3, CalculateFaceNormal(simplex.A.Difference, simplex.C.Difference, simplex.D.Difference)));
			Faces.Add(new PolytopeFace(0, 3, 1, CalculateFaceNormal(simplex.A.Difference, simplex.D.Difference, simplex.B.Difference)));
			Faces.Add(new PolytopeFace(1, 3, 2, CalculateFaceNormal(simplex.B.Difference, simplex.D.Difference, simplex.C.Difference)));

			ClosestFace closestFace = default;

			int iteration;
			for (iteration = 0; iteration < maxIterations; iteration++)
			{
				closestFace = FindClosestFace(Faces);

				var searchDirection = closestFace.Face.Normal;
				var supportPoint = MinkowskiDifference.Calculate(shapeA, shapeB, searchDirection);

				var minkowskiDistance = FVector3.Dot(supportPoint.Difference, searchDirection);
				if (minkowskiDistance - closestFace.Distance < Tolerance)
				{
					break;
				}

				Vertices.Add(supportPoint);

				ExpandPolytope(supportPoint);
			}

			if (iteration >= maxIterations)
			{
				throw new Exception();
			}

			var barycentric = Barycentric(
				Vertices[closestFace.Face.A].Difference,
				Vertices[closestFace.Face.B].Difference,
				Vertices[closestFace.Face.C].Difference,
				closestFace.Face.Normal * closestFace.Distance);

			var supportAA = Vertices[closestFace.Face.A].SupportA;
			var supportAB = Vertices[closestFace.Face.B].SupportA;
			var supportAC = Vertices[closestFace.Face.C].SupportA;
			var supportBA = Vertices[closestFace.Face.A].SupportB;
			var supportBB = Vertices[closestFace.Face.B].SupportB;
			var supportBC = Vertices[closestFace.Face.C].SupportB;

			var point1 = barycentric.X * supportAA + barycentric.Y * supportAB + barycentric.Z * supportAC;
			var point2 = barycentric.X * supportBA + barycentric.Y * supportBB + barycentric.Z * supportBC;

			return new Collision(new ContactPoint(point1), new ContactPoint(point2), closestFace.Face.Normal, closestFace.Distance + Tolerance);
		}

		public static void ExpandPolytope(MinkowskiDifference supportPoint)
		{
			LooseEdges.Clear();

			for (var i = 0; i < Faces.Count; i++)
			{
				var face = Faces[i];

				if (FVector3.Dot(face.Normal, supportPoint.Difference - Vertices[face.A].Difference) > NormalBias)
				{
					var edgeAB = new PolytopeEdge(face.A, face.B);
					var edgeBC = new PolytopeEdge(face.B, face.C);
					var edgeCA = new PolytopeEdge(face.C, face.A);

					RemoveIfExistsOrAdd(LooseEdges, edgeAB);
					RemoveIfExistsOrAdd(LooseEdges, edgeBC);
					RemoveIfExistsOrAdd(LooseEdges, edgeCA);

					Faces.RemoveAt(i);
					i -= 1;
				}
			}

			var c = Vertices.Count - 1;
			foreach (var (a, b) in LooseEdges)
			{
				var vA = Vertices[a].Difference;
				var vB = Vertices[b].Difference;
				var vC = Vertices[c].Difference;

				var face = new PolytopeFace(a, b, c, FVector3.Normalize(FVector3.Cross(vA - vB, vA - vC)));

				if (FVector3.Dot(vA, face.Normal) < -NormalBias)
				{
					(face.A, face.B) = (face.B, face.A);
					face.Normal = -face.Normal;
				}

				Faces.Add(face);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ClosestFace FindClosestFace(List<PolytopeFace> faces)
		{
			var closest = new ClosestFace(FP.MaxValue, default);

			for (var i = 0; i < faces.Count; i++)
			{
				var face = faces[i];
				var distance = FVector3.Dot(Vertices[face.A].Difference, face.Normal);

				if (distance < closest.Distance)
				{
					closest.Distance = distance;
					closest.Face = face;
				}
			}

			return closest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static FVector3 CalculateFaceNormal(FVector3 a, FVector3 b, FVector3 c)
		{
			var ab = b - a;
			var ac = c - a;
			return FVector3.Normalize(FVector3.Cross(ab, ac));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void RemoveIfExistsOrAdd(List<PolytopeEdge> edges, PolytopeEdge edge)
		{
			var edgeIndex = -1;

			for (var index = 0; index < edges.Count; index++)
			{
				var pair = edges[index];

				if (pair.A == edge.B && pair.B == edge.A)
				{
					edgeIndex = index;
					break;
				}
			}

			if (edgeIndex != -1)
			{
				edges.RemoveAt(edgeIndex);
			}
			else
			{
				edges.Add(edge);
			}
		}
	}
}
