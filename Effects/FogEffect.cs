// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.FogEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class FogEffect(GraphicsDevice iGraphicsDevice, EffectPool iPool) : PostProcessingEffect(iGraphicsDevice, FogEffectCode.CODE, iPool)
{
  private EffectTechnique mExponentialTechnique;
  private EffectTechnique mExponential2Technique;
  private EffectTechnique mLinearTechnique;
  private EffectParameter mTextureOffset0Parameter;
  private EffectParameter mTextureOffset1Parameter;
  private EffectParameter mColorParameter;
  private EffectParameter mAlphaParameter;
  private EffectParameter mFogStartParameter;
  private EffectParameter mFogEndParameter;
  private EffectParameter mFogDensityParameter;

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mExponentialTechnique = this.Techniques["Exponential"];
    this.mExponential2Technique = this.Techniques["Exponential2"];
    this.mLinearTechnique = this.Techniques["Linear"];
    this.mTextureOffset0Parameter = this.Parameters["TextureOffset0"];
    this.mTextureOffset1Parameter = this.Parameters["TextureOffset1"];
    this.mColorParameter = this.Parameters["Color"];
    this.mAlphaParameter = this.Parameters["Alpha"];
    this.mFogStartParameter = this.Parameters["FogStart"];
    this.mFogEndParameter = this.Parameters["FogEnd"];
    this.mFogDensityParameter = this.Parameters["FogDensity"];
  }

  public void SetTechnique(FogEffect.Technique iTechnique)
  {
    switch (iTechnique)
    {
      case FogEffect.Technique.Exponential:
        this.CurrentTechnique = this.mExponentialTechnique;
        break;
      case FogEffect.Technique.ExponentialSqare:
        this.CurrentTechnique = this.mExponential2Technique;
        break;
      case FogEffect.Technique.Linear:
        this.CurrentTechnique = this.mLinearTechnique;
        break;
    }
  }

  public Vector2 TextureOffset0
  {
    get => this.mTextureOffset0Parameter.GetValueVector2();
    set => this.mTextureOffset0Parameter.SetValue(value);
  }

  public Vector2 TextureOffset1
  {
    get => this.mTextureOffset1Parameter.GetValueVector2();
    set => this.mTextureOffset1Parameter.SetValue(value);
  }

  public Vector3 Color
  {
    get => this.mColorParameter.GetValueVector3();
    set => this.mColorParameter.SetValue(value);
  }

  public float Alpha
  {
    get => this.mAlphaParameter.GetValueSingle();
    set => this.mAlphaParameter.SetValue(value);
  }

  public float FogStart
  {
    get => this.mFogStartParameter.GetValueSingle();
    set => this.mFogStartParameter.SetValue(value);
  }

  public float FogEnd
  {
    get => this.mFogEndParameter.GetValueSingle();
    set => this.mFogEndParameter.SetValue(value);
  }

  public float FogDensity
  {
    get => this.mFogDensityParameter.GetValueSingle();
    set => this.mFogDensityParameter.SetValue(value);
  }

  public enum Technique
  {
    Exponential,
    ExponentialSqare,
    Linear,
  }
}
