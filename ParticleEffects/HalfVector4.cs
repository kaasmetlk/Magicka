// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.HalfVector4
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

#nullable disable
namespace PolygonHead.ParticleEffects;

public struct HalfVector4
{
  public HalfSingle X;
  public HalfSingle Y;
  public HalfSingle Z;
  public HalfSingle W;

  public static explicit operator HalfVector4(Vector4 iVector)
  {
    HalfVector4 halfVector4;
    halfVector4.X = new HalfSingle(iVector.X);
    halfVector4.Y = new HalfSingle(iVector.Y);
    halfVector4.Z = new HalfSingle(iVector.Z);
    halfVector4.W = new HalfSingle(iVector.W);
    return halfVector4;
  }

  public static implicit operator Vector4(HalfVector4 iVector)
  {
    return new Vector4()
    {
      X = iVector.X.ToSingle(),
      Y = iVector.Y.ToSingle(),
      Z = iVector.Z.ToSingle(),
      W = iVector.W.ToSingle()
    };
  }
}
