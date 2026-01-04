// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.BuffModifySpellRange
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct BuffModifySpellRange
{
  public float RangeMultiplier;
  public float RangeModifier;

  public BuffModifySpellRange(float iRangeMultiplier, float iRangeModififier)
  {
    this.RangeMultiplier = iRangeMultiplier;
    this.RangeModifier = iRangeModififier;
  }

  public BuffModifySpellRange(ContentReader iInput)
  {
    this.RangeMultiplier = iInput.ReadSingle();
    this.RangeModifier = iInput.ReadSingle();
  }

  public float Execute(Character iOwner) => 1f;
}
