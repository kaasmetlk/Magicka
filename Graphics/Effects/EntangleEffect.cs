// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.EntangleEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class EntangleEffect : Effect
{
  public const int MAXBONES = 10;
  public static readonly int TYPEHASH = typeof (EntangleEffect).GetHashCode();
  private EffectParameter mDiffuseColorParameter;
  private EffectParameter mEmissiveAmountParameter;
  private EffectParameter mSpecularAmountParameter;
  private EffectParameter mSpecularPowerParameter;
  private EffectParameter mDiffuseMapEnabledParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mMatBonesParameter;
  private EffectParameter mAlphaBiasParameter;
  private EffectParameter mDiffuseMapParameter;
  private EffectParameter mColorTintParameter;

  public EntangleEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/EntangleEffect"))
  {
    this.mDiffuseColorParameter = this.Parameters[nameof (DiffuseColor)];
    this.mEmissiveAmountParameter = this.Parameters[nameof (EmissiveAmount)];
    this.mSpecularAmountParameter = this.Parameters[nameof (SpecularAmount)];
    this.mSpecularPowerParameter = this.Parameters[nameof (SpecularPower)];
    this.mDiffuseMapEnabledParameter = this.Parameters[nameof (DiffuseMapEnabled)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mMatBonesParameter = this.Parameters[nameof (MatBones)];
    this.mAlphaBiasParameter = this.Parameters[nameof (AlphaBias)];
    this.mDiffuseMapParameter = this.Parameters[nameof (DiffuseMap)];
    this.mColorTintParameter = this.Parameters[nameof (ColorTint)];
  }

  public void SetTechnique(EntangleEffect.Technique iTechnique)
  {
    this.CurrentTechnique = this.Techniques[(int) iTechnique];
  }

  public Vector3 DiffuseColor
  {
    get => this.mDiffuseColorParameter.GetValueVector3();
    set => this.mDiffuseColorParameter.SetValue(value);
  }

  public Vector4 ColorTint
  {
    get => this.mColorTintParameter.GetValueVector4();
    set => this.mColorTintParameter.SetValue(value);
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

  public float SpecularPower
  {
    get => this.mSpecularPowerParameter.GetValueSingle();
    set => this.mSpecularPowerParameter.SetValue(value);
  }

  public bool DiffuseMapEnabled
  {
    get => this.mDiffuseMapEnabledParameter.GetValueBoolean();
    set => this.mDiffuseMapEnabledParameter.SetValue(value);
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

  public Matrix[] MatBones
  {
    get => this.mMatBonesParameter.GetValueMatrixArray(10);
    set => this.mMatBonesParameter.SetValue(value);
  }

  public float AlphaBias
  {
    get => this.mAlphaBiasParameter.GetValueSingle();
    set => this.mAlphaBiasParameter.SetValue(value);
  }

  public Texture2D DiffuseMap
  {
    get => this.mDiffuseMapParameter.GetValueTexture2D();
    set => this.mDiffuseMapParameter.SetValue((Texture) value);
  }

  public enum Technique
  {
    Default,
    Additive,
    Depth,
    Shadow,
  }
}
