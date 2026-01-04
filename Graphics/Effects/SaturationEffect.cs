// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.SaturationEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

#nullable disable
namespace Magicka.Graphics.Effects;

public class SaturationEffect : Effect
{
  private EffectParameter mTransformParameter;
  private EffectParameter mColorParameter;
  private EffectParameter mTextureEnabledParameter;
  private EffectParameter mVertexColorEnabledParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mTextureOffsetParameter;
  private EffectParameter mTransformToScreenParameter;
  private EffectParameter mSaturationParameter;

  public SaturationEffect()
    : base(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content.Load<Effect>("shaders/SaturationEffect"))
  {
    this.mTransformParameter = this.Parameters[nameof (Transform)];
    this.mColorParameter = this.Parameters[nameof (Color)];
    this.mTextureEnabledParameter = this.Parameters[nameof (TextureEnabled)];
    this.mVertexColorEnabledParameter = this.Parameters[nameof (VertexColorEnabled)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
    this.mTransformToScreenParameter = this.Parameters["TransformToScreen"];
    this.mTextureOffsetParameter = this.Parameters[nameof (TextureOffset)];
    this.mSaturationParameter = this.Parameters[nameof (Saturation)];
  }

  public void SetScreenSize(int iWidth, int iHeight)
  {
    Matrix oTransform;
    RenderManager.Instance.CreateTransformPixelsToScreen((float) iWidth, (float) iHeight, out oTransform);
    this.mTransformToScreenParameter.SetValue(oTransform);
  }

  public Vector2 TextureOffset
  {
    get => this.mTextureOffsetParameter.GetValueVector2();
    set => this.mTextureOffsetParameter.SetValue(value);
  }

  public Matrix Transform
  {
    get => this.mTransformParameter.GetValueMatrix();
    set => this.mTransformParameter.SetValue(value);
  }

  public Vector4 Color
  {
    get => this.mColorParameter.GetValueVector4();
    set => this.mColorParameter.SetValue(value);
  }

  public bool TextureEnabled
  {
    get => this.mTextureEnabledParameter.GetValueBoolean();
    set => this.mTextureEnabledParameter.SetValue(value);
  }

  public bool VertexColorEnabled
  {
    get => this.mVertexColorEnabledParameter.GetValueBoolean();
    set => this.mVertexColorEnabledParameter.SetValue(value);
  }

  public Texture2D Texture
  {
    get => this.mTextureParameter.GetValueTexture2D();
    set => this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }

  public float Saturation
  {
    get => this.mSaturationParameter.GetValueSingle();
    set => this.mSaturationParameter.SetValue(value);
  }
}
