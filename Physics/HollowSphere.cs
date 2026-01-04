// Decompiled with JetBrains decompiler
// Type: Magicka.Physics.HollowSphere
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Physics;

public class HollowSphere : Primitive
{
  private float radius;
  public static float THICKNESS = 0.5f;
  private float maxAngle;

  public HollowSphere(Vector3 pos, float radius)
    : base(PrimitiveType.NumTypes)
  {
    this.transform.Position = pos;
    this.radius = radius;
    this.maxAngle = 3.14159274f;
  }

  public override Primitive Clone()
  {
    return (Primitive) new HollowSphere(this.transform.Position, this.radius);
  }

  public bool ArcIntersect(
    ref Vector3 iOrigin,
    ref Vector3 iDelta,
    float iSpread,
    out Vector3 oPos)
  {
    Segment seg;
    seg.Origin = iOrigin;
    seg.Delta = iDelta;
    float ts;
    bool flag = this.SegmentHollowSphereIntersection(out ts, seg);
    Vector3.Multiply(ref seg.Delta, ts, out seg.Delta);
    Vector3.Add(ref seg.Origin, ref seg.Delta, out oPos);
    return flag;
  }

  public bool SegmentHollowSphereIntersection(out float ts, Segment seg)
  {
    Vector3 result1 = this.Position;
    Vector3.Subtract(ref result1, ref seg.Origin, out result1);
    float result2;
    Vector3.Dot(ref seg.Delta, ref seg.Delta, out result2);
    float result3;
    Vector3.Dot(ref seg.Delta, ref result1, out result3);
    float result4;
    Vector3.Dot(ref result1, ref result1, out result4);
    float num1 = result4 - this.radius * this.radius;
    float num2 = (float) System.Math.Sqrt((double) result3 * (double) result3 - (double) result2 * (double) num1);
    float num3 = 1f / result2;
    float num4 = (result3 + num2) * num3;
    float num5 = (result3 - num2) * num3;
    bool flag1 = (double) num4 > 0.0 & (double) num4 < 1.0 & !float.IsNaN(num4);
    bool flag2 = (double) num5 > 0.0 & (double) num5 < 1.0 & !float.IsNaN(num5);
    if (!flag1 & !flag2)
    {
      ts = 0.0f;
      return false;
    }
    Vector3 forward = this.transform.Orientation.Forward;
    Vector3 vector1_1;
    seg.GetPoint(num4, out vector1_1);
    Vector3.Subtract(ref vector1_1, ref this.transform.Position, out vector1_1);
    vector1_1.Normalize();
    float result5;
    Vector3.Dot(ref vector1_1, ref forward, out result5);
    float num6 = (float) System.Math.Acos((double) result5);
    Vector3 vector1_2;
    seg.GetPoint(num5, out vector1_2);
    Vector3.Subtract(ref vector1_2, ref this.transform.Position, out vector1_2);
    vector1_2.Normalize();
    float result6;
    Vector3.Dot(ref vector1_2, ref forward, out result6);
    float num7 = (float) System.Math.Acos((double) result6);
    if ((double) num6 > (double) this.maxAngle)
      flag1 = false;
    if ((double) num7 > (double) this.maxAngle)
      flag2 = false;
    if (flag1 & ((double) num4 < (double) num5 | !flag2))
      ts = num4;
    else if (flag2 & ((double) num5 < (double) num4 | !flag1))
    {
      ts = num5;
    }
    else
    {
      ts = 0.0f;
      return false;
    }
    return true;
  }

  public override bool SegmentIntersect(
    out float frac,
    out Vector3 pos,
    out Vector3 normal,
    ref Segment seg)
  {
    bool flag = this.SegmentHollowSphereIntersection(out frac, seg);
    if (flag)
    {
      seg.GetPoint(frac, out pos);
      normal = pos - this.transform.Position;
      JiggleMath.NormalizeSafe(ref normal);
    }
    else
    {
      pos = Vector3.Zero;
      normal = Vector3.Zero;
    }
    return flag;
  }

  public override void GetMassProperties(
    PrimitiveProperties primitiveProperties,
    out float mass,
    out Vector3 centerOfMass,
    out Matrix inertiaTensor)
  {
    mass = primitiveProperties.MassType != PrimitiveProperties.MassTypeEnum.Mass ? (primitiveProperties.MassDistribution != PrimitiveProperties.MassDistributionEnum.Solid ? this.GetSurfaceArea() * primitiveProperties.MassOrDensity : this.GetVolume() * primitiveProperties.MassOrDensity) : primitiveProperties.MassOrDensity;
    centerOfMass = this.transform.Position;
    float num = primitiveProperties.MassDistribution != PrimitiveProperties.MassDistributionEnum.Solid ? 0.6666667f * mass * this.radius : 0.4f * mass * this.radius;
    inertiaTensor = Matrix.Identity;
    inertiaTensor.M11 = inertiaTensor.M22 = inertiaTensor.M33 = num;
    inertiaTensor.M11 += mass * (float) ((double) centerOfMass.Y * (double) centerOfMass.Y + (double) centerOfMass.Z * (double) centerOfMass.Z);
    inertiaTensor.M22 += mass * (float) ((double) centerOfMass.Z * (double) centerOfMass.Z + (double) centerOfMass.X * (double) centerOfMass.X);
    inertiaTensor.M33 += mass * (float) ((double) centerOfMass.X * (double) centerOfMass.X + (double) centerOfMass.Y * (double) centerOfMass.Y);
    inertiaTensor.M12 = inertiaTensor.M21 = inertiaTensor.M12 - mass * centerOfMass.X * centerOfMass.Y;
    inertiaTensor.M23 = inertiaTensor.M32 = inertiaTensor.M23 - mass * centerOfMass.Y * centerOfMass.Z;
    inertiaTensor.M31 = inertiaTensor.M13 = inertiaTensor.M31 - mass * centerOfMass.Z * centerOfMass.X;
  }

  public override float GetVolume() => 4.18879032f * this.radius * this.radius * this.radius;

  public override float GetSurfaceArea() => 12.566371f * this.radius * this.radius;

  public Vector3 Position
  {
    get => this.transform.Position;
    set => this.transform.Position = value;
  }

  public float Radius
  {
    get => this.radius;
    set => this.radius = value;
  }

  public float MaxAngle
  {
    get => this.maxAngle;
    set => this.maxAngle = value;
  }

  public override void GetBoundingBox(out AABox box)
  {
    Vector3 position1 = this.transform.Position;
    position1.X -= this.radius + HollowSphere.THICKNESS * 2f;
    position1.Y -= this.radius + HollowSphere.THICKNESS * 2f;
    position1.Z -= this.radius + HollowSphere.THICKNESS * 2f;
    Vector3 position2 = this.transform.Position;
    position2.X += this.radius + HollowSphere.THICKNESS * 2f;
    position2.Y += this.radius + HollowSphere.THICKNESS * 2f;
    position2.Z += this.radius + HollowSphere.THICKNESS * 2f;
    box = new AABox(position1, position2);
  }
}
