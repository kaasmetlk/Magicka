using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000623 RID: 1571
	internal class CustomCapsuleStaticMesh : DetectFunctor
	{
		// Token: 0x06002F1D RID: 12061 RVA: 0x0017E2A8 File Offset: 0x0017C4A8
		public CustomCapsuleStaticMesh() : base("CustomCapsuleStaticMesh", PrimitiveType.Capsule, PrimitiveType.TriangleMesh)
		{
		}

		// Token: 0x06002F1E RID: 12062 RVA: 0x0017E2B8 File Offset: 0x0017C4B8
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

		// Token: 0x06002F1F RID: 12063 RVA: 0x0017E358 File Offset: 0x0017C558
		private unsafe void CollDetectCapsuleStaticMeshOverlap(Capsule oldCapsule, Capsule newCapsule, TriangleMesh mesh, CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			Vector3 value = (info.Skin0.Owner != null) ? info.Skin0.Owner.OldPosition : Vector3.Zero;
			Vector3 value2 = (info.Skin1.Owner != null) ? info.Skin1.Owner.OldPosition : Vector3.Zero;
			float num = collTolerance + newCapsule.Radius;
			float num2 = num * num;
			BoundingBox initialBox = BoundingBoxHelper.InitialBox;
			BoundingBoxHelper.AddCapsule(newCapsule, ref initialBox);
			SmallCollPointInfo[] array = DetectFunctor.SCPIStackAlloc();
			fixed (SmallCollPointInfo* ptr = array)
			{
				int[] array2 = DetectFunctor.IntStackAlloc();
				fixed (int* ptr2 = array2)
				{
					int num3 = 0;
					int trianglesIntersectingtAABox = mesh.GetTrianglesIntersectingtAABox(ptr2, 2048, ref initialBox);
					Vector3 position = newCapsule.Position;
					Vector3 end = newCapsule.GetEnd();
					Matrix inverseTransformMatrix = mesh.InverseTransformMatrix;
					Vector3 value3 = Vector3.Transform(position, inverseTransformMatrix);
					Vector3 value4 = Vector3.Transform(end, inverseTransformMatrix);
					Vector3 position2 = oldCapsule.Position;
					Vector3 end2 = oldCapsule.GetEnd();
					float num4 = Math.Min(oldCapsule.Position.Y, oldCapsule.GetEnd().Y) - oldCapsule.Radius;
					float num5 = Math.Min(newCapsule.Position.Y, newCapsule.GetEnd().Y) - newCapsule.Radius;
					for (int i = 0; i < trianglesIntersectingtAABox; i++)
					{
						IndexedTriangle triangle = mesh.GetTriangle(ptr2[i]);
						float num6 = triangle.Plane.DotCoordinate(value3);
						float num7 = triangle.Plane.DotCoordinate(value4);
						if ((num6 <= num || num7 <= num) && (num6 >= -num || num7 >= -num))
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
							Matrix transformMatrix = mesh.TransformMatrix;
							Vector3.Transform(ref vector, ref transformMatrix, out vector);
							Vector3.Transform(ref vector2, ref transformMatrix, out vector2);
							Vector3.Transform(ref vector3, ref transformMatrix, out vector3);
							Triangle triangle2 = new Triangle(ref vector, ref vector2, ref vector3);
							Segment segment;
							segment.Origin = position;
							Vector3.Subtract(ref end, ref position, out segment.Delta);
							float t;
							float t2;
							float t3;
							float num8 = Distance.SegmentTriangleDistanceSq(out t, out t2, out t3, ref segment, ref triangle2);
							if (num8 < num2)
							{
								Segment segment2 = new Segment(position2, end2 - position2);
								num8 = Distance.SegmentTriangleDistanceSq(out t, out t2, out t3, ref segment2, ref triangle2);
								float num9 = (float)Math.Sqrt((double)num8);
								float initialPenetration = oldCapsule.Radius - num9;
								Vector3 value5;
								triangle2.GetPoint(t2, t3, out value5);
								Vector3 normal;
								if (num8 > 1E-06f)
								{
									segment2.GetPoint(t, out normal);
									Vector3.Subtract(ref normal, ref value5, out normal);
									JiggleMath.NormalizeSafe(ref normal);
								}
								else
								{
									normal = triangle2.Normal;
								}
								Vector3 vector4;
								if (info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) is Capsule)
								{
									vector4 = info.Skin0.Owner.Velocity;
								}
								else if (info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) is Capsule)
								{
									vector4 = info.Skin1.Owner.Velocity;
								}
								else
								{
									vector4 = default(Vector3);
								}
								Vector3 vector5 = vector4;
								JiggleMath.NormalizeSafe(ref vector5);
								if (Vector3.Dot(triangle2.Normal, vector5) <= 0.7f)
								{
									float num10 = Math.Max(Math.Max(vector.Y, vector2.Y), vector3.Y);
									float num11 = Math.Min(Math.Min(vector.Y, vector2.Y), vector3.Y);
									bool flag = num10 - num5 <= 0.51f;
									bool flag2 = num10 - num4 <= 0.51f && triangle2.Normal.Y > 0.7f;
									bool flag3 = num11 - num5 <= 0.51f && triangle2.Normal.Y > 0.7f;
									if (flag || flag2 || flag3)
									{
										Vector3 origin;
										segment2.GetPoint(t, out origin);
										origin.Y -= oldCapsule.Radius;
										float scaleFactor = Math.Abs(position2.Y - end2.Y) + 2f * oldCapsule.Radius;
										Segment segment3 = new Segment(origin, Vector3.Up * scaleFactor);
										float t4;
										float t5;
										float t6;
										bool flag4 = Intersection.SegmentTriangleIntersection(out t4, out t5, out t6, ref segment3, ref triangle2, false);
										if (flag4)
										{
											Vector3 vector6;
											segment3.GetPoint(t4, out vector6);
											Vector3 vector7;
											triangle2.GetPoint(t5, t6, out vector7);
											float num12 = vector6.Y - origin.Y;
											initialPenetration = num12;
										}
										normal.X = 0f;
										normal.Z = 0f;
										JiggleMath.NormalizeSafe(ref normal);
									}
									else if (triangle2.Normal.Y < 0f)
									{
										normal.Y = 0f;
										JiggleMath.NormalizeSafe(ref normal);
									}
									else
									{
										flag2 = (num10 - num4 <= 0.51f && triangle2.Normal.Y < 0.3f);
										flag3 = (num11 - num5 <= 0.51f && triangle2.Normal.Y < 0.3f);
										if (!flag && !flag2 && !flag3)
										{
											normal.Y = 0f;
											JiggleMath.NormalizeSafe(ref normal);
										}
									}
									if (num3 < 10)
									{
										SmallCollPointInfo smallCollPointInfo;
										smallCollPointInfo.R0 = value5 - value;
										smallCollPointInfo.R1 = value5 - value2;
										smallCollPointInfo.InitialPenetration = initialPenetration;
										*ptr = smallCollPointInfo;
									}
									if (normal.Y < 0f)
									{
										normal.Y *= -1f;
									}
									collisionFunctor.CollisionNotify(ref info, ref normal, ptr, 1);
								}
							}
						}
					}
				}
				DetectFunctor.FreeStackAlloc(array2);
			}
			DetectFunctor.FreeStackAlloc(array);
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x0017E938 File Offset: 0x0017CB38
		private void CollDetectOverlap(CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			Capsule oldCapsule = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as Capsule;
			Capsule newCapsule = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as Capsule;
			TriangleMesh mesh = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as TriangleMesh;
			this.CollDetectCapsuleStaticMeshOverlap(oldCapsule, newCapsule, mesh, info, collTolerance, collisionFunctor);
		}

		// Token: 0x06002F21 RID: 12065 RVA: 0x0017E99C File Offset: 0x0017CB9C
		private void CollDetectCapsulseStaticMeshSweep(Capsule oldCapsule, Capsule newCapsule, TriangleMesh mesh, CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			if ((newCapsule.Position - oldCapsule.Position).LengthSquared() < 0.25f * newCapsule.Radius * newCapsule.Radius)
			{
				this.CollDetectCapsuleStaticMeshOverlap(oldCapsule, newCapsule, mesh, info, collTolerance, collisionFunctor);
				return;
			}
			float length = oldCapsule.Length;
			float radius = oldCapsule.Radius;
			int num = 2 + (int)(length / (2f * oldCapsule.Radius));
			for (int i = 0; i < num; i++)
			{
				float scaleFactor = (float)i * length / ((float)num - 1f);
				BoundingSphere oldSphere = new BoundingSphere(oldCapsule.Position + oldCapsule.Orientation.Backward * scaleFactor, radius);
				BoundingSphere newSphere = new BoundingSphere(newCapsule.Position + newCapsule.Orientation.Backward * scaleFactor, radius);
				CollDetectSphereStaticMesh.CollDetectSphereStaticMeshSweep(oldSphere, newSphere, mesh, info, collTolerance, collisionFunctor);
			}
		}

		// Token: 0x06002F22 RID: 12066 RVA: 0x0017EA8C File Offset: 0x0017CC8C
		private void CollDetectSweep(CollDetectInfo info, float collTolerance, ICollisionFunctor collisionFunctor)
		{
			Capsule oldCapsule = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as Capsule;
			Capsule newCapsule = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as Capsule;
			TriangleMesh mesh = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as TriangleMesh;
			this.CollDetectCapsulseStaticMeshSweep(oldCapsule, newCapsule, mesh, info, collTolerance, collisionFunctor);
		}

		// Token: 0x0400334A RID: 13130
		public const float STEPSIZE = 0.51f;
	}
}
