// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.NotifierButtonEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class NotifierButtonEffect : Effect
{
  private EffectParameter mScreenSizeParameter;
  private EffectParameter mPositionParameter;
  private EffectParameter mWidthParameter;
  private EffectParameter mColorParameter;
  private EffectParameter mAlphaParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mScaleParameter;

  public NotifierButtonEffect(GraphicsDevice iDevice, ContentManager iContent)
    : base(iDevice, iContent.Load<Effect>("shaders/NotifierButtonEffect"))
  {
    this.mScreenSizeParameter = this.Parameters[nameof (ScreenSize)];
    this.mPositionParameter = this.Parameters[nameof (Position)];
    this.mWidthParameter = this.Parameters[nameof (Width)];
    this.mColorParameter = this.Parameters[nameof (Color)];
    this.mAlphaParameter = this.Parameters["Alpha"];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
    this.mScaleParameter = this.Parameters[nameof (Scale)];
  }

  public Vector2 ScreenSize
  {
    get => this.mScreenSizeParameter.GetValueVector2();
    set => this.mScreenSizeParameter.SetValue(value);
  }

  public Vector4 Color
  {
    get => this.mColorParameter.GetValueVector4();
    set => this.mColorParameter.SetValue(value);
  }

  public Vector2 Position
  {
    get => this.mPositionParameter.GetValueVector2();
    set => this.mPositionParameter.SetValue(value);
  }

  public float Width
  {
    get => this.mWidthParameter.GetValueSingle();
    set => this.mWidthParameter.SetValue(value);
  }

  public Texture2D Texture
  {
    get => this.mTextureParameter.GetValueTexture2D();
    set => this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }

  public Vector2 Scale
  {
    get => this.mScaleParameter.GetValueVector2();
    set => this.mScaleParameter.SetValue(value);
  }
}
