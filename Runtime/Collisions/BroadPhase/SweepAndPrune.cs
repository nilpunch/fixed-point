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
		private static readonly ComparerX _comparerX = new ComparerX();
		private static List<int> _sortedX = new List<int>();

		public static void FindPairs(List<BoradPhasePair> result, List<FAABB> aabbs)
		{
			_sortedX.Clear();

			for (var i = 0; i < aabbs.Count; i++)
			{
				_sortedX.Add(i);
			}

			FindPairs(result, aabbs, _sortedX);
		}

		public static void FindPairs(List<BoradPhasePair> result, List<FAABB> aabbs, List<int> sortedX)
		{
			if (sortedX.Count != aabbs.Count)
			{
				throw new InvalidOperationException("Number of elemets in provided lists are messed up.");
			}

			result.Clear();

			var count = aabbs.Count;

			_comparerX.AABBs = aabbs;
			sortedX.Sort(_comparerX);

			for (var i = 0; i < count; i++)
			{
				var aIndex = sortedX[i];
				var a = aabbs[aIndex];

				for (var j = i + 1; j < sortedX.Count; j++)
				{
					var bIndex = sortedX[j];
					var b = aabbs[bIndex];

					if (a.UpperBound.X < b.LowerBound.X)
					{
						break;
					}

					if (a.UpperBound.Z >= b.LowerBound.Z && b.UpperBound.Z >= a.LowerBound.Z
						&& a.UpperBound.Y >= b.LowerBound.Y && b.UpperBound.Y >= a.LowerBound.Y)
					{
						result.Add(new BoradPhasePair(i, j));
					}
				}
			}
		}

		public class ComparerX : IComparer<int>
		{
			public List<FAABB> AABBs;

			public int Compare(int x, int y)
			{
				return AABBs[x].LowerBound.X.CompareTo(AABBs[y].LowerBound.X);
			}
		}
	}
}
