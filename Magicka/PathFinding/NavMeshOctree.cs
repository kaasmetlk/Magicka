using System;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Magicka.PathFinding
{
	// Token: 0x02000096 RID: 150
	internal class NavMeshOctree : Octree
	{
		// Token: 0x06000477 RID: 1143 RVA: 0x00016111 File Offset: 0x00014311
		public NavMeshOctree(Vector3[] iPositions, TriangleVertexIndices[] iTris, MovementProperties[] iTriangleProperties) : base(iPositions, iTris)
		{
			this.mProperties = iTriangleProperties;
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00016122 File Offset: 0x00014322
		public float FindClosestPoint(ref Vector3 iPoint, out Vector3 oPoint, out int oTriangle, MovementProperties iProperties)
		{
			return this.FindClosestPoint(ref iPoint, out oPoint, out oTriangle, iProperties, float.MaxValue);
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00016134 File Offset: 0x00014334
		public float FindClosestPoint(ref Vector3 iPoint, out Vector3 oPoint, out int oTriangle, MovementProperties iProperties, float iStartDist)
		{
			oPoint = default(Vector3);
			oTriangle = -1;
			if (this.nodes.Length == 0)
			{
				return float.PositiveInfinity;
			}
			int i = 0;
			int num = 1;
			this.nodeStack[0] = 0;
			float num2 = iStartDist;
			while (i < num)
			{
				ushort num3 = this.nodeStack[i];
				Octree.Node node = this.nodes[(int)num3];
				i++;
				for (int j = 0; j < node.triIndices.Length; j++)
				{
					int num4 = node.triIndices[j];
					if (!(this.mProperties[num4] != MovementProperties.Default & (this.mProperties[num4] & iProperties) == MovementProperties.Default))
					{
						TriangleVertexIndices triangleVertexIndices = this.tris[num4];
						Vector3 vector = this.positions[triangleVertexIndices.I0];
						Vector3 vector2 = this.positions[triangleVertexIndices.I1];
						Vector3 vector3 = this.positions[triangleVertexIndices.I2];
						Triangle triangle = new Triangle(ref vector, ref vector2, ref vector3);
						float t;
						float t2;
						float num5 = Distance.PointTriangleDistanceSq(out t, out t2, ref iPoint, ref triangle);
						if (num5 < num2)
						{
							num2 = num5;
							oTriangle = num4;
							triangle.GetPoint(t, t2, out oPoint);
						}
					}
				}
				int num6 = node.nodeIndices.Length;
				for (int k = 0; k < num6; k++)
				{
					float num8;
					float num9;
					float num10;
					float num7 = Distance.SqrDistance(ref iPoint, ref this.nodes[(int)node.nodeIndices[k]].box, out num8, out num9, out num10);
					if (num7 < num2)
					{
						this.nodeStack[num++] = node.nodeIndices[k];
					}
				}
			}
			return num2;
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x000162DC File Offset: 0x000144DC
		public unsafe int GetDynamicTrianglesIntersectingtAABox(int* triangles, int maxTriangles, ref BoundingBox testBox)
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
				Octree.Node node = this.nodes[(int)num3];
				i++;
				ContainmentType containmentType;
				testBox.Contains(ref node.box, out containmentType);
				if (containmentType == ContainmentType.Intersects)
				{
					int num4 = node.triIndices.Length;
					for (int j = 0; j < num4; j++)
					{
						testBox.Contains(ref this.triBoxes[node.triIndices[j]], out containmentType);
						if (containmentType != ContainmentType.Disjoint && (num2 < maxTriangles & (this.mProperties[node.triIndices[j]] & MovementProperties.Dynamic) == MovementProperties.Dynamic))
						{
							triangles[num2++] = node.triIndices[j];
						}
					}
					int num5 = node.nodeIndices.Length;
					for (int k = 0; k < num5; k++)
					{
						this.nodeStack[num++] = node.nodeIndices[k];
					}
				}
				else if (containmentType == ContainmentType.Contains)
				{
					int num6 = node.triIndices.Length;
					for (int l = 0; l < num6; l++)
					{
						if (num2 < maxTriangles & (this.mProperties[node.triIndices[l]] & MovementProperties.Dynamic) == MovementProperties.Dynamic)
						{
							triangles[num2++] = node.triIndices[l];
						}
					}
					int num7 = node.nodeIndices.Length;
					for (int m = 0; m < num7; m++)
					{
						this.nodeStack[num++] = node.nodeIndices[m];
					}
				}
			}
			return num2;
		}

		// Token: 0x040002F3 RID: 755
		private MovementProperties[] mProperties;
	}
}
