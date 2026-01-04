// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.SwayEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class SwayEffect : Effect
{
  private EffectTechnique mSwayTechnique;
  private EffectTechnique mCharacterOffsetTechnique;
  private EffectParameter mWorldParameter;
  private Matrix mWorld;
  private EffectParameter mViewProjectionParameter;
  private Matrix mViewProjection;
  private EffectParameter mTextureTransformParameter;
  private Matrix mTextureTransform;
  private EffectParameter mTextureParameter;
  private Texture2D mTexture;

  public SwayEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/SwayEffect"))
  {
    this.mSwayTechnique = this.Techniques["Sway"];
    this.mCharacterOffsetTechnique = this.Techniques["CharacterOffset"];
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mTextureTransformParameter = this.Parameters[nameof (TextureTransform)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
  }

  public void SetTechnique(SwayEffect.Technique iTechnique)
  {
    switch (iTechnique)
    {
      case SwayEffect.Technique.Sway:
        this.CurrentTechnique = this.mSwayTechnique;
        break;
      case SwayEffect.Technique.CharacterOffset:
        this.CurrentTechnique = this.mCharacterOffsetTechnique;
        break;
    }
  }

  public Matrix World
  {
    get => this.mWorld;
    set
    {
      if (!(this.mWorld != value))
        return;
      this.mWorld = value;
      this.mWorldParameter.SetValue(value);
    }
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjection;
    set
    {
      if (!(this.mViewProjection != value))
        return;
      this.mViewProjection = value;
      this.mViewProjectionParameter.SetValue(value);
    }
  }

  public Matrix TextureTransform
  {
    get => this.mTextureTransform;
    set
    {
      if (!(this.mTextureTransform != value))
        return;
      this.mTextureTransform = value;
      this.mTextureTransformParameter.SetValue(value);
    }
  }

  public Texture2D Texture
  {
    get => this.mTexture;
    set
    {
      if (this.mTexture == value)
        return;
      this.mTexture = value;
      this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
    }
  }

  public enum Technique
  {
    Sway,
    CharacterOffset,
  }
}
