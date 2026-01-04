// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.HollowSphereStaticMesh
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Physics;

public class HollowSphereStaticMesh : DetectFunctor
{
  public HollowSphereStaticMesh()
    : base(nameof (HollowSphereStaticMesh), PrimitiveType.NumTypes, PrimitiveType.TriangleMesh)
  {
  }

  public static unsafe void CollDetectSphereStaticMeshOverlap(
    BoundingSphere oldSphere,
    BoundingSphere newSphere,
    TriangleMesh mesh,
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    Vector3 vector3_1 = info.Skin0.Owner != null ? info.Skin0.Owner.OldPosition : Vector3.Zero;
    Vector3 vector3_2 = info.Skin1.Owner != null ? info.Skin1.Owner.OldPosition : Vector3.Zero;
    float num1 = collTolerance + newSphere.Radius + HollowSphere.THICKNESS;
    float num2 = collTolerance + newSphere.Radius - HollowSphere.THICKNESS;
    float num3 = num1 * num1;
    float num4 = num2 * num2;
    SmallCollPointInfo[] alloced1 = DetectFunctor.SCPIStackAlloc();
    fixed (SmallCollPointInfo* pointInfos = alloced1)
    {
      int[] alloced2 = DetectFunctor.IntStackAlloc();
      fixed (int* triangles = alloced2)
      {
        int numCollPts = 0;
        Vector3 zero = Vector3.Zero;
        BoundingBox initialBox = BoundingBoxHelper.InitialBox;
        BoundingBoxHelper.AddSphere(ref newSphere, ref initialBox);
        int intersectingtAaBox = mesh.GetTrianglesIntersectingtAABox(triangles, 2048 /*0x0800*/, ref initialBox);
        Vector3 rkPoint1 = Vector3.Transform(newSphere.Center, mesh.InverseTransformMatrix);
        Vector3 rkPoint2 = Vector3.Transform(oldSphere.Center, mesh.InverseTransformMatrix);
        for (int index = 0; index < intersectingtAaBox; ++index)
        {
          IndexedTriangle triangle = mesh.GetTriangle(triangles[index]);
          float num5 = triangle.Plane.DotCoordinate(rkPoint1);
          if ((double) num5 > 0.0 && (double) num5 < (double) num1)
          {
            int i0;
            int i1;
            int i2;
            triangle.GetVertexIndices(out i0, out i1, out i2);
            Vector3 result1;
            mesh.GetVertex(i0, out result1);
            Vector3 result2;
            mesh.GetVertex(i1, out result2);
            Vector3 result3;
            mesh.GetVertex(i2, out result3);
            Triangle rkTri = new Triangle(ref result1, ref result2, ref result3);
            float pfSParam;
            float pfTParam;
            if ((double) Distance.PointTriangleDistanceSq(out pfSParam, out pfTParam, ref rkPoint1, ref rkTri) < (double) num3)
            {
              float result4;
              Vector3.DistanceSquared(ref result1, ref rkPoint1, out result4);
              float result5;
              Vector3.DistanceSquared(ref result2, ref rkPoint1, out result5);
              float val1 = System.Math.Max(result4, result5);
              Vector3.DistanceSquared(ref result2, ref rkPoint1, out result5);
              result4 = System.Math.Max(val1, result5);
              if ((double) result4 > (double) num4)
              {
                float num6 = (float) System.Math.Sqrt((double) Distance.PointTriangleDistanceSq(out pfSParam, out pfTParam, ref rkPoint2, ref rkTri));
                float initialPenetration = oldSphere.Radius - num6;
                Vector3 vec;
                rkTri.GetPoint(pfSParam, pfTParam, out vec);
                Vector3.Subtract(ref rkPoint2, ref vec, out vec);
                JiggleMath.NormalizeSafe(ref vec);
                Vector3 vector3_3 = (double) num6 > 1.4012984643248171E-45 ? vec : rkTri.Normal;
                Vector3 vector3_4 = oldSphere.Center - oldSphere.Radius * vector3_3;
                if (numCollPts < 10)
                  pointInfos[numCollPts++] = new SmallCollPointInfo(vector3_4 - vector3_1, vector3_4 - vector3_2, initialPenetration);
                zero += vector3_3;
              }
            }
          }
        }
        if (numCollPts > 0)
        {
          JiggleMath.NormalizeSafe(ref zero);
          collisionFunctor.CollisionNotify(ref info, ref zero, pointInfos, numCollPts);
        }
        DetectFunctor.FreeStackAlloc(alloced2);
      }
      DetectFunctor.FreeStackAlloc(alloced1);
    }
  }

  private void CollDetectOverlap(
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    HollowSphere primitiveOldWorld = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as HollowSphere;
    HollowSphere primitiveNewWorld = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as HollowSphere;
    HollowSphereStaticMesh.CollDetectSphereStaticMeshOverlap(new BoundingSphere(primitiveOldWorld.Position, primitiveOldWorld.Radius), new BoundingSphere(primitiveNewWorld.Position, primitiveNewWorld.Radius), info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as TriangleMesh, info, collTolerance, collisionFunctor);
  }

  public static unsafe void CollDetectSphereStaticMeshSweep(
    BoundingSphere oldSphere,
    BoundingSphere newSphere,
    TriangleMesh mesh,
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    if ((double) (newSphere.Center - oldSphere.Center).LengthSquared() < 0.25 * (double) newSphere.Radius * (double) newSphere.Radius)
    {
      HollowSphereStaticMesh.CollDetectSphereStaticMeshOverlap(oldSphere, newSphere, mesh, info, collTolerance, collisionFunctor);
    }
    else
    {
      Vector3 vector3_1 = info.Skin0.Owner != null ? info.Skin0.Owner.OldPosition : Vector3.Zero;
      Vector3 vector3_2 = info.Skin1.Owner != null ? info.Skin1.Owner.OldPosition : Vector3.Zero;
      float num1 = collTolerance + oldSphere.Radius;
      float num2 = num1 * num1;
      Vector3 zero = Vector3.Zero;
      BoundingBox initialBox = BoundingBoxHelper.InitialBox;
      BoundingBoxHelper.AddSphere(ref oldSphere, ref initialBox);
      BoundingBoxHelper.AddSphere(ref newSphere, ref initialBox);
      Vector3 vector3_3 = Vector3.Transform(newSphere.Center, mesh.InverseTransformMatrix);
      Vector3 rkPoint = Vector3.Transform(oldSphere.Center, mesh.InverseTransformMatrix);
      SmallCollPointInfo[] alloced1 = DetectFunctor.SCPIStackAlloc();
      fixed (SmallCollPointInfo* pointInfos = alloced1)
      {
        int[] alloced2 = DetectFunctor.IntStackAlloc();
        fixed (int* triangles = alloced2)
        {
          int numCollPts = 0;
          int intersectingtAaBox = mesh.GetTrianglesIntersectingtAABox(triangles, 2048 /*0x0800*/, ref initialBox);
          for (int index = 0; index < intersectingtAaBox; ++index)
          {
            IndexedTriangle triangle = mesh.GetTriangle(triangles[index]);
            float oldCentreDistToPlane = triangle.Plane.DotCoordinate(rkPoint);
            if ((double) oldCentreDistToPlane > 0.0)
            {
              float newCentreDistToPlane = triangle.Plane.DotCoordinate(vector3_3);
              if ((double) newCentreDistToPlane <= (double) num1)
              {
                int i0;
                int i1;
                int i2;
                triangle.GetVertexIndices(out i0, out i1, out i2);
                Vector3 result1;
                mesh.GetVertex(i0, out result1);
                Vector3 result2;
                mesh.GetVertex(i1, out result2);
                Vector3 result3;
                mesh.GetVertex(i2, out result3);
                Triangle rkTri = new Triangle(ref result1, ref result2, ref result3);
                float pfSParam;
                float pfTParam;
                float d = Distance.PointTriangleDistanceSq(out pfSParam, out pfTParam, ref rkPoint, ref rkTri);
                if ((double) d < (double) num2)
                {
                  float num3 = (float) System.Math.Sqrt((double) d);
                  float initialPenetration = oldSphere.Radius - num3;
                  Vector3 normal = rkTri.Normal;
                  Vector3 vec;
                  rkTri.GetPoint(pfSParam, pfTParam, out vec);
                  Vector3.Subtract(ref rkPoint, ref vec, out vec);
                  JiggleMath.NormalizeSafe(ref vec);
                  Vector3 vector3_4 = (double) num3 > 1.4012984643248171E-45 ? vec : normal;
                  Vector3 vector3_5 = oldSphere.Center - oldSphere.Radius * vector3_4;
                  if (numCollPts < 10)
                    pointInfos[numCollPts++] = new SmallCollPointInfo(vector3_5 - vector3_1, vector3_5 - vector3_2, initialPenetration);
                  zero += vector3_4;
                }
                else
                {
                  float depth;
                  if ((double) newCentreDistToPlane < (double) oldCentreDistToPlane && Intersection.SweptSphereTriangleIntersection(out Vector3 _, out Vector3 _, out depth, oldSphere, newSphere, rkTri, oldCentreDistToPlane, newCentreDistToPlane, Intersection.EdgesToTest.EdgeAll, Intersection.CornersToTest.CornerAll))
                  {
                    float num4 = (float) System.Math.Sqrt((double) d);
                    Vector3 normal = rkTri.Normal;
                    Vector3 vec;
                    rkTri.GetPoint(pfSParam, pfTParam, out vec);
                    Vector3.Subtract(ref rkPoint, ref vec, out vec);
                    JiggleMath.NormalizeSafe(ref vec);
                    Vector3 vector3_6 = (double) num4 > 9.9999999747524271E-07 ? vec : normal;
                    Vector3 vector3_7 = oldSphere.Center - oldSphere.Radius * vector3_6;
                    if (numCollPts < 10)
                      pointInfos[numCollPts++] = new SmallCollPointInfo(vector3_7 - vector3_1, vector3_7 - vector3_2, depth);
                    zero += vector3_6;
                  }
                }
              }
            }
          }
          if (numCollPts > 0)
          {
            JiggleMath.NormalizeSafe(ref zero);
            collisionFunctor.CollisionNotify(ref info, ref zero, pointInfos, numCollPts);
          }
        }
        DetectFunctor.FreeStackAlloc(alloced2);
      }
      DetectFunctor.FreeStackAlloc(alloced1);
    }
  }

  private void CollDetectSweep(
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    HollowSphere primitiveOldWorld = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as HollowSphere;
    HollowSphere primitiveNewWorld = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as HollowSphere;
    HollowSphereStaticMesh.CollDetectSphereStaticMeshSweep(new BoundingSphere(primitiveOldWorld.Position, primitiveOldWorld.Radius), new BoundingSphere(primitiveNewWorld.Position, primitiveNewWorld.Radius), info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as TriangleMesh, info, collTolerance, collisionFunctor);
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
}
