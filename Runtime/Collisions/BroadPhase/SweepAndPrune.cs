using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
	public struct FAABB
	{
		public FVector3 LowerBound;
		public FVector3 UpperBound;
	}

	public struct BoradPhasePair
	{
		public int A;
		public int B;

		public BoradPhasePair(int a, int b)
		{
			A = a;
			B = b;
		}
	}

	public static class SweepAndPrune
	{
		private static readonly byte[] DeBruijn =
		{
			0, 1, 17, 2, 18, 50, 3, 57, 47, 19, 22, 51, 29, 4, 33, 58,
			15, 48, 20, 27, 25, 23, 52, 41, 54, 30, 38, 5, 43, 34, 59, 8,
			63, 16, 49, 56, 46, 21, 28, 32, 14, 26, 24, 40, 53, 37, 42, 7,
			62, 55, 45, 31, 13, 39, 36, 6, 61, 44, 12, 35, 60, 11, 10, 9,
		};

		private static readonly ComparerX _comparerX = new ComparerX();
		private static readonly ComparerY _comparerY = new ComparerY();
		private static readonly ComparerZ _comparerZ = new ComparerZ();
		private static List<int> _sortedX = new List<int>();
		private static List<int> _sortedY = new List<int>();
		private static List<int> _sortedZ = new List<int>();
		private static ulong[] _collisionsX = new ulong[8];
		private static ulong[] _collisionsY = new ulong[8];
		private static ulong[] _collisionsZ = new ulong[8];

		public static void FindPairs(List<BoradPhasePair> result, List<FAABB> aabbs)
		{
			_sortedX.Clear();
			_sortedY.Clear();
			_sortedZ.Clear();

			for (var i = 0; i < aabbs.Count; i++)
			{
				_sortedX.Add(i);
				_sortedY.Add(i);
				_sortedZ.Add(i);
			}

			FindPairs(result, aabbs, _sortedX, _sortedY, _sortedZ);
		}

		public static void FindPairs(List<BoradPhasePair> result, List<FAABB> aabbs, List<int> sortedX, List<int> sortedY, List<int> sortedZ)
		{
			if (sortedX.Count != sortedY.Count || sortedX.Count != sortedZ.Count || sortedX.Count != aabbs.Count)
			{
				throw new InvalidOperationException("Number of elemets in provided lists are messed up.");
			}

			result.Clear();
			var deBruijn = DeBruijn;

			var count = aabbs.Count;

			var stride = RoundUpToPowerOfTwo(count);
			var stridePower = (int)deBruijn[(int)(((ulong)stride * 0x37E84A99DAE458FUL) >> 58)];
			
			var totalBits  = stride * count;
			var totalLongs = (totalBits + 63) >> 6;
			if (_collisionsX.Length < totalLongs)
			{
				_collisionsX = new ulong[totalLongs];
				_collisionsY = new ulong[totalLongs];
				_collisionsZ = new ulong[totalLongs];
			}
			Array.Clear(_collisionsX, 0, totalLongs);
			Array.Clear(_collisionsY, 0, totalLongs);
			Array.Clear(_collisionsZ, 0, totalLongs);

			_comparerX.AABBs = aabbs;
			_comparerY.AABBs = aabbs;
			_comparerZ.AABBs = aabbs;

			sortedX.Sort(_comparerX);
			sortedY.Sort(_comparerY);
			sortedZ.Sort(_comparerZ);

			for (var i = 0; i < sortedX.Count; i++)
			{
				var bodyAIndex = sortedX[i];
				var bodyA = aabbs[bodyAIndex];

				for (var j = i + 1; j < sortedX.Count; j++)
				{
					var bodyBIndex = sortedX[j];
					var bodyB = aabbs[bodyBIndex];

					if (bodyA.UpperBound.X < bodyB.LowerBound.X)
					{
						break;
					}

					SetBit(_collisionsX, bodyAIndex, bodyBIndex, stridePower);
				}
			}

			for (var i = 0; i < sortedY.Count; i++)
			{
				var bodyAIndex = sortedY[i];
				var bodyA = aabbs[bodyAIndex];

				for (var j = i + 1; j < sortedY.Count; j++)
				{
					var bodyBIndex = sortedY[j];
					var bodyB = aabbs[bodyBIndex];

					if (bodyA.UpperBound.Y < bodyB.LowerBound.Y)
					{
						break;
					}

					SetBit(_collisionsY, bodyAIndex, bodyBIndex, stridePower);
				}
			}
			
			for (var i = 0; i < sortedZ.Count; i++)
			{
				var bodyAIndex = sortedZ[i];
				var bodyA = aabbs[bodyAIndex];

				for (var j = i + 1; j < sortedZ.Count; j++)
				{
					var bodyBIndex = sortedZ[j];
					var bodyB = aabbs[bodyBIndex];

					if (bodyA.UpperBound.Z < bodyB.LowerBound.Z)
					{
						break;
					}

					SetBit(_collisionsZ, bodyAIndex, bodyBIndex, stridePower);
				}
			}

			for (var i = 0; i < totalLongs; i++)
			{
				_collisionsX[i] &= _collisionsY[i] & _collisionsZ[i];
			}

			for (var blockIndex = 0; blockIndex < totalLongs; blockIndex++)
			{
				var block = _collisionsX[blockIndex];

				var offset = blockIndex << 6;

				while (block != 0UL)
				{
					var setBit = (int)deBruijn[(int)(((block & (ulong)-(long)block) * 0x37E84A99DAE458FUL) >> 58)];
					block &= block - 1UL;

					var pairIndex = setBit + offset;
					var i = pairIndex >> stridePower;
					var j = pairIndex - (i << stridePower);

					result.Add(new BoradPhasePair(i, j));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SetBit(ulong[] bitset, int i, int j, int stridePower)
		{
			if (i > j)
			{
				(i, j) = (j, i);
			}

			var index = (i << stridePower) + j;
			bitset[index >> 6] |= 1UL << (index & 63);
		}

		private static int RoundUpToPowerOfTwo(int value)
		{
			var v = (uint)value;

			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;

			return (int)v;
		}

		public class ComparerX : IComparer<int>
		{
			public List<FAABB> AABBs;

			public int Compare(int x, int y)
			{
				return AABBs[x].LowerBound.X.CompareTo(AABBs[y].LowerBound.X);
			}
		}

		public class ComparerY : IComparer<int>
		{
			public List<FAABB> AABBs;

			public int Compare(int x, int y)
			{
				return AABBs[x].LowerBound.Y.CompareTo(AABBs[y].LowerBound.Y);
			}
		}

		public class ComparerZ : IComparer<int>
		{
			public List<FAABB> AABBs;

			public int Compare(int x, int y)
			{
				return AABBs[x].LowerBound.Z.CompareTo(AABBs[y].LowerBound.Z);
			}
		}
	}
}
