// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.CustomCapsuleStaticMesh
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Physics;

internal class CustomCapsuleStaticMesh : DetectFunctor
{
  public const float STEPSIZE = 0.51f;

  public CustomCapsuleStaticMesh()
    : base(nameof (CustomCapsuleStaticMesh), PrimitiveType.Capsule, PrimitiveType.TriangleMesh)
  {
  }

  public override void CollDetect(
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    if (info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0).Type == this.Type1)
    {
      CollisionSkin skin0 = info.Skin0;
      info.Skin0 = info.Skin1;
      info.Skin1 = skin0;
      int indexPrim0 = info.IndexPrim0;
      info.IndexPrim0 = info.IndexPrim1;
      info.IndexPrim1 = indexPrim0;
    }
    if (info.Skin0.CollisionSystem != null && info.Skin0.CollisionSystem.UseSweepTests)
      this.CollDetectSweep(info, collTolerance, collisionFunctor);
    else
      this.CollDetectOverlap(info, collTolerance, collisionFunctor);
  }

  private unsafe void CollDetectCapsuleStaticMeshOverlap(
    Capsule oldCapsule,
    Capsule newCapsule,
    TriangleMesh mesh,
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    Vector3 vector3_1 = info.Skin0.Owner != null ? info.Skin0.Owner.OldPosition : Vector3.Zero;
    Vector3 vector3_2 = info.Skin1.Owner != null ? info.Skin1.Owner.OldPosition : Vector3.Zero;
    float num1 = collTolerance + newCapsule.Radius;
    float num2 = num1 * num1;
    BoundingBox initialBox = BoundingBoxHelper.InitialBox;
    BoundingBoxHelper.AddCapsule(newCapsule, ref initialBox);
    SmallCollPointInfo[] alloced1 = DetectFunctor.SCPIStackAlloc();
    fixed (SmallCollPointInfo* pointInfos = alloced1)
    {
      int[] alloced2 = DetectFunctor.IntStackAlloc();
      fixed (int* triangles = alloced2)
      {
        int num3 = 0;
        int intersectingtAaBox = mesh.GetTrianglesIntersectingtAABox(triangles, 2048 /*0x0800*/, ref initialBox);
        Vector3 position1 = newCapsule.Position;
        Vector3 end1 = newCapsule.GetEnd();
        Matrix inverseTransformMatrix = mesh.InverseTransformMatrix;
        Vector3 vector3_3 = Vector3.Transform(position1, inverseTransformMatrix);
        Vector3 vector3_4 = Vector3.Transform(end1, inverseTransformMatrix);
        Vector3 position2 = oldCapsule.Position;
        Vector3 end2 = oldCapsule.GetEnd();
        float num4 = System.Math.Min(oldCapsule.Position.Y, oldCapsule.GetEnd().Y) - oldCapsule.Radius;
        float num5 = System.Math.Min(newCapsule.Position.Y, newCapsule.GetEnd().Y) - newCapsule.Radius;
        for (int index = 0; index < intersectingtAaBox; ++index)
        {
          IndexedTriangle triangle1 = mesh.GetTriangle(triangles[index]);
          Microsoft.Xna.Framework.Plane plane = triangle1.Plane;
          float num6 = plane.DotCoordinate(vector3_3);
          plane = triangle1.Plane;
          float num7 = plane.DotCoordinate(vector3_4);
          if (((double) num6 <= (double) num1 || (double) num7 <= (double) num1) && ((double) num6 >= -(double) num1 || (double) num7 >= -(double) num1))
          {
            int i0;
            int i1;
            int i2;
            triangle1.GetVertexIndices(out i0, out i1, out i2);
            Vector3 result1;
            mesh.GetVertex(i0, out result1);
            Vector3 result2;
            mesh.GetVertex(i1, out result2);
            Vector3 result3;
            mesh.GetVertex(i2, out result3);
            Matrix transformMatrix = mesh.TransformMatrix;
            Vector3.Transform(ref result1, ref transformMatrix, out result1);
            Vector3.Transform(ref result2, ref transformMatrix, out result2);
            Vector3.Transform(ref result3, ref transformMatrix, out result3);
            Triangle triangle2 = new Triangle(ref result1, ref result2, ref result3);
            Segment seg1;
            seg1.Origin = position1;
            Vector3.Subtract(ref end1, ref position1, out seg1.Delta);
            float segT;
            float triT0;
            float triT1;
            if ((double) Distance.SegmentTriangleDistanceSq(out segT, out triT0, out triT1, ref seg1, ref triangle2) < (double) num2)
            {
              Segment seg2 = new Segment(position2, end2 - position2);
              float d = Distance.SegmentTriangleDistanceSq(out segT, out triT0, out triT1, ref seg2, ref triangle2);
              float num8 = (float) System.Math.Sqrt((double) d);
              float num9 = oldCapsule.Radius - num8;
              Vector3 point1;
              triangle2.GetPoint(triT0, triT1, out point1);
              Vector3 normal;
              if ((double) d > 9.9999999747524271E-07)
              {
                seg2.GetPoint(segT, out normal);
                Vector3.Subtract(ref normal, ref point1, out normal);
                JiggleMath.NormalizeSafe(ref normal);
              }
              else
                normal = triangle2.Normal;
              Vector3 vec = !(info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) is Capsule) ? (!(info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) is Capsule) ? new Vector3() : info.Skin1.Owner.Velocity) : info.Skin0.Owner.Velocity;
              JiggleMath.NormalizeSafe(ref vec);
              if ((double) Vector3.Dot(triangle2.Normal, vec) <= 0.699999988079071)
              {
                float num10 = System.Math.Max(System.Math.Max(result1.Y, result2.Y), result3.Y);
                float num11 = System.Math.Min(System.Math.Min(result1.Y, result2.Y), result3.Y);
                bool flag1 = (double) num10 - (double) num5 <= 0.50999999046325684;
                bool flag2 = (double) num10 - (double) num4 <= 0.50999999046325684 && (double) triangle2.Normal.Y > 0.699999988079071;
                bool flag3 = (double) num11 - (double) num5 <= 0.50999999046325684 && (double) triangle2.Normal.Y > 0.699999988079071;
                if (flag1 || flag2 || flag3)
                {
                  Vector3 point2;
                  seg2.GetPoint(segT, out point2);
                  point2.Y -= oldCapsule.Radius;
                  float num12 = System.Math.Abs(position2.Y - end2.Y) + 2f * oldCapsule.Radius;
                  Segment seg3 = new Segment(point2, Vector3.Up * num12);
                  float tS;
                  float tT0;
                  float tT1;
                  if (Intersection.SegmentTriangleIntersection(out tS, out tT0, out tT1, ref seg3, ref triangle2, false))
                  {
                    Vector3 point3;
                    seg3.GetPoint(tS, out point3);
                    triangle2.GetPoint(tT0, tT1, out Vector3 _);
                    num9 = point3.Y - point2.Y;
                  }
                  normal.X = 0.0f;
                  normal.Z = 0.0f;
                  JiggleMath.NormalizeSafe(ref normal);
                }
                else if ((double) triangle2.Normal.Y < 0.0)
                {
                  normal.Y = 0.0f;
                  JiggleMath.NormalizeSafe(ref normal);
                }
                else
                {
                  bool flag4 = (double) num10 - (double) num4 <= 0.50999999046325684 && (double) triangle2.Normal.Y < 0.30000001192092896;
                  bool flag5 = (double) num11 - (double) num5 <= 0.50999999046325684 && (double) triangle2.Normal.Y < 0.30000001192092896;
                  if (!flag1 && !flag4 && !flag5)
                  {
                    normal.Y = 0.0f;
                    JiggleMath.NormalizeSafe(ref normal);
                  }
                }
                if (num3 < 10)
                {
                  SmallCollPointInfo smallCollPointInfo;
                  smallCollPointInfo.R0 = point1 - vector3_1;
                  smallCollPointInfo.R1 = point1 - vector3_2;
                  smallCollPointInfo.InitialPenetration = num9;
                  pointInfos[0] = smallCollPointInfo;
                }
                if ((double) normal.Y < 0.0)
                  normal.Y *= -1f;
                collisionFunctor.CollisionNotify(ref info, ref normal, pointInfos, 1);
              }
            }
          }
        }
      }
      DetectFunctor.FreeStackAlloc(alloced2);
    }
    DetectFunctor.FreeStackAlloc(alloced1);
  }

  private void CollDetectOverlap(
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    this.CollDetectCapsuleStaticMeshOverlap(info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as Capsule, info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as Capsule, info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as TriangleMesh, info, collTolerance, collisionFunctor);
  }

  private void CollDetectCapsulseStaticMeshSweep(
    Capsule oldCapsule,
    Capsule newCapsule,
    TriangleMesh mesh,
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    if ((double) (newCapsule.Position - oldCapsule.Position).LengthSquared() < 0.25 * (double) newCapsule.Radius * (double) newCapsule.Radius)
    {
      this.CollDetectCapsuleStaticMeshOverlap(oldCapsule, newCapsule, mesh, info, collTolerance, collisionFunctor);
    }
    else
    {
      float length = oldCapsule.Length;
      float radius = oldCapsule.Radius;
      int num1 = 2 + (int) ((double) length / (2.0 * (double) oldCapsule.Radius));
      for (int index = 0; index < num1; ++index)
      {
        float num2 = (float) ((double) index * (double) length / ((double) num1 - 1.0));
        CollDetectSphereStaticMesh.CollDetectSphereStaticMeshSweep(new BoundingSphere(oldCapsule.Position + oldCapsule.Orientation.Backward * num2, radius), new BoundingSphere(newCapsule.Position + newCapsule.Orientation.Backward * num2, radius), mesh, info, collTolerance, collisionFunctor);
      }
    }
  }

  private void CollDetectSweep(
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    this.CollDetectCapsulseStaticMeshSweep(info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as Capsule, info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as Capsule, info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as TriangleMesh, info, collTolerance, collisionFunctor);
  }
}
