// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.AuraLifeSteal
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct AuraLifeSteal
{
  public float Amount;

  public AuraLifeSteal(float iAmount) => this.Amount = iAmount;

  public AuraLifeSteal(ContentReader iInput) => this.Amount = iInput.ReadSingle();

  public float Execute(Character iOwner, AuraTarget iAuraTarget, int iEffect, float iRadius)
  {
    return 666f;
  }
}
