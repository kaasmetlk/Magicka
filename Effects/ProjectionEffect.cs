// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.ProjectionEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class ProjectionEffect : Effect
{
  public static readonly int TYPEHASH = typeof (ProjectionEffect).GetHashCode();
  private EffectParameter mWorldParameter;
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mInverseViewProjectionParameter;
  private EffectParameter mInvProjectedMatrixParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mNormalMapParameter;
  private EffectParameter mNormalMapEnabledParameter;
  private EffectParameter mTextureParameter;
  private EffectParameter mTextureScaleParameter;
  private EffectParameter mTextureOffsetParameter;
  private EffectParameter mBumpScaleParameter;
  private EffectParameter mDepthMapParameter;
  private EffectParameter mPixelSizeParameter;
  private EffectParameter mAlphaParameter;
  private EffectParameter mGlowParameter;

  public ProjectionEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, ProjectionEffectCode.CODE, CompilerOptions.None, iPool)
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mInverseViewProjectionParameter = this.Parameters[nameof (InverseViewProjection)];
    this.mInvProjectedMatrixParameter = this.Parameters[nameof (InvProjectedMatrix)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mNormalMapParameter = this.Parameters[nameof (NormalMap)];
    this.mNormalMapEnabledParameter = this.Parameters[nameof (NormalMapEnabled)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
    this.mPixelSizeParameter = this.Parameters[nameof (PixelSize)];
    this.mTextureParameter = this.Parameters[nameof (Texture)];
    this.mGlowParameter = this.Parameters[nameof (Glow)];
    this.mTextureScaleParameter = this.Parameters[nameof (TextureScale)];
    this.mTextureOffsetParameter = this.Parameters[nameof (TextureOffset)];
    this.mBumpScaleParameter = this.Parameters[nameof (BumpScale)];
    this.mAlphaParameter = this.Parameters[nameof (Alpha)];
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

  public Matrix InverseViewProjection
  {
    get => this.mInverseViewProjectionParameter.GetValueMatrix();
    set => this.mInverseViewProjectionParameter.SetValue(value);
  }

  public Matrix InvProjectedMatrix
  {
    get => this.mInvProjectedMatrixParameter.GetValueMatrix();
    set => this.mInvProjectedMatrixParameter.SetValue(value);
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public float Glow
  {
    get => this.mGlowParameter.GetValueSingle();
    set => this.mGlowParameter.SetValue(value);
  }

  public bool NormalMapEnabled
  {
    get => this.mNormalMapEnabledParameter.GetValueBoolean();
    set => this.mNormalMapEnabledParameter.SetValue(value);
  }

  public Texture2D NormalMap
  {
    get => this.mNormalMapParameter.GetValueTexture2D();
    set => this.mNormalMapParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
  }

  public Vector2 TextureScale
  {
    get => this.mTextureScaleParameter.GetValueVector2();
    set => this.mTextureScaleParameter.SetValue(value);
  }

  public Vector2 TextureOffset
  {
    get => this.mTextureOffsetParameter.GetValueVector2();
    set => this.mTextureOffsetParameter.SetValue(value);
  }

  public float BumpScale
  {
    get => this.mBumpScaleParameter.GetValueSingle();
    set => this.mBumpScaleParameter.SetValue(value);
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

  public Vector2 PixelSize
  {
    get => this.mPixelSizeParameter.GetValueVector2();
    set => this.mPixelSizeParameter.SetValue(value);
  }

  public float Alpha
  {
    get => this.mAlphaParameter.GetValueSingle();
    set => this.mAlphaParameter.SetValue(value);
  }
}
