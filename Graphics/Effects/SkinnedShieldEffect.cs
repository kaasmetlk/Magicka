// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.SkinnedShieldEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace Magicka.Graphics.Effects;

internal class SkinnedShieldEffect : Effect
{
  public static readonly int TYPEHASH = typeof (SkinnedShieldEffect).GetHashCode();
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mColorParameter;
  private EffectParameter mBloatParameter;
  private EffectParameter mTextureScaleParameter;
  private EffectParameter mTextureOffset0Parameter;
  private EffectParameter mTextureOffset1Parameter;
  private EffectParameter mTextureOffset2Parameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mProjectionMapMatrix0Parameter;
  private EffectParameter mProjectionMapMatrix1Parameter;
  private EffectParameter mProjectionMapMatrix2Parameter;
  private EffectParameter mBonesParameter;
  private EffectParameter mProjectionMapParameter;

  public SkinnedShieldEffect(GraphicsDevice iDevice, ContentManager iContent)
    : base(iDevice, iContent.Load<Effect>("shaders/skinnedShieldEffect"))
  {
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mColorParameter = this.Parameters[nameof (Color)];
    this.mBloatParameter = this.Parameters[nameof (Bloat)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
    this.mTextureOffset0Parameter = this.Parameters[nameof (TextureOffset0)];
    this.mTextureOffset1Parameter = this.Parameters[nameof (TextureOffset1)];
    this.mTextureOffset2Parameter = this.Parameters[nameof (TextureOffset2)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mProjectionMapMatrix0Parameter = this.Parameters[nameof (ProjectionMapMatrix0)];
    this.mProjectionMapMatrix1Parameter = this.Parameters[nameof (ProjectionMapMatrix1)];
    this.mProjectionMapMatrix2Parameter = this.Parameters[nameof (ProjectionMapMatrix2)];
    this.mBonesParameter = this.Parameters[nameof (Bones)];
    this.mProjectionMapParameter = this.Parameters[nameof (ProjectionMap)];
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public Vector4 Color
  {
    get => this.mColorParameter.GetValueVector4();
    set => this.mColorParameter.SetValue(value);
  }

  public float Bloat
  {
    get => this.mBloatParameter.GetValueSingle();
    set => this.mBloatParameter.SetValue(value);
  }

  public Vector2 TextureScale
  {
    get => this.mTextureScaleParameter.GetValueVector2();
    set => this.mTextureScaleParameter.SetValue(value);
  }

  public Vector2 TextureOffset0
  {
    get => this.mTextureOffset0Parameter.GetValueVector2();
    set => this.mTextureOffset0Parameter.SetValue(value);
  }

  public Vector2 TextureOffset1
  {
    get => this.mTextureOffset1Parameter.GetValueVector2();
    set => this.mTextureOffset1Parameter.SetValue(value);
  }

  public Vector2 TextureOffset2
  {
    get => this.mTextureOffset2Parameter.GetValueVector2();
    set => this.mTextureOffset2Parameter.SetValue(value);
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

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Matrix ProjectionMapMatrix0
  {
    get => this.mProjectionMapMatrix0Parameter.GetValueMatrix();
    set => this.mProjectionMapMatrix0Parameter.SetValue(value);
  }

  public Matrix ProjectionMapMatrix1
  {
    get => this.mProjectionMapMatrix1Parameter.GetValueMatrix();
    set => this.mProjectionMapMatrix1Parameter.SetValue(value);
  }

  public Matrix ProjectionMapMatrix2
  {
    get => this.mProjectionMapMatrix2Parameter.GetValueMatrix();
    set => this.mProjectionMapMatrix2Parameter.SetValue(value);
  }

  public Matrix[] Bones
  {
    get => this.mBonesParameter.GetValueMatrixArray(80 /*0x50*/);
    set => this.mBonesParameter.SetValue(value);
  }

  public Texture2D ProjectionMap
  {
    get => this.mProjectionMapParameter.GetValueTexture2D();
    set => this.mProjectionMapParameter.SetValue((Texture) value);
  }
}
