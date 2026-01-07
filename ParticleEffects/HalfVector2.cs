// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.HalfVector2
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

#nullable disable
namespace PolygonHead.ParticleEffects;

public struct HalfVector2
{
  public HalfSingle X;
  public HalfSingle Y;

  public static explicit operator HalfVector2(Vector2 iVector)
  {
    HalfVector2 halfVector2;
    halfVector2.X = new HalfSingle(iVector.X);
    halfVector2.Y = new HalfSingle(iVector.Y);
    return halfVector2;
  }

  public static implicit operator Vector2(HalfVector2 iVector)
  {
    return new Vector2()
    {
      X = iVector.X.ToSingle(),
      Y = iVector.Y.ToSingle()
    };
  }
}
