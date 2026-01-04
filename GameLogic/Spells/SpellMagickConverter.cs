// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Spells.SpellMagickConverter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Spells;

[StructLayout(LayoutKind.Explicit)]
public struct SpellMagickConverter
{
  [FieldOffset(0)]
  public Spell Spell;
  [FieldOffset(0)]
  public Magick Magick;

  public bool IsMagick => this.Magick.Element == Elements.All;
}
