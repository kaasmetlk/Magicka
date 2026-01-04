// Decompiled with JetBrains decompiler
// Type: Magicka.Elements
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

[Flags]
public enum Elements
{
  None = 0,
  Earth = 1,
  Physical = Earth, // 0x00000001
  Water = 2,
  Cold = 4,
  Fire = 8,
  Lightning = 16, // 0x00000010
  Arcane = 32, // 0x00000020
  Life = 64, // 0x00000040
  Shield = 128, // 0x00000080
  Ice = 256, // 0x00000100
  Steam = 512, // 0x00000200
  Poison = 1024, // 0x00000400
  Offensive = 65343, // 0x0000FF3F
  Defensive = Shield | Arcane | Lightning, // 0x000000B0
  All = 65535, // 0x0000FFFF
  Magick = All, // 0x0000FFFF
  Basic = Defensive | Life | Fire | Cold | Water | Physical, // 0x000000FF
  Instant = Steam | Ice | Life | Arcane | Lightning | Physical, // 0x00000371
  InstantPhysical = Ice | Life | Arcane | Lightning | Physical, // 0x00000171
  InstantNonPhysical = Steam | Life | Arcane | Lightning, // 0x00000270
  StatusEffects = Poison | Steam | Life | Fire | Cold | Water, // 0x0000064E
  ShieldElements = Shield | Life | Arcane, // 0x000000E0
  PhysicalElements = Ice | Physical, // 0x00000101
  Beams = Life | Arcane, // 0x00000060
}
