// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.TextBoxEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class TextBoxEffect : Effect
{
  private EffectTechnique mTechnique1Technique;
  private EffectParameter mScreenSizeParameter;
  private Vector2 mScreenSize;
  private EffectParameter mPositionParameter;
  private Vector2 mPosition;
  private EffectParameter mSizeParameter;
  private Vector2 mSize;
  private EffectParameter mBorderSizeParameter;
  private float mBorderSize;
  private EffectParameter mScaleParameter;
  private float mScale;
  private EffectParameter mColorParameter;
  private Vector4 mColor;
  private EffectParameter mTextureParameter;
  private Texture mTexture;

  public TextBoxEffect(GraphicsDevice iDevice, ContentManager iContent)
    : base(iDevice, iContent.Load<Effect>("shaders/textboxeffect"))
  {
    this.mTechnique1Technique = this.Techniques["Technique1"];
    this.mScreenSizeParameter = this.Parameters[nameof (ScreenSize)];
    this.mPositionParameter = this.Parameters[nameof (Position)];
    this.mSizeParameter = this.Parameters[nameof (Size)];
    this.mScaleParameter = this.Parameters[nameof (Scale)];
    this.mBorderSizeParameter = this.Parameters[nameof (BorderSize)];
    this.mColorParameter = this.Parameters[nameof (Color)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
  }

  public void SetTechnique(TextBoxEffect.Technique iTechnique)
  {
    if (iTechnique != TextBoxEffect.Technique.Technique1)
      return;
    this.CurrentTechnique = this.mTechnique1Technique;
  }

  public Vector2 ScreenSize
  {
    get => this.mScreenSize;
    set
    {
      this.mScreenSize = value;
      this.mScreenSizeParameter.SetValue(value);
    }
  }

  public Vector2 Position
  {
    get => this.mPosition;
    set
    {
      this.mPosition = value;
      this.mPositionParameter.SetValue(value);
    }
  }

  public Vector2 Size
  {
    get => this.mSize;
    set
    {
      this.mSize = value;
      this.mSizeParameter.SetValue(value);
    }
  }

  public float BorderSize
  {
    get => this.mBorderSize;
    set
    {
      this.mBorderSize = value;
      this.mBorderSizeParameter.SetValue(value);
    }
  }

  public float Scale
  {
    get => this.mScale;
    set
    {
      this.mScale = value;
      this.mScaleParameter.SetValue(value);
    }
  }

  public Vector4 Color
  {
    get => this.mColor;
    set
    {
      this.mColor = value;
      this.mColorParameter.SetValue(value);
    }
  }

  public Texture Texture
  {
    get => this.mTexture;
    set
    {
      this.mTexture = value;
      this.mTextureParameter.SetValue(value);
    }
  }

  public enum Technique
  {
    Technique1,
  }
}
