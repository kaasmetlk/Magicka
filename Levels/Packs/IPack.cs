// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Packs.IPack
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Packs;

internal interface IPack
{
  bool Enabled { get; set; }

  int Name { get; }

  int Descritpion { get; }

  Vector2 ThumbOffset { get; }

  uint StoreURL { get; }

  HackHelper.License License { get; }

  bool IsUsed { get; }

  void SetUsed(bool forceSave);
}
