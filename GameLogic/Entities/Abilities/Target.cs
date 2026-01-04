// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.Target
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities;

public enum Target : byte
{
  S = 1,
  Self = 1,
  E = 2,
  Enemy = 2,
  F = 3,
  Friendly = 3,
}
