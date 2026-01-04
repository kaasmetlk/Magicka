// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.VortexEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class VortexEffect : Effect
{
  public static readonly int TYPEHASH = typeof (VortexEffect).GetHashCode();
  private EffectParameter mWorldParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mDistortionParameter;
  private EffectParameter mDistortionPowerParameter;
  private EffectParameter mPixelSizeParameter;
  private EffectParameter mSourceTextureParameter;
  private EffectParameter mDepthTextureParameter;
  private EffectParameter mTextureParameter;

  public VortexEffect()
    : base(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content.Load<Effect>("Shaders/VortexEffect"))
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mDistortionParameter = this.Parameters[nameof (Distortion)];
    this.mDistortionPowerParameter = this.Parameters[nameof (DistortionPower)];
    this.mPixelSizeParameter = this.Parameters[nameof (PixelSize)];
    this.mSourceTextureParameter = this.Parameters[nameof (SourceTexture)];
    this.mDepthTextureParameter = this.Parameters[nameof (DepthTexture)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
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

  public float Distortion
  {
    get => this.mDistortionParameter.GetValueSingle();
    set => this.mDistortionParameter.SetValue(value);
  }

  public float DistortionPower
  {
    get => this.mDistortionPowerParameter.GetValueSingle();
    set => this.mDistortionPowerParameter.SetValue(value);
  }

  public Vector2 PixelSize
  {
    get => this.mPixelSizeParameter.GetValueVector2();
    set => this.mPixelSizeParameter.SetValue(value);
  }

  public Texture2D SourceTexture
  {
    get => this.mSourceTextureParameter.GetValueTexture2D();
    set => this.mSourceTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }

  public Texture2D DepthTexture
  {
    get => this.mDepthTextureParameter.GetValueTexture2D();
    set => this.mDepthTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }

  public Texture2D Texture
  {
    get => this.mTextureParameter.GetValueTexture2D();
    set => this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }
}
