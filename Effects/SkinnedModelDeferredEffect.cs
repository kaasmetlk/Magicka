// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.SkinnedModelDeferredEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace PolygonHead.Effects;

public class SkinnedModelDeferredEffect : Effect
{
  public const int MAX_SUPPORTED_BONES = 80 /*0x50*/;
  private static readonly float ONEOVERLOG05 = 1f / (float) Math.Log(0.5);
  public static EffectPool DefaultEffectPool;
  public static readonly int TYPEHASH = typeof (SkinnedModelDeferredEffect).GetHashCode();
  protected EffectParameter mDiffuseColorParameter;
  protected EffectParameter mTintColorParameter;
  protected EffectParameter mEmissiveAmountParameter;
  protected EffectParameter mSpecularAmountParameter;
  protected EffectParameter mSpecularBiasParameter;
  protected EffectParameter mSpecularPowerParameter;
  protected EffectParameter mAlphaParameter;
  protected EffectParameter mNormalPowerParameter;
  protected EffectParameter mRimLightGlowParameter;
  protected EffectParameter mRimLightPowerParameter;
  protected EffectParameter mRimLightBiasParameter;
  protected EffectParameter mDamageParameter;
  protected EffectParameter mBloatParameter;
  protected EffectParameter mCubeMapColorParameter;
  protected EffectParameter mColorizeParameter;
  protected EffectParameter mOverlayAlphaParameter;
  protected EffectParameter mFresnelPowerParameter;
  protected EffectParameter mDiffuseMap0EnabledParameter;
  protected EffectParameter mDiffuseMap1EnabledParameter;
  protected EffectParameter mSpecularMapEnabledParameter;
  protected EffectParameter mNormalMapEnabledParameter;
  protected EffectParameter mDamageMap0EnabledParameter;
  protected EffectParameter mDamageMap1EnabledParameter;
  protected EffectParameter mProjectionMapEnabledParameter;
  protected EffectParameter mCubeMapEnabledParameter;
  protected EffectParameter mCubeNormalMapEnabledParameter;
  protected EffectParameter mUseSoftLightBlendParameter;
  protected EffectParameter mViewParameter;
  protected EffectParameter mProjectionParameter;
  protected EffectParameter mViewProjectionParameter;
  protected EffectParameter mProjectionMapMatrixParameter;
  protected EffectParameter mCubeMapRotationParameter;
  protected EffectParameter mBonesParameter;
  protected EffectParameter mCameraPositionParameter;
  protected EffectParameter mOverrideColorParameter;
  protected EffectParameter mDiffuseMap0Parameter;
  protected EffectParameter mDiffuseMap1Parameter;
  protected EffectParameter mMaterialMapParameter;
  protected EffectParameter mNormalMapParameter;
  protected EffectParameter mDamageMap0Parameter;
  protected EffectParameter mDamageMap1Parameter;
  protected EffectParameter mProjectionMapParameter;
  protected EffectParameter mCubeMapParameter;
  protected EffectParameter mCubeNormalMapParameter;

  public Vector3 DiffuseColor
  {
    get => this.mDiffuseColorParameter.GetValueVector3();
    set => this.mDiffuseColorParameter.SetValue(value);
  }

  public Vector3 TintColor
  {
    get => this.mTintColorParameter.GetValueVector3();
    set => this.mTintColorParameter.SetValue(value);
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

  public float NormalPower
  {
    get => this.mNormalPowerParameter.GetValueSingle();
    set => this.mNormalPowerParameter.SetValue(value);
  }

  public float RimLightGlow
  {
    get => this.mRimLightGlowParameter.GetValueSingle();
    set => this.mRimLightGlowParameter.SetValue(value);
  }

  public float RimLightPower
  {
    get => this.mRimLightPowerParameter.GetValueSingle();
    set => this.mRimLightPowerParameter.SetValue(value);
  }

  public float RimLightBias
  {
    get => this.mRimLightBiasParameter.GetValueSingle();
    set => this.mRimLightBiasParameter.SetValue(value);
  }

  public float Damage
  {
    get => this.mDamageParameter.GetValueSingle();
    set => this.mDamageParameter.SetValue(value);
  }

  public float Bloat
  {
    get => this.mBloatParameter.GetValueSingle();
    set => this.mBloatParameter.SetValue(value);
  }

  public Vector4 CubeMapColor
  {
    get => this.mCubeMapColorParameter.GetValueVector4();
    set => this.mCubeMapColorParameter.SetValue(value);
  }

  public Vector4 Colorize
  {
    get => this.mColorizeParameter.GetValueVector4();
    set => this.mColorizeParameter.SetValue(value);
  }

  public float OverlayAlpha
  {
    get => this.mOverlayAlphaParameter.GetValueSingle();
    set => this.mOverlayAlphaParameter.SetValue(value);
  }

  public float FresnelPower
  {
    get => this.mFresnelPowerParameter.GetValueSingle();
    set => this.mFresnelPowerParameter.SetValue(value);
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

  public bool DamageMap0Enabled
  {
    get => this.mDamageMap0EnabledParameter.GetValueBoolean();
    set => this.mDamageMap0EnabledParameter.SetValue(value);
  }

  public bool DamageMap1Enabled
  {
    get => this.mDamageMap1EnabledParameter.GetValueBoolean();
    set => this.mDamageMap1EnabledParameter.SetValue(value);
  }

  public bool ProjectionMapEnabled
  {
    get => this.mProjectionMapEnabledParameter.GetValueBoolean();
    set => this.mProjectionMapEnabledParameter.SetValue(value);
  }

  public bool CubeMapEnabled
  {
    get => this.mCubeMapEnabledParameter.GetValueBoolean();
    set => this.mCubeMapEnabledParameter.SetValue(value);
  }

  public bool CubeNormalMapEnabled
  {
    get => this.mCubeNormalMapEnabledParameter.GetValueBoolean();
    set => this.mCubeNormalMapEnabledParameter.SetValue(value);
  }

  public bool UseSoftLightBlend
  {
    get => this.mUseSoftLightBlendParameter.GetValueBoolean();
    set => this.mUseSoftLightBlendParameter.SetValue(value);
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

  public Matrix ProjectionMapMatrix
  {
    get => this.mProjectionMapMatrixParameter.GetValueMatrix();
    set => this.mProjectionMapMatrixParameter.SetValue(value);
  }

  public Matrix CubeMapRotation
  {
    get => this.mCubeMapRotationParameter.GetValueMatrix();
    set => this.mCubeMapRotationParameter.SetValue(value);
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

  public Vector4 OverrideColor
  {
    get => this.mOverrideColorParameter.GetValueVector4();
    set => this.mOverrideColorParameter.SetValue(value);
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

  public Texture2D MaterialMap
  {
    get => this.mMaterialMapParameter.GetValueTexture2D();
    set => this.mMaterialMapParameter.SetValue((Texture) value);
  }

  public Texture2D NormalMap
  {
    get => this.mNormalMapParameter.GetValueTexture2D();
    set => this.mNormalMapParameter.SetValue((Texture) value);
  }

  public Texture2D DamageMap0
  {
    get => this.mDamageMap0Parameter.GetValueTexture2D();
    set => this.mDamageMap0Parameter.SetValue((Texture) value);
  }

  public Texture2D DamageMap1
  {
    get => this.mDamageMap1Parameter.GetValueTexture2D();
    set => this.mDamageMap1Parameter.SetValue((Texture) value);
  }

  public Texture2D ProjectionMap
  {
    get => this.mProjectionMapParameter.GetValueTexture2D();
    set => this.mProjectionMapParameter.SetValue((Texture) value);
  }

  public TextureCube CubeMap
  {
    get => this.mCubeMapParameter.GetValueTextureCube();
    set => this.mCubeMapParameter.SetValue((Texture) value);
  }

  public TextureCube CubeNormalMap
  {
    get => this.mCubeNormalMapParameter.GetValueTextureCube();
    set => this.mCubeNormalMapParameter.SetValue((Texture) value);
  }

  protected SkinnedModelDeferredEffect(
    GraphicsDevice graphicsDevice,
    SkinnedModelDeferredEffect clone)
    : base(graphicsDevice, (Effect) clone)
  {
    this.CacheEffectParams();
    this.InitializeEffectParams();
  }

  public SkinnedModelDeferredEffect(GraphicsDevice graphicsDevice, EffectPool effectPool)
    : base(graphicsDevice, SkinnedModelDeferredEffectCode.CODE, CompilerOptions.None, effectPool)
  {
    this.CacheEffectParams();
    this.InitializeEffectParams();
  }

  private void CacheEffectParams()
  {
    this.mDiffuseColorParameter = this.Parameters["DiffuseColor"];
    this.mTintColorParameter = this.Parameters["TintColor"];
    this.mEmissiveAmountParameter = this.Parameters["EmissiveAmount"];
    this.mSpecularAmountParameter = this.Parameters["SpecularAmount"];
    this.mSpecularBiasParameter = this.Parameters["SpecularBias"];
    this.mSpecularPowerParameter = this.Parameters["SpecularPower"];
    this.mAlphaParameter = this.Parameters["Alpha"];
    this.mNormalPowerParameter = this.Parameters["NormalPower"];
    this.mRimLightGlowParameter = this.Parameters["RimLightGlow"];
    this.mRimLightPowerParameter = this.Parameters["RimLightPower"];
    this.mRimLightBiasParameter = this.Parameters["RimLightBias"];
    this.mDamageParameter = this.Parameters["Damage"];
    this.mBloatParameter = this.Parameters["Bloat"];
    this.mCubeMapColorParameter = this.Parameters["CubeMapColor"];
    this.mColorizeParameter = this.Parameters["Colorize"];
    this.mOverlayAlphaParameter = this.Parameters["OverlayAlpha"];
    this.mFresnelPowerParameter = this.Parameters["FresnelPower"];
    this.mDiffuseMap0EnabledParameter = this.Parameters["DiffuseMap0Enabled"];
    this.mDiffuseMap1EnabledParameter = this.Parameters["DiffuseMap1Enabled"];
    this.mSpecularMapEnabledParameter = this.Parameters["SpecularMapEnabled"];
    this.mNormalMapEnabledParameter = this.Parameters["NormalMapEnabled"];
    this.mDamageMap0EnabledParameter = this.Parameters["DamageMap0Enabled"];
    this.mDamageMap1EnabledParameter = this.Parameters["DamageMap1Enabled"];
    this.mProjectionMapEnabledParameter = this.Parameters["ProjectionMapEnabled"];
    this.mCubeMapEnabledParameter = this.Parameters["CubeMapEnabled"];
    this.mCubeNormalMapEnabledParameter = this.Parameters["CubeNormalMapEnabled"];
    this.mUseSoftLightBlendParameter = this.Parameters["UseSoftLightBlend"];
    this.mViewParameter = this.Parameters["View"];
    this.mProjectionParameter = this.Parameters["Projection"];
    this.mViewProjectionParameter = this.Parameters["ViewProjection"];
    this.mProjectionMapMatrixParameter = this.Parameters["ProjectionMapMatrix"];
    this.mCubeMapRotationParameter = this.Parameters["CubeMapRotation"];
    this.mBonesParameter = this.Parameters["Bones"];
    this.mCameraPositionParameter = this.Parameters["CameraPosition"];
    this.mOverrideColorParameter = this.Parameters["OverrideColor"];
    this.mDiffuseMap0Parameter = this.Parameters["DiffuseMap0"];
    this.mDiffuseMap1Parameter = this.Parameters["DiffuseMap1"];
    this.mMaterialMapParameter = this.Parameters["MaterialMap"];
    this.mNormalMapParameter = this.Parameters["NormalMap"];
    this.mDamageMap0Parameter = this.Parameters["DamageMap0"];
    this.mDamageMap1Parameter = this.Parameters["DamageMap1"];
    this.mProjectionMapParameter = this.Parameters["ProjectionMap"];
    this.mCubeMapParameter = this.Parameters["CubeMap"];
    this.mCubeNormalMapParameter = this.Parameters["CubeNormalMap"];
  }

  private void InitializeEffectParams()
  {
    this.View = Matrix.Identity;
    this.Projection = Matrix.Identity;
    this.ViewProjection = Matrix.Identity;
    this.CubeMapRotation = Matrix.Identity;
    for (int index = 0; index < 80 /*0x50*/; ++index)
      this.mBonesParameter.Elements[index].SetValue(Matrix.Identity);
    this.DiffuseMap0Enabled = false;
    this.DiffuseMap1Enabled = false;
    this.SpecularMapEnabled = false;
    this.NormalMapEnabled = false;
    this.RimLightGlow = 0.8f;
    this.RimLightPower = 1.5f;
    this.RimLightBias = 0.2f;
    this.EmissiveAmount = 0.0f;
    this.DiffuseColor = Vector3.One;
    this.TintColor = Vector3.One;
    this.SpecularAmount = 1f;
    this.SpecularPower = 16f;
  }

  private ShaderProfile GetShaderProfile(string annotation)
  {
    return (ShaderProfile) Enum.Parse(typeof (ShaderProfile), annotation, true);
  }

  public void SetTechnique(SkinnedModelDeferredEffect.Technique iTechnique)
  {
    this.CurrentTechnique = this.Techniques[(int) iTechnique];
  }

  internal static SkinnedModelDeferredEffect Read(ContentReader iInput)
  {
    GraphicsDevice graphicsDevice = ((IGraphicsDeviceService) iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceService))).GraphicsDevice;
    if (SkinnedModelDeferredEffect.DefaultEffectPool == null)
      throw new Exception("The default effect pool is not set!");
    SkinnedModelDeferredEffect modelDeferredEffect;
    lock (graphicsDevice)
      modelDeferredEffect = new SkinnedModelDeferredEffect(graphicsDevice, SkinnedModelDeferredEffect.DefaultEffectPool);
    modelDeferredEffect.DiffuseColor = iInput.ReadVector3();
    modelDeferredEffect.TintColor = iInput.ReadVector3();
    modelDeferredEffect.EmissiveAmount = (float) Math.Log(1.0 - (double) Math.Min(iInput.ReadSingle(), 0.9999f)) * SkinnedModelDeferredEffect.ONEOVERLOG05;
    modelDeferredEffect.SpecularAmount = iInput.ReadSingle();
    modelDeferredEffect.SpecularBias = iInput.ReadSingle();
    modelDeferredEffect.SpecularPower = iInput.ReadSingle();
    modelDeferredEffect.Alpha = iInput.ReadSingle();
    modelDeferredEffect.NormalPower = iInput.ReadSingle();
    modelDeferredEffect.RimLightGlow = iInput.ReadSingle();
    modelDeferredEffect.RimLightPower = iInput.ReadSingle();
    modelDeferredEffect.Damage = iInput.ReadSingle();
    modelDeferredEffect.Bloat = iInput.ReadSingle();
    modelDeferredEffect.CubeMapColor = iInput.ReadVector4();
    modelDeferredEffect.Colorize = iInput.ReadVector4();
    modelDeferredEffect.DiffuseMap0Enabled = iInput.ReadBoolean();
    modelDeferredEffect.DiffuseMap1Enabled = iInput.ReadBoolean();
    modelDeferredEffect.SpecularMapEnabled = iInput.ReadBoolean();
    modelDeferredEffect.NormalMapEnabled = iInput.ReadBoolean();
    modelDeferredEffect.DamageMap0Enabled = false;
    modelDeferredEffect.DamageMap1Enabled = false;
    modelDeferredEffect.ProjectionMapEnabled = false;
    modelDeferredEffect.CubeMapEnabled = false;
    modelDeferredEffect.CubeNormalMapEnabled = false;
    modelDeferredEffect.DiffuseMap0 = iInput.ReadExternalReference<Texture2D>();
    modelDeferredEffect.DiffuseMap1 = iInput.ReadExternalReference<Texture2D>();
    modelDeferredEffect.MaterialMap = iInput.ReadExternalReference<Texture2D>();
    modelDeferredEffect.NormalMap = iInput.ReadExternalReference<Texture2D>();
    modelDeferredEffect.DamageMap0 = iInput.ReadExternalReference<Texture2D>();
    modelDeferredEffect.DamageMap1 = iInput.ReadExternalReference<Texture2D>();
    modelDeferredEffect.ProjectionMap = (Texture2D) null;
    modelDeferredEffect.CubeMap = (TextureCube) null;
    modelDeferredEffect.CubeNormalMap = (TextureCube) null;
    return modelDeferredEffect;
  }

  public enum Technique : byte
  {
    Default,
    AlphaBlended,
    Additive,
    Depth,
    Shadow,
    NonDeffered,
    AdditiveFresnel,
  }
}
