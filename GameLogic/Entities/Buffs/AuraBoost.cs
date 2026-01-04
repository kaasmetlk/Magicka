// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.AuraBoost
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

public struct AuraBoost
{
  public float Magnitude;

  public AuraBoost(float iMagnitude) => this.Magnitude = iMagnitude;

  public AuraBoost(ContentReader iInput) => this.Magnitude = iInput.ReadSingle();

  public float Execute(Character iOwner, AuraTarget iAuraTarget, int iEffect, float iRadius) => 1f;
}
