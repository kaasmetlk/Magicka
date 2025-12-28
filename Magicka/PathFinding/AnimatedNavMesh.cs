using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.PathFinding
{
	// Token: 0x02000280 RID: 640
	internal class AnimatedNavMesh
	{
		// Token: 0x060012E5 RID: 4837 RVA: 0x000750E4 File Offset: 0x000732E4
		public AnimatedNavMesh(ContentReader iInput)
		{
			this.mVertices = new Vector3[(int)iInput.ReadUInt16()];
			for (int i = 0; i < this.mVertices.Length; i++)
			{
				this.mVertices[i] = iInput.ReadVector3();
			}
			this.mTriangles = new Triangle[(int)iInput.ReadUInt16()];
			for (int j = 0; j < this.mTriangles.Length; j++)
			{
				this.mTriangles[j] = new Triangle(iInput);
			}
			TriangleVertexIndices[] array = new TriangleVertexIndices[this.mTriangles.Length];
			MovementProperties[] array2 = new MovementProperties[this.mTriangles.Length];
			for (int k = 0; k < this.mTriangles.Length; k++)
			{
				array[k].I0 = (int)this.mTriangles[k].VertexA;
				array[k].I1 = (int)this.mTriangles[k].VertexB;
				array[k].I2 = (int)this.mTriangles[k].VertexC;
				if ((this.mTriangles[k].Properties & MovementProperties.Dynamic) == MovementProperties.Dynamic)
				{
					this.mDynamicNeighbours.Add((ushort)k, new List<NeighbourInfo>(10));
				}
				array2[k] = this.mTriangles[k].Properties;
			}
			this.mMesh = new NavMeshOctree(this.mVertices, array, array2);
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x00075288 File Offset: 0x00073488
		public void GetNearestPosition(ref Vector3 iPoint, out Vector3 oPoint, MovementProperties iProperties)
		{
			int num;
			this.mMesh.FindClosestPoint(ref iPoint, out oPoint, out num, iProperties);
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x000752A6 File Offset: 0x000734A6
		public void UpdateTransform(ref Matrix iTransform)
		{
			this.mDirty = true;
			this.mTransform = iTransform;
			Matrix.Invert(ref iTransform, out this.mInvTransform);
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x000752C8 File Offset: 0x000734C8
		internal void ClearNeighbours()
		{
			foreach (List<NeighbourInfo> list in this.mDynamicNeighbours.Values)
			{
				list.Clear();
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x060012E9 RID: 4841 RVA: 0x00075320 File Offset: 0x00073520
		internal NavMeshOctree Mesh
		{
			get
			{
				return this.mMesh;
			}
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x060012EA RID: 4842 RVA: 0x00075328 File Offset: 0x00073528
		internal Dictionary<ushort, List<NeighbourInfo>> DynamicNeighbours
		{
			get
			{
				return this.mDynamicNeighbours;
			}
		}

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x060012EB RID: 4843 RVA: 0x00075330 File Offset: 0x00073530
		public Triangle[] Triangles
		{
			get
			{
				return this.mTriangles;
			}
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x060012EC RID: 4844 RVA: 0x00075338 File Offset: 0x00073538
		public Vector3[] Vertices
		{
			get
			{
				return this.mVertices;
			}
		}

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x060012ED RID: 4845 RVA: 0x00075340 File Offset: 0x00073540
		public bool Dirty
		{
			get
			{
				return this.mDirty;
			}
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x00075348 File Offset: 0x00073548
		internal void FindNeighbours(ushort iThisId, List<AnimatedNavMesh> iMeshes)
		{
			this.mDirty = false;
			Vector3 vector = default(Vector3);
			vector.X = (vector.Y = (vector.Z = 0.25f));
			foreach (KeyValuePair<ushort, List<NeighbourInfo>> keyValuePair in this.mDynamicNeighbours)
			{
				Vector3 vector2 = this.mVertices[(int)this.mTriangles[(int)keyValuePair.Key].VertexA];
				Vector3 vector3 = this.mVertices[(int)this.mTriangles[(int)keyValuePair.Key].VertexB];
				Vector3 vector4 = this.mVertices[(int)this.mTriangles[(int)keyValuePair.Key].VertexC];
				Vector3.Transform(ref vector2, ref this.mTransform, out vector2);
				Vector3.Transform(ref vector3, ref this.mTransform, out vector3);
				Vector3.Transform(ref vector4, ref this.mTransform, out vector4);
				for (int i = (int)(iThisId + 1); i < iMeshes.Count; i++)
				{
					AnimatedNavMesh animatedNavMesh = iMeshes[i];
					Vector3 vector5;
					Vector3.Transform(ref vector2, ref animatedNavMesh.mInvTransform, out vector5);
					Vector3 vector6;
					Vector3.Transform(ref vector3, ref animatedNavMesh.mInvTransform, out vector6);
					Vector3 vector7;
					Vector3.Transform(ref vector4, ref animatedNavMesh.mInvTransform, out vector7);
					this.FindNeighbours(keyValuePair.Key, ref vector5, ref vector6, ref vector7, keyValuePair.Value, animatedNavMesh, iThisId, (ushort)i);
				}
			}
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x000754F4 File Offset: 0x000736F4
		private unsafe void FindNeighbours(ushort iTri, ref Vector3 iA, ref Vector3 iB, ref Vector3 iC, List<NeighbourInfo> iNeighbours, AnimatedNavMesh iMesh, ushort iThisId, ushort iMeshID)
		{
			BoundingBox boundingBox;
			Vector3.Max(ref iA, ref iB, out boundingBox.Max);
			Vector3.Max(ref iC, ref boundingBox.Max, out boundingBox.Max);
			Vector3.Add(ref boundingBox.Max, ref AnimatedNavMesh.BBBias, out boundingBox.Max);
			Vector3.Min(ref iA, ref iB, out boundingBox.Min);
			Vector3.Min(ref iC, ref boundingBox.Min, out boundingBox.Min);
			Vector3.Subtract(ref boundingBox.Min, ref AnimatedNavMesh.BBBias, out boundingBox.Min);
			Triangle triangle = new Triangle(ref iA, ref iB, ref iC);
			int[] array = DetectFunctor.IntStackAlloc();
			int dynamicTrianglesIntersectingtAABox;
			fixed (int* ptr = array)
			{
				dynamicTrianglesIntersectingtAABox = iMesh.Mesh.GetDynamicTrianglesIntersectingtAABox(ptr, 2048, ref boundingBox);
			}
			Vector3 vector;
			Vector3.Add(ref iA, ref iB, out vector);
			Vector3.Add(ref iC, ref vector, out vector);
			Vector3.Multiply(ref vector, 0.33333334f, out vector);
			for (int i = 0; i < dynamicTrianglesIntersectingtAABox; i++)
			{
				int iVertex;
				int iVertex2;
				int iVertex3;
				iMesh.Mesh.GetTriangle(array[i]).GetVertexIndices(out iVertex, out iVertex2, out iVertex3);
				Vector3 vector2;
				iMesh.Mesh.GetVertex(iVertex, out vector2);
				Vector3 vector3;
				iMesh.Mesh.GetVertex(iVertex2, out vector3);
				Vector3 vector4;
				iMesh.Mesh.GetVertex(iVertex3, out vector4);
				Triangle triangle2 = new Triangle(ref vector2, ref vector3, ref vector4);
				Vector3 vector5;
				Vector3.Add(ref vector2, ref vector3, out vector5);
				Vector3.Add(ref vector4, ref vector5, out vector5);
				Vector3.Multiply(ref vector5, 0.33333334f, out vector5);
				float num = Distance.TriangleTriangleDistanceSq(ref triangle2, ref triangle);
				if (num <= 0.0625f)
				{
					NeighbourInfo item;
					item.Mesh = iMeshID;
					item.Triangle = (ushort)array[i];
					Vector3.Distance(ref vector, ref vector5, out item.Cost);
					Vector3.Add(ref vector2, ref vector3, out item.EdgePos);
					Vector3.Add(ref vector4, ref item.EdgePos, out item.EdgePos);
					Vector3.Multiply(ref item.EdgePos, 0.33333334f, out item.EdgePos);
					Vector3.Transform(ref item.EdgePos, ref this.mTransform, out item.EdgePos);
					iNeighbours.Add(item);
					item.Triangle = iTri;
					item.Mesh = iThisId;
					iMesh.DynamicNeighbours[(ushort)array[i]].Add(item);
				}
			}
			DetectFunctor.FreeStackAlloc(array);
		}

		// Token: 0x040014B5 RID: 5301
		public const float MAXDISTANCE = 0.25f;

		// Token: 0x040014B6 RID: 5302
		public const float MAXDISTANCESQ = 0.0625f;

		// Token: 0x040014B7 RID: 5303
		private static Vector3 BBBias = new Vector3(0.25f);

		// Token: 0x040014B8 RID: 5304
		internal Matrix mTransform = Matrix.Identity;

		// Token: 0x040014B9 RID: 5305
		internal Matrix mInvTransform = Matrix.Identity;

		// Token: 0x040014BA RID: 5306
		private Dictionary<ushort, List<NeighbourInfo>> mDynamicNeighbours = new Dictionary<ushort, List<NeighbourInfo>>();

		// Token: 0x040014BB RID: 5307
		private Vector3[] mVertices;

		// Token: 0x040014BC RID: 5308
		private Triangle[] mTriangles;

		// Token: 0x040014BD RID: 5309
		private NavMeshOctree mMesh;

		// Token: 0x040014BE RID: 5310
		private bool mDirty = true;
	}
}
