using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fixed32;
using FP64 = Fixed64.FP;

namespace Fixed
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

		public TreeNode[] Nodes { get; private set; } = Array.Empty<TreeNode>();
		public int NodesCapacity { get; private set; }
		public int NodesCount { get; private set; }
		public int FreeList { set; private get; } = NullIndex;

		public int Root { get; private set; } = NullIndex;

		public int ProxyCount { get; private set; }

		private readonly Stack<int> _stack = new Stack<int>(1024);

		private TreeNode DefaultNode { get; } = new TreeNode()
		{
			AABB = new FAABB(),
			CategoryBits = DefaultCategory,
			Children = new Children()
			{
				Left = NullIndex,
				Right = NullIndex,
			},
			Parent = NullIndex,
			Height = 0,
			Flags = AllocatedNode,
		};

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
			var areaD = SurfaceAreaHeuristic(boxD);

			var nodes = Nodes;
			var rootIndex = Root;
			ref var root = ref nodes[rootIndex];

			// Area of current node.
			var areaBase = SurfaceAreaHeuristic(root.AABB);

			// Area of inflated node.
			var directCost = SurfaceAreaHeuristic(FAABB.Union(root.AABB, boxD));
			var inheritedCost = FP64.Zero;

			var bestSibling = rootIndex;
			var bestCost = directCost;

			var index = rootIndex;

			// Descend the tree from root, following a single greedy path.
			while (nodes[index].Height > 0)
			{
				ref var node = ref nodes[index];
				var left = node.Children.Left;
				var right = node.Children.Right;

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

				ref var leftNode = ref nodes[left];
				ref var rightNode = ref nodes[right];

				var leaf1 = leftNode.Height == 0;
				var leaf2 = rightNode.Height == 0;

				// Cost of descending into child 1.
				var lowerCost1 = FP64.MaxValue;
				var directCost1 = SurfaceAreaHeuristic(FAABB.Union(leftNode.AABB, boxD));
				var area1 = FP64.Zero;
				if (leaf1)
				{
					// Child 1 is a leaf.
					// Cost of creating new node and increasing area of node P.
					var cost1 = directCost1 + inheritedCost;

					// Need this here due to while condition above.
					if (cost1 < bestCost)
					{
						bestSibling = left;
						bestCost = cost1;
					}
				}
				else
				{
					// Child 1 is an internal node.
					area1 = SurfaceAreaHeuristic(leftNode.AABB);

					// Lower bound cost of inserting under child 1. The minimum accounts for two possibilities:
					// 1. Child1 could be the sibling with cost1 = inheritedCost + directCost1
					// 2. A descendent of child1 could be the sibling with the lower bound cost of
					//       cost1 = inheritedCost + (directCost1 - area1) + areaD
					// This minimum here leads to the minimum of these two costs.
					lowerCost1 = inheritedCost + directCost1 + FP64.Min(areaD - area1, FP64.Zero);
				}

				// Cost of descending into child 2.
				var lowerCost2 = FP64.MaxValue;
				var directCost2 = SurfaceAreaHeuristic(FAABB.Union(rightNode.AABB, boxD));
				var area2 = FP64.Zero;
				if (leaf2)
				{
					var cost2 = directCost2 + inheritedCost;
					if (cost2 < bestCost)
					{
						bestSibling = right;
						bestCost = cost2;
					}
				}
				else
				{
					area2 = SurfaceAreaHeuristic(rightNode.AABB);
					lowerCost2 = inheritedCost + directCost2 + FP64.Min(areaD - area2, FP64.Zero);
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
					lowerCost1 = LengthSqr(leftNode.AABB.Center - centerD);
					lowerCost2 = LengthSqr(rightNode.AABB.Center - centerD);
				}

				// Descend.
				if (lowerCost1 < lowerCost2 && !leaf1)
				{
					index = left;
					areaBase = area1;
					directCost = directCost1;
				}
				else
				{
					index = right;
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

			var iB = A.Children.Left;
			var iC = A.Children.Right;

			ref var B = ref Nodes[iB];
			ref var C = ref Nodes[iC];

			if (B.Height == 0)
			{
				// B is a leaf and C is internal.
				var iF = C.Children.Left;
				var iG = C.Children.Right;
				ref var F = ref Nodes[iF];
				ref var G = ref Nodes[iG];

				// Base cost.
				var costBase = SurfaceAreaHeuristic(C.AABB);

				// Cost of swapping B and F.
				var aabbBG = FAABB.Union(B.AABB, G.AABB);
				var costBF = SurfaceAreaHeuristic(aabbBG);

				// Cost of swapping B and G.
				var aabbBF = FAABB.Union(B.AABB, F.AABB);
				var costBG = SurfaceAreaHeuristic(aabbBF);

				if (costBase < costBF && costBase < costBG)
				{
					// Rotation does not improve cost.
					return;
				}

				if (costBF < costBG)
				{
					// Swap B and F.
					A.Children.Left = iF;
					C.Children.Left = iB;

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
					A.Children.Left = iG;
					C.Children.Right = iB;

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
				var iD = B.Children.Left;
				var iE = B.Children.Right;
				ref var D = ref Nodes[iD];
				ref var E = ref Nodes[iE];

				// Base cost.
				var costBase = SurfaceAreaHeuristic(B.AABB);

				// Cost of swapping C and D.
				var aabbCE = FAABB.Union(C.AABB, E.AABB);
				var costCD = SurfaceAreaHeuristic(aabbCE);

				// Cost of swapping C and E.
				var aabbCD = FAABB.Union(C.AABB, D.AABB);
				var costCE = SurfaceAreaHeuristic(aabbCD);

				if (costBase < costCD && costBase < costCE)
				{
					// Rotation does not improve cost.
					return;
				}

				if (costCD < costCE)
				{
					// Swap C and D.
					A.Children.Right = iD;
					B.Children.Left = iC;

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
					A.Children.Right = iE;
					B.Children.Right = iC;

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
				var iD = B.Children.Left;
				var iE = B.Children.Right;
				var iF = C.Children.Left;
				var iG = C.Children.Right;

				ref var D = ref Nodes[iD];
				ref var E = ref Nodes[iE];
				ref var F = ref Nodes[iF];
				ref var G = ref Nodes[iG];

				// Base cost.
				var areaB = SurfaceAreaHeuristic(B.AABB);
				var areaC = SurfaceAreaHeuristic(C.AABB);
				var costBase = areaB + areaC;

				var bestCost = costBase;
				var bestRotation = RotateType.None;

				// Cost of swapping B and F.
				var aabbBG = FAABB.Union(B.AABB, G.AABB);
				var costBF = areaB + SurfaceAreaHeuristic(aabbBG);
				if (costBF < bestCost)
				{
					bestCost = costBF;
					bestRotation = RotateType.BF;
				}

				// Cost of swapping B and G.
				var aabbBF = FAABB.Union(B.AABB, F.AABB);
				var costBG = areaB + SurfaceAreaHeuristic(aabbBF);
				if (costBG < bestCost)
				{
					bestCost = costBG;
					bestRotation = RotateType.BG;
				}

				// Cost of swapping C and D.
				var aabbCE = FAABB.Union(C.AABB, E.AABB);
				var costCD = areaC + SurfaceAreaHeuristic(aabbCE);
				if (costCD < bestCost)
				{
					bestCost = costCD;
					bestRotation = RotateType.CD;
				}

				// Cost of swapping C and E.
				var aabbCD = FAABB.Union(C.AABB, D.AABB);
				var costCE = areaC + SurfaceAreaHeuristic(aabbCD);
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
						A.Children.Left = iF;
						C.Children.Left = iB;

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
						A.Children.Left = iG;
						C.Children.Right = iB;

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
						A.Children.Right = iD;
						B.Children.Left = iC;

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
						A.Children.Right = iE;
						B.Children.Right = iC;

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
				if (Nodes[oldParent].Children.Left == sibling)
				{
					Nodes[oldParent].Children.Left = newParent;
				}
				else
				{
					Nodes[oldParent].Children.Right = newParent;
				}
			}
			else
			{
				// The sibling was the root.
				Root = newParent;
			}

			newNode.Children.Left = sibling;
			newNode.Children.Right = leaf;
			Nodes[sibling].Parent = newParent;
			Nodes[leaf].Parent = newParent;

			// Stage 3: walk back up the tree fixing heights and AABBs.
			var index = Nodes[leaf].Parent;
			while (index != NullIndex)
			{
				ref var node = ref Nodes[index];
				var c1 = node.Children.Left;
				var c2 = node.Children.Right;

				node.AABB = FAABB.Union(Nodes[c1].AABB, Nodes[c2].AABB);
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

			var sibling = (Nodes[parent].Children.Left == leaf)
				? Nodes[parent].Children.Right
				: Nodes[parent].Children.Left;

			if (grandParent != NullIndex)
			{
				// Destroy parent and connect sibling to grandParent.
				if (Nodes[grandParent].Children.Left == parent)
				{
					Nodes[grandParent].Children.Left = sibling;
				}
				else
				{
					Nodes[grandParent].Children.Right = sibling;
				}

				Nodes[sibling].Parent = grandParent;
				FreeNode(parent);

				// Adjust ancestor bounds.
				var index = grandParent;
				while (index != NullIndex)
				{
					ref var node = ref Nodes[index];
					ref var c1 = ref Nodes[node.Children.Left];
					ref var c2 = ref Nodes[node.Children.Right];

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
			// Update and flag nodes as enlarged.
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
							stack[stackCount++] = node.Children.Left;
							stack[stackCount++] = node.Children.Right;
						}
					}
				}
			}

			return result;
		}

		public TreeStats RayCast(RayCastInput input, ulong maskBits, TreeRayCastCallback callback, object context = null)
		{
			TreeStats result = default;

			if (NodesCount == 0)
			{
				return result;
			}

			var maxFraction = input.MaxFraction;

			var subInput = input;

			_stack.Push(Root);

			while (_stack.Count > 0)
			{
				var nodeId = _stack.Pop();

				ref var node = ref Nodes[nodeId];
				result.NodeVisits++;

				if ((node.CategoryBits & maskBits) == 0)
				{
					continue;
				}

				if (!node.AABB.RayIntersect(input.Origin, input.Direction, out var enter))
				{
					continue;
				}

				if (enter > maxFraction)
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
					}

					continue;
				}

				ref var left = ref Nodes[node.Children.Left];
				ref var right = ref Nodes[node.Children.Right];

				var dLeft = LengthSqr(left.AABB.Center - input.Origin);
				var dRight = LengthSqr(right.AABB.Center - input.Origin);

				if (dLeft < dRight)
				{
					_stack.Push(node.Children.Right);
					_stack.Push(node.Children.Left);
				}
				else
				{
					_stack.Push(node.Children.Left);
					_stack.Push(node.Children.Right);
				}
			}

			return result;
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
		private static FP64 SurfaceAreaHeuristic(FAABB aabb)
		{
			var size = aabb.UpperBound - aabb.LowerBound;
			var x = size.X.To64();
			var y = size.Y.To64();
			var z = size.Z.To64();
			return x * y + x * z + y * z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP64 LengthSqr(FVector3 v)
		{
			var x = v.X.To64();
			var y = v.Y.To64();
			var z = v.Z.To64();
			return x * x + y * y + z * z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ushort Max(ushort a, ushort b)
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
			public int Left;
			public int Right;
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
			public FVector3 Direction;
			public FP MaxFraction;
		}
	}
}
