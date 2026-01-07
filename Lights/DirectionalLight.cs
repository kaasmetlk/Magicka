// Decompiled with JetBrains decompiler
// Type: PolygonHead.Lights.DirectionalLight
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead.Lights;

public class DirectionalLight : Light
{
  private int mShadowMapSize = 1024 /*0x0400*/;
  private Vector3 mLightDirection;
  private Matrix mLightOrientation;
  private Matrix mInvLightOrientation;
  private Matrix mLightViewProjection;
  private BoundingFrustum mFrustum;
  private RenderTarget2D mShadowMap;
  private DepthStencilBuffer mShadowDepthStencil;
  private bool mCastShadow;
  private GraphicsDevice mDevice;
  private Vector3 mDiffuseColor = new Vector3(0.8f);
  private Vector3 mAmbientColor = new Vector3(0.2f);
  private float mSpecularAmount = 1f;
  private float mFocalDistance = 224f;
  private Random mRandom = new Random();
  private static VertexBuffer mVertexBuffer;
  private static VertexDeclaration mVertexDeclaration;
  private Vector3 mSnappedGroundPosition;
  private Texture2D mProjectedTexture;
  private Vector2 mProjectedTextureOffset;
  private Vector2 mProjectedTextureScale;
  private float mProjectedTextureIntensity;

  public DirectionalLight(GraphicsDevice iDevice, Vector3 iDirection, bool iCastShadows)
  {
    this.mDevice = iDevice;
    this.mCastShadow = iCastShadows;
    if (this.mCastShadow)
      this.CreateShadowMap();
    this.mFrustum = new BoundingFrustum(Matrix.Identity);
    if (!(RenderManager.Instance.GetEffect(DirectionalLightEffect.TYPEHASH) is DirectionalLightEffect iEffect))
    {
      lock (this.mDevice)
        iEffect = new DirectionalLightEffect(iDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
      RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) iEffect);
    }
    Vector2 vector2 = new Vector2();
    Point screenSize = RenderManager.Instance.ScreenSize;
    vector2.X = (float) screenSize.X;
    vector2.Y = (float) screenSize.Y;
    iEffect.DestinationDimensions = vector2;
    this.mLightDirection = Vector3.Normalize(iDirection);
    this.mLightOrientation = Matrix.CreateLookAt(Vector3.Zero, this.mLightDirection, Vector3.Up);
    Matrix.Invert(ref this.mLightOrientation, out this.mInvLightOrientation);
    if (DirectionalLight.mVertexBuffer != null && !DirectionalLight.mVertexBuffer.IsDisposed)
      return;
    this.CreateVertices(iDevice);
  }

  public virtual void CreateVertices(GraphicsDevice iDevice)
  {
    lock (iDevice)
    {
      DirectionalLight.mVertexBuffer = new VertexBuffer(iDevice, VertexPositionTexture.SizeInBytes * 4, BufferUsage.WriteOnly);
      DirectionalLight.mVertexDeclaration = new VertexDeclaration(iDevice, VertexPositionTexture.VertexElements);
    }
    VertexPositionTexture[] data = new VertexPositionTexture[4];
    data[0].Position = new Vector3(1f, 1f, 1f);
    data[0].TextureCoordinate = new Vector2(1f, 0.0f);
    data[1].Position = new Vector3(1f, -1f, 1f);
    data[1].TextureCoordinate = new Vector2(1f, 1f);
    data[2].Position = new Vector3(-1f, 1f, 1f);
    data[2].TextureCoordinate = new Vector2(0.0f, 0.0f);
    data[3].Position = new Vector3(-1f, -1f, 1f);
    data[3].TextureCoordinate = new Vector2(0.0f, 1f);
    DirectionalLight.mVertexBuffer.SetData<VertexPositionTexture>(data);
  }

  protected void UpdateMatrices(ref Vector3 iScreenCenter)
  {
    if (!this.mCastShadow)
      return;
    Vector3 result1;
    Vector3.Transform(ref iScreenCenter, ref this.mLightOrientation, out result1);
    float num1 = (float) this.mShadowMapSize / 60f;
    float num2 = 1f / num1;
    Vector3 position = new Vector3();
    position.X = (float) Math.Floor((double) result1.X * (double) num1) * num2;
    position.Y = (float) Math.Floor((double) result1.Y * (double) num1) * num2;
    position.Z = (float) Math.Floor((double) result1.Z * (double) num1) * num2;
    Matrix result2;
    Matrix.CreateOrthographicOffCenter(position.X - 30f, position.X + 30f, position.Y - 30f, position.Y + 30f, (float) -((double) position.Z + 200.0), (float) -((double) position.Z - 100.0), out result2);
    Matrix.Multiply(ref this.mLightOrientation, ref result2, out this.mLightViewProjection);
    Vector3.Transform(ref position, ref this.mInvLightOrientation, out this.mSnappedGroundPosition);
    this.mFrustum.Matrix = this.mLightViewProjection;
  }

  protected internal override void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    base.Update(iDataChannel, iDeltaTime, ref iCameraPosition, ref iCameraDirection);
    Vector3 result = iCameraDirection;
    Vector3.Multiply(ref result, this.mFocalDistance, out result);
    Vector3.Add(ref iCameraPosition, ref result, out result);
    this.UpdateMatrices(ref result);
  }

  public override void DisposeShadowMap()
  {
    if (this.mShadowMap != null && !this.mShadowMap.IsDisposed)
      this.mShadowMap.Dispose();
    if (this.mShadowDepthStencil == null || this.mShadowDepthStencil.IsDisposed)
      return;
    this.mShadowDepthStencil.Dispose();
  }

  public override void CreateShadowMap()
  {
    if (!this.mCastShadow)
      return;
    if (this.mShadowMap == null || this.mShadowMap.IsDisposed)
      this.mShadowMap = new RenderTarget2D(this.mDevice, this.mShadowMapSize, this.mShadowMapSize, 1, SurfaceFormat.Single, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents);
    if (this.mShadowDepthStencil != null && !this.mShadowDepthStencil.IsDisposed)
      return;
    this.mShadowDepthStencil = new DepthStencilBuffer(this.mDevice, this.mShadowMapSize, this.mShadowMapSize, DepthFormat.Depth24, MultiSampleType.None, 0);
  }

  public override void DrawShadows(DataChannel iDataChannel, float iDeltaTime, Scene iScene)
  {
    this.mDevice.DepthStencilBuffer = this.mShadowDepthStencil;
    this.mDevice.SetRenderTarget(0, this.mShadowMap);
    this.mDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1f, 0);
    RenderManager.Instance.GlobalDummyEffect.ViewProjection = this.mLightViewProjection;
    DummyEffect localDummyEffect = RenderManager.Instance.LocalDummyEffect;
    if (localDummyEffect != null)
      localDummyEffect.ViewProjection = this.mLightViewProjection;
    iScene.DrawShadows(iDataChannel, this.mFrustum, iDeltaTime);
  }

  public override void Draw(
    Microsoft.Xna.Framework.Graphics.Effect iEffect,
    DataChannel iDataChannel,
    float iDeltaTime,
    Texture2D iNormalMap,
    Texture2D iDepthMap)
  {
    DirectionalLightEffect directionalLightEffect = iEffect as DirectionalLightEffect;
    directionalLightEffect.LightDirection = this.mLightDirection;
    directionalLightEffect.LightViewProjection = this.mLightViewProjection;
    Vector3 result1;
    Vector3.Multiply(ref this.mDiffuseColor, this.mIntensity, out result1);
    Vector3 result2;
    Vector3.Multiply(ref this.mAmbientColor, this.mIntensity, out result2);
    float num = this.mSpecularAmount * this.mIntensity;
    directionalLightEffect.DiffuseColor = result1;
    directionalLightEffect.AmbientColor = result2;
    directionalLightEffect.SpecularAmount = num;
    directionalLightEffect.SourceTexture0 = iNormalMap;
    directionalLightEffect.SourceTexture1 = iDepthMap;
    directionalLightEffect.SnappedGroundPosition = this.mSnappedGroundPosition;
    directionalLightEffect.ProjectedTextureIntensity = this.mProjectedTextureIntensity;
    if ((double) this.mProjectedTextureIntensity > 1.4012984643248171E-45)
    {
      directionalLightEffect.ProjectedTextureScale = this.mProjectedTextureScale;
      directionalLightEffect.ProjectedTextureOffset = this.mProjectedTextureOffset;
      directionalLightEffect.ProjectedTexture = this.mProjectedTexture;
    }
    if (this.mCastShadow)
    {
      try
      {
        directionalLightEffect.SourceTexture3 = this.mShadowMap.GetTexture();
      }
      catch
      {
      }
      directionalLightEffect.ShadowMapSize = this.mShadowMap.Width;
    }
    directionalLightEffect.CommitChanges();
    this.mDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
  }

  public RenderTarget2D ShadowRenderTarget => this.mShadowMap;

  public BoundingFrustum Frustum => this.mFrustum;

  public Matrix ViewProjectionMatrix => this.ViewProjectionMatrix;

  public override float SpecularAmount
  {
    get => this.mSpecularAmount;
    set => this.mSpecularAmount = value;
  }

  public Vector3 LightDirection
  {
    get => this.mLightDirection;
    set
    {
      this.mLightDirection = value;
      this.mLightOrientation = Matrix.CreateLookAt(Vector3.Zero, this.mLightDirection, Vector3.Up);
      Matrix.Invert(ref this.mLightOrientation, out this.mInvLightOrientation);
    }
  }

  public Matrix LightOrientation => this.mLightOrientation;

  public override bool CastShadows
  {
    get => this.mCastShadow;
    set
    {
      this.mCastShadow = value;
      if (this.mShadowMap != null && this.mShadowMap.Width != this.mShadowMapSize)
        this.DisposeShadowMap();
      if (!this.mCastShadow || this.mShadowMap != null && !this.mShadowMap.IsDisposed)
        return;
      this.CreateShadowMap();
    }
  }

  public override int Effect => DirectionalLightEffect.TYPEHASH;

  public override int Technique => this.mCastShadow ? 0 : 1;

  public override int VertexStride => VertexPositionTexture.SizeInBytes;

  public override VertexBuffer VertexBuffer => DirectionalLight.mVertexBuffer;

  public override IndexBuffer IndexBuffer => (IndexBuffer) null;

  public override VertexDeclaration VertexDeclaration => DirectionalLight.mVertexDeclaration;

  public override int ShadowMapSize
  {
    get => this.mShadowMapSize;
    set
    {
      this.mShadowMapSize = value;
      if (this.mShadowMap != null && this.mShadowMap.Width != this.mShadowMapSize)
        this.DisposeShadowMap();
      if (!this.mCastShadow || this.mShadowMap != null && !this.mShadowMap.IsDisposed)
        return;
      this.CreateShadowMap();
    }
  }

  public override Vector3 DiffuseColor
  {
    get => this.mDiffuseColor;
    set => this.mDiffuseColor = value;
  }

  public override Vector3 AmbientColor
  {
    get => this.mAmbientColor;
    set => this.mAmbientColor = value;
  }

  public Texture2D ProjectedTexture
  {
    get => this.mProjectedTexture;
    set => this.mProjectedTexture = value;
  }

  public Vector2 ProjectedTextureScale
  {
    get => this.mProjectedTextureScale;
    set => this.mProjectedTextureScale = value;
  }

  public Vector2 ProjectedTextureOffset
  {
    get => this.mProjectedTextureOffset;
    set => this.mProjectedTextureOffset = value;
  }

  public float ProjectedTextureIntensity
  {
    get => this.mProjectedTextureIntensity;
    set => this.mProjectedTextureIntensity = value;
  }

  public override bool ShouldDraw(BoundingFrustum iViewFrustum) => true;
}
