// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.RenderDeferredLiquidEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class RenderDeferredLiquidEffect : Effect
{
  public static readonly int TYPEHASH = typeof (RenderDeferredLiquidEffect).GetHashCode();
  private EffectParameter mWorldParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mTimeParameter;
  private EffectParameter mReflectionMapParameter;
  private EffectParameter mWaveHeightParameter;
  private EffectParameter mWaveSpeed0Parameter;
  private EffectParameter mWaveSpeed1Parameter;
  private EffectParameter mWaterReflectivenessParameter;
  private EffectParameter mBottomColorParameter;
  private EffectParameter mDeepBottomColorParameter;
  private EffectParameter mWaterEmissiveAmountParameter;
  private EffectParameter mWaterSpecAmountParameter;
  private EffectParameter mWaterSpecPowerParameter;
  private EffectParameter mBottomTextureEnabledParameter;
  private EffectParameter mBottomTextureParameter;
  private EffectParameter mWaterNormalMapParameter;
  private EffectParameter mIceEdgeMapParameter;
  private EffectParameter mIceReflectivenessParameter;
  private EffectParameter mIceColorParameter;
  private EffectParameter mIceEmissiveAmountParameter;
  private EffectParameter mIceSpecAmountParameter;
  private EffectParameter mIceSpecPowerParameter;
  private EffectParameter mIceDiffuseMapParameter;
  private EffectParameter mIceNormalMapParameter;

  public RenderDeferredLiquidEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, RenderDeferredLiquidEffectCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mTimeParameter = this.Parameters[nameof (Time)];
    this.mReflectionMapParameter = this.Parameters[nameof (ReflectionMap)];
    this.mWaveHeightParameter = this.Parameters[nameof (WaveHeight)];
    this.mWaveSpeed0Parameter = this.Parameters[nameof (WaveSpeed0)];
    this.mWaveSpeed1Parameter = this.Parameters[nameof (WaveSpeed1)];
    this.mWaterReflectivenessParameter = this.Parameters[nameof (WaterReflectiveness)];
    this.mBottomColorParameter = this.Parameters[nameof (BottomColor)];
    this.mDeepBottomColorParameter = this.Parameters[nameof (DeepBottomColor)];
    this.mWaterEmissiveAmountParameter = this.Parameters[nameof (WaterEmissiveAmount)];
    this.mWaterSpecAmountParameter = this.Parameters[nameof (WaterSpecAmount)];
    this.mWaterSpecPowerParameter = this.Parameters[nameof (WaterSpecPower)];
    this.mBottomTextureEnabledParameter = this.Parameters["BottomTextureEnabled"];
    this.mBottomTextureParameter = this.Parameters[nameof (BottomTexture)];
    this.mWaterNormalMapParameter = this.Parameters[nameof (WaterNormalMap)];
    this.mIceReflectivenessParameter = this.Parameters[nameof (IceReflectiveness)];
    this.mIceColorParameter = this.Parameters[nameof (IceColor)];
    this.mIceEmissiveAmountParameter = this.Parameters[nameof (IceEmissiveAmount)];
    this.mIceSpecAmountParameter = this.Parameters[nameof (IceSpecAmount)];
    this.mIceSpecPowerParameter = this.Parameters[nameof (IceSpecPower)];
    this.mIceDiffuseMapParameter = this.Parameters[nameof (IceDiffuseMap)];
    this.mIceNormalMapParameter = this.Parameters[nameof (IceNormalMap)];
    this.mIceEdgeMapParameter = this.Parameters[nameof (IceEdgeMap)];
  }

  public Matrix World
  {
    get => this.mWorldParameter.GetValueMatrix();
    set => this.mWorldParameter.SetValue(value);
  }

  public Matrix View
  {
    get => this.mViewParameter.GetValueMatrix();
    set => this.mViewParameter.SetValue(value);
  }

  public Matrix Projection
  {
    get => this.mProjectionParameter.GetValueMatrix();
    set => this.mProjectionParameter.SetValue(value);
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public float Time
  {
    get => this.mTimeParameter.GetValueSingle();
    set => this.mTimeParameter.SetValue(value);
  }

  public TextureCube ReflectionMap
  {
    get => this.mReflectionMapParameter.GetValueTextureCube();
    set => this.mReflectionMapParameter.SetValue((Texture) value);
  }

  public float WaveHeight
  {
    get => this.mWaveHeightParameter.GetValueSingle();
    set => this.mWaveHeightParameter.SetValue(value);
  }

  public Vector2 WaveSpeed0
  {
    get => this.mWaveSpeed0Parameter.GetValueVector2();
    set => this.mWaveSpeed0Parameter.SetValue(value);
  }

  public Vector2 WaveSpeed1
  {
    get => this.mWaveSpeed1Parameter.GetValueVector2();
    set => this.mWaveSpeed1Parameter.SetValue(value);
  }

  public float WaterReflectiveness
  {
    get => this.mWaterReflectivenessParameter.GetValueSingle();
    set => this.mWaterReflectivenessParameter.SetValue(value);
  }

  public Vector3 BottomColor
  {
    get => this.mBottomColorParameter.GetValueVector3();
    set => this.mBottomColorParameter.SetValue(value);
  }

  public Vector3 DeepBottomColor
  {
    get => this.mDeepBottomColorParameter.GetValueVector3();
    set => this.mDeepBottomColorParameter.SetValue(value);
  }

  public float WaterEmissiveAmount
  {
    get => this.mWaterEmissiveAmountParameter.GetValueSingle();
    set => this.mWaterEmissiveAmountParameter.SetValue(value);
  }

  public float WaterSpecAmount
  {
    get => this.mWaterSpecAmountParameter.GetValueSingle();
    set => this.mWaterSpecAmountParameter.SetValue(value);
  }

  public float WaterSpecPower
  {
    get => this.mWaterSpecPowerParameter.GetValueSingle();
    set => this.mWaterSpecPowerParameter.SetValue(value);
  }

  public Texture2D BottomTexture
  {
    get => this.mBottomTextureParameter.GetValueTexture2D();
    set
    {
      this.mBottomTextureParameter.SetValue((Texture) value);
      this.mBottomTextureEnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D WaterNormalMap
  {
    get => this.mWaterNormalMapParameter.GetValueTexture2D();
    set => this.mWaterNormalMapParameter.SetValue((Texture) value);
  }

  public float IceReflectiveness
  {
    get => this.mIceReflectivenessParameter.GetValueSingle();
    set => this.mIceReflectivenessParameter.SetValue(value);
  }

  public Vector3 IceColor
  {
    get => this.mIceColorParameter.GetValueVector3();
    set => this.mIceColorParameter.SetValue(value);
  }

  public float IceEmissiveAmount
  {
    get => 0.0f;
    set
    {
    }
  }

  public float IceSpecAmount
  {
    get => this.mIceSpecAmountParameter.GetValueSingle();
    set => this.mIceSpecAmountParameter.SetValue(value);
  }

  public float IceSpecPower
  {
    get => this.mIceSpecPowerParameter.GetValueSingle();
    set => this.mIceSpecPowerParameter.SetValue(value);
  }

  public Texture2D IceDiffuseMap
  {
    get => this.mIceDiffuseMapParameter.GetValueTexture2D();
    set => this.mIceDiffuseMapParameter.SetValue((Texture) value);
  }

  public Texture2D IceNormalMap
  {
    get => this.mIceNormalMapParameter.GetValueTexture2D();
    set => this.mIceNormalMapParameter.SetValue((Texture) value);
  }

  public Texture2D IceEdgeMap
  {
    get => this.mIceEdgeMapParameter.GetValueTexture2D();
    set => this.mIceEdgeMapParameter.SetValue((Texture) value);
  }

  public static RenderDeferredLiquidEffect Read(ContentReader iInput)
  {
    return new RenderDeferredLiquidEffect(((IGraphicsDeviceService) iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceService))).GraphicsDevice, (EffectPool) null)
    {
      ReflectionMap = iInput.ReadExternalReference<Texture>() as TextureCube,
      WaveHeight = iInput.ReadSingle(),
      WaveSpeed0 = iInput.ReadVector2(),
      WaveSpeed1 = iInput.ReadVector2(),
      WaterReflectiveness = iInput.ReadSingle(),
      BottomColor = iInput.ReadVector3(),
      DeepBottomColor = iInput.ReadVector3(),
      WaterEmissiveAmount = iInput.ReadSingle(),
      WaterSpecAmount = iInput.ReadSingle(),
      WaterSpecPower = iInput.ReadSingle(),
      BottomTexture = iInput.ReadExternalReference<Texture2D>(),
      WaterNormalMap = iInput.ReadExternalReference<Texture2D>(),
      IceReflectiveness = iInput.ReadSingle(),
      IceColor = iInput.ReadVector3(),
      IceEmissiveAmount = iInput.ReadSingle(),
      IceSpecAmount = iInput.ReadSingle(),
      IceSpecPower = iInput.ReadSingle(),
      IceDiffuseMap = iInput.ReadExternalReference<Texture2D>(),
      IceNormalMap = iInput.ReadExternalReference<Texture2D>(),
      World = Matrix.Identity,
      Time = 0.0f
    };
  }

  public enum Technique
  {
    Default,
    Reflection,
    Depth,
    Shadow,
  }
}
