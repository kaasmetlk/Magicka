// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.ShieldEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

public class ShieldEffect : Effect
{
  public const int MAXDAMAGEPOINTS = 16 /*0x10*/;
  public static readonly int TYPEHASH = typeof (ShieldEffect).GetHashCode();
  private EffectParameter mWorldParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mNoise0OffsetParameter;
  private EffectParameter mNoise1OffsetParameter;
  private EffectParameter mNoise2OffsetParameter;
  private EffectParameter mTextureScaleParameter;
  private EffectParameter mColorTintParameter;
  private EffectParameter mMinDotProductParameter;
  private EffectParameter mDirectionParameter;
  private EffectParameter mThicknessParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mDepthMapParameter;
  private EffectParameter mDamagePointsParameter;

  public ShieldEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/ShieldEffect"))
  {
    this.mThicknessParameter = this.Parameters[nameof (Thickness)];
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mNoise0OffsetParameter = this.Parameters[nameof (Noise0Offset)];
    this.mNoise1OffsetParameter = this.Parameters[nameof (Noise1Offset)];
    this.mNoise2OffsetParameter = this.Parameters[nameof (Noise2Offset)];
    this.mMinDotProductParameter = this.Parameters[nameof (MinDotProduct)];
    this.mDirectionParameter = this.Parameters[nameof (Direction)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
    this.mColorTintParameter = this.Parameters[nameof (ColorTint)];
    this.mTextureParameter = this.Parameters["NormalMap"];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
    this.mDamagePointsParameter = this.Parameters[nameof (DamagePoints)];
  }

  public void SetTechnique(ShieldEffect.Technique iTechnique)
  {
    this.CurrentTechnique = this.Techniques[(int) iTechnique];
  }

  public float Thickness
  {
    get => this.mThicknessParameter.GetValueSingle();
    set => this.mThicknessParameter.SetValue(value);
  }

  public Matrix World
  {
    get => this.mWorldParameter.GetValueMatrix();
    set => this.mWorldParameter.SetValue(value);
  }

  public Vector4[] DamagePoints
  {
    get => this.mDamagePointsParameter.GetValueVector4Array(16 /*0x10*/);
    set => this.mDamagePointsParameter.SetValue(value);
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

  public Vector2 Noise0Offset
  {
    get => this.mNoise0OffsetParameter.GetValueVector2();
    set => this.mNoise0OffsetParameter.SetValue(value);
  }

  public Vector2 Noise1Offset
  {
    get => this.mNoise1OffsetParameter.GetValueVector2();
    set => this.mNoise1OffsetParameter.SetValue(value);
  }

  public Vector2 Noise2Offset
  {
    get => this.mNoise2OffsetParameter.GetValueVector2();
    set => this.mNoise2OffsetParameter.SetValue(value);
  }

  public float MinDotProduct
  {
    get => this.mMinDotProductParameter.GetValueSingle();
    set => this.mMinDotProductParameter.SetValue(value);
  }

  public Vector3 Direction
  {
    get => this.mDirectionParameter.GetValueVector3();
    set => this.mDirectionParameter.SetValue(value);
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

  public Texture2D DepthMap
  {
    get => this.mDepthMapParameter.GetValueTexture2D();
    set => this.mDepthMapParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }

  public enum Technique
  {
    Sphere,
    Wall,
  }
}
