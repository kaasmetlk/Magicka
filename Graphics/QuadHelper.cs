// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.QuadHelper
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Graphics;

internal static class QuadHelper
{
  public static int CreateQuadFan(Vector4[] iVertices, int iIndex, Vector2 iOrigin, Vector2 iSize)
  {
    return QuadHelper.CreateQuadFan(iVertices, iIndex, iOrigin, iSize, Vector2.Zero, Vector2.One);
  }

  public static int CreateQuadFan(
    Vector4[] iVertices,
    int iIndex,
    Vector2 iOrigin,
    Vector2 iSize,
    Vector2 iUVOffset,
    Vector2 iUVSize)
  {
    iVertices[iIndex].X = 0.0f - iOrigin.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y;
    iVertices[iIndex].Z = iUVOffset.X;
    iVertices[iIndex].W = iUVOffset.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f - iOrigin.X + iSize.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y;
    iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iIndex].W = iUVOffset.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f - iOrigin.X + iSize.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y + iSize.Y;
    iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f - iOrigin.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y + iSize.Y;
    iVertices[iIndex].Z = iUVOffset.X;
    iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
    return 4;
  }

  public static int CreateQuadList(
    Vector4[] iVertices,
    int iIndex,
    Vector2 iOrigin,
    Vector2 iSize)
  {
    return QuadHelper.CreateQuadList(iVertices, iIndex, iOrigin, iSize, Vector2.Zero, Vector2.One);
  }

  public static int CreateQuadList(
    Vector4[] iVertices,
    int iIndex,
    Vector2 iOrigin,
    Vector2 iSize,
    Vector2 iUVOffset,
    Vector2 iUVSize)
  {
    iVertices[iIndex].X = 0.0f - iOrigin.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y;
    iVertices[iIndex].Z = iUVOffset.X;
    iVertices[iIndex].W = iUVOffset.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f - iOrigin.X + iSize.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y;
    iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iIndex].W = iUVOffset.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f - iOrigin.X + iSize.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y + iSize.Y;
    iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f - iOrigin.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y;
    iVertices[iIndex].Z = iUVOffset.X;
    iVertices[iIndex].W = iUVOffset.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f - iOrigin.X + iSize.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y + iSize.Y;
    iVertices[iIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
    ++iIndex;
    iVertices[iIndex].X = 0.0f - iOrigin.X;
    iVertices[iIndex].Y = 0.0f - iOrigin.Y + iSize.Y;
    iVertices[iIndex].Z = iUVOffset.X;
    iVertices[iIndex].W = iUVOffset.Y + iUVSize.Y;
    return 6;
  }
}
