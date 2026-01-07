// Decompiled with JetBrains decompiler
// Type: PolygonHead.IProjectionObject
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead;

public interface IProjectionObject
{
  int Effect { get; }

  int Technique { get; }

  VertexBuffer Vertices { get; }

  int VerticesHashCode { get; }

  int VertexStride { get; }

  IndexBuffer Indices { get; }

  VertexDeclaration VertexDeclaration { get; }

  bool Cull(BoundingFrustum iViewFrustum);

  void Draw(Microsoft.Xna.Framework.Graphics.Effect iEffect, Texture2D iDepthMap);
}
