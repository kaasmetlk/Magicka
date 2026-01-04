// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.PieEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

#nullable disable
namespace Magicka.Graphics.Effects;

public class PieEffect : Effect
{
  private EffectTechnique mTechnique1;
  private EffectParameter mTransformParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mTextureOffsetParameter;
  private EffectParameter mTransformToScreenParameter;
  private EffectParameter mMaxAngleParameter;
  private EffectParameter mRadiusParameter;

  public PieEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/PieEffect"))
  {
    this.mTechnique1 = this.Techniques["Technique1"];
    this.mTransformParameter = this.Parameters[nameof (Transform)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
    this.mTextureOffsetParameter = this.Parameters[nameof (TextureOffset)];
    this.mTransformToScreenParameter = this.Parameters["TransformToScreen"];
    this.mMaxAngleParameter = this.Parameters[nameof (MaxAngle)];
    this.mRadiusParameter = this.Parameters[nameof (Radius)];
  }

  public float Radius
  {
    get => this.mRadiusParameter.GetValueSingle();
    set => this.mRadiusParameter.SetValue(value);
  }

  public float MaxAngle
  {
    get => this.mMaxAngleParameter.GetValueSingle();
    set => this.mMaxAngleParameter.SetValue(value);
  }

  public void SetScreenSize(int iWidth, int iHeight)
  {
    Matrix oTransform;
    RenderManager.Instance.CreateTransformPixelsToScreen((float) iWidth, (float) iHeight, out oTransform);
    this.mTransformToScreenParameter.SetValue(oTransform);
  }

  public void SetTechnique(PieEffect.Technique iTechnique)
  {
    if (iTechnique != PieEffect.Technique.Technique1)
      return;
    this.CurrentTechnique = this.mTechnique1;
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

  public Texture Texture
  {
    set => this.mTextureParameter.SetValue(value);
  }

  public enum Technique
  {
    Technique1,
  }
}
