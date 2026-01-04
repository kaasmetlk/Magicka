// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.NormalDistortionEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class NormalDistortionEffect : Effect
{
  private EffectParameter mWorldParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mDistortionParameter;
  private EffectParameter mTimeParameter;
  private EffectParameter mTextureScaleParameter;
  private EffectParameter mPixelSizeParameter;
  private EffectParameter mSourceTextureParameter;
  private EffectParameter mDepthTextureParameter;
  private EffectParameter mNormalTextureParameter;

  public NormalDistortionEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/NormalDistortionEffect"))
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
    this.mTimeParameter = this.Parameters[nameof (Time)];
    this.mDistortionParameter = this.Parameters[nameof (Distortion)];
    this.mPixelSizeParameter = this.Parameters[nameof (PixelSize)];
    this.mSourceTextureParameter = this.Parameters[nameof (SourceTexture)];
    this.mDepthTextureParameter = this.Parameters[nameof (DepthTexture)];
    this.mNormalTextureParameter = this.Parameters[nameof (NormalTexture)];
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

  public float Time
  {
    get => this.mTimeParameter.GetValueSingle();
    set => this.mTimeParameter.SetValue(value);
  }

  public float Distortion
  {
    get => this.mDistortionParameter.GetValueSingle();
    set => this.mDistortionParameter.SetValue(value);
  }

  public float TextureScale
  {
    get => this.mTextureScaleParameter.GetValueSingle();
    set => this.mTextureScaleParameter.SetValue(value);
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

  public Texture2D NormalTexture
  {
    get => this.mNormalTextureParameter.GetValueTexture2D();
    set => this.mNormalTextureParameter.SetValue((Texture) value);
  }
}
