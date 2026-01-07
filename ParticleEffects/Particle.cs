// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.Particle
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;

#nullable disable
namespace PolygonHead.ParticleEffects;

public struct Particle
{
  public Vector3 Position;
  public Vector3 Velocity;
  public float Gravity;
  public float Rotation;
  public float TTL;
  public float StartSize;
  public float EndSize;
  public float RotationVelocity;
  public float Drag;
  public byte Sprite;
  public bool AlphaBlended;
  public bool HSV;
  public bool Colorize;
  public bool RotationAligned;
  public Vector4 Color;
}
