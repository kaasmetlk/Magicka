// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.InvisibilityEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class InvisibilityEffect : Effect
{
  private EffectParameter mBonesParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mDistortionParameter;
  private EffectParameter mBloatParameter;
  private EffectParameter mPixelSizeParameter;
  private EffectParameter mSourceTextureParameter;
  private EffectParameter mDepthTextureParameter;

  public InvisibilityEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/InvisibilityEffect"))
  {
    this.mBonesParameter = this.Parameters[nameof (Bones)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mDistortionParameter = this.Parameters[nameof (Distortion)];
    this.mBloatParameter = this.Parameters[nameof (Bloat)];
    this.mPixelSizeParameter = this.Parameters[nameof (PixelSize)];
    this.mSourceTextureParameter = this.Parameters["SourceMap"];
    this.mDepthTextureParameter = this.Parameters["DepthMap"];
  }

  public Matrix[] Bones
  {
    get => this.mBonesParameter.GetValueMatrixArray(80 /*0x50*/);
    set => this.mBonesParameter.SetValue(value);
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

  public float Distortion
  {
    get => this.mDistortionParameter.GetValueSingle();
    set => this.mDistortionParameter.SetValue(value);
  }

  public float Bloat
  {
    get => this.mBloatParameter.GetValueSingle();
    set => this.mBloatParameter.SetValue(value);
  }

  public Vector2 PixelSize
  {
    get => this.mPixelSizeParameter.GetValueVector2();
    set => this.mPixelSizeParameter.SetValue(value);
  }

  public Texture2D SourceTexture
  {
    get => this.mSourceTextureParameter.GetValueTexture2D();
    set => this.mSourceTextureParameter.SetValue((Texture) value);
  }

  public Texture2D DepthTexture
  {
    get => this.mDepthTextureParameter.GetValueTexture2D();
    set => this.mDepthTextureParameter.SetValue((Texture) value);
  }
}
