// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Popup.RenderSection
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.GameLogic.UI.Popup;

internal struct RenderSection(
  Vector2 iPosition,
  Vector2 iSize,
  Vector2 iTextureOffset,
  Vector2 iTextureSize)
{
  public readonly Vector2 Position = iPosition;
  public readonly Vector2 Size = iSize;
  public readonly Vector2 TextureOffset = iTextureOffset;
  public readonly Vector2 TextureSize = iTextureSize;
}
