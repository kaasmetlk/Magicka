// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Effects.RadialBlur
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Graphics.Effects;

public class RadialBlur : Effect, IPostEffect, IAbilityEffect
{
  private static List<RadialBlur> mCache;
  private static ContentManager mContent;
  private EffectParameter mWorldParameter;
  private EffectParameter mViewParameter;
  private EffectParameter mProjectionParameter;
  private EffectParameter mFarDistanceParameter;
  private EffectParameter mStartRadiusParameter;
  private EffectParameter mMidRadiusParameter;
  private EffectParameter mEndRadiusParameter;
  private EffectParameter mSpreadParameter;
  private EffectParameter mAlphaParameter;
  private EffectParameter mPixelSizeParameter;
  private EffectParameter mSourceTextureParameter;
  private EffectParameter mDepthTextureParameter;
  private VertexDeclaration mDeclaration;
  private VertexBuffer mVertices;
  private IndexBuffer mIndices;
  private int mPrimitiveCount;
  private int mNumVertices;
  private float mTTL;
  private float mLifeTime;
  private float mMaxRadius;
  private Scene mScene;

  private RadialBlur(GraphicsDevice iDevice, ContentManager iContentManager)
    : base(iDevice, iContentManager.Load<Effect>("Shaders/RadialBlur"))
  {
    this.mWorldParameter = this.Parameters[nameof (World)];
    this.mViewParameter = this.Parameters[nameof (View)];
    this.mProjectionParameter = this.Parameters[nameof (Projection)];
    this.mFarDistanceParameter = this.Parameters[nameof (FarDistance)];
    this.mStartRadiusParameter = this.Parameters[nameof (StartRadius)];
    this.mMidRadiusParameter = this.Parameters[nameof (MidRadius)];
    this.mEndRadiusParameter = this.Parameters[nameof (EndRadius)];
    this.mSpreadParameter = this.Parameters["Spread"];
    this.mAlphaParameter = this.Parameters[nameof (Alpha)];
    this.mPixelSizeParameter = this.Parameters[nameof (PixelSize)];
    this.mSourceTextureParameter = this.Parameters[nameof (SourceTexture)];
    this.mDepthTextureParameter = this.Parameters[nameof (DepthTexture)];
    RadialBlur.RadialBlurVertex[] data1 = new RadialBlur.RadialBlurVertex[51];
    for (int index = 0; index <= 16 /*0x10*/; ++index)
    {
      RadialBlur.RadialBlurVertex radialBlurVertex = new RadialBlur.RadialBlurVertex();
      radialBlurVertex.RotationOffset = (float) (1.0 - (double) index * 2.0 / 16.0);
      radialBlurVertex.Amount = 0.0f;
      radialBlurVertex.StartOffset = 1f;
      data1[index * 3] = radialBlurVertex;
      radialBlurVertex.Amount = 1f;
      radialBlurVertex.StartOffset = 0.0f;
      radialBlurVertex.MidOffset = 1f;
      data1[index * 3 + 1] = radialBlurVertex;
      radialBlurVertex.Amount = 0.0f;
      radialBlurVertex.MidOffset = 0.0f;
      radialBlurVertex.EndOffset = 1f;
      data1[index * 3 + 2] = radialBlurVertex;
    }
    ushort[] data2 = new ushort[192 /*0xC0*/];
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      data2[index * 12] = (ushort) (index * 3 + 3);
      data2[index * 12 + 1] = (ushort) (index * 3 + 1);
      data2[index * 12 + 2] = (ushort) (index * 3);
      data2[index * 12 + 3] = (ushort) (index * 3 + 3);
      data2[index * 12 + 4] = (ushort) (index * 3 + 4);
      data2[index * 12 + 5] = (ushort) (index * 3 + 1);
      data2[index * 12 + 6] = (ushort) (index * 3 + 4);
      data2[index * 12 + 7] = (ushort) (index * 3 + 2);
      data2[index * 12 + 8] = (ushort) (index * 3 + 1);
      data2[index * 12 + 9] = (ushort) (index * 3 + 4);
      data2[index * 12 + 10] = (ushort) (index * 3 + 5);
      data2[index * 12 + 11] = (ushort) (index * 3 + 2);
    }
    this.mVertices = new VertexBuffer(iDevice, 20 * data1.Length, BufferUsage.WriteOnly);
    this.mVertices.SetData<RadialBlur.RadialBlurVertex>(data1);
    this.mIndices = new IndexBuffer(iDevice, 2 * data2.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
    this.mIndices.SetData<ushort>(data2);
    this.mDeclaration = new VertexDeclaration(iDevice, RadialBlur.RadialBlurVertex.VertexElements);
    this.mNumVertices = data1.Length;
    this.mPrimitiveCount = data2.Length / 3;
  }

  public static void InitializeCache(ContentManager iContent, int iSize)
  {
    RadialBlur.mContent = iContent;
    RadialBlur.mCache = new List<RadialBlur>(iSize);
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    for (int index = 0; index < iSize; ++index)
    {
      lock (graphicsDevice)
        RadialBlur.mCache.Add(new RadialBlur(graphicsDevice, RadialBlur.mContent));
    }
  }

  public static RadialBlur GetRadialBlur()
  {
    if (RadialBlur.mCache.Count <= 0)
      return new RadialBlur(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
    RadialBlur radialBlur = RadialBlur.mCache[RadialBlur.mCache.Count - 1];
    RadialBlur.mCache.RemoveAt(RadialBlur.mCache.Count - 1);
    return radialBlur;
  }

  public Matrix World
  {
    get => this.mWorldParameter.GetValueMatrix();
    set => this.mWorldParameter.SetValue(value);
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

  public float Angle
  {
    get => this.mSpreadParameter.GetValueSingle();
    set => this.mSpreadParameter.SetValue(MathHelper.Clamp(value, 0.0f, 3.14159274f));
  }

  public float FarDistance
  {
    get => this.mFarDistanceParameter.GetValueSingle();
    set => this.mFarDistanceParameter.SetValue(value);
  }

  public float StartRadius
  {
    get => this.mStartRadiusParameter.GetValueSingle();
    set => this.mStartRadiusParameter.SetValue(value);
  }

  public float MidRadius
  {
    get => this.mMidRadiusParameter.GetValueSingle();
    set => this.mMidRadiusParameter.SetValue(value);
  }

  public float EndRadius
  {
    get => this.mEndRadiusParameter.GetValueSingle();
    set => this.mEndRadiusParameter.SetValue(value);
  }

  public float Alpha
  {
    get => this.mAlphaParameter.GetValueSingle();
    set => this.mAlphaParameter.SetValue(value);
  }

  public Vector2 PixelSize
  {
    get => this.mPixelSizeParameter.GetValueVector2();
    set => this.mPixelSizeParameter.SetValue(value);
  }

  public Texture2D SourceTexture
  {
    get => this.mSourceTextureParameter.GetValueTexture2D();
    set => this.mSourceTextureParameter.SetValue((Texture) value);
  }

  public Texture2D DepthTexture
  {
    get => this.mDepthTextureParameter.GetValueTexture2D();
    set => this.mDepthTextureParameter.SetValue((Texture) value);
  }

  public bool Dead => (double) this.mTTL <= 0.0;

  public void Initialize(ref Vector3 iPosition, float iMaxRadius, float iTTL, Scene iScene)
  {
    Vector3 forward = Vector3.Forward;
    this.Initialize(ref iPosition, ref forward, 3.14159274f, iMaxRadius, iTTL, iScene);
  }

  public void Initialize(
    ref Vector3 iPosition,
    ref Vector3 iDirection,
    float iAngle,
    float iRadius,
    float iTTL,
    Scene iScene)
  {
    Matrix identity = Matrix.Identity;
    Vector3 result1 = Vector3.Up;
    Vector3 result2;
    Vector3.Cross(ref iDirection, ref result1, out result2);
    Vector3.Cross(ref result2, ref iDirection, out result1);
    result1.Normalize();
    result2.Normalize();
    identity.Up = result1;
    identity.Right = result2;
    identity.Forward = iDirection;
    identity.Translation = iPosition;
    this.World = identity;
    this.Angle = iAngle;
    this.mTTL = iTTL;
    this.mLifeTime = iTTL;
    this.mMaxRadius = iRadius * 1.5f;
    this.mScene = iScene;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
  }

  public void Draw(
    float iDeltaTime,
    ref Vector2 iPixelSize,
    ref Matrix iViewMatrix,
    ref Matrix iProjectionMatrix,
    Texture2D iCandidate,
    Texture2D iDepthMap,
    Texture2D iNormalMap)
  {
    this.mTTL -= iDeltaTime;
    float num1 = this.mTTL / this.mLifeTime;
    float num2 = (float) Math.Pow(1.0 - (double) num1, 0.10000000149011612) * this.mMaxRadius;
    this.Alpha = (float) Math.Sqrt((double) num1 * (1.0 - (double) num1) * 4.0);
    this.StartRadius = num2 * 0.0f;
    this.MidRadius = num2 * 0.666f;
    this.EndRadius = num2;
    this.PixelSize = iPixelSize;
    this.View = iViewMatrix;
    this.Projection = iProjectionMatrix;
    this.SourceTexture = iCandidate;
    this.DepthTexture = iDepthMap;
    this.GraphicsDevice.VertexDeclaration = this.mDeclaration;
    this.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 20);
    this.GraphicsDevice.Indices = this.mIndices;
    this.Begin();
    for (int index = 0; index < this.CurrentTechnique.Passes.Count; ++index)
    {
      EffectPass pass = this.CurrentTechnique.Passes[index];
      pass.Begin();
      this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
      pass.End();
    }
    this.End();
  }

  public void Kill() => this.mTTL = 0.0f;

  public int ZIndex => 0;

  internal static void DisposeCache()
  {
    for (int index = 0; index < RadialBlur.mCache.Count; ++index)
      RadialBlur.mCache[index].Dispose();
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mScene.AddPostEffect(iDataChannel, (IPostEffect) this);
  }

  public void OnRemove() => RadialBlur.mCache.Add(this);

  protected struct RadialBlurVertex
  {
    public const int SizeInBytes = 20;
    public float StartOffset;
    public float MidOffset;
    public float EndOffset;
    public float RotationOffset;
    public float Amount;
    public static readonly VertexElement[] VertexElements = new VertexElement[3]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 0, (short) 16 /*0x10*/, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 1)
    };
  }
}
