// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.KeyedValue
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

#nullable disable
namespace PolygonHead.ParticleEffects;

public struct KeyedValue(float iTime, float iValue)
{
  public float Time = iTime;
  public float Value = iValue;

  public static float Blend(KeyedValue iA, KeyedValue iB, float iTime)
  {
    float num = (float) (((double) iTime - (double) iA.Time) / ((double) iB.Time - (double) iA.Time));
    return (float) ((double) iA.Value * (1.0 - (double) num) + (double) iB.Value * (double) num);
  }
}
