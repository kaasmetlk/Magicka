// Decompiled with JetBrains decompiler
// Type: Magicka.EntityFeatures
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

[Flags]
public enum EntityFeatures : ushort
{
  None = 0,
  Position = 1,
  Direction = 2,
  Velocity = 4,
  Orientation = 8,
  Character = 16, // 0x0010
  Damageable = 32, // 0x0020
  StatusEffected = 64, // 0x0040
  GenericBool = 128, // 0x0080
  GenericInt = 256, // 0x0100
  GenericFloat = 512, // 0x0200
  WanderAngle = 1024, // 0x0400
  SelfShield = 2048, // 0x0800
  Etherealized = 4096, // 0x1000
  GenericUShort = 8192, // 0x2000
}
