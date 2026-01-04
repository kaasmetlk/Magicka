// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.HollowSphereCapsule
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.Physics;

public class HollowSphereCapsule : DetectFunctor
{
  private Random random = new Random();

  public HollowSphereCapsule()
    : base(nameof (HollowSphereCapsule), PrimitiveType.NumTypes, PrimitiveType.Capsule)
  {
  }

  public override unsafe void CollDetect(
    CollDetectInfo infoOrig,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    CollDetectInfo collDetectInfo = infoOrig;
    if (collDetectInfo.Skin0.GetPrimitiveOldWorld(collDetectInfo.IndexPrim0).Type == this.Type1)
    {
      CollisionSkin skin0 = collDetectInfo.Skin0;
      collDetectInfo.Skin0 = collDetectInfo.Skin1;
      collDetectInfo.Skin1 = skin0;
      int indexPrim0 = collDetectInfo.IndexPrim0;
      collDetectInfo.IndexPrim0 = collDetectInfo.IndexPrim1;
      collDetectInfo.IndexPrim1 = indexPrim0;
    }
    Vector3 vector3_1 = collDetectInfo.Skin0.Owner != null ? collDetectInfo.Skin0.Owner.OldPosition : Vector3.Zero;
    Vector3 vector3_2 = collDetectInfo.Skin1.Owner != null ? collDetectInfo.Skin1.Owner.OldPosition : Vector3.Zero;
    HollowSphere primitiveOldWorld1 = collDetectInfo.Skin0.GetPrimitiveOldWorld(collDetectInfo.IndexPrim0) as HollowSphere;
    HollowSphere primitiveNewWorld1 = collDetectInfo.Skin0.GetPrimitiveNewWorld(collDetectInfo.IndexPrim0) as HollowSphere;
    Capsule primitiveOldWorld2 = collDetectInfo.Skin1.GetPrimitiveOldWorld(collDetectInfo.IndexPrim1) as Capsule;
    Capsule primitiveNewWorld2 = collDetectInfo.Skin1.GetPrimitiveNewWorld(collDetectInfo.IndexPrim1) as Capsule;
    Segment seg1 = new Segment(primitiveOldWorld2.Position, primitiveOldWorld2.Length * primitiveOldWorld2.Orientation.Backward);
    Segment seg2 = new Segment(primitiveOldWorld2.Position, primitiveNewWorld2.Length * primitiveNewWorld2.Orientation.Backward);
    Vector3 vector = Vector3.TransformNormal(primitiveNewWorld2.Position - primitiveNewWorld1.Position, primitiveNewWorld1.InverseTransformMatrix);
    vector.Normalize();
    if ((double) MagickaMath.Angle(vector, Vector3.Forward) > (double) primitiveNewWorld1.MaxAngle)
      return;
    float num1 = primitiveNewWorld2.Radius + primitiveNewWorld1.Radius;
    float t;
    float d = Distance.PointSegmentDistanceSq(out t, primitiveOldWorld1.Position, seg1);
    float num2 = Distance.PointSegmentDistanceSq(out float _, primitiveNewWorld1.Position, seg2);
    float num3 = primitiveNewWorld1.Radius + collTolerance;
    float num4 = num3 * num3;
    float num5 = num1 + HollowSphere.THICKNESS + collTolerance;
    float num6 = num5 * num5;
    float num7 = primitiveNewWorld1.Radius - primitiveNewWorld2.Radius - HollowSphere.THICKNESS + collTolerance;
    float num8 = num7 * num7;
    if ((double) d < (double) num4 && (double) num2 > (double) num8)
    {
      Vector3 point;
      seg1.GetPoint(t, out point);
      Vector3 vector3_3 = point - primitiveOldWorld1.Position;
      float num9 = (float) Math.Sqrt((double) d);
      float initialPenetration = num9 + primitiveNewWorld2.Radius - primitiveNewWorld1.Radius + HollowSphere.THICKNESS;
      Vector3 dirToBody0 = (double) num9 <= 9.9999999747524271E-07 ? Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float) this.random.Next(360)))) : vector3_3 / num9;
      Vector3 vector3_4 = point + (primitiveOldWorld2.Radius - 0.5f * initialPenetration) * dirToBody0;
      SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(vector3_4 - vector3_1, vector3_4 - vector3_2, initialPenetration);
      collisionFunctor.CollisionNotify(ref collDetectInfo, ref dirToBody0, &smallCollPointInfo, 1);
    }
    else
    {
      if ((double) d <= (double) num4 || (double) num2 >= (double) num6)
        return;
      Vector3 point;
      seg1.GetPoint(t, out point);
      Vector3 vector3_5 = primitiveOldWorld1.Position - point;
      float num10 = (float) Math.Sqrt((double) d);
      float initialPenetration = num1 - num10 + HollowSphere.THICKNESS;
      Vector3 dirToBody0 = (double) num10 <= 9.9999999747524271E-07 ? Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float) this.random.Next(360)))) : vector3_5 / num10;
      Vector3 vector3_6 = point + (primitiveOldWorld2.Radius - 0.5f * initialPenetration) * dirToBody0;
      SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(vector3_6 - vector3_1, vector3_6 - vector3_2, initialPenetration);
      collisionFunctor.CollisionNotify(ref collDetectInfo, ref dirToBody0, &smallCollPointInfo, 1);
    }
  }
}
