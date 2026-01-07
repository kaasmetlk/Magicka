// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.ClearEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

internal class ClearEffect(GraphicsDevice iGraphicsDevice, EffectPool iPool) : PostProcessingEffect(iGraphicsDevice, ClearEffectCode.CODE, iPool)
{
}
