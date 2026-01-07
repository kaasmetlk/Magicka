// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.GUIBasicEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class GUIBasicEffect : Effect
{
  private EffectTechnique mTechnique2DTechnique;
  private EffectTechnique mTechnique3DTechnique;
  private EffectParameter mTransformParameter;
  private EffectParameter mColorParameter;
  private EffectParameter mTextureEnabledParameter;
  private EffectParameter mVertexColorEnabledParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mOverlayTextureEnabledParameter;
  private EffectParameter mWParameter;
  private EffectParameter mOverlayTextureParameter;
  private EffectParameter mOverlayTintParameter;
  private EffectParameter mTextureOffsetParameter;
  private EffectParameter mTextureScaleParameter;
  private EffectParameter mSaturationParameter;
  private EffectParameter mScaleToHDRParameter;
  private EffectParameter mTransformToScreenParameter;
  private Vector2 mOffset;
  private Matrix mTransformToScreen = Matrix.Identity;

  public GUIBasicEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, GUIBasicEffectCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mTechnique2DTechnique = this.Techniques["Technique2D"];
    this.mTechnique3DTechnique = this.Techniques["Technique3D"];
    this.mTransformParameter = this.Parameters[nameof (Transform)];
    this.mColorParameter = this.Parameters[nameof (Color)];
    this.mTextureEnabledParameter = this.Parameters[nameof (TextureEnabled)];
    this.mVertexColorEnabledParameter = this.Parameters[nameof (VertexColorEnabled)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
    this.mOverlayTextureEnabledParameter = this.Parameters[nameof (OverlayTextureEnabled)];
    this.mWParameter = this.Parameters[nameof (W)];
    this.mOverlayTextureParameter = this.Parameters[nameof (OverlayTexture)];
    this.mOverlayTintParameter = this.Parameters[nameof (OverlayTint)];
    this.mTransformToScreenParameter = this.Parameters["TransformToScreen"];
    this.mTextureOffsetParameter = this.Parameters[nameof (TextureOffset)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
    this.mScaleToHDRParameter = this.Parameters[nameof (ScaleToHDR)];
    this.mSaturationParameter = this.Parameters[nameof (Saturation)];
  }

  public float Saturation
  {
    get => this.mSaturationParameter.GetValueSingle();
    set => this.mSaturationParameter.SetValue(value);
  }

  public bool ScaleToHDR
  {
    get => this.mScaleToHDRParameter.GetValueBoolean();
    set => this.mScaleToHDRParameter.SetValue(value);
  }

  public void SetScreenSize(int iWidth, int iHeight)
  {
    RenderManager.Instance.CreateTransformPixelsToScreen((float) iWidth, (float) iHeight, out this.mTransformToScreen);
    this.SetTransformToScreenParameter();
  }

  public void SetOffset(float iX, float iY)
  {
    this.mOffset.X = iX;
    this.mOffset.Y = iY;
    this.SetTransformToScreenParameter();
  }

  public void AddOffset(float iX, float iY)
  {
    this.mOffset.X += iX;
    this.mOffset.Y += iY;
    this.SetTransformToScreenParameter();
  }

  private void SetTransformToScreenParameter()
  {
    Matrix transformToScreen = this.mTransformToScreen;
    transformToScreen.M41 += this.mOffset.X * transformToScreen.M11;
    transformToScreen.M42 += this.mOffset.Y * transformToScreen.M22;
    this.mTransformToScreenParameter.SetValue(transformToScreen);
  }

  public void SetTechnique(GUIBasicEffect.Technique iTechnique)
  {
    switch (iTechnique)
    {
      case GUIBasicEffect.Technique.Texture2D:
        this.CurrentTechnique = this.mTechnique2DTechnique;
        break;
      case GUIBasicEffect.Technique.Texture3D:
        this.CurrentTechnique = this.mTechnique3DTechnique;
        break;
    }
  }

  public Vector2 TextureOffset
  {
    get => this.mTextureOffsetParameter.GetValueVector2();
    set => this.mTextureOffsetParameter.SetValue(value);
  }

  public Vector2 TextureScale
  {
    get => this.mTextureScaleParameter.GetValueVector2();
    set => this.mTextureScaleParameter.SetValue(value);
  }

  public Matrix Transform
  {
    get => this.mTransformParameter.GetValueMatrix();
    set => this.mTransformParameter.SetValue(value);
  }

  public bool OverlayTextureEnabled
  {
    get => this.mOverlayTextureEnabledParameter.GetValueBoolean();
    set => this.mOverlayTextureEnabledParameter.SetValue(value);
  }

  public float W
  {
    get => this.mWParameter.GetValueSingle();
    set => this.mWParameter.SetValue(value);
  }

  public Texture2D OverlayTexture
  {
    get => this.mOverlayTextureParameter.GetValueTexture2D();
    set => this.mOverlayTextureParameter.SetValue((Texture) value);
  }

  public Vector4 OverlayTint
  {
    get => this.mOverlayTintParameter.GetValueVector4();
    set => this.mOverlayTintParameter.SetValue(value);
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

  public Texture Texture
  {
    set => this.mTextureParameter.SetValue(value);
  }

  public enum Technique
  {
    Texture2D,
    Texture3D,
  }
}
