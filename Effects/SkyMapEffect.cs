// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.SkyMapEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class SkyMapEffect(GraphicsDevice iDevice, EffectPool iPool) : PostProcessingEffect(iDevice, SkyMapEffectCode.CODE, iPool)
{
  private EffectParameter mSourceDimensionsParameter;
  private EffectParameter mColorParameter;

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mColorParameter = this.Parameters["Color"];
    this.mSourceDimensionsParameter = this.Parameters["SourceDimensions"];
  }

  public Vector2 SourceDimensions
  {
    get => this.mSourceDimensionsParameter.GetValueVector2();
    set => this.mSourceDimensionsParameter.SetValue(value);
  }

  public Vector3 Color
  {
    get => this.mColorParameter.GetValueVector3();
    set => this.mColorParameter.SetValue(value);
  }
}
