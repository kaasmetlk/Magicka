// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.ActiveAura
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Graphics;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct ActiveAura
{
  private const int NR_OF_DECALS = 6;
  public AuraStorage Aura;
  public float StartTTL;
  public bool SelfCasted;
  private float mRotation;
  private float mAlpha;
  public unsafe fixed ulong Decals[6];
  public VisualEffectReference mEffect;

  public unsafe void Execute(Character iOwner, float iDeltaTime)
  {
    this.Aura.Execute(iOwner, iDeltaTime);
    if (this.Aura.VisualCategory == VisualCategory.None)
      return;
    this.mAlpha = Math.Min(this.mAlpha + iDeltaTime, 1f);
    this.mRotation = MathHelper.WrapAngle(this.mRotation + iDeltaTime * 0.2f);
    Vector4 iColor = new Vector4();
    iColor.X = this.Aura.Color.X;
    iColor.Y = this.Aura.Color.Y;
    iColor.Z = this.Aura.Color.Z;
    Vector2 iScale = new Vector2();
    iScale.X = iScale.Y = 1.5f;
    Vector3 iNormal = new Vector3();
    iNormal.Y = 1f;
    Vector3 position = iOwner.Position;
    Segment iSeg = new Segment();
    iSeg.Delta.Y = -8f;
    GameScene currentScene = iOwner.PlayState.Level.CurrentScene;
    Vector3 result1 = new Vector3(0.0f, 0.0f, 1f);
    Quaternion result2;
    Quaternion.CreateFromYawPitchRoll(this.mRotation, 0.0f, 0.0f, out result2);
    Vector3.Transform(ref result1, ref result2, out result1);
    DecalManager instance = DecalManager.Instance;
    float yaw = 1.04719758f;
    Matrix result3 = new Matrix();
    result3.M11 = -1.5f;
    result3.M23 = 1.5f;
    result3.M32 = 1f;
    result3.M44 = 1f;
    Matrix.Transform(ref result3, ref result2, out result3);
    Quaternion.CreateFromYawPitchRoll(yaw, 0.0f, 0.0f, out result2);
    fixed (ulong* numPtr = this.Decals)
    {
      float mAlpha = this.mAlpha;
      if (iOwner.PlayState.IsInCutscene)
        this.mAlpha = 0.0f;
      for (int index = 0; index < 6; ++index)
      {
        Vector3.Transform(ref result1, ref result2, out result1);
        Vector3.Multiply(ref result1, this.Aura.Radius, out iSeg.Origin);
        Vector3.Add(ref iSeg.Origin, ref position, out iSeg.Origin);
        iSeg.Origin.Y += 4f;
        Matrix.Transform(ref result3, ref result2, out result3);
        Vector3 iPosition;
        if (!currentScene.SegmentIntersect(out float _, out iPosition, out Vector3 _, iSeg))
          iSeg.GetPoint(0.5f, out iPosition);
        result3.M41 = iPosition.X;
        result3.M42 = iPosition.Y;
        result3.M43 = iPosition.Z;
        if (!instance.SetDecal(ref ((DecalManager.DecalReference*) numPtr)[index], 1f, ref result3, this.mAlpha * 0.666f))
          instance.AddAlphaBlendedDecal((Decal) ((byte) 29 + this.Aura.VisualCategory - (byte) 1), (AnimatedLevelPart) null, ref iScale, ref iPosition, new Vector3?(result1), ref iNormal, 1f, ref iColor, out ((DecalManager.DecalReference*) numPtr)[index]);
      }
      this.mAlpha = mAlpha;
    }
  }
}
