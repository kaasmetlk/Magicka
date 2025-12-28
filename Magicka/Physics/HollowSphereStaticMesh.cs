using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000595 RID: 1429
	public class HollowSphereStaticMesh : DetectFunctor
	{
		// Token: 0x06002AA8 RID: 10920 RVA: 0x00150756 File Offset: 0x0014E956
		public HollowSphereStaticMesh() : base("HollowSphereStaticMesh", PrimitiveType.NumTypes, PrimitiveType.TriangleMesh)
		{
		}

		// Token: 0x06002AA9 RID: 10921 RVA: 0x00150768 File Offset: 0x0014E968
		public unsafe static void CollDetectSphereStaticMeshOverlap(BoundingSphere oldSphere, BoundingSphere newSphere, TriangleMesh mesh, CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			Vector3 value = (info.Skin0.Owner != null) ? info.Skin0.Owner.OldPosition : Vector3.Zero;
			Vector3 value2 = (info.Skin1.Owner != null) ? info.Skin1.Owner.OldPosition : Vector3.Zero;
			float num = collTolerance + newSphere.Radius + HollowSphere.THICKNESS;
			float num2 = collTolerance + newSphere.Radius - HollowSphere.THICKNESS;
			float num3 = num * num;
			float num4 = num2 * num2;
			SmallCollPointInfo[] array = DetectFunctor.SCPIStackAlloc();
			fixed (SmallCollPointInfo* ptr = array)
			{
				int[] array2 = DetectFunctor.IntStackAlloc();
				fixed (int* ptr2 = array2)
				{
					int num5 = 0;
					Vector3 value3 = Vector3.Zero;
					BoundingBox initialBox = BoundingBoxHelper.InitialBox;
					BoundingBoxHelper.AddSphere(ref newSphere, ref initialBox);
					int trianglesIntersectingtAABox = mesh.GetTrianglesIntersectingtAABox(ptr2, 2048, ref initialBox);
					Vector3 value4 = Vector3.Transform(newSphere.Center, mesh.InverseTransformMatrix);
					Vector3 vector = Vector3.Transform(oldSphere.Center, mesh.InverseTransformMatrix);
					for (int i = 0; i < trianglesIntersectingtAABox; i++)
					{
						IndexedTriangle triangle = mesh.GetTriangle(ptr2[i]);
						float num6 = triangle.Plane.DotCoordinate(value4);
						if (num6 > 0f && num6 < num)
						{
							int iVertex;
							int iVertex2;
							int iVertex3;
							triangle.GetVertexIndices(out iVertex, out iVertex2, out iVertex3);
							Vector3 vector2;
							mesh.GetVertex(iVertex, out vector2);
							Vector3 vector3;
							mesh.GetVertex(iVertex2, out vector3);
							Vector3 vector4;
							mesh.GetVertex(iVertex3, out vector4);
							Triangle triangle2 = new Triangle(ref vector2, ref vector3, ref vector4);
							float t;
							float t2;
							float num7 = Distance.PointTriangleDistanceSq(out t, out t2, ref value4, ref triangle2);
							if (num7 < num3)
							{
								float num8;
								Vector3.DistanceSquared(ref vector2, ref value4, out num8);
								float val;
								Vector3.DistanceSquared(ref vector3, ref value4, out val);
								num8 = Math.Max(num8, val);
								Vector3.DistanceSquared(ref vector3, ref value4, out val);
								num8 = Math.Max(num8, val);
								if (num8 > num4)
								{
									float num9 = Distance.PointTriangleDistanceSq(out t, out t2, ref vector, ref triangle2);
									float num10 = (float)Math.Sqrt((double)num9);
									float initialPenetration = oldSphere.Radius - num10;
									Vector3 vector5;
									triangle2.GetPoint(t, t2, out vector5);
									Vector3.Subtract(ref vector, ref vector5, out vector5);
									JiggleMath.NormalizeSafe(ref vector5);
									Vector3 vector6 = (num10 > float.Epsilon) ? vector5 : triangle2.Normal;
									Vector3 value5 = oldSphere.Center - oldSphere.Radius * vector6;
									if (num5 < 10)
									{
										ptr[(IntPtr)(num5++) * (IntPtr)sizeof(SmallCollPointInfo)] = new SmallCollPointInfo(value5 - value, value5 - value2, initialPenetration);
									}
									value3 += vector6;
								}
							}
						}
					}
					if (num5 > 0)
					{
						JiggleMath.NormalizeSafe(ref value3);
						collisionFunctor.CollisionNotify(ref info, ref value3, ptr, num5);
					}
					DetectFunctor.FreeStackAlloc(array2);
				}
				DetectFunctor.FreeStackAlloc(array);
			}
		}

		// Token: 0x06002AAA RID: 10922 RVA: 0x00150A5C File Offset: 0x0014EC5C
		private void CollDetectOverlap(CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			HollowSphere hollowSphere = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as HollowSphere;
			HollowSphere hollowSphere2 = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as HollowSphere;
			BoundingSphere oldSphere = new BoundingSphere(hollowSphere.Position, hollowSphere.Radius);
			BoundingSphere newSphere = new BoundingSphere(hollowSphere2.Position, hollowSphere2.Radius);
			TriangleMesh mesh = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as TriangleMesh;
			HollowSphereStaticMesh.CollDetectSphereStaticMeshOverlap(oldSphere, newSphere, mesh, info, collTolerance, collisionFunctor);
		}

		// Token: 0x06002AAB RID: 10923 RVA: 0x00150AE8 File Offset: 0x0014ECE8
		public unsafe static void CollDetectSphereStaticMeshSweep(BoundingSphere oldSphere, BoundingSphere newSphere, TriangleMesh mesh, CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			if ((newSphere.Center - oldSphere.Center).LengthSquared() < 0.25f * newSphere.Radius * newSphere.Radius)
			{
				HollowSphereStaticMesh.CollDetectSphereStaticMeshOverlap(oldSphere, newSphere, mesh, info, collTolerance, collisionFunctor);
				return;
			}
			Vector3 value = (info.Skin0.Owner != null) ? info.Skin0.Owner.OldPosition : Vector3.Zero;
			Vector3 value2 = (info.Skin1.Owner != null) ? info.Skin1.Owner.OldPosition : Vector3.Zero;
			float num = collTolerance + oldSphere.Radius;
			float num2 = num * num;
			Vector3 value3 = Vector3.Zero;
			BoundingBox initialBox = BoundingBoxHelper.InitialBox;
			BoundingBoxHelper.AddSphere(ref oldSphere, ref initialBox);
			BoundingBoxHelper.AddSphere(ref newSphere, ref initialBox);
			Vector3 value4 = Vector3.Transform(newSphere.Center, mesh.InverseTransformMatrix);
			Vector3 value5 = Vector3.Transform(oldSphere.Center, mesh.InverseTransformMatrix);
			SmallCollPointInfo[] array = DetectFunctor.SCPIStackAlloc();
			fixed (SmallCollPointInfo* ptr = array)
			{
				int[] array2 = DetectFunctor.IntStackAlloc();
				fixed (int* ptr2 = array2)
				{
					int num3 = 0;
					int trianglesIntersectingtAABox = mesh.GetTrianglesIntersectingtAABox(ptr2, 2048, ref initialBox);
					for (int i = 0; i < trianglesIntersectingtAABox; i++)
					{
						IndexedTriangle triangle = mesh.GetTriangle(ptr2[i]);
						float num4 = triangle.Plane.DotCoordinate(value5);
						if (num4 > 0f)
						{
							float num5 = triangle.Plane.DotCoordinate(value4);
							if (num5 <= num)
							{
								int iVertex;
								int iVertex2;
								int iVertex3;
								triangle.GetVertexIndices(out iVertex, out iVertex2, out iVertex3);
								Vector3 vector;
								mesh.GetVertex(iVertex, out vector);
								Vector3 vector2;
								mesh.GetVertex(iVertex2, out vector2);
								Vector3 vector3;
								mesh.GetVertex(iVertex3, out vector3);
								Triangle triangle2 = new Triangle(ref vector, ref vector2, ref vector3);
								float t;
								float t2;
								float num6 = Distance.PointTriangleDistanceSq(out t, out t2, ref value5, ref triangle2);
								Vector3 vector6;
								Vector3 vector7;
								float initialPenetration2;
								if (num6 < num2)
								{
									float num7 = (float)Math.Sqrt((double)num6);
									float initialPenetration = oldSphere.Radius - num7;
									Vector3 normal = triangle2.Normal;
									Vector3 vector4;
									triangle2.GetPoint(t, t2, out vector4);
									Vector3.Subtract(ref value5, ref vector4, out vector4);
									JiggleMath.NormalizeSafe(ref vector4);
									Vector3 vector5 = (num7 > float.Epsilon) ? vector4 : normal;
									Vector3 value6 = oldSphere.Center - oldSphere.Radius * vector5;
									if (num3 < 10)
									{
										ptr[(IntPtr)(num3++) * (IntPtr)sizeof(SmallCollPointInfo)] = new SmallCollPointInfo(value6 - value, value6 - value2, initialPenetration);
									}
									value3 += vector5;
								}
								else if (num5 < num4 && Intersection.SweptSphereTriangleIntersection(out vector6, out vector7, out initialPenetration2, oldSphere, newSphere, triangle2, num4, num5, Intersection.EdgesToTest.EdgeAll, Intersection.CornersToTest.CornerAll))
								{
									float num8 = (float)Math.Sqrt((double)num6);
									Vector3 normal2 = triangle2.Normal;
									Vector3 vector8;
									triangle2.GetPoint(t, t2, out vector8);
									Vector3.Subtract(ref value5, ref vector8, out vector8);
									JiggleMath.NormalizeSafe(ref vector8);
									Vector3 vector9 = (num8 > 1E-06f) ? vector8 : normal2;
									Vector3 value7 = oldSphere.Center - oldSphere.Radius * vector9;
									if (num3 < 10)
									{
										ptr[(IntPtr)(num3++) * (IntPtr)sizeof(SmallCollPointInfo)] = new SmallCollPointInfo(value7 - value, value7 - value2, initialPenetration2);
									}
									value3 += vector9;
								}
							}
						}
					}
					if (num3 > 0)
					{
						JiggleMath.NormalizeSafe(ref value3);
						collisionFunctor.CollisionNotify(ref info, ref value3, ptr, num3);
					}
				}
				DetectFunctor.FreeStackAlloc(array2);
			}
			DetectFunctor.FreeStackAlloc(array);
		}

		// Token: 0x06002AAC RID: 10924 RVA: 0x00150E98 File Offset: 0x0014F098
		private void CollDetectSweep(CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			HollowSphere hollowSphere = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as HollowSphere;
			HollowSphere hollowSphere2 = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as HollowSphere;
			BoundingSphere oldSphere = new BoundingSphere(hollowSphere.Position, hollowSphere.Radius);
			BoundingSphere newSphere = new BoundingSphere(hollowSphere2.Position, hollowSphere2.Radius);
			TriangleMesh mesh = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as TriangleMesh;
			HollowSphereStaticMesh.CollDetectSphereStaticMeshSweep(oldSphere, newSphere, mesh, info, collTolerance, collisionFunctor);
		}

		// Token: 0x06002AAD RID: 10925 RVA: 0x00150F24 File Offset: 0x0014F124
		public override void CollDetect(CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			if (info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0).Type == base.Type1)
			{
				CollisionSkin skin = info.Skin0;
				info.Skin0 = info.Skin1;
				info.Skin1 = skin;
				int indexPrim = info.IndexPrim0;
				info.IndexPrim0 = info.IndexPrim1;
				info.IndexPrim1 = indexPrim;
			}
			if (info.Skin0.CollisionSystem != null && info.Skin0.CollisionSystem.UseSweepTests)
			{
				this.CollDetectSweep(info, collTolerance, collisionFunctor);
				return;
			}
			this.CollDetectOverlap(info, collTolerance, collisionFunctor);
		}
	}
}
