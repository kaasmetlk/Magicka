// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.AdditiveEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class AdditiveEffect : Effect
{
  public static readonly int TYPEHASH = typeof (AdditiveEffect).GetHashCode();
  private EffectTechnique mTechnique1Technique;
  private EffectParameter mWorldParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mTextureOffsetParameter;
  private EffectParameter mTextureScaleParameter;
  private EffectParameter mColorTintParameter;
  private EffectParameter mVertexColorEnabledParameter;
  private EffectParameter mTextureEnabledParameter;
  private EffectParameter mTextureParameter;

  public AdditiveEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, AdditiveEffectCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mTechnique1Technique = this.Techniques["Technique1"];
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mTextureOffsetParameter = this.Parameters[nameof (TextureOffset)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
    this.mColorTintParameter = this.Parameters[nameof (ColorTint)];
    this.mVertexColorEnabledParameter = this.Parameters[nameof (VertexColorEnabled)];
    this.mTextureEnabledParameter = this.Parameters[nameof (TextureEnabled)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
  }

  public void SetTechnique(AdditiveEffect.Technique iTechnique)
  {
    if (iTechnique != AdditiveEffect.Technique.Technique1)
      return;
    this.CurrentTechnique = this.mTechnique1Technique;
  }

  public Matrix World
  {
    get => this.mWorldParameter.GetValueMatrix();
    set => this.mWorldParameter.SetValue(value);
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
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

  public Vector4 ColorTint
  {
    get => this.mColorTintParameter.GetValueVector4();
    set => this.mColorTintParameter.SetValue(value);
  }

  public Texture2D Texture
  {
    get => this.mTextureParameter.GetValueTexture2D();
    set => this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
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

  public static AdditiveEffect Read(ContentReader iInput)
  {
    return new AdditiveEffect((iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice, (EffectPool) null)
    {
      ColorTint = new Vector4(iInput.ReadVector3(), 1f),
      VertexColorEnabled = iInput.ReadBoolean(),
      TextureEnabled = iInput.ReadBoolean(),
      Texture = iInput.ReadExternalReference<Texture2D>()
    };
  }

  public enum Technique
  {
    Technique1,
  }
}
