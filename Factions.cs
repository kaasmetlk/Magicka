// Decompiled with JetBrains decompiler
// Type: Magicka.Factions
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

[Flags]
public enum Factions
{
  NONE = 0,
  EVIL = 1,
  WILD = 2,
  FRIENDLY = 4,
  DEMON = 8,
  UNDEAD = 16, // 0x00000010
  HUMAN = 32, // 0x00000020
  WIZARD = 64, // 0x00000040
  NEUTRAL = 255, // 0x000000FF
  PLAYER0 = 256, // 0x00000100
  PLAYER1 = 512, // 0x00000200
  PLAYER2 = 1024, // 0x00000400
  PLAYER3 = 2048, // 0x00000800
  TEAM_RED = 4096, // 0x00001000
  TEAM_BLUE = 8192, // 0x00002000
  PLAYERS = TEAM_BLUE | TEAM_RED | PLAYER3 | PLAYER2 | PLAYER1 | PLAYER0, // 0x00003F00
  NUMBER_OF_FACTIONS = DEMON | FRIENDLY | WILD | EVIL, // 0x0000000F
}
