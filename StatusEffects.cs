// Decompiled with JetBrains decompiler
// Type: Magicka.StatusEffects
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

[Flags]
public enum StatusEffects : short
{
  None = 0,
  Burning = 1,
  Wet = 2,
  Frozen = 4,
  Cold = 8,
  Poisoned = 16, // 0x0010
  Healing = 32, // 0x0020
  Life = Healing, // 0x0020
  Greased = 64, // 0x0040
  Steamed = 128, // 0x0080
  Bleeding = 256, // 0x0100
  NumberOfTypes = 512, // 0x0200
}
