// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.RenderDeferredEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class RenderDeferredEffect : Effect
{
  public static readonly int TYPEHASH = typeof (RenderDeferredEffect).GetHashCode();
  private EffectParameter mBloatParameter;
  private EffectParameter mWorldParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mAlphaParameter;
  private EffectParameter mSharpnessParameter;
  private EffectParameter mVertexColorEnabledParameter;
  private EffectParameter mUseMaterialTextureForReflectivenessParameter;
  private EffectParameter mReflectionMapParameter;
  private EffectParameter mDiffuseTexture0AlphaDisabledParameter;
  private EffectParameter mAlphaMask0EnabledParameter;
  private EffectParameter mDiffuseColor0Parameter;
  private EffectParameter mSpecAmount0Parameter;
  private EffectParameter mSpecPower0Parameter;
  private EffectParameter mEmissiveAmount0Parameter;
  private EffectParameter mNormalPower0Parameter;
  private EffectParameter mReflectiveness0Parameter;
  private EffectParameter mDiffuseTexture0Parameter;
  private EffectParameter mDiffuseTexture0EnabledParameter;
  private EffectParameter mMaterialTexture0Parameter;
  private EffectParameter mMaterialTexture0EnabledParameter;
  private EffectParameter mNormalTexture0Parameter;
  private EffectParameter mNormalTexture0EnabledParameter;
  private EffectParameter mDiffuseTexture1AlphaDisabledParameter;
  private EffectParameter mAlphaMask1EnabledParameter;
  private EffectParameter mDiffuseColor1Parameter;
  private EffectParameter mSpecAmount1Parameter;
  private EffectParameter mSpecPower1Parameter;
  private EffectParameter mEmissiveAmount1Parameter;
  private EffectParameter mNormalPower1Parameter;
  private EffectParameter mReflectiveness1Parameter;
  private EffectParameter mFresnelPowerParameter;
  private EffectParameter mDiffuseTexture1Parameter;
  private EffectParameter mDiffuseTexture1EnabledParameter;
  private EffectParameter mMaterialTexture1Parameter;
  private EffectParameter mMaterialTexture1EnabledParameter;
  private EffectParameter mNormalTexture1Parameter;
  private EffectParameter mNormalTexture1EnabledParameter;
  private EffectParameter mTimeParameter;
  private EffectParameter mSwayEnabledParameter;
  private EffectParameter mSwayParameter;
  private EffectParameter mEntityInfluenceParameter;
  private EffectParameter mGroundLevelParameter;
  private EffectParameter mSwayProjectionParameter;
  private EffectParameter mSwayTextureParameter;
  private static bool mUseVertexTexturing;

  public RenderDeferredEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, (RenderDeferredEffect.mUseVertexTexturing = RenderManager.Instance.SupportsVertexTexturing(SurfaceFormat.Vector4)) ? RenderDeferredEffectWVTCode.CODE : RenderDeferredEffectWOVTCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mAlphaParameter = this.Parameters[nameof (Alpha)];
    this.mSharpnessParameter = this.Parameters[nameof (Sharpness)];
    this.mVertexColorEnabledParameter = this.Parameters[nameof (VertexColorEnabled)];
    this.mUseMaterialTextureForReflectivenessParameter = this.Parameters[nameof (UseMaterialTextureForReflectiveness)];
    this.mReflectionMapParameter = this.Parameters[nameof (ReflectionMap)];
    this.mDiffuseTexture0AlphaDisabledParameter = this.Parameters[nameof (DiffuseTexture0AlphaDisabled)];
    this.mAlphaMask0EnabledParameter = this.Parameters[nameof (AlphaMask0Enabled)];
    this.mDiffuseColor0Parameter = this.Parameters[nameof (DiffuseColor0)];
    this.mSpecAmount0Parameter = this.Parameters[nameof (SpecAmount0)];
    this.mSpecPower0Parameter = this.Parameters[nameof (SpecPower0)];
    this.mEmissiveAmount0Parameter = this.Parameters[nameof (EmissiveAmount0)];
    this.mNormalPower0Parameter = this.Parameters[nameof (NormalPower0)];
    this.mReflectiveness0Parameter = this.Parameters[nameof (Reflectiveness0)];
    this.mDiffuseTexture0Parameter = this.Parameters[nameof (DiffuseTexture0)];
    this.mDiffuseTexture0EnabledParameter = this.Parameters["DiffuseTexture0Enabled"];
    this.mMaterialTexture0Parameter = this.Parameters[nameof (MaterialTexture0)];
    this.mMaterialTexture0EnabledParameter = this.Parameters["MaterialTexture0Enabled"];
    this.mNormalTexture0Parameter = this.Parameters[nameof (NormalTexture0)];
    this.mNormalTexture0EnabledParameter = this.Parameters["NormalMap0Enabled"];
    this.mDiffuseTexture1AlphaDisabledParameter = this.Parameters[nameof (DiffuseTexture1AlphaDisabled)];
    this.mAlphaMask1EnabledParameter = this.Parameters[nameof (AlphaMask1Enabled)];
    this.mDiffuseColor1Parameter = this.Parameters[nameof (DiffuseColor1)];
    this.mSpecAmount1Parameter = this.Parameters[nameof (SpecAmount1)];
    this.mSpecPower1Parameter = this.Parameters[nameof (SpecPower1)];
    this.mEmissiveAmount1Parameter = this.Parameters[nameof (EmissiveAmount1)];
    this.mNormalPower1Parameter = this.Parameters[nameof (NormalPower1)];
    this.mReflectiveness1Parameter = this.Parameters[nameof (Reflectiveness1)];
    this.mFresnelPowerParameter = this.Parameters[nameof (FresnelPower)];
    this.mDiffuseTexture1Parameter = this.Parameters[nameof (DiffuseTexture1)];
    this.mDiffuseTexture1EnabledParameter = this.Parameters["DiffuseTexture1Enabled"];
    this.mMaterialTexture1Parameter = this.Parameters[nameof (MaterialTexture1)];
    this.mMaterialTexture1EnabledParameter = this.Parameters["MaterialTexture1Enabled"];
    this.mNormalTexture1Parameter = this.Parameters[nameof (NormalTexture1)];
    this.mNormalTexture1EnabledParameter = this.Parameters["NormalMap1Enabled"];
    this.mBloatParameter = this.Parameters[nameof (Bloat)];
    this.mTimeParameter = this.Parameters[nameof (Time)];
    this.mSwayEnabledParameter = this.Parameters[nameof (SwayEnabled)];
    this.mSwayParameter = this.Parameters[nameof (Sway)];
    this.mEntityInfluenceParameter = this.Parameters[nameof (EntityInfluence)];
    this.mGroundLevelParameter = this.Parameters[nameof (GroundLevel)];
    if (!RenderDeferredEffect.mUseVertexTexturing)
      return;
    this.mSwayProjectionParameter = this.Parameters[nameof (SwayProjection)];
    this.mSwayTextureParameter = this.Parameters[nameof (SwayTexture)];
  }

  public float Bloat
  {
    get => this.mBloatParameter.GetValueSingle();
    set => this.mBloatParameter.SetValue(value);
  }

  public bool UseVertexTexturing => RenderDeferredEffect.mUseVertexTexturing;

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

  public float Alpha
  {
    get => this.mAlphaParameter.GetValueSingle();
    set => this.mAlphaParameter.SetValue(value);
  }

  public float Sharpness
  {
    get => this.mSharpnessParameter.GetValueSingle();
    set => this.mSharpnessParameter.SetValue(value);
  }

  public bool VertexColorEnabled
  {
    get => this.mVertexColorEnabledParameter.GetValueBoolean();
    set => this.mVertexColorEnabledParameter.SetValue(value);
  }

  public bool UseMaterialTextureForReflectiveness
  {
    get => this.mUseMaterialTextureForReflectivenessParameter.GetValueBoolean();
    set => this.mUseMaterialTextureForReflectivenessParameter.SetValue(value);
  }

  public TextureCube ReflectionMap
  {
    get => this.mReflectionMapParameter.GetValueTextureCube();
    set => this.mReflectionMapParameter.SetValue((Texture) value);
  }

  public bool DiffuseTexture0AlphaDisabled
  {
    get => this.mDiffuseTexture0AlphaDisabledParameter.GetValueBoolean();
    set => this.mDiffuseTexture0AlphaDisabledParameter.SetValue(value);
  }

  public bool AlphaMask0Enabled
  {
    get => this.mAlphaMask0EnabledParameter.GetValueBoolean();
    set => this.mAlphaMask0EnabledParameter.SetValue(value);
  }

  public Vector3 DiffuseColor0
  {
    get => this.mDiffuseColor0Parameter.GetValueVector3();
    set => this.mDiffuseColor0Parameter.SetValue(value);
  }

  public float SpecAmount0
  {
    get => this.mSpecAmount0Parameter.GetValueSingle();
    set => this.mSpecAmount0Parameter.SetValue(value);
  }

  public float SpecPower0
  {
    get => this.mSpecPower0Parameter.GetValueSingle();
    set => this.mSpecPower0Parameter.SetValue(value);
  }

  public float EmissiveAmount0
  {
    get => this.mEmissiveAmount0Parameter.GetValueSingle();
    set => this.mEmissiveAmount0Parameter.SetValue(value);
  }

  public float NormalPower0
  {
    get => this.mNormalPower0Parameter.GetValueSingle();
    set => this.mNormalPower0Parameter.SetValue(value);
  }

  public float Reflectiveness0
  {
    get => this.mReflectiveness0Parameter.GetValueSingle();
    set => this.mReflectiveness0Parameter.SetValue(value);
  }

  public Texture2D DiffuseTexture0
  {
    get => this.mDiffuseTexture0Parameter.GetValueTexture2D();
    set
    {
      this.mDiffuseTexture0Parameter.SetValue((Texture) value);
      this.mDiffuseTexture0EnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D MaterialTexture0
  {
    get => this.mMaterialTexture0Parameter.GetValueTexture2D();
    set
    {
      this.mMaterialTexture0Parameter.SetValue((Texture) value);
      this.mMaterialTexture0EnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D NormalTexture0
  {
    get => this.mNormalTexture0Parameter.GetValueTexture2D();
    set
    {
      this.mNormalTexture0Parameter.SetValue((Texture) value);
      this.mNormalTexture0EnabledParameter.SetValue(value != null);
    }
  }

  public bool DiffuseTexture1AlphaDisabled
  {
    get => this.mDiffuseTexture1AlphaDisabledParameter.GetValueBoolean();
    set => this.mDiffuseTexture1AlphaDisabledParameter.SetValue(value);
  }

  public bool AlphaMask1Enabled
  {
    get => this.mAlphaMask1EnabledParameter.GetValueBoolean();
    set => this.mAlphaMask1EnabledParameter.SetValue(value);
  }

  public Vector3 DiffuseColor1
  {
    get => this.mDiffuseColor1Parameter.GetValueVector3();
    set => this.mDiffuseColor1Parameter.SetValue(value);
  }

  public float SpecAmount1
  {
    get => this.mSpecAmount1Parameter.GetValueSingle();
    set => this.mSpecAmount1Parameter.SetValue(value);
  }

  public float SpecPower1
  {
    get => this.mSpecPower1Parameter.GetValueSingle();
    set => this.mSpecPower1Parameter.SetValue(value);
  }

  public float EmissiveAmount1
  {
    get => this.mEmissiveAmount1Parameter.GetValueSingle();
    set => this.mEmissiveAmount1Parameter.SetValue(value);
  }

  public float NormalPower1
  {
    get => this.mNormalPower1Parameter.GetValueSingle();
    set => this.mNormalPower1Parameter.SetValue(value);
  }

  public float Reflectiveness1
  {
    get => this.mReflectiveness1Parameter.GetValueSingle();
    set => this.mReflectiveness1Parameter.SetValue(value);
  }

  public Texture2D DiffuseTexture1
  {
    get => this.mDiffuseTexture1Parameter.GetValueTexture2D();
    set
    {
      this.mDiffuseTexture1Parameter.SetValue((Texture) value);
      this.mDiffuseTexture1EnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D MaterialTexture1
  {
    get => this.mMaterialTexture1Parameter.GetValueTexture2D();
    set
    {
      this.mMaterialTexture1Parameter.SetValue((Texture) value);
      this.mMaterialTexture1EnabledParameter.SetValue(value != null);
    }
  }

  public Texture2D NormalTexture1
  {
    get => this.mNormalTexture1Parameter.GetValueTexture2D();
    set
    {
      this.mNormalTexture1Parameter.SetValue((Texture) value);
      this.mNormalTexture1EnabledParameter.SetValue(value != null);
    }
  }

  public float Time
  {
    get => this.mTimeParameter.GetValueSingle();
    set => this.mTimeParameter.SetValue(value);
  }

  public bool SwayEnabled
  {
    get => this.mSwayEnabledParameter.GetValueBoolean();
    set => this.mSwayEnabledParameter.SetValue(value);
  }

  public float Sway
  {
    get => this.mSwayParameter.GetValueSingle();
    set => this.mSwayParameter.SetValue(value);
  }

  public float EntityInfluence
  {
    get => this.mEntityInfluenceParameter.GetValueSingle();
    set => this.mEntityInfluenceParameter.SetValue(value);
  }

  public float GroundLevel
  {
    get => this.mGroundLevelParameter.GetValueSingle();
    set => this.mGroundLevelParameter.SetValue(value);
  }

  public Matrix SwayProjection
  {
    get => this.mSwayProjectionParameter.GetValueMatrix();
    set => this.mSwayProjectionParameter.SetValue(value);
  }

  public Texture2D SwayTexture
  {
    get => this.mSwayTextureParameter.GetValueTexture2D();
    set => this.mSwayTextureParameter.SetValue((Texture) value);
  }

  public float FresnelPower
  {
    get => this.mFresnelPowerParameter.GetValueSingle();
    set => this.mFresnelPowerParameter.SetValue(value);
  }

  internal static RenderDeferredEffect Read(ContentReader iInput)
  {
    RenderDeferredEffect renderDeferredEffect = new RenderDeferredEffect((iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice, (EffectPool) null);
    renderDeferredEffect.Alpha = iInput.ReadSingle();
    renderDeferredEffect.Sharpness = iInput.ReadSingle();
    renderDeferredEffect.VertexColorEnabled = iInput.ReadBoolean();
    renderDeferredEffect.UseMaterialTextureForReflectiveness = iInput.ReadBoolean();
    renderDeferredEffect.ReflectionMap = iInput.ReadExternalReference<TextureCube>();
    renderDeferredEffect.DiffuseTexture0AlphaDisabled = iInput.ReadBoolean();
    renderDeferredEffect.AlphaMask0Enabled = iInput.ReadBoolean();
    renderDeferredEffect.DiffuseColor0 = iInput.ReadVector3();
    renderDeferredEffect.SpecAmount0 = iInput.ReadSingle();
    renderDeferredEffect.SpecPower0 = iInput.ReadSingle();
    renderDeferredEffect.EmissiveAmount0 = iInput.ReadSingle();
    renderDeferredEffect.NormalPower0 = iInput.ReadSingle();
    renderDeferredEffect.Reflectiveness0 = iInput.ReadSingle();
    renderDeferredEffect.DiffuseTexture0 = iInput.ReadExternalReference<Texture2D>();
    renderDeferredEffect.MaterialTexture0 = iInput.ReadExternalReference<Texture2D>();
    renderDeferredEffect.NormalTexture0 = iInput.ReadExternalReference<Texture2D>();
    if (iInput.ReadBoolean())
    {
      renderDeferredEffect.DiffuseTexture1AlphaDisabled = iInput.ReadBoolean();
      renderDeferredEffect.AlphaMask1Enabled = iInput.ReadBoolean();
      renderDeferredEffect.DiffuseColor1 = iInput.ReadVector3();
      renderDeferredEffect.SpecAmount1 = iInput.ReadSingle();
      renderDeferredEffect.SpecPower1 = iInput.ReadSingle();
      renderDeferredEffect.EmissiveAmount1 = iInput.ReadSingle();
      renderDeferredEffect.NormalPower1 = iInput.ReadSingle();
      renderDeferredEffect.Reflectiveness1 = iInput.ReadSingle();
      renderDeferredEffect.DiffuseTexture1 = iInput.ReadExternalReference<Texture2D>();
      renderDeferredEffect.MaterialTexture1 = iInput.ReadExternalReference<Texture2D>();
      renderDeferredEffect.NormalTexture1 = iInput.ReadExternalReference<Texture2D>();
    }
    renderDeferredEffect.World = Matrix.Identity;
    return renderDeferredEffect;
  }

  public enum Technique
  {
    SingleLayer,
    DualLayer,
    SingleLayerReflection,
    DualLayerReflection,
    Depth,
    Shadow,
    AdditiveFresnel,
  }
}
