// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.RainSplashEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

#nullable disable
namespace Magicka.Graphics.Effects;

public class RainSplashEffect : Effect, IPostEffect
{
  public const int MAXBONES = 10;
  public static readonly int TYPEHASH = typeof (RainSplashEffect).GetHashCode();
  private EffectTechnique mDefaultTechnique;
  private EffectParameter mInverseViewProjectionParameter;
  private Matrix mInverseViewProjection;
  private EffectParameter mTextureProjectionParameter;
  private Matrix mTextureProjection;
  private EffectParameter mCameraPositionParameter;
  private Vector3 mCameraPosition;
  private EffectParameter mColorParameter;
  private Vector4 mColor;
  private EffectParameter mWParameter;
  private float mW;
  private EffectParameter mYOffsetParameter;
  private float mYOffset;
  private EffectParameter mNormalMapParameter;
  private Texture2D mNormalMap;
  private EffectParameter mDepthMapParameter;
  private Texture2D mDepthMap;
  private EffectParameter mTextureParameter;
  private Texture3D mTexture;
  private EffectParameter mDestinationDimensionsParameter;
  private Vector2 mDestinationDimensions;
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;

  public RainSplashEffect(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/RainSplashEffect"))
  {
    this.mDefaultTechnique = this.Techniques["Default"];
    this.mInverseViewProjectionParameter = this.Parameters[nameof (InverseViewProjection)];
    this.mTextureProjectionParameter = this.Parameters[nameof (TextureProjection)];
    this.mCameraPositionParameter = this.Parameters[nameof (CameraPosition)];
    this.mColorParameter = this.Parameters[nameof (Color)];
    this.mWParameter = this.Parameters[nameof (W)];
    this.mYOffsetParameter = this.Parameters[nameof (YOffset)];
    this.mTextureParameter = this.Parameters["SourceTexture0"];
    this.mNormalMapParameter = this.Parameters["SourceTexture1"];
    this.mDepthMapParameter = this.Parameters["SourceTexture2"];
    this.mDestinationDimensionsParameter = this.Parameters[nameof (DestinationDimensions)];
    Vector2 vector2 = new Vector2();
    Point screenSize = RenderManager.Instance.ScreenSize;
    vector2.X = (float) screenSize.X;
    vector2.Y = (float) screenSize.Y;
    this.DestinationDimensions = vector2;
    this.UpdateVertices();
  }

  protected void UpdateVertices()
  {
    VertexPositionTexture[] data = new VertexPositionTexture[4];
    data[0].Position = new Vector3(-1f, -1f, 0.0f);
    data[0].TextureCoordinate = new Vector2(0.0f, 1f);
    data[1].Position = new Vector3(-1f, 1f, 0.0f);
    data[1].TextureCoordinate = new Vector2(0.0f, 0.0f);
    data[2].Position = new Vector3(1f, 1f, 0.0f);
    data[2].TextureCoordinate = new Vector2(1f, 0.0f);
    data[3].Position = new Vector3(1f, -1f, 0.0f);
    data[3].TextureCoordinate = new Vector2(1f, 1f);
    this.mVertices = new VertexBuffer(this.GraphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
    this.mVertices.SetData<VertexPositionTexture>(data);
    this.mVertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionTexture.VertexElements);
  }

  public void SetTechnique(RainSplashEffect.Technique iTechnique)
  {
    if (iTechnique != RainSplashEffect.Technique.Default)
      return;
    this.CurrentTechnique = this.mDefaultTechnique;
  }

  public Matrix InverseViewProjection
  {
    get => this.mInverseViewProjection;
    set
    {
      if (!(this.mInverseViewProjection != value))
        return;
      this.mInverseViewProjection = value;
      this.mInverseViewProjectionParameter.SetValue(value);
    }
  }

  public Matrix TextureProjection
  {
    get => this.mTextureProjection;
    set
    {
      if (!(this.mTextureProjection != value))
        return;
      this.mTextureProjection = value;
      this.mTextureProjectionParameter.SetValue(value);
    }
  }

  public Vector3 CameraPosition
  {
    get => this.mCameraPosition;
    set
    {
      if (!(this.mCameraPosition != value))
        return;
      this.mCameraPosition = value;
      this.mCameraPositionParameter.SetValue(value);
    }
  }

  public Vector4 Color
  {
    get => this.mColor;
    set
    {
      if (!(this.mColor != value))
        return;
      this.mColor = value;
      this.mColorParameter.SetValue(value);
    }
  }

  public float W
  {
    get => this.mW;
    set
    {
      if ((double) this.mW == (double) value)
        return;
      this.mW = value;
      this.mWParameter.SetValue(value);
    }
  }

  public float YOffset
  {
    get => this.mYOffset;
    set
    {
      if ((double) this.mYOffset == (double) value)
        return;
      this.mYOffset = value;
      this.mYOffsetParameter.SetValue(value);
    }
  }

  public Texture2D NormalMap
  {
    get => this.mNormalMap;
    set
    {
      if (this.mNormalMap == value)
        return;
      this.mNormalMap = value;
      this.mNormalMapParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
    }
  }

  public Texture2D DepthMap
  {
    get => this.mDepthMap;
    set
    {
      if (this.mDepthMap == value)
        return;
      this.mDepthMap = value;
      this.mDepthMapParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
    }
  }

  public Texture3D Texture
  {
    get => this.mTexture;
    set
    {
      if (this.mTexture == value)
        return;
      this.mTexture = value;
      this.mTextureParameter.SetValue((Microsoft.Xna.Framework.Graphics.Texture) value);
    }
  }

  public Vector2 DestinationDimensions
  {
    get => this.mDestinationDimensions;
    set
    {
      if (!(this.mDestinationDimensions != value))
        return;
      this.mDestinationDimensions = value;
      this.mDestinationDimensionsParameter.SetValue(value);
    }
  }

  public int ZIndex => 5;

  public bool Dead => false;

  public void Draw(
    float iDeltaTime,
    ref Vector2 iPixelSize,
    ref Matrix iViewMatrix,
    ref Matrix iProjectionMatrix,
    Texture2D iCandidate,
    Texture2D iDepthMap,
    Texture2D iNormalMap)
  {
    this.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, VertexPositionTexture.SizeInBytes);
    this.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    this.Color = new Vector4(1f);
    this.NormalMap = iNormalMap;
    this.DepthMap = iDepthMap;
    this.W += iDeltaTime * 4f;
    this.YOffset = 1f;
    this.Begin();
    this.CurrentTechnique.Passes[0].Begin();
    this.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    this.CurrentTechnique.Passes[0].End();
    this.End();
  }

  public enum Technique
  {
    Default,
  }
}
