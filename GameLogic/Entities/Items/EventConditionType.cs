// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.EventConditionType
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

[Flags]
public enum EventConditionType : byte
{
  Default = 1,
  Hit = 2,
  Collision = 4,
  Damaged = 8,
  Timer = 16, // 0x10
  Death = 32, // 0x20
  OverKill = 64, // 0x40
}
