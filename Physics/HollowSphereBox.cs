// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.HollowSphereBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.Physics;

public class HollowSphereBox : DetectFunctor
{
  private Random random = new Random();

  public HollowSphereBox()
    : base(nameof (HollowSphereBox), PrimitiveType.NumTypes, PrimitiveType.Box)
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
    HollowSphere primitiveOldWorld1 = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as HollowSphere;
    HollowSphere primitiveNewWorld1 = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as HollowSphere;
    Box primitiveOldWorld2 = info.Skin1.GetPrimitiveOldWorld(info.IndexPrim1) as Box;
    Box primitiveNewWorld2 = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as Box;
    Vector3 centre1;
    primitiveOldWorld2.GetCentre(out centre1);
    Vector3 oHalfSideLengths;
    primitiveOldWorld2.GetHalfSideLengths(out oHalfSideLengths);
    float num1 = oHalfSideLengths.Length();
    Vector3 centre2;
    primitiveNewWorld2.GetCentre(out centre2);
    primitiveNewWorld2.GetHalfSideLengths(out oHalfSideLengths);
    float num2 = oHalfSideLengths.Length();
    Vector3 vector3_3 = primitiveOldWorld1.Position - centre1;
    Vector3 vector3_4 = primitiveNewWorld1.Position - centre2;
    Vector3 vector = Vector3.TransformNormal(-vector3_4, primitiveNewWorld1.InverseTransformMatrix);
    vector.Normalize();
    if ((double) MagickaMath.Angle(vector, Vector3.Forward) > (double) primitiveNewWorld1.MaxAngle)
      return;
    float d = vector3_3.LengthSquared();
    float num3 = vector3_4.LengthSquared();
    float num4 = primitiveNewWorld1.Radius + num2;
    float num5 = primitiveNewWorld1.Radius + collTolerance;
    float num6 = num5 * num5;
    float num7 = num4 + collTolerance;
    float num8 = num7 * num7;
    float num9 = primitiveNewWorld1.Radius - num2 + collTolerance;
    float num10 = num9 * num9;
    if ((double) d < (double) num6 && (double) num3 > (double) num10)
    {
      float num11 = (float) Math.Sqrt((double) d);
      float initialPenetration = num11 + num2 - primitiveNewWorld1.Radius;
      Vector3 vector3_5 = (double) num11 <= 9.9999999747524271E-07 ? Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float) this.random.Next(360)))) : vector3_3 / num11;
      Vector3 vector3_6 = centre1 + (num1 - 0.5f * initialPenetration) * vector3_5;
      Vector3 dirToBody0 = vector3_5 * -1f;
      SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(vector3_6 - vector3_1, vector3_6 - vector3_2, initialPenetration);
      collisionFunctor.CollisionNotify(ref info, ref dirToBody0, &smallCollPointInfo, 1);
    }
    else
    {
      if ((double) d <= (double) num6 || (double) num3 >= (double) num8)
        return;
      float num12 = (float) Math.Sqrt((double) d);
      float initialPenetration = num4 - num12;
      Vector3 dirToBody0 = (double) num12 <= 9.9999999747524271E-07 ? Vector3.Transform(Vector3.Backward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians((float) this.random.Next(360)))) : vector3_3 / num12;
      Vector3 vector3_7 = centre1 + (num1 - 0.5f * initialPenetration) * dirToBody0;
      SmallCollPointInfo smallCollPointInfo = new SmallCollPointInfo(vector3_7 - vector3_1, vector3_7 - vector3_2, initialPenetration);
      collisionFunctor.CollisionNotify(ref info, ref dirToBody0, &smallCollPointInfo, 1);
    }
  }
}
