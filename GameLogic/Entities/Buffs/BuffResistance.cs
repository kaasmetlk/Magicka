// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.BuffResistance
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct BuffResistance
{
  public Resistance Resistance;

  public BuffResistance(Resistance iResistance) => this.Resistance = iResistance;

  public BuffResistance(ContentReader iInput)
  {
    this.Resistance = new Resistance();
    this.Resistance.ResistanceAgainst = (Elements) iInput.ReadInt32();
    this.Resistance.Modifier = iInput.ReadSingle();
    this.Resistance.Multiplier = iInput.ReadSingle();
    this.Resistance.StatusResistance = iInput.ReadBoolean();
  }

  public float Execute(Character iOwner) => 1f;
}
