// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.DecalHardwareInstancingEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class DecalHardwareInstancingEffect : Effect
{
  public const int MAXINSTANCES = 32 /*0x20*/;
  public static readonly int TYPEHASH = typeof (DecalHardwareInstancingEffect).GetHashCode();
  private EffectTechnique mDefaultTechnique;
  private EffectTechnique mNormalMappedTechnique;
  private EffectParameter mTransformsParameter;
  private EffectParameter mTextureOffsetsParameter;
  private EffectParameter mTTLsParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mDiffuseMapParameter;
  private EffectParameter mNormalMapParameter;
  private EffectParameter mTextureScaleParameter;

  public DecalHardwareInstancingEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/DecalHardwareInstancingEffect"))
  {
    this.mDefaultTechnique = this.Techniques["Default"];
    this.mNormalMappedTechnique = this.Techniques["NormalMapped"];
    this.mTransformsParameter = this.Parameters[nameof (Transforms)];
    this.mTextureOffsetsParameter = this.Parameters[nameof (TextureOffsets)];
    this.mTTLsParameter = this.Parameters[nameof (TTLs)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mDiffuseMapParameter = this.Parameters[nameof (DiffuseMap)];
    this.mNormalMapParameter = this.Parameters[nameof (NormalMap)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
  }

  public void SetTechnique(DecalHardwareInstancingEffect.Technique iTechnique)
  {
    switch (iTechnique)
    {
      case DecalHardwareInstancingEffect.Technique.Default:
        this.CurrentTechnique = this.mDefaultTechnique;
        break;
      case DecalHardwareInstancingEffect.Technique.NormalMapped:
        this.CurrentTechnique = this.mNormalMappedTechnique;
        break;
    }
  }

  public Matrix[] Transforms
  {
    get => this.mTransformsParameter.GetValueMatrixArray(32 /*0x20*/);
    set => this.mTransformsParameter.SetValue(value);
  }

  public Vector2[] TextureOffsets
  {
    get => this.mTextureOffsetsParameter.GetValueVector2Array(32 /*0x20*/);
    set => this.mTextureOffsetsParameter.SetValue(value);
  }

  public Vector3[] TTLs
  {
    get => this.mTTLsParameter.GetValueVector3Array(32 /*0x20*/);
    set => this.mTTLsParameter.SetValue(value);
  }

  public Vector2 TextureScale
  {
    get => this.mTextureScaleParameter.GetValueVector2();
    set => this.mTextureScaleParameter.SetValue(value);
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Texture2D DiffuseMap
  {
    get => this.mDiffuseMapParameter.GetValueTexture2D();
    set => this.mDiffuseMapParameter.SetValue((Texture) value);
  }

  public Texture2D NormalMap
  {
    get => this.mNormalMapParameter.GetValueTexture2D();
    set => this.mNormalMapParameter.SetValue((Texture) value);
  }

  public enum Technique
  {
    Default,
    NormalMapped,
  }
}
