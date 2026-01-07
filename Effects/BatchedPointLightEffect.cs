// Decompiled with JetBrains decompiler
// Type: PolygonHead.Effects.BatchedPointLightEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Effects;

public class BatchedPointLightEffect : Effect
{
  public static readonly int TYPEHASH = typeof (BatchedPointLightEffect).GetHashCode();
  private EffectParameter mViewProjectionParameter;
  private EffectParameter mCameraPositionParameter;
  private EffectParameter mInverseViewProjectionParameter;
  private EffectParameter mNormalMapParameter;
  private EffectParameter mDepthMapParameter;
  private EffectParameter mHalfPixelParameter;

  public BatchedPointLightEffect(GraphicsDevice iDevice, EffectPool iPool)
    : base(iDevice, BatchedPointLightEffectCode.CODE, CompilerOptions.NotCloneable, iPool)
  {
    this.mViewProjectionParameter = this.Parameters[nameof (ViewProjection)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mInverseViewProjectionParameter = this.Parameters[nameof (InverseViewProjection)];
    this.mNormalMapParameter = this.Parameters[nameof (NormalMap)];
    this.mDepthMapParameter = this.Parameters[nameof (DepthMap)];
    this.mHalfPixelParameter = this.Parameters[nameof (HalfPixel)];
  }

  public Matrix ViewProjection
  {
    get => this.mViewProjectionParameter.GetValueMatrix();
    set => this.mViewProjectionParameter.SetValue(value);
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPositionParameter.GetValueVector3();
    set => this.mCameraPositionParameter.SetValue(value);
  }

  public Matrix InverseViewProjection
  {
    get => this.mInverseViewProjectionParameter.GetValueMatrix();
    set => this.mInverseViewProjectionParameter.SetValue(value);
  }

  public Texture2D NormalMap
  {
    get => this.mNormalMapParameter.GetValueTexture2D();
    set => this.mNormalMapParameter.SetValue((Texture) value);
  }

  public Texture2D DepthMap
  {
    get => this.mDepthMapParameter.GetValueTexture2D();
    set => this.mDepthMapParameter.SetValue((Texture) value);
  }

  public Vector2 HalfPixel
  {
    get => this.mHalfPixelParameter.GetValueVector2();
    set => this.mHalfPixelParameter.SetValue(value);
  }

  public struct LightProperties
  {
    public const int SIZE_IN_BYTES = 48 /*0x30*/;
    public Vector3 Position;
    public float Radius;
    public Vector3 DiffuseColor;
    public float SpecularAmount;
    public Vector3 AmbientColor;
    public float Intensity;
    public static readonly VertexElement[] VERTEX_ELEMENTS = new VertexElement[3]
    {
      new VertexElement((short) 1, (short) 0, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 1),
      new VertexElement((short) 1, (short) 16 /*0x10*/, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 0),
      new VertexElement((short) 1, (short) 32 /*0x20*/, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 1)
    };
  }

  public enum Technique
  {
    Default,
  }
}
