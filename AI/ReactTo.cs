// Decompiled with JetBrains decompiler
// Type: Magicka.AI.ReactTo
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.AI;

[Flags]
public enum ReactTo : byte
{
  None = 0,
  Attack = 1,
  Proximity = 2,
}
