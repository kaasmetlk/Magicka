// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.CustomCapsuleCapsule
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.Physics;

public class CustomCapsuleCapsule : DetectFunctor
{
  private Random random = new Random();

  public CustomCapsuleCapsule()
    : base(nameof (CustomCapsuleCapsule), PrimitiveType.Capsule, PrimitiveType.Capsule)
  {
  }

  public override unsafe void CollDetect(
    CollDetectInfo info,
    float collTolerance,
    ICollisionFunctor collisionFunctor)
  {
    Vector3 vector3_1 = info.Skin0.Owner != null ? info.Skin0.Owner.OldPosition : Vector3.Zero;
    Vector3 vector3_2 = info.Skin1.Owner != null ? info.Skin1.Owner.OldPosition : Vector3.Zero;
    Capsule primitiveOldWorld1 = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as Capsule;
    Capsule primitiveNewWorld1 = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as Capsule;
    Segment seg0_1 = new Segment(primitiveOldWorld1.Position, primitiveOldWorld1.Length * primitiveOldWorld1.Orientation.Backward);
    Segment seg0_2 = new Segment(primitiveNewWorld1.Position, primitiveNewWorld1.Length * primitiveNewWorld1.Orientation.Backward);
    Capsule primitiveOldWorld2 = info.Skin1.GetPrimitiveOldWorld(info.IndexPrim1) as Capsule;
    Capsule primitiveNewWorld2 = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as Capsule;
    Segment seg1_1 = new Segment(primitiveOldWorld2.Position, primitiveOldWorld2.Length * primitiveOldWorld2.Orientation.Backward);
    Segment seg1_2 = new Segment(primitiveNewWorld2.Position, primitiveNewWorld2.Length * primitiveNewWorld2.Orientation.Backward);
    float num1 = primitiveNewWorld1.Radius + primitiveNewWorld2.Radius;
    float t0;
    float t1;
    float num2 = Distance.SegmentSegmentDistanceSq(out t0, out t1, seg0_1, seg1_1);
    float val2 = Distance.SegmentSegmentDistanceSq(out float _, out float _, seg0_2, seg1_2);
    if ((double) Math.Min(num2, val2) >= ((double) num1 + (double) collTolerance) * ((double) num1 + (double) collTolerance))
      return;
    Vector3 point1;
    seg0_1.GetPoint(t0, out point1);
    Vector3 point2;
    seg1_1.GetPoint(t1, out point2);
    point1.Y = (float) (((double) point1.Y + (double) point2.Y) * 0.5);
    point2.Y = point1.Y;
    Vector3 vector3_3 = point1 - point2;
    float num3 = (float) Math.Sqrt((double) num2);
    float initialPenetration = num1 - num3;
    Vector3 dirToBody0 = (double) num3 <= 9.9999999747524271E-07 ? Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float) this.random.Next(360)))) : vector3_3 / num3;
    Vector3 vector3_4 = point2 + (primitiveOldWorld2.Radius - 0.5f * initialPenetration) * dirToBody0;
    SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(vector3_4 - vector3_1, vector3_4 - vector3_2, initialPenetration);
    collisionFunctor.CollisionNotify(ref info, ref dirToBody0, &smallCollPointInfo, 1);
  }
}
