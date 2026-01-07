// Decompiled with JetBrains decompiler
// Type: PolygonHead.Pipeline.RenderDeferredLiquidEffectReader
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework.Content;
using PolygonHead.Effects;

#nullable disable
namespace PolygonHead.Pipeline;

public class RenderDeferredLiquidEffectReader : ContentTypeReader<RenderDeferredLiquidEffect>
{
  protected override RenderDeferredLiquidEffect Read(
    ContentReader input,
    RenderDeferredLiquidEffect existingInstance)
  {
    return RenderDeferredLiquidEffect.Read(input);
  }
}
