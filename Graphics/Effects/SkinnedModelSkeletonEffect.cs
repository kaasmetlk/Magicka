// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.SkinnedModelSkeletonEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class SkinnedModelSkeletonEffect : Effect
{
  public static readonly int TYPEHASH = typeof (SkinnedModelSkeletonEffect).GetHashCode();
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mBonesParameter;
  private EffectParameter mDiffuseMap0EnabledParameter;
  private EffectParameter mDiffuseMap1EnabledParameter;
  private EffectParameter mDiffuseMap0Parameter;
  private EffectParameter mDiffuseMap1Parameter;
  private EffectParameter mDepthMapParameter;

  public SkinnedModelSkeletonEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/SkinnedModelSkeletonEffect"))
  {
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mBonesParameter = this.Parameters[nameof (Bones)];
    this.mDiffuseMap0EnabledParameter = this.Parameters[nameof (DiffuseMap0Enabled)];
    this.mDiffuseMap1EnabledParameter = this.Parameters[nameof (DiffuseMap1Enabled)];
    this.mDiffuseMap0Parameter = this.Parameters[nameof (DiffuseMap0)];
    this.mDiffuseMap1Parameter = this.Parameters[nameof (DiffuseMap1)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
  }

  public void SetTechnique(SkinnedModelSkeletonEffect.Technique iTechnique)
  {
    this.CurrentTechnique = this.Techniques[(int) iTechnique];
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

  public Matrix[] Bones
  {
    get => this.mBonesParameter.GetValueMatrixArray(80 /*0x50*/);
    set => this.mBonesParameter.SetValue(value);
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

  public Texture2D DepthMap
  {
    get => this.mDepthMapParameter.GetValueTexture2D();
    set => this.mDepthMapParameter.SetValue((Texture) value);
  }

  public enum Technique
  {
    Body,
    Skeleton,
  }
}
