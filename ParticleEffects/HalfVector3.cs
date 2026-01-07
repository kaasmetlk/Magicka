// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.HalfVector3
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

#nullable disable
namespace PolygonHead.ParticleEffects;

public struct HalfVector3
{
  public HalfSingle X;
  public HalfSingle Y;
  public HalfSingle Z;

  public static explicit operator HalfVector3(Vector3 iVector)
  {
    HalfVector3 halfVector3;
    halfVector3.X = new HalfSingle(iVector.X);
    halfVector3.Y = new HalfSingle(iVector.Y);
    halfVector3.Z = new HalfSingle(iVector.Z);
    return halfVector3;
  }

  public static implicit operator Vector3(HalfVector3 iVector)
  {
    return new Vector3()
    {
      X = iVector.X.ToSingle(),
      Y = iVector.Y.ToSingle(),
      Z = iVector.Z.ToSingle()
    };
  }
}
