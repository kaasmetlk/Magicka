// Decompiled with JetBrains decompiler
// Type: PolygonHead.IPostEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead;

public interface IPostEffect
{
  int ZIndex { get; }

  void Draw(
    float iDeltaTime,
    ref Vector2 iPixelSize,
    ref Matrix iViewMatrix,
    ref Matrix iProjectionMatrix,
    Texture2D iCandidate,
    Texture2D iDepthMap,
    Texture2D iNormalMap);
}
