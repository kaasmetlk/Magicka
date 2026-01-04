// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.BuffReduceAgro
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct BuffReduceAgro
{
  public float Amount;

  public BuffReduceAgro(float iAmount) => this.Amount = iAmount;

  public BuffReduceAgro(ContentReader iInput) => this.Amount = iInput.ReadSingle();

  public float Execute(Character iOwner) => this.Amount;
}
