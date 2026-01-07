// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.DirectionalLightEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

internal class DirectionalLightEffect(GraphicsDevice iGraphicsDevice, EffectPool iPool) : 
  PostProcessingEffect(iGraphicsDevice, DirectionalLightEffectCode.CODE, iPool)
{
  public static readonly int TYPEHASH = typeof (DirectionalLightEffect).GetHashCode();
  private EffectTechnique mWithShadowsTechnique;
  private EffectTechnique mWithoutShadowsTechnique;
  private EffectParameter mInverseCameraViewProjectionParameter;
  private EffectParameter mLightViewProjectionParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mLightDirectionParameter;
  private EffectParameter mDiffuseColorParameter;
  private EffectParameter mAmbientColorParameter;
  private EffectParameter mSpecularAmountParameter;
  private EffectParameter mShadowMapSizeParameter;
  private EffectParameter mSnappedGroundPosition;
  private EffectParameter mProjectedTextureParameter;
  private EffectParameter mProjectedTextureOffsetParameter;
  private EffectParameter mProjectedTextureScaleParameter;
  private EffectParameter mProjectedTextureIntensityParameter;
  private EffectParameter mProjectedTextureEnabledParameter;

  public override void CacheParameters()
  {
    base.CacheParameters();
    this.mWithShadowsTechnique = this.Techniques["WithShadows"];
    this.mWithoutShadowsTechnique = this.Techniques["WithoutShadows"];
    this.mInverseCameraViewProjectionParameter = this.Parameters["InverseViewProjection"];
    this.mLightViewProjectionParameter = this.Parameters["LightViewProjection"];
    this.mCameraPositionParameter = this.Parameters["CameraPosition"];
    this.mLightDirectionParameter = this.Parameters["LightDirection"];
    this.mDiffuseColorParameter = this.Parameters["DiffuseColor"];
    this.mAmbientColorParameter = this.Parameters["AmbientColor"];
    this.mSpecularAmountParameter = this.Parameters["SpecularAmount"];
    this.mShadowMapSizeParameter = this.Parameters["ShadowMapSize"];
    this.mSnappedGroundPosition = this.Parameters["SnappedGroundPosition"];
    this.mProjectedTextureParameter = this.Parameters["ProjectedTexture"];
    this.mProjectedTextureOffsetParameter = this.Parameters["ProjectedTextureOffset"];
    this.mProjectedTextureScaleParameter = this.Parameters["ProjectedTextureScale"];
    this.mProjectedTextureIntensityParameter = this.Parameters["ProjectedTextureIntensity"];
    this.mProjectedTextureEnabledParameter = this.Parameters["ProjectedTextureEnabled"];
  }

  public void SetTechnique(DirectionalLightEffect.Technique iTechnique)
  {
    switch (iTechnique)
    {
      case DirectionalLightEffect.Technique.WithShadows:
        this.CurrentTechnique = this.mWithShadowsTechnique;
        break;
      case DirectionalLightEffect.Technique.WithoutShadows:
        this.CurrentTechnique = this.mWithoutShadowsTechnique;
        break;
    }
  }

  public Matrix InverseCameraViewProjection
  {
    get => this.mInverseCameraViewProjectionParameter.GetValueMatrix();
    set => this.mInverseCameraViewProjectionParameter.SetValue(value);
  }

  public Matrix LightViewProjection
  {
    get => this.mLightViewProjectionParameter.GetValueMatrix();
    set => this.mLightViewProjectionParameter.SetValue(value);
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public Vector3 LightDirection
  {
    get => this.mLightDirectionParameter.GetValueVector3();
    set => this.mLightDirectionParameter.SetValue(value);
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

  public int ShadowMapSize
  {
    get => this.mShadowMapSizeParameter.GetValueInt32();
    set => this.mShadowMapSizeParameter.SetValue((float) value);
  }

  public Vector3 SnappedGroundPosition
  {
    get => this.mSnappedGroundPosition.GetValueVector3();
    set => this.mSnappedGroundPosition.SetValue(value);
  }

  public Texture2D ProjectedTexture
  {
    get => this.mProjectedTextureParameter.GetValueTexture2D();
    set => this.mProjectedTextureParameter.SetValue((Texture) value);
  }

  public Vector2 ProjectedTextureOffset
  {
    get => this.mProjectedTextureOffsetParameter.GetValueVector2();
    set => this.mProjectedTextureOffsetParameter.SetValue(value);
  }

  public Vector2 ProjectedTextureScale
  {
    get => this.mProjectedTextureScaleParameter.GetValueVector2();
    set => this.mProjectedTextureScaleParameter.SetValue(value);
  }

  public float ProjectedTextureIntensity
  {
    get => this.mProjectedTextureIntensityParameter.GetValueSingle();
    set
    {
      this.mProjectedTextureIntensityParameter.SetValue(value);
      this.mProjectedTextureEnabledParameter.SetValue((double) value > 1.4012984643248171E-45);
    }
  }

  public enum Technique
  {
    WithShadows,
    WithoutShadows,
  }
}
