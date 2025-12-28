using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000372 RID: 882
	public class WaterMesh : TriangleMesh
	{
		// Token: 0x06001AE4 RID: 6884 RVA: 0x000B6178 File Offset: 0x000B4378
		public void CreateMesh(Vector3[] vertices, float[] frozenVertices, TriangleVertexIndices[] triangleVertexIndices, int maxTrianglesPerCell, float minCellSize)
		{
			this.octree.Clear(true);
			this.octree.AddTriangles(vertices, triangleVertexIndices);
			this.octree.BuildOctree(maxTrianglesPerCell, minCellSize);
			this.frozenVertices = frozenVertices;
			this.maxTrianglesPerCell = maxTrianglesPerCell;
			this.minCellSize = minCellSize;
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x000B61C4 File Offset: 0x000B43C4
		public unsafe int GetAllTrianglesIntersectingtAABox(int* triangles, int maxTriangles, ref BoundingBox bb)
		{
			return base.GetTrianglesIntersectingtAABox(triangles, maxTriangles, ref bb);
		}

		// Token: 0x06001AE6 RID: 6886 RVA: 0x000B61D0 File Offset: 0x000B43D0
		public unsafe override int GetTrianglesIntersectingtAABox(int* triangles, int maxTriangles, ref BoundingBox bb)
		{
			int num = base.GetTrianglesIntersectingtAABox(triangles, maxTriangles, ref bb);
			for (int i = 0; i < num; i++)
			{
				int num2;
				int num3;
				int num4;
				this.octree.GetTriangle(triangles[i]).GetVertexIndices(out num2, out num3, out num4);
				if (this.frozenVertices[num2] >= 0.5f || this.frozenVertices[num3] >= 0.5f || this.frozenVertices[num4] >= 0.5f)
				{
					for (int j = i + 1; j < num; j++)
					{
						triangles[j - 1] = triangles[j];
					}
					num--;
					i--;
				}
			}
			return num;
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x000B6274 File Offset: 0x000B4474
		public override Primitive Clone()
		{
			WaterMesh waterMesh = new WaterMesh();
			waterMesh.octree = this.octree;
			waterMesh.SetTransform(ref this.transform);
			waterMesh.frozenVertices = this.frozenVertices;
			return waterMesh;
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x000B62AC File Offset: 0x000B44AC
		public IceMesh CloneToIceMesh()
		{
			IceMesh iceMesh = new IceMesh();
			iceMesh.SetOctree(this.octree);
			iceMesh.SetTransform(ref this.transform);
			iceMesh.frozenVertices = this.frozenVertices;
			return iceMesh;
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x000B62E4 File Offset: 0x000B44E4
		public unsafe int GetVerticesIntersectingArc(int* iVertices, int iMaxVertices, ref Vector3 iOrigin, ref Vector3 iDirection, float iSpread)
		{
			iOrigin.Y = 0f;
			iDirection.Y = 0f;
			float num = iDirection.Length();
			Vector3 vector;
			Vector3.Divide(ref iDirection, num, out vector);
			BoundingBox initialBox = BoundingBoxHelper.InitialBox;
			initialBox.Min.Y = -10000f;
			initialBox.Max.Y = 10000f;
			BoundingBoxHelper.AddPoint(ref iOrigin, ref initialBox);
			Vector3 vector2 = default(Vector3);
			vector2.X = -1f;
			if (MagickaMath.Angle(ref vector, ref vector2) <= iSpread)
			{
				vector2 = iOrigin;
				vector2.X -= num;
				BoundingBoxHelper.AddPoint(ref vector2, ref initialBox);
			}
			vector2 = default(Vector3);
			vector2.X = 1f;
			if (MagickaMath.Angle(ref vector, ref vector2) <= iSpread)
			{
				vector2 = iOrigin;
				vector2.X += num;
				BoundingBoxHelper.AddPoint(ref vector2, ref initialBox);
			}
			vector2.Z = -1f;
			if (MagickaMath.Angle(ref vector, ref vector2) <= iSpread)
			{
				vector2 = iOrigin;
				vector2.Z -= num;
				BoundingBoxHelper.AddPoint(ref vector2, ref initialBox);
			}
			vector2 = default(Vector3);
			vector2.Z = 1f;
			if (MagickaMath.Angle(ref vector, ref vector2) <= iSpread)
			{
				vector2 = iOrigin;
				vector2.Z += num;
				BoundingBoxHelper.AddPoint(ref vector2, ref initialBox);
			}
			Quaternion quaternion;
			Quaternion.CreateFromYawPitchRoll(iSpread, 0f, 0f, out quaternion);
			Vector3.Transform(ref iDirection, ref quaternion, out vector2);
			Vector3.Add(ref iOrigin, ref vector2, out vector2);
			BoundingBoxHelper.AddPoint(ref vector2, ref initialBox);
			Quaternion.CreateFromYawPitchRoll(-iSpread, 0f, 0f, out quaternion);
			Vector3.Transform(ref iDirection, ref quaternion, out vector2);
			Vector3.Add(ref iOrigin, ref vector2, out vector2);
			BoundingBoxHelper.AddPoint(ref vector2, ref initialBox);
			float num2 = num * num;
			int num3 = 0;
			int[] array = DetectFunctor.IntStackAlloc();
			fixed (int* ptr = array)
			{
				int trianglesIntersectingtAABox = base.GetTrianglesIntersectingtAABox(ptr, 2048, ref initialBox);
				for (int i = 0; i < trianglesIntersectingtAABox; i++)
				{
					IndexedTriangle triangle = base.GetTriangle(ptr[i]);
					for (int j = 0; j < 3; j++)
					{
						int vertexIndex = triangle.GetVertexIndex(j);
						Vector3 vertex = this.octree.GetVertex(vertexIndex);
						vertex.Y = 0f;
						float num4;
						Vector3.DistanceSquared(ref iOrigin, ref vertex, out num4);
						if (num4 <= num2)
						{
							bool flag = false;
							for (int k = 0; k < num3; k++)
							{
								if (iVertices[k] == vertexIndex)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								num4 = (float)Math.Sqrt((double)num4);
								Vector3.Subtract(ref vertex, ref iOrigin, out vertex);
								Vector3.Divide(ref vertex, num4, out vertex);
								if (MagickaMath.Angle(ref vector, ref vertex) <= iSpread)
								{
									iVertices[num3++] = vertexIndex;
								}
							}
						}
						if (num3 == iMaxVertices)
						{
							break;
						}
					}
				}
			}
			DetectFunctor.FreeStackAlloc(array);
			return num3;
		}

		// Token: 0x04001D00 RID: 7424
		internal float[] frozenVertices;
	}
}
