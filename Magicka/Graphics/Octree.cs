using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Magicka.Graphics
{
	// Token: 0x020003EA RID: 1002
	public class Octree<T> where T : IVertex
	{
		// Token: 0x06001EB1 RID: 7857 RVA: 0x000D61B2 File Offset: 0x000D43B2
		public Octree()
		{
		}

		// Token: 0x06001EB2 RID: 7858 RVA: 0x000D61BA File Offset: 0x000D43BA
		public void Clear(bool NOTUSED)
		{
			this.positions = null;
			this.triBoxes = null;
			this.tris = null;
			this.nodes = null;
			this.boundingBox = null;
		}

		// Token: 0x06001EB3 RID: 7859 RVA: 0x000D61DF File Offset: 0x000D43DF
		public void AddTriangles(List<T> _positions, List<TriangleVertexIndices> _tris)
		{
			this.positions = new T[_positions.Count];
			_positions.CopyTo(this.positions);
			this.tris = new TriangleVertexIndices[_tris.Count];
			_tris.CopyTo(this.tris);
		}

		// Token: 0x06001EB4 RID: 7860 RVA: 0x000D621C File Offset: 0x000D441C
		public void BuildOctree(int _maxTrisPerCellNOTUSED, float _minCellSizeNOTUSED)
		{
			this.triBoxes = new BoundingBox[this.tris.Length];
			this.rootNodeBox = new BoundingBox(new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity), new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity));
			for (int i = 0; i < this.tris.Length; i++)
			{
				this.triBoxes[i].Min = Vector3.Min(this.positions[this.tris[i].I0].Position, Vector3.Min(this.positions[this.tris[i].I1].Position, this.positions[this.tris[i].I2].Position));
				this.triBoxes[i].Max = Vector3.Max(this.positions[this.tris[i].I0].Position, Vector3.Max(this.positions[this.tris[i].I1].Position, this.positions[this.tris[i].I2].Position));
				this.rootNodeBox.Min = Vector3.Min(this.rootNodeBox.Min, this.triBoxes[i].Min);
				this.rootNodeBox.Max = Vector3.Max(this.rootNodeBox.Max, this.triBoxes[i].Max);
			}
			this.boundingBox = new AABox(this.rootNodeBox.Min, this.rootNodeBox.Max);
			List<Octree<T>.BuildNode> list = new List<Octree<T>.BuildNode>();
			list.Add(new Octree<T>.BuildNode());
			list[0].box = this.rootNodeBox;
			BoundingBox[] array = new BoundingBox[8];
			for (int j = 0; j < this.tris.Length; j++)
			{
				int num = 0;
				BoundingBox aabb = this.rootNodeBox;
				while (aabb.Contains(this.triBoxes[j]) == ContainmentType.Contains)
				{
					int num2 = -1;
					for (int k = 0; k < 8; k++)
					{
						array[k] = this.CreateAABox(aabb, (Octree<T>.EChild)k);
						if (array[k].Contains(this.triBoxes[j]) == ContainmentType.Contains)
						{
							num2 = k;
							break;
						}
					}
					if (num2 == -1)
					{
						list[num].triIndices.Add(j);
						break;
					}
					int num3 = -1;
					for (int l = 0; l < list[num].nodeIndices.Count; l++)
					{
						if (list[list[num].nodeIndices[l]].childType == num2)
						{
							num3 = l;
							break;
						}
					}
					if (num3 == -1)
					{
						Octree<T>.BuildNode buildNode = list[num];
						list.Add(new Octree<T>.BuildNode
						{
							childType = num2,
							box = array[num2]
						});
						num = list.Count - 1;
						aabb = array[num2];
						buildNode.nodeIndices.Add(num);
					}
					else
					{
						num = list[num].nodeIndices[num3];
						aabb = array[num2];
					}
				}
			}
			this.nodes = new Octree<T>.Node[list.Count];
			this.nodeStack = new ushort[list.Count];
			for (int m = 0; m < this.nodes.Length; m++)
			{
				this.nodes[m].nodeIndices = new ushort[list[m].nodeIndices.Count];
				for (int n = 0; n < this.nodes[m].nodeIndices.Length; n++)
				{
					this.nodes[m].nodeIndices[n] = (ushort)list[m].nodeIndices[n];
				}
				this.nodes[m].triIndices = new int[list[m].triIndices.Count];
				list[m].triIndices.CopyTo(this.nodes[m].triIndices);
				this.nodes[m].box = list[m].box;
			}
		}

		// Token: 0x06001EB5 RID: 7861 RVA: 0x000D6705 File Offset: 0x000D4905
		public Octree(List<T> _positions, List<TriangleVertexIndices> _tris)
		{
			this.AddTriangles(_positions, _tris);
			this.BuildOctree(16, 1f);
		}

		// Token: 0x06001EB6 RID: 7862 RVA: 0x000D6724 File Offset: 0x000D4924
		private BoundingBox CreateAABox(BoundingBox aabb, Octree<T>.EChild child)
		{
			Vector3 vector = 0.5f * (aabb.Max - aabb.Min);
			Vector3 vector2 = default(Vector3);
			switch (child)
			{
			case Octree<T>.EChild.MMM:
				vector2 = new Vector3(0f, 0f, 0f);
				break;
			case Octree<T>.EChild.XP:
				vector2 = new Vector3(1f, 0f, 0f);
				break;
			case Octree<T>.EChild.YP:
				vector2 = new Vector3(0f, 1f, 0f);
				break;
			case Octree<T>.EChild.PPM:
				vector2 = new Vector3(1f, 1f, 0f);
				break;
			case Octree<T>.EChild.ZP:
				vector2 = new Vector3(0f, 0f, 1f);
				break;
			case Octree<T>.EChild.PMP:
				vector2 = new Vector3(1f, 0f, 1f);
				break;
			case Octree<T>.EChild.MPP:
				vector2 = new Vector3(0f, 1f, 1f);
				break;
			case Octree<T>.EChild.PPP:
				vector2 = new Vector3(1f, 1f, 1f);
				break;
			}
			BoundingBox result = default(BoundingBox);
			result.Min = aabb.Min + new Vector3(vector2.X * vector.X, vector2.Y * vector.Y, vector2.Z * vector.Z);
			result.Max = result.Min + vector;
			float scaleFactor = 1E-05f;
			result.Min -= scaleFactor * vector;
			result.Max += scaleFactor * vector;
			return result;
		}

		// Token: 0x06001EB7 RID: 7863 RVA: 0x000D68E8 File Offset: 0x000D4AE8
		private void GatherTriangles(int _nodeIndex, ref List<int> _tris)
		{
			_tris.AddRange(this.nodes[_nodeIndex].triIndices);
			int num = this.nodes[_nodeIndex].nodeIndices.Length;
			for (int i = 0; i < num; i++)
			{
				int nodeIndex = (int)this.nodes[_nodeIndex].nodeIndices[i];
				this.GatherTriangles(nodeIndex, ref _tris);
			}
		}

		// Token: 0x06001EB8 RID: 7864 RVA: 0x000D694C File Offset: 0x000D4B4C
		public unsafe int GetTrianglesIntersectingtAABox(int* triangles, int maxTriangles, ref BoundingBox testBox)
		{
			if (this.nodes.Length == 0)
			{
				return 0;
			}
			int i = 0;
			int num = 1;
			this.nodeStack[0] = 0;
			int num2 = 0;
			while (i < num)
			{
				ushort num3 = this.nodeStack[i];
				i++;
				if (this.nodes[(int)num3].box.Contains(testBox) != ContainmentType.Disjoint)
				{
					for (int j = 0; j < this.nodes[(int)num3].triIndices.Length; j++)
					{
						if (this.triBoxes[this.nodes[(int)num3].triIndices[j]].Contains(testBox) != ContainmentType.Disjoint && num2 < maxTriangles)
						{
							triangles[num2++] = this.nodes[(int)num3].triIndices[j];
						}
					}
					int num4 = this.nodes[(int)num3].nodeIndices.Length;
					for (int k = 0; k < num4; k++)
					{
						this.nodeStack[num++] = this.nodes[(int)num3].nodeIndices[k];
					}
				}
			}
			return num2;
		}

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x06001EB9 RID: 7865 RVA: 0x000D6A66 File Offset: 0x000D4C66
		public AABox BoundingBox
		{
			get
			{
				return this.boundingBox;
			}
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x06001EBA RID: 7866 RVA: 0x000D6A6E File Offset: 0x000D4C6E
		public Octree<T>.Node[] Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		// Token: 0x06001EBB RID: 7867 RVA: 0x000D6A78 File Offset: 0x000D4C78
		public IndexedTriangle GetTriangle(int _index)
		{
			TriangleVertexIndices triangleVertexIndices = this.tris[_index];
			return new IndexedTriangle(triangleVertexIndices.I0, triangleVertexIndices.I1, triangleVertexIndices.I2, this.positions[triangleVertexIndices.I0].Position, this.positions[triangleVertexIndices.I1].Position, this.positions[triangleVertexIndices.I2].Position);
		}

		// Token: 0x06001EBC RID: 7868 RVA: 0x000D6B0E File Offset: 0x000D4D0E
		public T GetVertex(int iVertex)
		{
			return this.positions[iVertex];
		}

		// Token: 0x06001EBD RID: 7869 RVA: 0x000D6B1C File Offset: 0x000D4D1C
		public void GetVertex(int iVertex, out T result)
		{
			result = this.positions[iVertex];
		}

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06001EBE RID: 7870 RVA: 0x000D6B30 File Offset: 0x000D4D30
		public int NumTriangles
		{
			get
			{
				return this.tris.Length;
			}
		}

		// Token: 0x040020FA RID: 8442
		private T[] positions;

		// Token: 0x040020FB RID: 8443
		private BoundingBox[] triBoxes;

		// Token: 0x040020FC RID: 8444
		private TriangleVertexIndices[] tris;

		// Token: 0x040020FD RID: 8445
		private Octree<T>.Node[] nodes;

		// Token: 0x040020FE RID: 8446
		private BoundingBox rootNodeBox;

		// Token: 0x040020FF RID: 8447
		private AABox boundingBox;

		// Token: 0x04002100 RID: 8448
		private ushort[] nodeStack;

		// Token: 0x020003EB RID: 1003
		[Flags]
		internal enum EChild
		{
			// Token: 0x04002102 RID: 8450
			XP = 1,
			// Token: 0x04002103 RID: 8451
			YP = 2,
			// Token: 0x04002104 RID: 8452
			ZP = 4,
			// Token: 0x04002105 RID: 8453
			PPP = 7,
			// Token: 0x04002106 RID: 8454
			PPM = 3,
			// Token: 0x04002107 RID: 8455
			PMP = 5,
			// Token: 0x04002108 RID: 8456
			PMM = 1,
			// Token: 0x04002109 RID: 8457
			MPP = 6,
			// Token: 0x0400210A RID: 8458
			MPM = 2,
			// Token: 0x0400210B RID: 8459
			MMP = 4,
			// Token: 0x0400210C RID: 8460
			MMM = 0
		}

		// Token: 0x020003EC RID: 1004
		public struct Node
		{
			// Token: 0x0400210D RID: 8461
			public ushort[] nodeIndices;

			// Token: 0x0400210E RID: 8462
			public int[] triIndices;

			// Token: 0x0400210F RID: 8463
			public BoundingBox box;
		}

		// Token: 0x020003ED RID: 1005
		private class BuildNode
		{
			// Token: 0x04002110 RID: 8464
			public int childType;

			// Token: 0x04002111 RID: 8465
			public List<int> nodeIndices = new List<int>();

			// Token: 0x04002112 RID: 8466
			public List<int> triIndices = new List<int>();

			// Token: 0x04002113 RID: 8467
			public BoundingBox box;
		}
	}
}
