// Decompiled with JetBrains decompiler
// Type: Magicka.ControllerDirection
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

[Flags]
internal enum ControllerDirection : byte
{
  Dead = 255, // 0xFF
  Center = 0,
  Right = 1,
  Up = 2,
  Left = 4,
  Down = 8,
  UpRight = Up | Right, // 0x03
  UpLeft = Left | Up, // 0x06
  DownRight = Down | Right, // 0x09
  DownLeft = Down | Left, // 0x0C
}
