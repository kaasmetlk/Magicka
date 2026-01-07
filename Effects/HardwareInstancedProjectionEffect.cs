// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.HardwareInstancedProjectionEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class HardwareInstancedProjectionEffect : Effect
{
  public const int MAX_INSTANCES = 30;
  public static readonly int TYPEHASH = typeof (HardwareInstancedProjectionEffect).GetHashCode();
  private EffectParameter mWorldTransformsParameter;
  private EffectParameter mInvWorldTransformsParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mInverseViewProjectionParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mPixelSizeParameter;
  private EffectParameter mArgsParameter;
  private EffectParameter mColorsParameter;
  private EffectParameter mTextureScaleParameter;
  private EffectParameter mBumpScaleParameter;
  private EffectParameter mDepthMapParameter;
  private EffectParameter mShadowIntensityParameter;

  public HardwareInstancedProjectionEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, HardwareInstancedProjectionEffectCode.CODE, CompilerOptions.None, iPool)
  {
    this.mWorldTransformsParameter = this.Parameters[nameof (WorldTransforms)];
    this.mInvWorldTransformsParameter = this.Parameters[nameof (InvWorldTransforms)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mInverseViewProjectionParameter = this.Parameters[nameof (InverseViewProjection)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mPixelSizeParameter = this.Parameters[nameof (PixelSize)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
    this.mArgsParameter = this.Parameters[nameof (Args)];
    this.mColorsParameter = this.Parameters[nameof (Colors)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
    this.mBumpScaleParameter = this.Parameters[nameof (BumpScale)];
    this.mShadowIntensityParameter = this.Parameters[nameof (ShadowIntensity)];
  }

  public Matrix[] WorldTransforms
  {
    get => this.mWorldTransformsParameter.GetValueMatrixArray(30);
    set => this.mWorldTransformsParameter.SetValue(value);
  }

  public Matrix[] InvWorldTransforms
  {
    get => this.mInvWorldTransformsParameter.GetValueMatrixArray(30);
    set => this.mInvWorldTransformsParameter.SetValue(value);
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Matrix InverseViewProjection
  {
    get => this.mInverseViewProjectionParameter.GetValueMatrix();
    set => this.mInverseViewProjectionParameter.SetValue(value);
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public float ShadowIntensity
  {
    get => this.mShadowIntensityParameter.GetValueSingle();
    set => this.mShadowIntensityParameter.SetValue(value);
  }

  public Vector3[] Args
  {
    get => this.mArgsParameter.GetValueVector3Array(30);
    set => this.mArgsParameter.SetValue(value);
  }

  public Vector4[] Colors
  {
    get => this.mColorsParameter.GetValueVector4Array(30);
    set => this.mColorsParameter.SetValue(value);
  }

  public Vector2 TextureScale
  {
    get => this.mTextureScaleParameter.GetValueVector2();
    set => this.mTextureScaleParameter.SetValue(value);
  }

  public float BumpScale
  {
    get => this.mBumpScaleParameter.GetValueSingle();
    set => this.mBumpScaleParameter.SetValue(value);
  }

  public Vector2 PixelSize
  {
    get => this.mPixelSizeParameter.GetValueVector2();
    set => this.mPixelSizeParameter.SetValue(value);
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
}
