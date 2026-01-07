// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.SkinnedModelDeferredNormalMappedEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace PolygonHead.Effects;

public class SkinnedModelDeferredNormalMappedEffect : Effect
{
  public const int MAX_SUPPORTED_BONES = 80 /*0x50*/;
  public static readonly int TYPEHASH = typeof (SkinnedModelDeferredNormalMappedEffect).GetHashCode();
  public static EffectPool DefaultEffectPool;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mBonesParameter;
  private EffectParameter mDamageParameter;
  private EffectParameter mBloatParameter;
  private EffectParameter mColorizeParameter;
  private EffectParameter mDiffuseColorParameter;
  private EffectParameter mOverrideColorParameter;
  private EffectParameter mSpecularAmountParameter;
  private EffectParameter mSpecularPowerParameter;
  private EffectParameter mEmissiveAmountParameter;
  private EffectParameter mNormalPowerParameter;
  private EffectParameter mDiffuseMapEnabledParameter;
  private EffectParameter mMaterialMapEnabledParameter;
  private EffectParameter mDamageMapEnabledParameter;
  private EffectParameter mNormalMapEnabledParameter;
  private EffectParameter mNormalDamageMapEnabledParameter;
  private EffectParameter mDiffuseMapParameter;
  private EffectParameter mMaterialMapParameter;
  private EffectParameter mDamageMapParameter;
  private EffectParameter mNormalMapParameter;
  private EffectParameter mNormalDamageMapParameter;

  public SkinnedModelDeferredNormalMappedEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, SkinnedModelDeferredNormalMappedEffectCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mBonesParameter = this.Parameters[nameof (Bones)];
    this.mDamageParameter = this.Parameters[nameof (Damage)];
    this.mBloatParameter = this.Parameters[nameof (Bloat)];
    this.mColorizeParameter = this.Parameters[nameof (Colorize)];
    this.mDiffuseColorParameter = this.Parameters[nameof (DiffuseColor)];
    this.mOverrideColorParameter = this.Parameters[nameof (OverrideColor)];
    this.mSpecularAmountParameter = this.Parameters[nameof (SpecularAmount)];
    this.mSpecularPowerParameter = this.Parameters[nameof (SpecularPower)];
    this.mEmissiveAmountParameter = this.Parameters[nameof (EmissiveAmount)];
    this.mNormalPowerParameter = this.Parameters[nameof (NormalPower)];
    this.mDiffuseMapEnabledParameter = this.Parameters["DiffuseMapEnabled"];
    this.mMaterialMapEnabledParameter = this.Parameters["MaterialMapEnabled"];
    this.mDamageMapEnabledParameter = this.Parameters["DamageMapEnabled"];
    this.mNormalMapEnabledParameter = this.Parameters["NormalMapEnabled"];
    this.mNormalDamageMapEnabledParameter = this.Parameters["NormalDamageMapEnabled"];
    this.mDiffuseMapParameter = this.Parameters[nameof (DiffuseMap)];
    this.mMaterialMapParameter = this.Parameters[nameof (MaterialMap)];
    this.mDamageMapParameter = this.Parameters[nameof (DamageMap)];
    this.mNormalMapParameter = this.Parameters[nameof (NormalMap)];
    this.mNormalDamageMapParameter = this.Parameters[nameof (NormalDamageMap)];
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

  public Vector4 Colorize
  {
    get => this.mColorizeParameter.GetValueVector4();
    set => this.mColorizeParameter.SetValue(value);
  }

  public Vector3 DiffuseColor
  {
    get => this.mDiffuseColorParameter.GetValueVector3();
    set => this.mDiffuseColorParameter.SetValue(value);
  }

  public Vector4 OverrideColor
  {
    get => this.mOverrideColorParameter.GetValueVector4();
    set => this.mOverrideColorParameter.SetValue(value);
  }

  public float SpecularAmount
  {
    get => this.mSpecularAmountParameter.GetValueSingle();
    set => this.mSpecularAmountParameter.SetValue(value);
  }

  public float SpecularPower
  {
    get => this.mSpecularPowerParameter.GetValueSingle();
    set => this.mSpecularPowerParameter.SetValue(value);
  }

  public float EmissiveAmount
  {
    get => this.mEmissiveAmountParameter.GetValueSingle();
    set => this.mEmissiveAmountParameter.SetValue(value);
  }

  public float NormalPower
  {
    get => this.mNormalPowerParameter.GetValueSingle();
    set => this.mNormalPowerParameter.SetValue(value);
  }

  public Texture2D DiffuseMap
  {
    get => this.mDiffuseMapParameter.GetValueTexture2D();
    set
    {
      this.mDiffuseMapParameter.SetValue((Texture) value);
      this.mDiffuseMapEnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D MaterialMap
  {
    get => this.mMaterialMapParameter.GetValueTexture2D();
    set
    {
      this.mMaterialMapParameter.SetValue((Texture) value);
      this.mMaterialMapEnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D DamageMap
  {
    get => this.mDamageMapParameter.GetValueTexture2D();
    set
    {
      this.mDamageMapParameter.SetValue((Texture) value);
      this.mDamageMapEnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D NormalMap
  {
    get => this.mNormalMapParameter.GetValueTexture2D();
    set
    {
      this.mNormalMapParameter.SetValue((Texture) value);
      this.mNormalMapEnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D NormalDamageMap
  {
    get => this.mNormalDamageMapParameter.GetValueTexture2D();
    set
    {
      this.mNormalDamageMapParameter.SetValue((Texture) value);
      this.mNormalDamageMapEnabledParameter.SetValue(value != null);
    }
  }

  internal static SkinnedModelDeferredNormalMappedEffect Read(ContentReader iInput)
  {
    GraphicsDevice graphicsDevice = ((IGraphicsDeviceService) iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceService))).GraphicsDevice;
    if (SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool == null)
      throw new Exception("The default effect pool is not set!");
    SkinnedModelDeferredNormalMappedEffect normalMappedEffect;
    lock (graphicsDevice)
      normalMappedEffect = new SkinnedModelDeferredNormalMappedEffect(graphicsDevice, SkinnedModelDeferredNormalMappedEffect.DefaultEffectPool);
    normalMappedEffect.DiffuseColor = iInput.ReadVector3();
    normalMappedEffect.OverrideColor = new Vector4();
    normalMappedEffect.SpecularAmount = iInput.ReadSingle();
    normalMappedEffect.SpecularPower = iInput.ReadSingle();
    normalMappedEffect.EmissiveAmount = iInput.ReadSingle();
    normalMappedEffect.NormalPower = iInput.ReadSingle();
    normalMappedEffect.DiffuseMap = iInput.ReadExternalReference<Texture2D>();
    normalMappedEffect.MaterialMap = iInput.ReadExternalReference<Texture2D>();
    normalMappedEffect.DamageMap = iInput.ReadExternalReference<Texture2D>();
    normalMappedEffect.NormalMap = iInput.ReadExternalReference<Texture2D>();
    normalMappedEffect.NormalDamageMap = iInput.ReadExternalReference<Texture2D>();
    return normalMappedEffect;
  }

  public enum Technique
  {
    Default,
    Depth,
    Shadow,
  }
}
