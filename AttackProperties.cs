// Decompiled with JetBrains decompiler
// Type: Magicka.AttackProperties
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

[Flags]
public enum AttackProperties : short
{
  Damage = 1,
  Knockdown = 2,
  Pushed = 4,
  Knockback = Pushed | Knockdown, // 0x0006
  Piercing = 8,
  ArmourPiercing = 16, // 0x0010
  Status = 32, // 0x0020
  Entanglement = 64, // 0x0040
  Stun = 128, // 0x0080
  Bleed = 256, // 0x0100
  NumberOfTypes = 512, // 0x0200
}
