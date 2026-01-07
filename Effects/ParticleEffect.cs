// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.ParticleEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class ParticleEffect : Effect
{
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mSpritesAParameter;
  private EffectParameter mSpritesBParameter;
  private EffectParameter mSpritesCParameter;
  private EffectParameter mSpritesDParameter;
  private EffectParameter mDestinationDimensionsParameter;
  private EffectParameter mDepthTextureParameter;

  public ParticleEffect(GraphicsDevice iDevice, EffectPool iEffectPool)
    : base(iDevice, ParticleEffectCode.CODE, CompilerOptions.NotCloneable, iEffectPool)
  {
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mSpritesAParameter = this.Parameters[nameof (SpritesA)];
    this.mSpritesBParameter = this.Parameters[nameof (SpritesB)];
    this.mSpritesCParameter = this.Parameters[nameof (SpritesC)];
    this.mSpritesDParameter = this.Parameters[nameof (SpritesD)];
    this.mDestinationDimensionsParameter = this.Parameters[nameof (DestinationDimensions)];
    this.mDepthTextureParameter = this.Parameters[nameof (DepthTexture)];
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

  public Texture3D SpritesA
  {
    get => this.mSpritesAParameter.GetValueTexture3D();
    set => this.mSpritesAParameter.SetValue((Texture) value);
  }

  public Texture3D SpritesB
  {
    get => this.mSpritesBParameter.GetValueTexture3D();
    set => this.mSpritesBParameter.SetValue((Texture) value);
  }

  public Texture3D SpritesC
  {
    get => this.mSpritesCParameter.GetValueTexture3D();
    set => this.mSpritesCParameter.SetValue((Texture) value);
  }

  public Texture3D SpritesD
  {
    get => this.mSpritesDParameter.GetValueTexture3D();
    set => this.mSpritesDParameter.SetValue((Texture) value);
  }

  public Vector2 DestinationDimensions
  {
    get => this.mDestinationDimensionsParameter.GetValueVector2();
    set => this.mDestinationDimensionsParameter.SetValue(value);
  }

  public Texture2D DepthTexture
  {
    get => this.mDepthTextureParameter.GetValueTexture2D();
    set => this.mDepthTextureParameter.SetValue((Texture) value);
  }

  public enum Technique
  {
    Default,
  }
}
