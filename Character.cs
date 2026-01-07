// Decompiled with JetBrains decompiler
// Type: PolygonHead.Character
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;

#nullable disable
namespace PolygonHead;

public struct Character
{
  public CharacterVertex VertexA;
  public CharacterVertex VertexB;
  public CharacterVertex VertexC;
  public CharacterVertex VertexD;

  public Vector4 Color
  {
    get => this.VertexA.Color;
    set
    {
      this.VertexA.Color = value;
      this.VertexB.Color = value;
      this.VertexC.Color = value;
      this.VertexD.Color = value;
    }
  }

  public void ApplyOffset(ref Vector2 iOffset)
  {
    this.VertexA.Position.X += iOffset.X;
    this.VertexA.Position.Y += iOffset.Y;
    this.VertexB.Position.X += iOffset.X;
    this.VertexB.Position.Y += iOffset.Y;
    this.VertexC.Position.X += iOffset.X;
    this.VertexC.Position.Y += iOffset.Y;
    this.VertexD.Position.X += iOffset.X;
    this.VertexD.Position.Y += iOffset.Y;
  }

  public static Character FromGlyph(
    Glyph iGlyph,
    Vector4 iColor,
    int iTextureWidth,
    float iTextureHeight)
  {
    Character character = new Character();
    if (iGlyph.ForceWhite)
      iColor.X = iColor.Y = iColor.Z = 1f;
    Vector2 vector2_1 = new Vector2();
    vector2_1.X = (float) iGlyph.Origin.X / (float) iTextureWidth;
    vector2_1.Y = (float) iGlyph.Origin.Y / iTextureHeight;
    Vector2 vector2_2 = new Vector2();
    vector2_2.X = vector2_1.X + (float) iGlyph.Size.X / (float) iTextureWidth;
    vector2_2.Y = vector2_1.Y + (float) iGlyph.Size.Y / iTextureHeight;
    character.VertexA.Position.X = (float) iGlyph.LeftSideBearing;
    character.VertexA.Position.Y = 0.0f;
    character.VertexA.TexCoord.X = vector2_1.X;
    character.VertexA.TexCoord.Y = vector2_1.Y;
    character.VertexA.Color = iColor;
    character.VertexB.Position.X = (float) (iGlyph.LeftSideBearing + iGlyph.Size.X);
    character.VertexB.Position.Y = 0.0f;
    character.VertexB.TexCoord.X = vector2_2.X;
    character.VertexB.TexCoord.Y = vector2_1.Y;
    character.VertexB.Color = iColor;
    character.VertexC.Position.X = (float) (iGlyph.LeftSideBearing + iGlyph.Size.X);
    character.VertexC.Position.Y = (float) iGlyph.Size.Y;
    character.VertexC.TexCoord.X = vector2_2.X;
    character.VertexC.TexCoord.Y = vector2_2.Y;
    character.VertexC.Color = iColor;
    character.VertexD.Position.X = (float) iGlyph.LeftSideBearing;
    character.VertexD.Position.Y = (float) iGlyph.Size.Y;
    character.VertexD.TexCoord.X = vector2_1.X;
    character.VertexD.TexCoord.Y = vector2_2.Y;
    character.VertexD.Color = iColor;
    return character;
  }
}
