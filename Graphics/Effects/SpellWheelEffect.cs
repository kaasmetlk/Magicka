// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.SpellWheelEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

#nullable disable
namespace Magicka.Graphics.Effects;

public class SpellWheelEffect : Effect
{
  private EffectParameter mDirectionRadiusParamter;
  private EffectParameter mTargetRadiusParameter;
  private EffectParameter mTargetDirectionParameter;
  private EffectParameter mTargetPositionParameter;
  private EffectParameter mWheelPositionParameter;
  private EffectParameter mTransformToScreenParameter;
  private EffectParameter mScreenRectangleParameter;
  private EffectParameter mAlphaParameter;
  private EffectParameter mUseAlphaParameter;
  private EffectParameter mTextureParameter;

  public SpellWheelEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("shaders/SpellWheel"))
  {
    this.mDirectionRadiusParamter = this.Parameters[nameof (DirectionRadius)];
    this.mTargetRadiusParameter = this.Parameters[nameof (TargetRadius)];
    this.mTargetPositionParameter = this.Parameters[nameof (TargetPosition)];
    this.mTargetDirectionParameter = this.Parameters[nameof (TargetDirection)];
    this.mWheelPositionParameter = this.Parameters[nameof (WheelPosition)];
    this.mTransformToScreenParameter = this.Parameters["TransformToScreen"];
    this.mScreenRectangleParameter = this.Parameters[nameof (ScreenRectangle)];
    this.mAlphaParameter = this.Parameters[nameof (Alpha)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
    this.mUseAlphaParameter = this.Parameters[nameof (UseAlpha)];
  }

  public bool UseAlpha
  {
    get => this.mUseAlphaParameter.GetValueBoolean();
    set => this.mUseAlphaParameter.SetValue(value);
  }

  public Texture2D Texture
  {
    get => this.mTextureParameter.GetValueTexture2D();
    set => this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }

  public float Alpha
  {
    get => this.mAlphaParameter.GetValueSingle();
    set => this.mAlphaParameter.SetValue(value);
  }

  public Rectangle ScreenRectangle
  {
    get
    {
      Rectangle screenRectangle = new Rectangle();
      Vector4 valueVector4 = this.mScreenRectangleParameter.GetValueVector4();
      screenRectangle.X = (int) valueVector4.X;
      screenRectangle.Y = (int) valueVector4.Y;
      screenRectangle.Width = (int) valueVector4.Z;
      screenRectangle.Height = (int) valueVector4.W;
      return screenRectangle;
    }
    set
    {
      this.mScreenRectangleParameter.SetValue(new Vector4((float) value.X, (float) value.Y, (float) value.Width, (float) value.Height));
    }
  }

  public void SetScreenSize(int iWidth, int iHeight)
  {
    Matrix oTransform;
    RenderManager.Instance.CreateTransformPixelsToScreen((float) iWidth, (float) iHeight, out oTransform);
    this.mTransformToScreenParameter.SetValue(oTransform);
  }

  public float TargetRadius
  {
    get => this.mTargetRadiusParameter.GetValueSingle();
    set => this.mTargetRadiusParameter.SetValue(value);
  }

  public float DirectionRadius
  {
    get => this.mDirectionRadiusParamter.GetValueSingle();
    set => this.mDirectionRadiusParamter.SetValue(value);
  }

  public Vector2 TargetPosition
  {
    get => this.mTargetPositionParameter.GetValueVector2();
    set => this.mTargetPositionParameter.SetValue(value);
  }

  public Vector2 TargetDirection
  {
    get => this.mTargetDirectionParameter.GetValueVector2();
    set => this.mTargetDirectionParameter.SetValue(value);
  }

  public Vector2 WheelPosition
  {
    get => this.mWheelPositionParameter.GetValueVector2();
    set => this.mWheelPositionParameter.SetValue(value);
  }
}
