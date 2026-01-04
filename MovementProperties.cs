// Decompiled with JetBrains decompiler
// Type: Magicka.MovementProperties
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka;

[Flags]
public enum MovementProperties
{
  Default = 0,
  Water = 1,
  Jump = 2,
  Fly = 4,
  Dynamic = 128, // 0x00000080
  All = 255, // 0x000000FF
}
