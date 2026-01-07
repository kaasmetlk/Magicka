// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.LavaEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class LavaEffect : Effect
{
  public static readonly int TYPEHASH = typeof (LavaEffect).GetHashCode();
  private EffectParameter mWorldParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mTimeParameter;
  private EffectParameter mMaskDistortionParameter;
  private EffectParameter mSpeed0Parameter;
  private EffectParameter mSpeed1Parameter;
  private EffectParameter mLavaHotEmissiveAmountParameter;
  private EffectParameter mLavaColdEmissiveAmountParameter;
  private EffectParameter mLavaSpecAmountParameter;
  private EffectParameter mLavaSpecPowerParameter;
  private EffectParameter mTempFrequencyParameter;
  private EffectParameter mToneMapParameter;
  private EffectParameter mTempMapParameter;
  private EffectParameter mMaskMapParameter;
  private EffectParameter mRockColorParameter;
  private EffectParameter mRockSpecAmountParameter;
  private EffectParameter mRockSpecPowerParameter;
  private EffectParameter mRockEmissiveAmountParameter;
  private EffectParameter mRockNormalPowerParameter;
  private EffectParameter mRockTextureParameter;
  private EffectParameter mRockNormalMapEnabledParameter;
  private EffectParameter mRockNormalMapParameter;

  public LavaEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, LavaEffectCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mTimeParameter = this.Parameters[nameof (Time)];
    this.mMaskDistortionParameter = this.Parameters[nameof (MaskDistortion)];
    this.mSpeed0Parameter = this.Parameters[nameof (Speed0)];
    this.mSpeed1Parameter = this.Parameters[nameof (Speed1)];
    this.mLavaHotEmissiveAmountParameter = this.Parameters[nameof (LavaHotEmissiveAmount)];
    this.mLavaColdEmissiveAmountParameter = this.Parameters[nameof (LavaColdEmissiveAmount)];
    this.mLavaSpecAmountParameter = this.Parameters[nameof (LavaSpecAmount)];
    this.mLavaSpecPowerParameter = this.Parameters[nameof (LavaSpecPower)];
    this.mTempFrequencyParameter = this.Parameters[nameof (TempFrequency)];
    this.mToneMapParameter = this.Parameters[nameof (ToneMap)];
    this.mTempMapParameter = this.Parameters[nameof (TempMap)];
    this.mMaskMapParameter = this.Parameters[nameof (MaskMap)];
    this.mRockColorParameter = this.Parameters[nameof (RockColor)];
    this.mRockSpecAmountParameter = this.Parameters[nameof (RockSpecAmount)];
    this.mRockSpecPowerParameter = this.Parameters[nameof (RockSpecPower)];
    this.mRockEmissiveAmountParameter = this.Parameters[nameof (RockEmissiveAmount)];
    this.mRockNormalPowerParameter = this.Parameters[nameof (RockNormalPower)];
    this.mRockTextureParameter = this.Parameters[nameof (RockTexture)];
    this.mRockNormalMapEnabledParameter = this.Parameters[nameof (RockNormalMapEnabled)];
    this.mRockNormalMapParameter = this.Parameters[nameof (RockNormalMap)];
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

  public float MaskDistortion
  {
    get => this.mMaskDistortionParameter.GetValueSingle();
    set => this.mMaskDistortionParameter.SetValue(value);
  }

  public Vector2 Speed0
  {
    get => this.mSpeed0Parameter.GetValueVector2();
    set => this.mSpeed0Parameter.SetValue(value);
  }

  public Vector2 Speed1
  {
    get => this.mSpeed1Parameter.GetValueVector2();
    set => this.mSpeed1Parameter.SetValue(value);
  }

  public float LavaHotEmissiveAmount
  {
    get => this.mLavaHotEmissiveAmountParameter.GetValueSingle();
    set => this.mLavaHotEmissiveAmountParameter.SetValue(value);
  }

  public float LavaColdEmissiveAmount
  {
    get => this.mLavaColdEmissiveAmountParameter.GetValueSingle();
    set => this.mLavaColdEmissiveAmountParameter.SetValue(value);
  }

  public float LavaSpecAmount
  {
    get => this.mLavaSpecAmountParameter.GetValueSingle();
    set => this.mLavaSpecAmountParameter.SetValue(value);
  }

  public float LavaSpecPower
  {
    get => this.mLavaSpecPowerParameter.GetValueSingle();
    set => this.mLavaSpecPowerParameter.SetValue(value);
  }

  public float TempFrequency
  {
    get => this.mTempFrequencyParameter.GetValueSingle();
    set => this.mTempFrequencyParameter.SetValue(value);
  }

  public Texture2D ToneMap
  {
    get => this.mToneMapParameter.GetValueTexture2D();
    set => this.mToneMapParameter.SetValue((Texture) value);
  }

  public Texture2D TempMap
  {
    get => this.mTempMapParameter.GetValueTexture2D();
    set => this.mTempMapParameter.SetValue((Texture) value);
  }

  public Texture2D MaskMap
  {
    get => this.mMaskMapParameter.GetValueTexture2D();
    set => this.mMaskMapParameter.SetValue((Texture) value);
  }

  public Vector3 RockColor
  {
    get => this.mRockColorParameter.GetValueVector3();
    set => this.mRockColorParameter.SetValue(value);
  }

  public float RockSpecAmount
  {
    get => this.mRockSpecAmountParameter.GetValueSingle();
    set => this.mRockSpecAmountParameter.SetValue(value);
  }

  public float RockSpecPower
  {
    get => this.mRockSpecPowerParameter.GetValueSingle();
    set => this.mRockSpecPowerParameter.SetValue(value);
  }

  public float RockEmissiveAmount
  {
    get => this.mRockEmissiveAmountParameter.GetValueSingle();
    set => this.mRockEmissiveAmountParameter.SetValue(value);
  }

  public float RockNormalPower
  {
    get => this.mRockNormalPowerParameter.GetValueSingle();
    set => this.mRockNormalPowerParameter.SetValue(value);
  }

  public Texture2D RockTexture
  {
    get => this.mRockTextureParameter.GetValueTexture2D();
    set => this.mRockTextureParameter.SetValue((Texture) value);
  }

  public bool RockNormalMapEnabled
  {
    get => this.mRockNormalMapEnabledParameter.GetValueBoolean();
    set => this.mRockNormalMapEnabledParameter.SetValue(value);
  }

  public Texture2D RockNormalMap
  {
    get => this.mRockNormalMapParameter.GetValueTexture2D();
    set => this.mRockNormalMapParameter.SetValue((Texture) value);
  }

  public static LavaEffect Read(ContentReader iInput)
  {
    return new LavaEffect((iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool)
    {
      MaskDistortion = iInput.ReadSingle(),
      Speed0 = iInput.ReadVector2(),
      Speed1 = iInput.ReadVector2(),
      LavaHotEmissiveAmount = iInput.ReadSingle(),
      LavaColdEmissiveAmount = iInput.ReadSingle(),
      LavaSpecAmount = iInput.ReadSingle(),
      LavaSpecPower = iInput.ReadSingle(),
      TempFrequency = iInput.ReadSingle(),
      ToneMap = iInput.ReadExternalReference<Texture2D>(),
      TempMap = iInput.ReadExternalReference<Texture2D>(),
      MaskMap = iInput.ReadExternalReference<Texture2D>(),
      RockColor = iInput.ReadVector3(),
      RockEmissiveAmount = iInput.ReadSingle(),
      RockSpecAmount = iInput.ReadSingle(),
      RockSpecPower = iInput.ReadSingle(),
      RockNormalPower = iInput.ReadSingle(),
      RockTexture = iInput.ReadExternalReference<Texture2D>(),
      RockNormalMap = iInput.ReadExternalReference<Texture2D>()
    };
  }

  public enum Technique
  {
    Default,
    Depth,
    Shadow,
  }
}
