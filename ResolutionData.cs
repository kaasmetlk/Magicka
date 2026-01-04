// Decompiled with JetBrains decompiler
// Type: Magicka.ResolutionData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace Magicka;

internal struct ResolutionData : IComparable<ResolutionData>
{
  public int Width;
  public int Height;
  public int RefreshRate;

  public ResolutionData(DisplayMode iMode)
  {
    this.RefreshRate = iMode.RefreshRate;
    this.Width = iMode.Width;
    this.Height = iMode.Height;
  }

  public int CompareTo(ResolutionData other)
  {
    if (other.RefreshRate != this.RefreshRate)
      return this.RefreshRate - other.RefreshRate;
    return other.Width != this.Width ? this.Width - other.Width : this.Height - other.Height;
  }
}
