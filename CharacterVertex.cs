// Decompiled with JetBrains decompiler
// Type: PolygonHead.CharacterVertex
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead;

public struct CharacterVertex
{
  public const int SIZEINBYTES = 32 /*0x20*/;
  public Vector2 Position;
  public Vector2 TexCoord;
  public Vector4 Color;
  public static readonly VertexElement[] VertexElements = new VertexElement[3]
  {
    new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
    new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
    new VertexElement((short) 0, (short) 16 /*0x10*/, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 0)
  };
}
