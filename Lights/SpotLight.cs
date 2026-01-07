// Decompiled with JetBrains decompiler
// Type: PolygonHead.Lights.SpotLight
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead.Lights;

public class SpotLight : Light
{
  protected static Matrix FLIPX = Matrix.CreateScale(-1f, 1f, 1f);
  private int mShadowMapSize = 128 /*0x80*/;
  private static VertexBuffer mVertices;
  private static VertexDeclaration mVertexDeclaration;
  private static IndexBuffer mIndices;
  private static int mNumVertices;
  private static int mPrimitiveCount;
  private float mRange;
  private float mCutoffAngle;
  private Vector3 mPosition;
  private Vector3 mDirection;
  private Matrix mLightOrientation;
  private Matrix mLightViewProjection;
  private BoundingFrustum mFrustum;
  private RenderTarget2D mShadowMap;
  private DepthStencilBuffer mShadowDepthStencil;
  private bool mCastShadow;
  private bool mMatricesDirty = true;
  private Vector3 mDiffuseColor = new Vector3(0.8f);
  private Vector3 mAmbientColor = new Vector3(0.2f);
  private float mSpecularAmount;
  private float mSharpness;
  private bool mUseAttenuation;
  private Random mRandom = new Random();
  private GraphicsDevice mDevice;

  public SpotLight(GraphicsDevice iDevice)
  {
    this.mDevice = iDevice;
    if (!(RenderManager.Instance.GetEffect(SpotLightEffect.TYPEHASH) is SpotLightEffect iEffect))
    {
      lock (this.mDevice)
        iEffect = new SpotLightEffect(iDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
      RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) iEffect);
    }
    Vector2 vector2 = new Vector2();
    Point screenSize = RenderManager.Instance.ScreenSize;
    vector2.X = 0.5f / (float) screenSize.X;
    vector2.Y = 0.5f / (float) screenSize.Y;
    iEffect.HalfPixel = vector2;
    this.mFrustum = new BoundingFrustum(Matrix.Identity);
    if (SpotLight.mVertices != null)
      return;
    this.CreateVertices(16 /*0x10*/, 4);
  }

  protected void CreateVertices(int iAxisDevisions, int iCapDevisions)
  {
    Vector3[] data1 = new Vector3[iAxisDevisions * (iCapDevisions + 1) + 2];
    data1[0] = new Vector3();
    int num1 = 1;
    for (int index1 = 0; index1 <= iCapDevisions; ++index1)
    {
      double num2 = 1.0 - (double) index1 / ((double) iCapDevisions + 1.0);
      for (int index2 = 0; index2 < iAxisDevisions; ++index2)
      {
        double num3 = 2.0 * Math.PI * (double) index2 / (double) iAxisDevisions;
        data1[num1++] = new Vector3()
        {
          Z = -1f,
          X = (float) (Math.Cos(num3) * num2),
          Y = (float) (Math.Sin(num3) * num2)
        };
      }
    }
    Vector3[] vector3Array = data1;
    int index3 = num1;
    int num4 = index3 + 1;
    vector3Array[index3] = Vector3.Forward;
    lock (this.mDevice)
    {
      SpotLight.mVertices = new VertexBuffer(this.mDevice, data1.Length * 12, BufferUsage.None);
      SpotLight.mVertices.SetData<Vector3>(data1);
    }
    SpotLight.mNumVertices = data1.Length;
    SpotLight.mVertexDeclaration = new VertexDeclaration(this.mDevice, new VertexElement[1]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0)
    });
    ushort[] data2 = new ushort[(iAxisDevisions * 2 + iAxisDevisions * iCapDevisions * 2) * 3];
    int num5 = 0;
    for (int index4 = 0; index4 < iAxisDevisions; ++index4)
    {
      ushort[] numArray1 = data2;
      int index5 = num5;
      int num6 = index5 + 1;
      numArray1[index5] = (ushort) 0;
      ushort[] numArray2 = data2;
      int index6 = num6;
      int num7 = index6 + 1;
      int num8 = (int) (ushort) ((index4 + 1) % iAxisDevisions + 1);
      numArray2[index6] = (ushort) num8;
      ushort[] numArray3 = data2;
      int index7 = num7;
      num5 = index7 + 1;
      int num9 = (int) (ushort) (index4 + 1);
      numArray3[index7] = (ushort) num9;
    }
    for (int index8 = 0; index8 < iCapDevisions; ++index8)
    {
      int num10 = index8 * iAxisDevisions + 1;
      for (int index9 = 0; index9 < iAxisDevisions; ++index9)
      {
        ushort[] numArray4 = data2;
        int index10 = num5;
        int num11 = index10 + 1;
        int num12 = (int) (ushort) (num10 + index9);
        numArray4[index10] = (ushort) num12;
        ushort[] numArray5 = data2;
        int index11 = num11;
        int num13 = index11 + 1;
        int num14 = (int) (ushort) (num10 + (index9 + 1) % iAxisDevisions);
        numArray5[index11] = (ushort) num14;
        ushort[] numArray6 = data2;
        int index12 = num13;
        int num15 = index12 + 1;
        int num16 = (int) (ushort) (num10 + index9 + iAxisDevisions);
        numArray6[index12] = (ushort) num16;
        ushort[] numArray7 = data2;
        int index13 = num15;
        int num17 = index13 + 1;
        int num18 = (int) (ushort) (num10 + (index9 + 1) % iAxisDevisions);
        numArray7[index13] = (ushort) num18;
        ushort[] numArray8 = data2;
        int index14 = num17;
        int num19 = index14 + 1;
        int num20 = (int) (ushort) (num10 + (index9 + 1) % iAxisDevisions + iAxisDevisions);
        numArray8[index14] = (ushort) num20;
        ushort[] numArray9 = data2;
        int index15 = num19;
        num5 = index15 + 1;
        int num21 = (int) (ushort) (num10 + index9 + iAxisDevisions);
        numArray9[index15] = (ushort) num21;
      }
    }
    for (int index16 = 0; index16 < iAxisDevisions; ++index16)
    {
      ushort[] numArray10 = data2;
      int index17 = num5;
      int num22 = index17 + 1;
      int num23 = (int) (ushort) (data1.Length - 1);
      numArray10[index17] = (ushort) num23;
      ushort[] numArray11 = data2;
      int index18 = num22;
      int num24 = index18 + 1;
      int num25 = (int) (ushort) (data1.Length - 1 - ((index16 + 1) % iAxisDevisions + 1));
      numArray11[index18] = (ushort) num25;
      ushort[] numArray12 = data2;
      int index19 = num24;
      num5 = index19 + 1;
      int num26 = (int) (ushort) (data1.Length - 1 - (index16 + 1));
      numArray12[index19] = (ushort) num26;
    }
    lock (this.mDevice)
    {
      SpotLight.mIndices = new IndexBuffer(this.mDevice, data2.Length * 2, BufferUsage.None, IndexElementSize.SixteenBits);
      SpotLight.mIndices.SetData<ushort>(data2);
    }
    SpotLight.mPrimitiveCount = data2.Length / 3;
  }

  protected void UpdateMatrices()
  {
    this.mMatricesDirty = false;
    Vector3 result1 = Vector3.Up;
    float result2;
    Vector3.Dot(ref this.mDirection, ref result1, out result2);
    if ((double) Math.Abs(result2) >= 0.89999997615814209)
      result1 = Vector3.Forward;
    Vector3 result3;
    Vector3.Add(ref this.mPosition, ref this.mDirection, out result3);
    Matrix result4;
    Matrix.CreateLookAt(ref this.mPosition, ref result3, ref result1, out result4);
    Matrix result5;
    Matrix.CreatePerspectiveFieldOfView(this.mCutoffAngle * 2f, 1f, 0.01f, this.mRange, out result5);
    Matrix.Multiply(ref result4, ref result5, out this.mLightViewProjection);
    Vector3 result6;
    Vector3.Cross(ref this.mDirection, ref result1, out result6);
    result6.Normalize();
    Vector3.Cross(ref result6, ref this.mDirection, out result1);
    this.mLightOrientation = Matrix.Identity;
    this.mLightOrientation.Forward = this.mDirection;
    this.mLightOrientation.Up = result1;
    this.mLightOrientation.Right = result6;
    this.mLightOrientation.Translation = this.mPosition;
    this.mFrustum.Matrix = this.mLightViewProjection;
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

  public override bool ShouldDraw(BoundingFrustum iViewFrustum)
  {
    return this.mFrustum.Contains(iViewFrustum) != ContainmentType.Disjoint;
  }

  protected internal override void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    base.Update(iDataChannel, iDeltaTime, ref iCameraPosition, ref iCameraDirection);
    if (!this.mMatricesDirty)
      return;
    this.UpdateMatrices();
  }

  public override void DrawShadows(DataChannel iDataChannel, float iDeltaTime, Scene iScene)
  {
    if (!this.Enabled)
      return;
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
    if (!this.Enabled)
      return;
    SpotLightEffect spotLightEffect = iEffect as SpotLightEffect;
    Vector3 result1;
    Vector3.Multiply(ref this.mDiffuseColor, this.mIntensity, out result1);
    Vector3 result2;
    Vector3.Multiply(ref this.mAmbientColor, this.mIntensity, out result2);
    float num = this.mSpecularAmount * this.mIntensity;
    spotLightEffect.DiffuseColor = result1;
    spotLightEffect.AmbientColor = result2;
    spotLightEffect.SpecularAmount = num;
    spotLightEffect.LightCutoffAngle = this.mCutoffAngle;
    spotLightEffect.LightSharpness = this.mSharpness;
    spotLightEffect.LightDirection = this.mDirection;
    spotLightEffect.World = this.mLightOrientation;
    spotLightEffect.LightPosition = this.mPosition;
    spotLightEffect.LightRange = this.mRange;
    spotLightEffect.NormalMap = iNormalMap;
    spotLightEffect.DepthMap = iDepthMap;
    if (this.mCastShadow)
    {
      spotLightEffect.ShadowMapSize = (float) this.mShadowMapSize;
      spotLightEffect.ShadowMap = this.mShadowMap.GetTexture();
      spotLightEffect.LightViewProjection = this.mLightViewProjection;
    }
    spotLightEffect.CommitChanges();
    this.mDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, SpotLight.mNumVertices, 0, SpotLight.mPrimitiveCount);
  }

  public override float SpecularAmount
  {
    get => this.mSpecularAmount;
    set => this.mSpecularAmount = value;
  }

  public float CutoffAngle
  {
    get => this.mCutoffAngle;
    set
    {
      this.mCutoffAngle = value;
      this.mMatricesDirty = true;
    }
  }

  public float Sharpness
  {
    get => this.mSharpness;
    set => this.mSharpness = value;
  }

  public bool UseAttenuation
  {
    get => this.mUseAttenuation;
    set => this.mUseAttenuation = value;
  }

  public float Range
  {
    get => this.mRange;
    set
    {
      value = Math.Max(value, 0.1f);
      this.mRange = value;
      this.mMatricesDirty = true;
    }
  }

  public Vector3 Direction
  {
    get => this.mDirection;
    set
    {
      this.mDirection = value;
      this.mMatricesDirty = true;
    }
  }

  public Vector3 Position
  {
    get => this.mPosition;
    set
    {
      this.mPosition = value;
      this.mMatricesDirty = true;
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

  public override int Effect => SpotLightEffect.TYPEHASH;

  public override int Technique
  {
    get => this.mCastShadow ? (this.mUseAttenuation ? 0 : 2) : (this.mUseAttenuation ? 1 : 3);
  }

  public override int VertexStride => 12;

  public override VertexBuffer VertexBuffer => SpotLight.mVertices;

  public override IndexBuffer IndexBuffer => SpotLight.mIndices;

  public override VertexDeclaration VertexDeclaration => SpotLight.mVertexDeclaration;

  public override bool CastShadows
  {
    get => this.mCastShadow;
    set => this.mCastShadow = value;
  }

  public override int ShadowMapSize
  {
    get => this.mShadowMapSize;
    set => this.mShadowMapSize = value;
  }
}
