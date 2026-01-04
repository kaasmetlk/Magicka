// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.ChasmEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class ChasmEffect : Effect
{
  public static readonly int TYPEHASH = typeof (ChasmEffect).GetHashCode();
  private EffectParameter mDiffuseColorParameter;
  private EffectParameter mEmissiveAmountParameter;
  private EffectParameter mSpecularAmountParameter;
  private EffectParameter mSpecularBiasParameter;
  private EffectParameter mSpecularPowerParameter;
  private EffectParameter mAlphaParameter;
  private EffectParameter mNormalPower0Parameter;
  private EffectParameter mReflectivenessParameter;
  private EffectParameter mReflectColorParameter;
  private EffectParameter mIsLavaParameter;
  private EffectParameter mPixelSizeParameter;
  private EffectParameter mDiffuseMap0EnabledParameter;
  private EffectParameter mDiffuseMap1EnabledParameter;
  private EffectParameter mSpecularMapEnabledParameter;
  private EffectParameter mNormalMapEnabledParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mBonesParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mTimeParameter;
  private EffectParameter mDiffuseMap0Parameter;
  private EffectParameter mDiffuseMap1Parameter;
  private EffectParameter mNormalMap0Parameter;
  private EffectParameter mSpecularMap0Parameter;
  private EffectParameter mDepthMapParameter;

  public ChasmEffect()
    : base(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content.Load<Effect>("Shaders/ChasmEffect"))
  {
    this.mDiffuseColorParameter = this.Parameters[nameof (DiffuseColor)];
    this.mEmissiveAmountParameter = this.Parameters[nameof (EmissiveAmount)];
    this.mSpecularAmountParameter = this.Parameters[nameof (SpecularAmount)];
    this.mSpecularBiasParameter = this.Parameters[nameof (SpecularBias)];
    this.mSpecularPowerParameter = this.Parameters[nameof (SpecularPower)];
    this.mAlphaParameter = this.Parameters[nameof (Alpha)];
    this.mNormalPower0Parameter = this.Parameters[nameof (NormalPower0)];
    this.mReflectivenessParameter = this.Parameters[nameof (Reflectiveness)];
    this.mReflectColorParameter = this.Parameters[nameof (ReflectColor)];
    this.mIsLavaParameter = this.Parameters[nameof (IsLava)];
    this.mPixelSizeParameter = this.Parameters[nameof (PixelSize)];
    this.mDiffuseMap0EnabledParameter = this.Parameters[nameof (DiffuseMap0Enabled)];
    this.mDiffuseMap1EnabledParameter = this.Parameters[nameof (DiffuseMap1Enabled)];
    this.mSpecularMapEnabledParameter = this.Parameters[nameof (SpecularMapEnabled)];
    this.mNormalMapEnabledParameter = this.Parameters[nameof (NormalMapEnabled)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mBonesParameter = this.Parameters[nameof (Bones)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mTimeParameter = this.Parameters[nameof (Time)];
    this.mDiffuseMap0Parameter = this.Parameters[nameof (DiffuseMap0)];
    this.mDiffuseMap1Parameter = this.Parameters[nameof (DiffuseMap1)];
    this.mNormalMap0Parameter = this.Parameters[nameof (NormalMap0)];
    this.mSpecularMap0Parameter = this.Parameters[nameof (SpecularMap0)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
  }

  public Vector3 DiffuseColor
  {
    get => this.mDiffuseColorParameter.GetValueVector3();
    set => this.mDiffuseColorParameter.SetValue(value);
  }

  public float EmissiveAmount
  {
    get => this.mEmissiveAmountParameter.GetValueSingle();
    set => this.mEmissiveAmountParameter.SetValue(value);
  }

  public float SpecularAmount
  {
    get => this.mSpecularAmountParameter.GetValueSingle();
    set => this.mSpecularAmountParameter.SetValue(value);
  }

  public float SpecularBias
  {
    get => this.mSpecularBiasParameter.GetValueSingle();
    set => this.mSpecularBiasParameter.SetValue(value);
  }

  public float SpecularPower
  {
    get => this.mSpecularPowerParameter.GetValueSingle();
    set => this.mSpecularPowerParameter.SetValue(value);
  }

  public float Alpha
  {
    get => this.mAlphaParameter.GetValueSingle();
    set => this.mAlphaParameter.SetValue(value);
  }

  public float NormalPower0
  {
    get => this.mNormalPower0Parameter.GetValueSingle();
    set => this.mNormalPower0Parameter.SetValue(value);
  }

  public float Reflectiveness
  {
    get => this.mReflectivenessParameter.GetValueSingle();
    set => this.mReflectivenessParameter.SetValue(value);
  }

  public Vector3 ReflectColor
  {
    get => this.mReflectColorParameter.GetValueVector3();
    set => this.mReflectColorParameter.SetValue(value);
  }

  public bool IsLava
  {
    get => this.mIsLavaParameter.GetValueBoolean();
    set => this.mIsLavaParameter.SetValue(value);
  }

  public Vector2 PixelSize
  {
    get => this.mPixelSizeParameter.GetValueVector2();
    set => this.mPixelSizeParameter.SetValue(value);
  }

  public bool DiffuseMap0Enabled
  {
    get => this.mDiffuseMap0EnabledParameter.GetValueBoolean();
    set => this.mDiffuseMap0EnabledParameter.SetValue(value);
  }

  public bool DiffuseMap1Enabled
  {
    get => this.mDiffuseMap1EnabledParameter.GetValueBoolean();
    set => this.mDiffuseMap1EnabledParameter.SetValue(value);
  }

  public bool SpecularMapEnabled
  {
    get => this.mSpecularMapEnabledParameter.GetValueBoolean();
    set => this.mSpecularMapEnabledParameter.SetValue(value);
  }

  public bool NormalMapEnabled
  {
    get => this.mNormalMapEnabledParameter.GetValueBoolean();
    set => this.mNormalMapEnabledParameter.SetValue(value);
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

  public Matrix[] Bones
  {
    get => this.mBonesParameter.GetValueMatrixArray(80 /*0x50*/);
    set => this.mBonesParameter.SetValue(value);
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

  public Texture2D DiffuseMap0
  {
    get => this.mDiffuseMap0Parameter.GetValueTexture2D();
    set => this.mDiffuseMap0Parameter.SetValue((Texture) value);
  }

  public Texture2D DiffuseMap1
  {
    get => this.mDiffuseMap1Parameter.GetValueTexture2D();
    set => this.mDiffuseMap1Parameter.SetValue((Texture) value);
  }

  public Texture2D NormalMap0
  {
    get => this.mNormalMap0Parameter.GetValueTexture2D();
    set => this.mNormalMap0Parameter.SetValue((Texture) value);
  }

  public Texture2D SpecularMap0
  {
    get => this.mSpecularMap0Parameter.GetValueTexture2D();
    set => this.mSpecularMap0Parameter.SetValue((Texture) value);
  }

  public Texture2D DepthMap
  {
    get => this.mDepthMapParameter.GetValueTexture2D();
    set => this.mDepthMapParameter.SetValue((Texture) value);
  }
}
