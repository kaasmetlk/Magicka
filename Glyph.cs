// Decompiled with JetBrains decompiler
// Type: PolygonHead.Glyph
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace PolygonHead;

public struct Glyph
{
  public char Character;
  public Point Origin;
  public Point Size;
  public int AdvanceWidth;
  public int LeftSideBearing;
  public bool ForceWhite;

  public static Glyph Read(ContentReader iInput)
  {
    Glyph glyph;
    glyph.Character = iInput.ReadChar();
    glyph.Origin = new Point();
    glyph.Origin.X = iInput.ReadInt32();
    glyph.Origin.Y = iInput.ReadInt32();
    glyph.Size = new Point();
    glyph.Size.X = iInput.ReadInt32();
    glyph.Size.Y = iInput.ReadInt32();
    glyph.AdvanceWidth = iInput.ReadInt32();
    glyph.LeftSideBearing = iInput.ReadInt32();
    glyph.ForceWhite = iInput.ReadBoolean();
    return glyph;
  }
}
