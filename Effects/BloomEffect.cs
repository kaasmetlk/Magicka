// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.BloomEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

internal class BloomEffect(GraphicsDevice iGraphicsDevice, EffectPool iPool) : PostProcessingEffect(iGraphicsDevice, BloomEffectCode.CODE, iPool)
{
  private EffectParameter mThresholdParameter;
  private EffectParameter mMultiplierParameter;
  private EffectParameter mSigmaParameter;
  private EffectParameter mSourceDimensionsParameter;

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mThresholdParameter = this.Parameters["Threshold"];
    this.mMultiplierParameter = this.Parameters["Multiplier"];
    this.mSigmaParameter = this.Parameters["Sigma"];
    this.mSourceDimensionsParameter = this.Parameters["SourceDimensions"];
  }

  public void SetTechnique(BloomEffect.Technique iTechnique)
  {
    this.CurrentTechnique = this.Techniques[(int) iTechnique];
  }

  public float Threshold
  {
    get => this.mThresholdParameter.GetValueSingle();
    set => this.mThresholdParameter.SetValue(value);
  }

  public float Multiplier
  {
    get => this.mMultiplierParameter.GetValueSingle();
    set => this.mMultiplierParameter.SetValue(value);
  }

  public float Sigma
  {
    get => this.mSigmaParameter.GetValueSingle();
    set => this.mSigmaParameter.SetValue(value);
  }

  public Vector2 SourceDimensions
  {
    get => this.mSourceDimensionsParameter.GetValueVector2();
    set => this.mSourceDimensionsParameter.SetValue(value);
  }

  public enum Technique
  {
    HorizontalBlur,
    VerticalBlur,
    Combine,
    DownSample,
  }
}
