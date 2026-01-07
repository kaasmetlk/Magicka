// Decompiled with JetBrains decompiler
// Type: PolygonHead.IPreRenderRenderer
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;

#nullable disable
namespace PolygonHead;

public interface IPreRenderRenderer
{
  void PreRenderUpdate(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Matrix iViewProjectionMatrix,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection);
}
