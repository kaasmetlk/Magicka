// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.BuffModifyHitPoints
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct BuffModifyHitPoints
{
  public float HitPointsMultiplier;
  public float HitPointsModifier;

  public BuffModifyHitPoints(float iHitPointsMultiplier, float iHitPointsModififier)
  {
    this.HitPointsMultiplier = iHitPointsMultiplier;
    this.HitPointsModifier = iHitPointsModififier;
  }

  public BuffModifyHitPoints(ContentReader iInput)
  {
    this.HitPointsMultiplier = iInput.ReadSingle();
    this.HitPointsModifier = iInput.ReadSingle();
  }

  public float Execute(Character iOwner)
  {
    iOwner.MaxHitPoints *= this.HitPointsMultiplier;
    iOwner.MaxHitPoints += this.HitPointsModifier;
    return 1f;
  }
}
