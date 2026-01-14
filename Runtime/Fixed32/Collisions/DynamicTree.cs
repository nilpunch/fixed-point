using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed32
{
	// Adapted from Box2D v3.1
	// https://github.com/erincatto/box2d/blob/c05c48738fbe5c27625e36c5f0cfbdaddfc8359a/src/dynamic_tree.c
	public class DynamicTree
	{
		public const int DefaultCategory = 1;
		public const int NullIndex = -1;

		public const ushort AllocatedNode = 1 << 0;
		public const ushort EnlargedNode = 1 << 1;
		public const ushort LeafNode = 1 << 2;

		public TreeNode[] Nodes { get; set; } = Array.Empty<TreeNode>();
		public int NodesCapacity { get; set; }
		public int NodesCount { get; set; }
		public int FreeList { set; get; } = NullIndex;

		public int Root { get; set; } = NullIndex;

		public int ProxyCount { get; set; }

		private TreeNode DefaultNode { get; } = new TreeNode()
		{
			AABB = new FAABB(),
			CategoryBits = DefaultCategory,
			Children = new Children()
			{
				Child1 = NullIndex,
				Child2 = NullIndex,
			},
			Parent = NullIndex,
			Height = 0,
			Flags = AllocatedNode,
		};

		private int[] leafIndices;
		private int[] leafBoxes;
		private int[] leafCenters;
		private int[] binIndices;
		private int rebuildCapacity = 0;

		private int AllocateNode()
		{
			if (FreeList == NullIndex)
			{
				NodesCapacity = (NodesCount + 1) << 1;
				var nodes = Nodes;
				Array.Resize(ref nodes, NodesCapacity);
				Nodes = nodes;

				for (var i = NodesCount; i < NodesCapacity - 1; ++i)
				{
					Nodes[i].NextFree = i + 1;
				}

				Nodes[NodesCapacity - 1].NextFree = NullIndex;
				FreeList = NodesCount;
			}

			var nodeIndex = FreeList;
			ref var node = ref Nodes[nodeIndex];
			FreeList = node.NextFree;
			node = DefaultNode;
			NodesCount += 1;
			return nodeIndex;
		}

		private void FreeNode(int nodeId)
		{
			Nodes[nodeId].NextFree = FreeList;
			Nodes[nodeId].Flags = 0;
			FreeList = nodeId;
			NodesCount -= 1;
		}

		// Greedy algorithm for sibling selection using the SAH
		// We have three nodes A-(B,C) and want to add a leaf D, there are three choices.
		// 1: make a new parent for A and D : E-(A-(B,C), D)
		// 2: associate D with B
		//   a: B is a leaf : A-(E-(B,D), C)
		//   b: B is an internal node: A-(B{D},C)
		// 3: associate D with C
		//   a: C is a leaf : A-(B, E-(C,D))
		//   b: C is an internal node: A-(B, C{D})
		// All of these have a clear cost except when B or C is an internal node. Hence we need to be greedy.

		// The cost for cases 1, 2a, and 3a can be computed using the sibling cost formula.
		// cost of sibling H = area(union(H, D)) + increased area of ancestors

		// Suppose B (or C) is an internal node, then the lowest cost would be one of two cases:
		// case1: D becomes a sibling of B
		// case2: D becomes a descendant of B along with a new internal node of area(D).
		private int FindBestSibling(FAABB boxD)
		{
			var centerD = boxD.Center;
			var areaD = boxD.SurfaceArea;

			var nodes = Nodes;
			var rootIndex = Root;
			ref var root = ref nodes[rootIndex];

			// Area of current node.
			var areaBase = root.AABB.SurfaceArea;

			// Area of inflated node.
			var directCost = FAABB.Union(root.AABB, boxD).SurfaceArea;
			var inheritedCost = FP.Zero;

			var bestSibling = rootIndex;
			var bestCost = directCost;

			var index = rootIndex;

			// Descend the tree from root, following a single greedy path.
			while (nodes[index].Height > 0)
			{
				ref var node = ref nodes[index];
				var child1 = node.Children.Child1;
				var child2 = node.Children.Child2;

				// Cost of creating a new parent for this node and the new leaf.
				var cost = directCost + inheritedCost;

				// Sometimes there are multiple identical costs within tolerance.
				// This breaks the ties using the centroid distance.
				if (cost < bestCost)
				{
					bestSibling = index;
					bestCost = cost;
				}

				// Inheritance cost seen by children.
				inheritedCost += directCost - areaBase;

				ref var n1 = ref nodes[child1];
				ref var n2 = ref nodes[child2];

				var leaf1 = n1.Height == 0;
				var leaf2 = n2.Height == 0;

				// Cost of descending into child 1.
				var lowerCost1 = FP.MaxValue;
				var directCost1 = FAABB.Union(n1.AABB, boxD).SurfaceArea;
				var area1 = FP.Zero;
				if (leaf1)
				{
					// Child 1 is a leaf.
					// Cost of creating new node and increasing area of node P.
					var cost1 = directCost1 + inheritedCost;

					// Need this here due to while condition above.
					if (cost1 < bestCost)
					{
						bestSibling = child1;
						bestCost = cost1;
					}
				}
				else
				{
					// Child 1 is an internal node.
					area1 = n1.AABB.SurfaceArea;

					// Lower bound cost of inserting under child 1. The minimum accounts for two possibilities:
					// 1. Child1 could be the sibling with cost1 = inheritedCost + directCost1
					// 2. A descendent of child1 could be the sibling with the lower bound cost of
					//       cost1 = inheritedCost + (directCost1 - area1) + areaD
					// This minimum here leads to the minimum of these two costs.
					lowerCost1 = inheritedCost + directCost1 + FMath.Min(areaD - area1, FP.Zero);
				}

				// Cost of descending into child 2.
				var lowerCost2 = FP.MaxValue;
				var directCost2 = FAABB.Union(n2.AABB, boxD).SurfaceArea;
				var area2 = FP.Zero;
				if (leaf2)
				{
					var cost2 = directCost2 + inheritedCost;
					if (cost2 < bestCost)
					{
						bestSibling = child2;
						bestCost = cost2;
					}
				}
				else
				{
					area2 = n2.AABB.SurfaceArea;
					lowerCost2 = inheritedCost + directCost2 + FMath.Min(areaD - area2, FP.Zero);
				}

				if (leaf1 && leaf2)
				{
					break;
				}

				// Can the cost possibly be decreased?
				if (bestCost <= lowerCost1 && bestCost <= lowerCost2)
				{
					break;
				}

				if (lowerCost1 == lowerCost2 && !leaf1)
				{
					// No clear choice based on lower bound surface area. This can happen when both
					// children fully contain D. Fall back to node distance.
					var d1 = n1.AABB.Center - centerD;
					var d2 = n2.AABB.Center - centerD;
					lowerCost1 = FVector3.LengthSqr(d1);
					lowerCost2 = FVector3.LengthSqr(d2);
				}

				// Descend.
				if (lowerCost1 < lowerCost2 && !leaf1)
				{
					index = child1;
					areaBase = area1;
					directCost = directCost1;
				}
				else
				{
					index = child2;
					areaBase = area2;
					directCost = directCost2;
				}
			}

			return bestSibling;
		}

		private enum RotateType
		{
			None,
			BF,
			BG,
			CD,
			CE
		}

		// Perform a left or right rotation if node A is imbalanced.
		// Returns the new root index.
		private void RotateNodes(int iA)
		{
			if (iA == NullIndex)
			{
				return;
			}

			ref var A = ref Nodes[iA];
			if (A.Height < 2)
			{
				return;
			}

			var iB = A.Children.Child1;
			var iC = A.Children.Child2;

			ref var B = ref Nodes[iB];
			ref var C = ref Nodes[iC];

			if (B.Height == 0)
			{
				// B is a leaf and C is internal.
				var iF = C.Children.Child1;
				var iG = C.Children.Child2;
				ref var F = ref Nodes[iF];
				ref var G = ref Nodes[iG];

				// Base cost.
				var costBase = C.AABB.SurfaceArea;

				// Cost of swapping B and F.
				var aabbBG = FAABB.Union(B.AABB, G.AABB);
				var costBF = aabbBG.SurfaceArea;

				// Cost of swapping B and G.
				var aabbBF = FAABB.Union(B.AABB, F.AABB);
				var costBG = aabbBF.SurfaceArea;

				if (costBase < costBF && costBase < costBG)
				{
					// Rotation does not improve cost.
					return;
				}

				if (costBF < costBG)
				{
					// Swap B and F.
					A.Children.Child1 = iF;
					C.Children.Child1 = iB;

					B.Parent = iC;
					F.Parent = iA;

					C.AABB = aabbBG;
					C.Height = (ushort)(1 + Max(B.Height, G.Height));
					A.Height = (ushort)(1 + Max(C.Height, F.Height));
					C.CategoryBits = B.CategoryBits | G.CategoryBits;
					A.CategoryBits = C.CategoryBits | F.CategoryBits;
					C.Flags |= (ushort)((B.Flags | G.Flags) & EnlargedNode);
					A.Flags |= (ushort)((C.Flags | F.Flags) & EnlargedNode);
				}
				else
				{
					// Swap B and G.
					A.Children.Child1 = iG;
					C.Children.Child2 = iB;

					B.Parent = iC;
					G.Parent = iA;

					C.AABB = aabbBF;
					C.Height = (ushort)(1 + Max(B.Height, F.Height));
					A.Height = (ushort)(1 + Max(C.Height, G.Height));
					C.CategoryBits = B.CategoryBits | F.CategoryBits;
					A.CategoryBits = C.CategoryBits | G.CategoryBits;
					C.Flags |= (ushort)((B.Flags | F.Flags) & EnlargedNode);
					A.Flags |= (ushort)((C.Flags | G.Flags) & EnlargedNode);
				}
			}
			else if (C.Height == 0)
			{
				// C is a leaf and B is internal.
				var iD = B.Children.Child1;
				var iE = B.Children.Child2;
				ref var D = ref Nodes[iD];
				ref var E = ref Nodes[iE];

				// Base cost.
				var costBase = B.AABB.SurfaceArea;

				// Cost of swapping C and D.
				var aabbCE = FAABB.Union(C.AABB, E.AABB);
				var costCD = aabbCE.SurfaceArea;

				// Cost of swapping C and E.
				var aabbCD = FAABB.Union(C.AABB, D.AABB);
				var costCE = aabbCD.SurfaceArea;

				if (costBase < costCD && costBase < costCE)
				{
					// Rotation does not improve cost.
					return;
				}

				if (costCD < costCE)
				{
					// Swap C and D.
					A.Children.Child2 = iD;
					B.Children.Child1 = iC;

					C.Parent = iB;
					D.Parent = iA;

					B.AABB = aabbCE;
					B.Height = (ushort)(1 + Max(C.Height, E.Height));
					A.Height = (ushort)(1 + Max(B.Height, D.Height));
					B.CategoryBits = C.CategoryBits | E.CategoryBits;
					A.CategoryBits = B.CategoryBits | D.CategoryBits;
					B.Flags |= (ushort)((C.Flags | E.Flags) & EnlargedNode);
					A.Flags |= (ushort)((B.Flags | D.Flags) & EnlargedNode);
				}
				else
				{
					// Swap C and E.
					A.Children.Child2 = iE;
					B.Children.Child2 = iC;

					C.Parent = iB;
					E.Parent = iA;

					B.AABB = aabbCD;
					B.Height = (ushort)(1 + Max(C.Height, D.Height));
					A.Height = (ushort)(1 + Max(B.Height, E.Height));
					B.CategoryBits = C.CategoryBits | D.CategoryBits;
					A.CategoryBits = B.CategoryBits | E.CategoryBits;
					B.Flags |= (ushort)((C.Flags | D.Flags) & EnlargedNode);
					A.Flags |= (ushort)((B.Flags | E.Flags) & EnlargedNode);
				}
			}
			else
			{
				var iD = B.Children.Child1;
				var iE = B.Children.Child2;
				var iF = C.Children.Child1;
				var iG = C.Children.Child2;

				ref var D = ref Nodes[iD];
				ref var E = ref Nodes[iE];
				ref var F = ref Nodes[iF];
				ref var G = ref Nodes[iG];

				// Base cost.
				var areaB = B.AABB.SurfaceArea;
				var areaC = C.AABB.SurfaceArea;
				var costBase = areaB + areaC;

				var bestCost = costBase;
				var bestRotation = RotateType.None;

				// Cost of swapping B and F.
				var aabbBG = FAABB.Union(B.AABB, G.AABB);
				var costBF = areaB + aabbBG.SurfaceArea;
				if (costBF < bestCost)
				{
					bestCost = costBF;
					bestRotation = RotateType.BF;
				}

				// Cost of swapping B and G.
				var aabbBF = FAABB.Union(B.AABB, F.AABB);
				var costBG = areaB + aabbBF.SurfaceArea;
				if (costBG < bestCost)
				{
					bestCost = costBG;
					bestRotation = RotateType.BG;
				}

				// Cost of swapping C and D.
				var aabbCE = FAABB.Union(C.AABB, E.AABB);
				var costCD = areaC + aabbCE.SurfaceArea;
				if (costCD < bestCost)
				{
					bestCost = costCD;
					bestRotation = RotateType.CD;
				}

				// Cost of swapping C and E.
				var aabbCD = FAABB.Union(C.AABB, D.AABB);
				var costCE = areaC + aabbCD.SurfaceArea;
				if (costCE < bestCost)
				{
					bestRotation = RotateType.CE;
					// bestCost = costCE;
				}

				switch (bestRotation)
				{
					case RotateType.None:
						break;

					case RotateType.BF:
						A.Children.Child1 = iF;
						C.Children.Child1 = iB;

						B.Parent = iC;
						F.Parent = iA;

						C.AABB = aabbBG;
						C.Height = (ushort)(1 + Max(B.Height, G.Height));
						A.Height = (ushort)(1 + Max(C.Height, F.Height));
						C.CategoryBits = B.CategoryBits | G.CategoryBits;
						A.CategoryBits = C.CategoryBits | F.CategoryBits;
						C.Flags |= (ushort)((B.Flags | G.Flags) & EnlargedNode);
						A.Flags |= (ushort)((C.Flags | F.Flags) & EnlargedNode);
						break;

					case RotateType.BG:
						A.Children.Child1 = iG;
						C.Children.Child2 = iB;

						B.Parent = iC;
						G.Parent = iA;

						C.AABB = aabbBF;
						C.Height = (ushort)(1 + Max(B.Height, F.Height));
						A.Height = (ushort)(1 + Max(C.Height, G.Height));
						C.CategoryBits = B.CategoryBits | F.CategoryBits;
						A.CategoryBits = C.CategoryBits | G.CategoryBits;
						C.Flags |= (ushort)((B.Flags | F.Flags) & EnlargedNode);
						A.Flags |= (ushort)((C.Flags | G.Flags) & EnlargedNode);
						break;

					case RotateType.CD:
						A.Children.Child2 = iD;
						B.Children.Child1 = iC;

						C.Parent = iB;
						D.Parent = iA;

						B.AABB = aabbCE;
						B.Height = (ushort)(1 + Max(C.Height, E.Height));
						A.Height = (ushort)(1 + Max(B.Height, D.Height));
						B.CategoryBits = C.CategoryBits | E.CategoryBits;
						A.CategoryBits = B.CategoryBits | D.CategoryBits;
						B.Flags |= (ushort)((C.Flags | E.Flags) & EnlargedNode);
						A.Flags |= (ushort)((B.Flags | D.Flags) & EnlargedNode);
						break;

					case RotateType.CE:
						A.Children.Child2 = iE;
						B.Children.Child2 = iC;

						C.Parent = iB;
						E.Parent = iA;

						B.AABB = aabbCD;
						B.Height = (ushort)(1 + Max(C.Height, D.Height));
						A.Height = (ushort)(1 + Max(B.Height, E.Height));
						B.CategoryBits = C.CategoryBits | D.CategoryBits;
						A.CategoryBits = B.CategoryBits | E.CategoryBits;
						B.Flags |= (ushort)((C.Flags | D.Flags) & EnlargedNode);
						A.Flags |= (ushort)((B.Flags | E.Flags) & EnlargedNode);
						break;
				}
			}
		}

		private void InsertLeaf(int leaf, bool shouldRotate)
		{
			if (Root == NullIndex)
			{
				Root = leaf;
				if (NodesCapacity == 0)
				{
					AllocateNode();
				}
				Nodes[Root].Parent = NullIndex;
				return;
			}

			// Stage 1: find the best sibling for this node.
			var leafAABB = Nodes[leaf].AABB;
			var sibling = FindBestSibling(leafAABB);

			// Stage 2: create a new parent for the leaf and sibling.
			var oldParent = Nodes[sibling].Parent;
			var newParent = AllocateNode();

			// Warning: node ref can change after allocation.
			ref var newNode = ref Nodes[newParent];
			newNode.Parent = oldParent;
			newNode.UserData = ulong.MaxValue;
			var b = Nodes[sibling].AABB;
			newNode.AABB = FAABB.Union(leafAABB, b);
			newNode.CategoryBits = Nodes[leaf].CategoryBits | Nodes[sibling].CategoryBits;
			newNode.Height = (ushort)(Nodes[sibling].Height + 1);

			if (oldParent != NullIndex)
			{
				// The sibling was not the root.
				if (Nodes[oldParent].Children.Child1 == sibling)
				{
					Nodes[oldParent].Children.Child1 = newParent;
				}
				else
				{
					Nodes[oldParent].Children.Child2 = newParent;
				}
			}
			else
			{
				// The sibling was the root.
				Root = newParent;
			}

			newNode.Children.Child1 = sibling;
			newNode.Children.Child2 = leaf;
			Nodes[sibling].Parent = newParent;
			Nodes[leaf].Parent = newParent;

			// Stage 3: walk back up the tree fixing heights and AABBs.
			var index = Nodes[leaf].Parent;
			while (index != NullIndex)
			{
				ref var node = ref Nodes[index];
				var c1 = node.Children.Child1;
				var c2 = node.Children.Child2;

				var b1 = Nodes[c2].AABB;
				node.AABB = FAABB.Union(Nodes[c1].AABB, b1);
				node.CategoryBits = Nodes[c1].CategoryBits | Nodes[c2].CategoryBits;
				node.Height = (ushort)(1 + Max(Nodes[c1].Height, Nodes[c2].Height));
				node.Flags |= (ushort)((Nodes[c1].Flags | Nodes[c2].Flags) & EnlargedNode);

				if (shouldRotate)
				{
					RotateNodes(index);
				}

				index = node.Parent;
			}
		}

		private void RemoveLeaf(int leaf)
		{
			if (leaf == Root)
			{
				Root = NullIndex;
				return;
			}

			ref var leafNode = ref Nodes[leaf];
			var parent = leafNode.Parent;
			var grandParent = Nodes[parent].Parent;

			var sibling = (Nodes[parent].Children.Child1 == leaf)
				? Nodes[parent].Children.Child2
				: Nodes[parent].Children.Child1;

			if (grandParent != NullIndex)
			{
				// Destroy parent and connect sibling to grandParent.
				if (Nodes[grandParent].Children.Child1 == parent)
				{
					Nodes[grandParent].Children.Child1 = sibling;
				}
				else
				{
					Nodes[grandParent].Children.Child2 = sibling;
				}

				Nodes[sibling].Parent = grandParent;
				FreeNode(parent);

				// Adjust ancestor bounds.
				var index = grandParent;
				while (index != NullIndex)
				{
					ref var node = ref Nodes[index];
					ref var c1 = ref Nodes[node.Children.Child1];
					ref var c2 = ref Nodes[node.Children.Child2];

					node.AABB = FAABB.Union(c1.AABB, c2.AABB);
					node.CategoryBits = c1.CategoryBits | c2.CategoryBits;
					node.Height = (ushort)(1 + Max(c1.Height, c2.Height));

					index = node.Parent;
				}
			}
			else
			{
				Root = sibling;
				Nodes[sibling].Parent = NullIndex;
				FreeNode(parent);
			}
		}

		public int CreateProxy(FAABB aabb, ulong categoryBits, ulong userData)
		{
			// Create a proxy in the tree as a leaf node.
			// We return the index of the node instead of a pointer so that we can grow the node pool.
			var proxyId = AllocateNode();

			ref var node = ref Nodes[proxyId];
			node.AABB = aabb;
			node.UserData = userData;
			node.CategoryBits = categoryBits;
			node.Height = 0;
			node.Flags = AllocatedNode | LeafNode;

			InsertLeaf(proxyId, shouldRotate: true);

			ProxyCount++;
			return proxyId;
		}

		public void DestroyProxy(int proxyId)
		{
			// Remove and free the node.
			RemoveLeaf(proxyId);
			FreeNode(proxyId);
			ProxyCount--;
		}

		public int GetProxyCount()
		{
			return ProxyCount;
		}

		public void MoveProxy(int proxyId, FAABB aabb)
		{
			// Update the position of a proxy in the tree.
			RemoveLeaf(proxyId);
			Nodes[proxyId].AABB = aabb;
			InsertLeaf(proxyId, shouldRotate: false);
		}

		public void EnlargeProxy(int proxyId, FAABB aabb)
		{
			// Update and flag nodes as enlarged
			ref var node = ref Nodes[proxyId];
			node.AABB = aabb;

			var parentIndex = node.Parent;
			while (parentIndex != NullIndex)
			{
				ref var parent = ref Nodes[parentIndex];
				var changed = EnlargeAABB(ref parent.AABB, aabb);
				parent.Flags |= EnlargedNode;
				if (!changed)
				{
					break;
				}

				parentIndex = parent.Parent;
			}

			while (parentIndex != NullIndex)
			{
				ref var parent = ref Nodes[parentIndex];
				if ((parent.Flags & EnlargedNode) != 0)
				{
					break;
				}

				parent.Flags |= EnlargedNode;
				parentIndex = parent.Parent;
			}
		}

		public TreeStats Query(FAABB aabb, ulong maskBits, TreeQueryCallback callback, object context = null)
		{
			TreeStats result = default;

			if (NodesCount == 0)
			{
				return result;
			}

			// Fixed-size stack.
			const int StackSize = 1024;
			Span<int> stack = stackalloc int[StackSize];
			var stackCount = 0;

			stack[stackCount++] = Root;

			while (stackCount > 0)
			{
				var nodeId = stack[--stackCount];

				ref var node = ref Nodes[nodeId];
				result.NodeVisits++;

				if (FAABB.Overlaps(node.AABB, aabb) && (node.CategoryBits & maskBits) != 0)
				{
					if (node.IsLeaf())
					{
						// Callback to user code with proxy id.
						var proceed = callback(nodeId, node.UserData, context);
						result.LeafVisits++;

						if (!proceed)
						{
							return result;
						}
					}
					else
					{
						if (stackCount < StackSize - 1)
						{
							stack[stackCount++] = node.Children.Child1;
							stack[stackCount++] = node.Children.Child2;
						}
					}
				}
			}

			return result;
		}

		public TreeStats RayCast(in RayCastInput input, ulong maskBits, TreeRayCastCallback callback, object context = null)
		{
			TreeStats result = default;

			if (NodesCount == 0)
			{
				return result;
			}

			var p1 = input.Origin;
			var d = FVector3.NormalizeSafe(input.Translation);

			var maxFraction = input.MaxFraction;
			var b = d * maxFraction;
			var p2 = p1 + b;

			var segmentAABB = new FAABB
			{
				LowerBound = FVector3.MinComponents(p1, p2),
				UpperBound = FVector3.MaxComponents(p1, p2)
			};

			const int StackSize = 1024;
			Span<int> stack = stackalloc int[StackSize];
			var stackCount = 0;
			stack[stackCount++] = Root;

			var subInput = input;

			while (stackCount > 0)
			{
				var nodeId = stack[--stackCount];

				ref var node = ref Nodes[nodeId];
				result.NodeVisits++;

				if ((node.CategoryBits & maskBits) == 0 || !FAABB.Overlaps(node.AABB, segmentAABB))
				{
					continue;
				}

				if (!RayAABBTest(p1, p2, node.AABB))
				{
					continue;
				}

				if (node.IsLeaf())
				{
					subInput.MaxFraction = maxFraction;
					var value = callback(ref subInput, nodeId, node.UserData, context);
					result.LeafVisits++;

					if (value == FP.Zero)
					{
						return result;
					}

					if (value > FP.Zero && value <= maxFraction)
					{
						maxFraction = value;
						var b1 = d * maxFraction;
						p2 = p1 + b1;
						segmentAABB.LowerBound = FVector3.MinComponents(p1, p2);
						segmentAABB.UpperBound = FVector3.MaxComponents(p1, p2);
					}
				}
				else
				{
					ref var c1 = ref Nodes[node.Children.Child1];
					ref var c2 = ref Nodes[node.Children.Child2];

					var d1 = FVector3.DistanceSqr(c1.AABB.Center, p1);
					var d2 = FVector3.DistanceSqr(c2.AABB.Center, p1);

					if (d1 < d2)
					{
						stack[stackCount++] = node.Children.Child2;
						stack[stackCount++] = node.Children.Child1;
					}
					else
					{
						stack[stackCount++] = node.Children.Child1;
						stack[stackCount++] = node.Children.Child2;
					}
				}
			}

			return result;
		}

		private static bool RayAABBTest(FVector3 rayStart, FVector3 rayEnd, FAABB aabb)
		{
			var tmin = FP.Zero;
			var tmax = FP.One;

			var rayDir = rayEnd - rayStart;

			for (var i = 0; i < 3; i++)
			{
				if (FMath.Abs(rayDir[i]) < FP.Epsilon)
				{
					if (rayStart[i] < aabb.LowerBound[i] || rayStart[i] > aabb.UpperBound[i])
					{
						return false;
					}
				}
				else
				{
					var invDir = FP.One / rayDir[i];
					var t1 = (aabb.LowerBound[i] - rayStart[i]) * invDir;
					var t2 = (aabb.UpperBound[i] - rayStart[i]) * invDir;

					if (t1 > t2)
					{
						var temp = t1;
						t1 = t2;
						t2 = temp;
					}

					tmin = FMath.Max(tmin, t1);
					tmax = FMath.Min(tmax, t2);

					if (tmin > tmax)
					{
						return false;
					}
				}
			}

			return tmin <= FP.One && tmax >= FP.Zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool EnlargeAABB(ref FAABB a, FAABB b)
		{
			var changed = false;

			if (b.LowerBound.X < a.LowerBound.X)
			{
				a.LowerBound.X = b.LowerBound.X;
				changed = true;
			}
			if (b.LowerBound.Y < a.LowerBound.Y)
			{
				a.LowerBound.Y = b.LowerBound.Y;
				changed = true;
			}
			if (b.LowerBound.Z < a.LowerBound.Z)
			{
				a.LowerBound.Z = b.LowerBound.Z;
				changed = true;
			}

			if (a.UpperBound.X < b.UpperBound.X)
			{
				a.UpperBound.X = b.UpperBound.X;
				changed = true;
			}
			if (a.UpperBound.Y < b.UpperBound.Y)
			{
				a.UpperBound.Y = b.UpperBound.Y;
				changed = true;
			}
			if (a.UpperBound.Z < b.UpperBound.Z)
			{
				a.UpperBound.Z = b.UpperBound.Z;
				changed = true;
			}

			return changed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort Max(ushort a, ushort b)
		{
			return a > b ? a : b;
		}

		/// <summary>
		/// 48-byte.
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		public struct TreeNode
		{
			[FieldOffset(0)] public FAABB AABB;

			[FieldOffset(24)] public ulong CategoryBits;

			/// <summary>
			/// Children (internal node).
			/// </summary>
			[FieldOffset(32)] public Children Children;

			/// <summary>
			/// User data (leaf node).
			/// </summary>
			[FieldOffset(32)] public ulong UserData;

			/// <summary>
			/// The node parent index (allocated node).
			/// </summary>
			[FieldOffset(40)] public int Parent;

			/// <summary>
			/// The node freelist next index (free node).
			/// </summary>
			[FieldOffset(40)] public int NextFree;

			[FieldOffset(44)] public ushort Height;

			[FieldOffset(46)] public ushort Flags;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsLeaf()
			{
				return (Flags & LeafNode) != 0;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsAllocated()
			{
				return (Flags & AllocatedNode) != 0;
			}
		}

		/// <summary>
		/// 8-byte.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Children
		{
			public int Child1;
			public int Child2;
		}

		public struct TreeStats
		{
			public int NodeVisits;
			public int LeafVisits;
		}

		public delegate bool TreeQueryCallback(int nodeId, ulong userData, object context);

		public delegate FP TreeRayCastCallback(ref RayCastInput input, int nodeId, ulong userData, object context);

		public struct RayCastInput
		{
			public FVector3 Origin;
			public FVector3 Translation;
			public FP MaxFraction;
		}
	}
}
