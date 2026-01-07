// Decompiled with JetBrains decompiler
// Type: PolygonHead.Lights.PointLight
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead.Lights;

public class PointLight : Light
{
  protected static Matrix FLIPX = Matrix.CreateScale(-1f, 1f, 1f);
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private IndexBuffer mIndices;
  private int mShadowMapSize = 128 /*0x80*/;
  private float mRange;
  private Vector3 mPosition;
  private Matrix mLightOrientation;
  private Matrix[] mLightViewProjection;
  private BoundingFrustum mFrustum;
  private RenderTargetCube mShadowMap;
  private DepthStencilBuffer mShadowDepthStencil;
  private bool mCastShadow;
  private Vector3 mDiffuseColor = new Vector3(0.8f);
  private Vector3 mAmbientColor = new Vector3(0.2f);
  private float mSpecularAmount = 1f;
  private Vector3 mOffset;
  private BoundingSphere mBoundingSphere;
  private Random mRandom = new Random();
  private GraphicsDevice mDevice;

  public PointLight(GraphicsDevice iDevice)
  {
    this.mDevice = iDevice;
    LightHelper.CreateSphere(iDevice, out this.mVertices, out this.mVertexDeclaration, out this.mIndices);
    this.mLightViewProjection = new Matrix[6];
    this.mFrustum = new BoundingFrustum(Matrix.Identity);
    if (!(RenderManager.Instance.GetEffect(PointLightEffect.TYPEHASH) is PointLightEffect iEffect))
    {
      lock (this.mDevice)
        iEffect = new PointLightEffect(iDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
      RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) iEffect);
    }
    this.mLightViewProjection[0] = Matrix.CreateLookAt(Vector3.Zero, Vector3.Right, Vector3.Up);
    this.mLightViewProjection[1] = Matrix.CreateLookAt(Vector3.Zero, Vector3.Left, Vector3.Up);
    this.mLightViewProjection[2] = Matrix.CreateLookAt(Vector3.Zero, Vector3.Up, Vector3.Forward);
    this.mLightViewProjection[3] = Matrix.CreateLookAt(Vector3.Zero, Vector3.Down, Vector3.Backward);
    this.mLightViewProjection[4] = Matrix.CreateLookAt(Vector3.Zero, Vector3.Backward, Vector3.Up);
    this.mLightViewProjection[5] = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
    Matrix perspectiveFieldOfView = Matrix.CreatePerspectiveFieldOfView(1.57079637f, 1f, 0.005f, 1f);
    for (int index = 0; index < 6; ++index)
      Matrix.Multiply(ref this.mLightViewProjection[index], ref perspectiveFieldOfView, out this.mLightViewProjection[index]);
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
      this.mShadowMap = new RenderTargetCube(this.mDevice, this.mShadowMapSize, 1, SurfaceFormat.Single, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents);
    if (this.mShadowDepthStencil != null && !this.mShadowDepthStencil.IsDisposed)
      return;
    this.mShadowDepthStencil = new DepthStencilBuffer(this.mDevice, this.mShadowMapSize, this.mShadowMapSize, DepthFormat.Depth24, MultiSampleType.None, 0);
  }

  protected void UpdateMatrices()
  {
    Matrix.CreateScale(this.mRange, out this.mLightOrientation);
    this.mLightOrientation.Translation = this.mPosition;
  }

  public override void DrawShadows(DataChannel iDataChannel, float iDeltaTime, Scene iScene)
  {
    this.mDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
    this.mDevice.DepthStencilBuffer = this.mShadowDepthStencil;
    Matrix lightOrientation = this.mLightOrientation;
    Vector3 result1 = lightOrientation.Translation;
    Vector3.Add(ref result1, ref this.mOffset, out result1);
    lightOrientation.Translation = result1;
    Matrix result2;
    Matrix.Invert(ref lightOrientation, out result2);
    for (int faceType = 0; faceType < 6; ++faceType)
    {
      Matrix result3;
      Matrix.Multiply(ref result2, ref this.mLightViewProjection[faceType], out result3);
      Matrix.Multiply(ref result3, ref PointLight.FLIPX, out result3);
      this.mFrustum.Matrix = result3;
      this.mDevice.SetRenderTarget(0, this.mShadowMap, (CubeMapFace) faceType);
      this.mDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1f, 0);
      RenderManager.Instance.GlobalDummyEffect.ViewProjection = result3;
      DummyEffect localDummyEffect = RenderManager.Instance.LocalDummyEffect;
      if (localDummyEffect != null)
        localDummyEffect.ViewProjection = result3;
      iScene.DrawShadows(iDataChannel, this.mFrustum, iDeltaTime);
    }
    this.mDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
  }

  public override void Draw(
    Microsoft.Xna.Framework.Graphics.Effect iEffect,
    DataChannel iDataChannel,
    float iDeltaTime,
    Texture2D iNormalMap,
    Texture2D iDepthMap)
  {
    PointLightEffect pointLightEffect = iEffect as PointLightEffect;
    GraphicsDevice graphicsDevice = pointLightEffect.GraphicsDevice;
    Matrix lightOrientation = this.mLightOrientation;
    Vector3 result1 = lightOrientation.Translation;
    Vector3.Add(ref result1, ref this.mOffset, out result1);
    lightOrientation.Translation = result1;
    Vector3 result2;
    Vector3.Multiply(ref this.mDiffuseColor, this.mIntensity, out result2);
    Vector3 result3;
    Vector3.Multiply(ref this.mAmbientColor, this.mIntensity, out result3);
    float num = this.mSpecularAmount * this.mIntensity;
    pointLightEffect.DiffuseColor = result2;
    pointLightEffect.AmbientColor = result3;
    pointLightEffect.SpecularAmount = num;
    pointLightEffect.World = lightOrientation;
    pointLightEffect.LightPosition = lightOrientation.Translation;
    pointLightEffect.LightRadius = this.mRange;
    pointLightEffect.NormalMap = iNormalMap;
    pointLightEffect.DepthMap = iDepthMap;
    pointLightEffect.HalfPixel = new Vector2()
    {
      X = 0.5f / (float) iNormalMap.Width,
      Y = 0.5f / (float) iNormalMap.Height
    };
    if (this.mCastShadow)
      pointLightEffect.ShadowMap = this.mShadowMap.GetTexture();
    pointLightEffect.CommitChanges();
    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 42, 0, 80 /*0x50*/);
  }

  public override float SpecularAmount
  {
    get => this.mSpecularAmount;
    set => this.mSpecularAmount = value;
  }

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

  public Vector3 Position
  {
    get => this.mPosition;
    set
    {
      this.mPosition = value;
      this.mBoundingSphere.Center = value;
      this.UpdateMatrices();
    }
  }

  public float Radius
  {
    get => this.mRange;
    set
    {
      this.mRange = value;
      this.mBoundingSphere.Radius = value;
      this.UpdateMatrices();
    }
  }

  public override int Effect => PointLightEffect.TYPEHASH;

  public override int Technique => this.mCastShadow ? 0 : 1;

  public override int VertexStride => 12;

  public override VertexBuffer VertexBuffer => this.mVertices;

  public override IndexBuffer IndexBuffer => this.mIndices;

  public override VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

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

  public override bool ShouldDraw(BoundingFrustum iViewFrustum)
  {
    return iViewFrustum.Contains(this.mBoundingSphere) != ContainmentType.Disjoint;
  }
}
