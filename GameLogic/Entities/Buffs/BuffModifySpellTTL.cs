// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.BuffModifySpellTTL
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct BuffModifySpellTTL
{
  public float TTLMultiplier;
  public float TTLModifier;

  public BuffModifySpellTTL(float iTTLMultiplier, float iTTLModififier)
  {
    this.TTLMultiplier = iTTLMultiplier;
    this.TTLModifier = iTTLModififier;
  }

  public BuffModifySpellTTL(ContentReader iInput)
  {
    this.TTLMultiplier = iInput.ReadSingle();
    this.TTLModifier = iInput.ReadSingle();
  }

  public float Execute(Character iOwner) => 1f;
}
