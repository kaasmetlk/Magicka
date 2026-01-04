// Decompiled with JetBrains decompiler
// Type: Magicka.Audio.Banks
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.Audio;

[Flags]
public enum Banks : ushort
{
  WaveBank = 1,
  Music = 2,
  Ambience = 4,
  UI = 8,
  Spells = 16, // 0x0010
  Characters = 32, // 0x0020
  Footsteps = 64, // 0x0040
  Weapons = 128, // 0x0080
  Misc = 256, // 0x0100
  Additional = 512, // 0x0200
  AdditionalMusic = 1024, // 0x0400
}
