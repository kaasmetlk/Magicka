// Decompiled with JetBrains decompiler
// Type: PolygonHead.VertexPositionNormalTextureColor
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead;

public struct VertexPositionNormalTextureColor
{
  public const int SizeInBytes = 36;
  public Vector3 Position;
  public Vector3 Normal;
  public Vector2 TexCoord;
  public Color Color;
  public static readonly VertexElement[] VertexElements = new VertexElement[4]
  {
    new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
    new VertexElement((short) 0, (short) 12, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, (byte) 0),
    new VertexElement((short) 0, (short) 24, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
    new VertexElement((short) 0, (short) 32 /*0x20*/, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 0)
  };
}
