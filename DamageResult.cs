// Decompiled with JetBrains decompiler
// Type: Magicka.DamageResult
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

[Flags]
public enum DamageResult
{
  None = 0,
  Damaged = 1,
  Hit = 2,
  Knockeddown = 4,
  Knockedback = 8,
  Pushed = 16, // 0x00000010
  Statusadded = 32, // 0x00000020
  Statusremoved = 64, // 0x00000040
  Healed = 128, // 0x00000080
  Deflected = 256, // 0x00000100
  Killed = 512, // 0x00000200
  Pierced = 1024, // 0x00000400
  OverKilled = 2048, // 0x00000800
}
