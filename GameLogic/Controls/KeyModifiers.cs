// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.KeyModifiers
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.GameLogic.Controls;

[Flags]
internal enum KeyModifiers
{
  None = 0,
  LeftControl = 1,
  RightControl = 2,
  Control = RightControl | LeftControl, // 0x00000003
  LeftAlt = 4,
  RightAlt = 8,
  Alt = RightAlt | LeftAlt, // 0x0000000C
  LeftShift = 16, // 0x00000010
  RightShift = 32, // 0x00000020
  Shift = RightShift | LeftShift, // 0x00000030
}
