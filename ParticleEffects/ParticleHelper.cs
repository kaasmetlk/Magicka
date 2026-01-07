// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.ParticleHelper
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using PolygonHead.Lights;
using System;

#nullable disable
namespace PolygonHead.ParticleEffects;

public static class ParticleHelper
{
  public const int RANDOM_CACHE_SIZE = 65536 /*0x010000*/;
  private static float[] mRandomFloats;
  private static byte[] mRandomBytes;

  static ParticleHelper()
  {
    Random random = new Random(0);
    ParticleHelper.mRandomFloats = new float[65536 /*0x010000*/];
    ParticleHelper.mRandomBytes = new byte[65536 /*0x010000*/];
    for (int index = 0; index < 65536 /*0x010000*/; ++index)
      ParticleHelper.mRandomFloats[index] = (float) random.NextDouble();
    for (int index = 0; index < 65536 /*0x010000*/; ++index)
      ParticleHelper.mRandomBytes[index] = (byte) random.Next(100);
  }

  public static float GetKeyedValue(KeyedValue[] iArray, float iTime)
  {
    if (iArray == null)
      return 1f;
    if (iArray.Length == 1)
      return iArray[0].Value;
    bool flag1 = false;
    bool flag2 = false;
    KeyedValue iA = new KeyedValue();
    KeyedValue iB = new KeyedValue();
    for (int index = 0; index < iArray.Length; ++index)
    {
      if ((double) iArray[index].Time <= (double) iTime)
      {
        iA = iArray[index];
        flag1 = true;
      }
      if ((double) iArray[index].Time > (double) iTime)
      {
        iB = iArray[index];
        flag2 = true;
        break;
      }
    }
    if (flag1 && flag2)
      return KeyedValue.Blend(iA, iB, iTime);
    return flag2 ? iB.Value : iA.Value;
  }

  public static float GetRandomValue(ref ushort iRandomOffset, float iMin, float iMax)
  {
    return ParticleHelper.mRandomFloats[(int) iRandomOffset++] * (iMax - iMin) + iMin;
  }

  public static float GetRandomValue(
    ref ushort iRandomOffset,
    float iMin,
    float iMax,
    float iDistribution)
  {
    return (float) Math.Pow((double) ParticleHelper.mRandomFloats[(int) iRandomOffset++], (double) iDistribution) * (iMax - iMin) + iMin;
  }

  public static int GetRandomSign(ref ushort iRandomOffset, int iPersentNegative)
  {
    return (int) ParticleHelper.mRandomBytes[(int) iRandomOffset++] >= iPersentNegative ? 1 : -1;
  }

  public static void GetRandomDirectionCone(
    ref ushort iRandomOffset,
    ref Matrix iOrientation,
    float iAngle,
    float iDistribution,
    out Vector3 oDirection)
  {
    float num1 = (float) (Math.Pow((double) ParticleHelper.mRandomFloats[(int) iRandomOffset++], (double) iDistribution) * ((double) iAngle / 3.1415927410125732) * 2.0 - 1.0);
    float num2 = ParticleHelper.mRandomFloats[(int) iRandomOffset++] * 6.28318548f;
    float num3 = (float) Math.Sqrt(1.0 - (double) num1 * (double) num1);
    oDirection.X = num3 * (float) Math.Cos((double) num2);
    oDirection.Y = num3 * (float) Math.Sin((double) num2);
    oDirection.Z = num1;
    Vector3.TransformNormal(ref oDirection, ref iOrientation, out oDirection);
  }

  public static void GetRandomDirectionArc(
    ref ushort iRandomOffset,
    ref Matrix iOrientation,
    float iHorizontalAngle,
    float iHorizontalDistribution,
    float iVerticalMin,
    float iVerticalMax,
    float iVerticalDistribution,
    out Vector3 oDirection)
  {
    float num1 = (float) ((double) ParticleHelper.mRandomFloats[(int) iRandomOffset++] * 2.0 - 1.0);
    float num2 = (float) Math.Pow((double) Math.Abs(num1), (double) iHorizontalDistribution) * (float) Math.Sign(num1) * iHorizontalAngle;
    oDirection.X = (float) Math.Sin((double) num2);
    oDirection.Z = -(float) Math.Cos((double) num2);
    float num3 = (float) ((double) ParticleHelper.mRandomFloats[(int) iRandomOffset++] * 2.0 - 1.0);
    float num4 = (float) ((Math.Pow((double) Math.Abs(num3), (double) iVerticalDistribution) * (double) Math.Sign(num3) * ((double) iVerticalMax - (double) iVerticalMin) + ((double) iVerticalMin + (double) iVerticalMax)) * 0.5);
    float num5 = (float) Math.Cos((double) num4);
    oDirection.Y = (float) Math.Sin((double) num4);
    oDirection.X *= num5;
    oDirection.Z *= num5;
    Vector3.TransformNormal(ref oDirection, ref iOrientation, out oDirection);
  }

  public static float GetRandomValue(ref ushort iRandomOffset)
  {
    return ParticleHelper.mRandomFloats[(int) iRandomOffset++];
  }

  public static void SpawnLight(ref Particle iParticle, ref ParticleLightProperties iLight)
  {
    ParticleLightBatcher.ParticleLight iLight1;
    iLight1.Position = iParticle.Position;
    iLight1.Velocity = iParticle.Velocity;
    iLight1.Gravity = iParticle.Gravity;
    iLight1.Drag = iParticle.Drag;
    iLight1.TTL = iParticle.TTL;
    iLight1.DiffuseColor = iLight.DiffuseColor;
    iLight1.AmbientColor = iLight.AmbientColor;
    iLight1.SpecularAmount = iLight.SpecularAmount;
    iLight1.RadiusStart = iLight.RadiusStart;
    iLight1.RadiusEnd = iLight.RadiusEnd;
    ParticleLightBatcher.Instance.SpawnLight(ref iLight1);
  }
}
