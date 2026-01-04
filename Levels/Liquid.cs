// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Liquid
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.Levels;

public abstract class Liquid
{
  protected AnimatedLevelPart mParent;

  protected Liquid(AnimatedLevelPart iParent) => this.mParent = iParent;

  public AnimatedLevelPart Parent => this.mParent;

  public static Liquid Read(ContentReader iInput, LevelModel iLevel, AnimatedLevelPart iParent)
  {
    Effect iEffect = iInput.ReadObject<Effect>();
    switch (iEffect)
    {
      case RenderDeferredLiquidEffect _:
        return (Liquid) new Water(iEffect as RenderDeferredLiquidEffect, iInput, iLevel, iParent);
      case LavaEffect _:
        return (Liquid) new Lava(iEffect as LavaEffect, iInput, iLevel, iParent);
      default:
        throw new NotImplementedException();
    }
  }

  public abstract void Initialize();

  public void Update(DataChannel iDataChannel, float iDeltaTime, Scene iScene)
  {
    Matrix identity = Matrix.Identity;
    this.Update(iDataChannel, iDeltaTime, iScene, ref identity, ref identity);
  }

  public abstract void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    Scene iScene,
    ref Matrix iTransform,
    ref Matrix iInvTransform);

  public abstract bool SegmentIntersect(
    out float frac,
    out Vector3 pos,
    out Vector3 normal,
    ref Segment seg,
    bool ignoreBackfaces,
    bool ignoreWater,
    bool ignoreIce);

  protected abstract void Freeze(
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iSpread,
    float iMagnitude);

  public abstract void FreezeAll(float iAmount);

  public abstract CollisionSkin CollisionSkin { get; }

  internal abstract bool AutoFreeze { get; }

  public static unsafe void Freeze(
    GameScene iScene,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iSpread,
    float iMultiplyer,
    ref Damage iDamage)
  {
    fixed (Damage* iDamage1 = &iDamage)
      Liquid.Freeze(iScene, ref iOrigin, ref iDirection, iSpread, iMultiplyer, iDamage1, 1);
  }

  public static unsafe void Freeze(
    GameScene iScene,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iSpread,
    float iMultiplyer,
    ref DamageCollection5 iDamage)
  {
    fixed (Damage* iDamage1 = &iDamage.A)
      Liquid.Freeze(iScene, ref iOrigin, ref iDirection, iSpread, iMultiplyer, iDamage1, 5);
  }

  public static unsafe void Freeze(
    GameScene iScene,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iSpread,
    float iMultiplyer,
    Damage* iDamage,
    int iNrOfDamages)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    for (int index = 0; index < iNrOfDamages; ++index)
    {
      if ((iDamage[index].Element & Elements.Cold) != Elements.None)
        num1 += iDamage[index].Magnitude;
      if ((iDamage[index].Element & Elements.Water) != Elements.None)
        num2 += iDamage[index].Magnitude;
      if ((iDamage[index].Element & Elements.Fire) != Elements.None)
        num3 += iDamage[index].Magnitude;
    }
    if (iScene == null || iScene.Liquids == null)
      return;
    for (int index = 0; index < iScene.Liquids.Length; ++index)
    {
      Liquid liquid = iScene.Liquids[index];
      float num4 = num1 - num3;
      if (liquid is Lava)
        num4 += num2;
      float iMagnitude = (1f - (float) Math.Pow(0.75, (double) Math.Abs(num4) * 3.0)) * 3f * (float) Math.Sign(num4) * iMultiplyer;
      liquid.Freeze(ref iOrigin, ref iDirection, iSpread, iMagnitude);
    }
  }
}
