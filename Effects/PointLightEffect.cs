// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.PointLightEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class PointLightEffect : Effect
{
  public static readonly int TYPEHASH = typeof (PointLightEffect).GetHashCode();
  private EffectParameter mLightRadiusParameter;
  private EffectParameter mLightPositionParameter;
  private EffectParameter mWorldParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mDiffuseColorParameter;
  private EffectParameter mAmbientColorParameter;
  private EffectParameter mSpecularAmountParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mInverseViewProjectionParameter;
  private EffectParameter mNormalMapParameter;
  private EffectParameter mDepthMapParameter;
  private EffectParameter mShadowMapParameter;
  private EffectParameter mHalfPixelParameter;

  public PointLightEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, PointLightEffectCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mDiffuseColorParameter = this.Parameters[nameof (DiffuseColor)];
    this.mAmbientColorParameter = this.Parameters[nameof (AmbientColor)];
    this.mSpecularAmountParameter = this.Parameters[nameof (SpecularAmount)];
    this.mInverseViewProjectionParameter = this.Parameters[nameof (InverseViewProjection)];
    this.mLightRadiusParameter = this.Parameters[nameof (LightRadius)];
    this.mLightPositionParameter = this.Parameters[nameof (LightPosition)];
    this.mNormalMapParameter = this.Parameters[nameof (NormalMap)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
    this.mShadowMapParameter = this.Parameters[nameof (ShadowMap)];
    this.mHalfPixelParameter = this.Parameters[nameof (HalfPixel)];
  }

  public void SetTechnique(PointLightEffect.Technique iTechnique)
  {
    this.CurrentTechnique = this.Techniques[(int) iTechnique];
  }

  public float LightRadius
  {
    get => this.mLightRadiusParameter.GetValueSingle();
    set => this.mLightRadiusParameter.SetValue(value);
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

  public TextureCube ShadowMap
  {
    get => this.mShadowMapParameter.GetValueTextureCube();
    set => this.mShadowMapParameter.SetValue((Texture) value);
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
  }
}
