// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.CustomCapsulePlane
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Physics;

internal class CustomCapsulePlane : DetectFunctor
{
  public CustomCapsulePlane()
    : base(nameof (CustomCapsulePlane), PrimitiveType.Capsule, PrimitiveType.Plane)
  {
  }

  public override unsafe void CollDetect(
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
    Vector3 vector3_1 = info.Skin0.Owner != null ? info.Skin0.Owner.OldPosition : Vector3.Zero;
    Vector3 vector3_2 = info.Skin1.Owner != null ? info.Skin1.Owner.OldPosition : Vector3.Zero;
    Capsule primitiveOldWorld1 = (Capsule) info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0);
    Capsule primitiveNewWorld1 = (Capsule) info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0);
    JigLibX.Geometry.Plane primitiveOldWorld2 = (JigLibX.Geometry.Plane) info.Skin1.GetPrimitiveOldWorld(info.IndexPrim1);
    JigLibX.Geometry.Plane primitiveNewWorld2 = (JigLibX.Geometry.Plane) info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1);
    Matrix inverseTransformMatrix1 = primitiveNewWorld2.InverseTransformMatrix;
    Matrix inverseTransformMatrix2 = primitiveOldWorld2.InverseTransformMatrix;
    SmallCollPointInfo[] alloced = DetectFunctor.SCPIStackAlloc();
    fixed (SmallCollPointInfo* pointInfos = alloced)
    {
      int numCollPts = 0;
      Vector3 pt1 = Vector3.Transform(primitiveOldWorld1.Position, inverseTransformMatrix2);
      Vector3 pt2 = Vector3.Transform(primitiveNewWorld1.Position, inverseTransformMatrix1);
      float num = Distance.PointPlaneDistance(ref pt1, primitiveOldWorld2);
      if ((double) MathHelper.Min(Distance.PointPlaneDistance(ref pt2, primitiveNewWorld2), num) < (double) collTolerance + (double) primitiveNewWorld1.Radius)
      {
        float initialPenetration = primitiveOldWorld1.Radius - num;
        Vector3 vector3_3 = primitiveOldWorld1.Position - primitiveOldWorld1.Radius * primitiveOldWorld2.Normal;
        pointInfos[numCollPts++] = new SmallCollPointInfo(vector3_3 - vector3_1, vector3_3 - vector3_2, initialPenetration);
      }
      Vector3 pt3 = Vector3.Transform(primitiveOldWorld1.GetEnd(), inverseTransformMatrix2);
      Vector3 pt4 = Vector3.Transform(primitiveNewWorld1.GetEnd(), inverseTransformMatrix1);
      float val2 = Distance.PointPlaneDistance(ref pt3, primitiveOldWorld2);
      if ((double) System.Math.Min(Distance.PointPlaneDistance(ref pt4, primitiveNewWorld2), val2) < (double) collTolerance + (double) primitiveNewWorld1.Radius)
      {
        float initialPenetration = primitiveOldWorld1.Radius - val2;
        Vector3 vector3_4 = primitiveOldWorld1.GetEnd() - primitiveOldWorld1.Radius * primitiveOldWorld2.Normal;
        pointInfos[numCollPts++] = new SmallCollPointInfo(vector3_4 - vector3_1, vector3_4 - vector3_2, initialPenetration);
      }
      if (numCollPts > 0)
      {
        Vector3 normal = primitiveOldWorld2.Normal with
        {
          Y = 0.0f
        };
        JiggleMath.NormalizeSafe(ref normal);
        collisionFunctor.CollisionNotify(ref info, ref normal, pointInfos, numCollPts);
      }
    }
    DetectFunctor.FreeStackAlloc(alloced);
  }
}
