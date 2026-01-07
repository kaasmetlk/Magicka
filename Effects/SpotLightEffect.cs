// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.SpotLightEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class SpotLightEffect : Effect
{
  public static readonly int TYPEHASH = typeof (SpotLightEffect).GetHashCode();
  private EffectTechnique mWithShadowsTechnique;
  private EffectTechnique mWithoutShadowsTechnique;
  private EffectTechnique mWithShadowsAttenuationTechnique;
  private EffectTechnique mWithoutShadowsAttenuationTechnique;
  private EffectParameter mLightRangeParameter;
  private EffectParameter mLightPositionParameter;
  private EffectParameter mLightDirectionParameter;
  private EffectParameter mLightCutoffAngleParameter;
  private EffectParameter mLightSharpnessParameter;
  private EffectParameter mWorldParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mDiffuseColorParameter;
  private EffectParameter mAmbientColorParameter;
  private EffectParameter mSpecularAmountParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mInverseViewProjectionParameter;
  private EffectParameter mLightViewProjectionParameter;
  private EffectParameter mNormalMapParameter;
  private EffectParameter mDepthMapParameter;
  private EffectParameter mShadowMapParameter;
  private EffectParameter mShadowMapSizeParameter;
  private EffectParameter mHalfPixelParameter;

  public SpotLightEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, SpotLightEffectCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mWithShadowsTechnique = this.Techniques["WithShadows"];
    this.mWithoutShadowsTechnique = this.Techniques["WithoutShadows"];
    this.mWithShadowsAttenuationTechnique = this.Techniques["WithShadowsAttenuation"];
    this.mWithoutShadowsAttenuationTechnique = this.Techniques["WithoutShadowsAttenuation"];
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mDiffuseColorParameter = this.Parameters[nameof (DiffuseColor)];
    this.mAmbientColorParameter = this.Parameters[nameof (AmbientColor)];
    this.mSpecularAmountParameter = this.Parameters[nameof (SpecularAmount)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mInverseViewProjectionParameter = this.Parameters[nameof (InverseViewProjection)];
    this.mLightViewProjectionParameter = this.Parameters[nameof (LightViewProjection)];
    this.mLightRangeParameter = this.Parameters[nameof (LightRange)];
    this.mLightPositionParameter = this.Parameters[nameof (LightPosition)];
    this.mLightDirectionParameter = this.Parameters[nameof (LightDirection)];
    this.mLightCutoffAngleParameter = this.Parameters[nameof (LightCutoffAngle)];
    this.mLightSharpnessParameter = this.Parameters[nameof (LightSharpness)];
    this.mNormalMapParameter = this.Parameters[nameof (NormalMap)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
    this.mShadowMapParameter = this.Parameters[nameof (ShadowMap)];
    this.mShadowMapSizeParameter = this.Parameters[nameof (ShadowMapSize)];
    this.mHalfPixelParameter = this.Parameters[nameof (HalfPixel)];
  }

  public void SetTechnique(SpotLightEffect.Technique iTechnique)
  {
    switch (iTechnique)
    {
      case SpotLightEffect.Technique.WithShadows:
        this.CurrentTechnique = this.mWithShadowsAttenuationTechnique;
        break;
      case SpotLightEffect.Technique.WithoutShadows:
        this.CurrentTechnique = this.mWithoutShadowsAttenuationTechnique;
        break;
      case SpotLightEffect.Technique.WithShadowsWithoutAttenuation:
        this.CurrentTechnique = this.mWithShadowsTechnique;
        break;
      case SpotLightEffect.Technique.WithoutShadowsWithoutAttenuation:
        this.CurrentTechnique = this.mWithoutShadowsTechnique;
        break;
    }
  }

  public float LightRange
  {
    get => this.mLightRangeParameter.GetValueSingle();
    set => this.mLightRangeParameter.SetValue(value);
  }

  public Vector3 LightPosition
  {
    get => this.mLightPositionParameter.GetValueVector3();
    set => this.mLightPositionParameter.SetValue(value);
  }

  public Matrix World
  {
    get => this.mWorldParameter.GetValueMatrix();
    set => this.mWorldParameter.SetValue(value);
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Vector3 DiffuseColor
  {
    get => this.mDiffuseColorParameter.GetValueVector3();
    set => this.mDiffuseColorParameter.SetValue(value);
  }

  public Vector3 AmbientColor
  {
    get => this.mAmbientColorParameter.GetValueVector3();
    set => this.mAmbientColorParameter.SetValue(value);
  }

  public float SpecularAmount
  {
    get => this.mSpecularAmountParameter.GetValueSingle();
    set => this.mSpecularAmountParameter.SetValue(value);
  }

  public float LightCutoffAngle
  {
    get => this.mLightCutoffAngleParameter.GetValueSingle();
    set => this.mLightCutoffAngleParameter.SetValue(value);
  }

  public float LightSharpness
  {
    get => this.mLightSharpnessParameter.GetValueSingle();
    set => this.mLightSharpnessParameter.SetValue(value);
  }

  public Vector3 LightDirection
  {
    get => this.mLightDirectionParameter.GetValueVector3();
    set => this.mLightDirectionParameter.SetValue(value);
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public Matrix InverseViewProjection
  {
    get => this.mInverseViewProjectionParameter.GetValueMatrix();
    set => this.mInverseViewProjectionParameter.SetValue(value);
  }

  public Matrix LightViewProjection
  {
    get => this.mLightViewProjectionParameter.GetValueMatrix();
    set => this.mLightViewProjectionParameter.SetValue(value);
  }

  public Texture2D NormalMap
  {
    get => this.mNormalMapParameter.GetValueTexture2D();
    set => this.mNormalMapParameter.SetValue((Texture) value);
  }

  public Texture2D DepthMap
  {
    get => this.mDepthMapParameter.GetValueTexture2D();
    set => this.mDepthMapParameter.SetValue((Texture) value);
  }

  public Texture2D ShadowMap
  {
    get => this.mShadowMapParameter.GetValueTexture2D();
    set => this.mShadowMapParameter.SetValue((Texture) value);
  }

  public float ShadowMapSize
  {
    get => this.mShadowMapSizeParameter.GetValueSingle();
    set => this.mShadowMapSizeParameter.SetValue(value);
  }

  public Vector2 HalfPixel
  {
    get => this.mHalfPixelParameter.GetValueVector2();
    set => this.mHalfPixelParameter.SetValue(value);
  }

  public enum Technique
  {
    WithShadows,
    WithoutShadows,
    WithShadowsWithoutAttenuation,
    WithoutShadowsWithoutAttenuation,
  }
}
