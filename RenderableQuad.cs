// Decompiled with JetBrains decompiler
// Type: PolygonHead.RenderableQuad
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using System;

#nullable disable
namespace PolygonHead;

public class RenderableQuad : IDisposable
{
  private RenderableQuad.RenderData[] mRenderData;
  private float mAlpha;
  private PivotPosition mPivotPosition;
  private Texture2D mTexture;
  private RenderType mRenderType;
  private Scene mScene;
  private float mWidth;
  private float mHeight;
  private float mRotation;
  private Vector2 mPosition;
  private Vector2 mScale;
  private VertexBuffer mVertexBuffer;
  private VertexDeclaration mVertexDeclaration;
  private GUIBasicEffect mBasicEffect;
  private Matrix mTransformation;
  private int mFadeDirection;
  private float mFadeTime;
  private bool mFade;
  private int mZIndex;
  private bool mIsDisposed;

  public float Width => this.mWidth;

  public float Height => this.mHeight;

  public Texture2D Texture
  {
    get => this.mTexture;
    set
    {
      this.mTexture = value;
      this.mBasicEffect.Texture = (Microsoft.Xna.Framework.Graphics.Texture) value;
      this.RefreshVertexBuffer();
    }
  }

  public Vector2 Position
  {
    get => this.mPosition;
    set
    {
      this.mPosition = value;
      this.mTransformation.M41 = this.mPosition.X;
      this.mTransformation.M42 = this.mPosition.Y;
    }
  }

  public float Rotation
  {
    get => this.mRotation;
    set
    {
      this.mRotation = value;
      Matrix result1;
      Matrix.CreateScale(this.mScale.X, this.mScale.Y, 1f, out result1);
      Matrix result2;
      Matrix.CreateRotationZ(this.mRotation, out result2);
      Matrix.Multiply(ref result1, ref result2, out result2);
      result2.M41 = this.mPosition.X;
      result2.M42 = this.mPosition.Y;
      this.mTransformation = result2;
    }
  }

  public float Scale
  {
    get => this.mScale.X * this.mScale.Y;
    set
    {
      this.mScale.X = value;
      this.mScale.Y = value;
      Matrix result1;
      Matrix.CreateScale(this.mScale.X, this.mScale.Y, 1f, out result1);
      Matrix result2;
      Matrix.CreateRotationZ(this.mRotation, out result2);
      Matrix.Multiply(ref result1, ref result2, out result2);
      result2.M41 = this.mPosition.X;
      result2.M42 = this.mPosition.Y;
      this.mTransformation = result2;
    }
  }

  public Vector2 Scale2D
  {
    get => this.mScale;
    set
    {
      this.mScale = value;
      Matrix result1;
      Matrix.CreateScale(this.mScale.X, this.mScale.Y, 1f, out result1);
      Matrix result2;
      Matrix.CreateRotationZ(this.mRotation, out result2);
      Matrix.Multiply(ref result1, ref result2, out result2);
      result2.M41 = this.mPosition.X;
      result2.M42 = this.mPosition.Y;
      this.mTransformation = result2;
    }
  }

  public float Alpha
  {
    get => this.mAlpha;
    set
    {
      this.mAlpha = value;
      this.mFade = false;
    }
  }

  public bool Visible
  {
    get => (double) this.mAlpha > 0.0;
    set
    {
      this.mFade = false;
      if (value)
        this.mAlpha = 1f;
      else
        this.mAlpha = 0.0f;
    }
  }

  public RenderableQuad(
    Texture2D iTexture,
    GraphicsDevice iGraphicsDevice,
    PivotPosition iPivotPosition,
    RenderType iRenderType,
    Scene iScene,
    int iZIndex)
  {
    this.mRenderType = iRenderType;
    this.mScene = iScene;
    this.mPivotPosition = iPivotPosition;
    this.mAlpha = 1f;
    this.mBasicEffect = new GUIBasicEffect(iGraphicsDevice, (EffectPool) null);
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
    if (iTexture != null)
    {
      this.mTexture = iTexture;
      this.mBasicEffect.Texture = (Microsoft.Xna.Framework.Graphics.Texture) this.mTexture;
      this.mBasicEffect.TextureEnabled = true;
      this.mWidth = (float) iTexture.Width;
      this.mHeight = (float) iTexture.Height;
    }
    else
    {
      this.mWidth = 1f;
      this.mHeight = 1f;
    }
    this.mVertexBuffer = new VertexBuffer(iGraphicsDevice, VertexPositionTexture.SizeInBytes * 4, BufferUsage.WriteOnly);
    this.RefreshVertexBuffer();
    this.mVertexDeclaration = new VertexDeclaration(iGraphicsDevice, VertexPositionTexture.VertexElements);
    this.mTransformation = new Matrix();
    this.Scale = 1f;
    this.mZIndex = iZIndex;
    this.mRenderData = new RenderableQuad.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      RenderableQuad.RenderData renderData = new RenderableQuad.RenderData();
      renderData.Vertices = this.mVertexBuffer;
      renderData.VertexDeclaration = this.mVertexDeclaration;
      if (this.mRenderType == RenderType.GUI)
        renderData.Effect = (Effect) this.mBasicEffect;
      renderData.ZIndex = iZIndex;
      this.mRenderData[index] = renderData;
    }
  }

  private void RefreshVertexBuffer()
  {
    Vector2 vector2 = new Vector2();
    switch (this.mPivotPosition)
    {
      case PivotPosition.CENTER:
        vector2.X = (float) (-(double) this.mWidth * 0.5);
        vector2.Y = (float) (-(double) this.mHeight * 0.5);
        break;
      case PivotPosition.BOTTOMLEFT:
        vector2.Y = -this.mHeight;
        break;
    }
    VertexPositionTexture[] data = new VertexPositionTexture[4];
    data[0].Position.X = vector2.X;
    data[0].Position.Y = vector2.Y;
    data[0].TextureCoordinate.X = 0.0f;
    data[0].TextureCoordinate.Y = 0.0f;
    data[1].Position.X = vector2.X + this.mWidth;
    data[1].Position.Y = vector2.Y;
    data[1].TextureCoordinate.X = 1f;
    data[1].TextureCoordinate.Y = 0.0f;
    data[2].Position.X = vector2.X + this.mWidth;
    data[2].Position.Y = vector2.Y + this.mHeight;
    data[2].TextureCoordinate.X = 1f;
    data[2].TextureCoordinate.Y = 1f;
    data[3].Position.X = vector2.X;
    data[3].Position.Y = vector2.Y + this.mHeight;
    data[3].TextureCoordinate.X = 0.0f;
    data[3].TextureCoordinate.Y = 1f;
    this.mVertexBuffer.SetData<VertexPositionTexture>(data);
  }

  public void Fade(float iSeconds)
  {
    if (this.mFade)
      return;
    this.mFadeDirection = !this.Visible ? 1 : -1;
    this.mFadeTime = iSeconds;
    this.mFade = true;
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    RenderableQuad.RenderData iObject = this.mRenderData[(int) iDataChannel];
    if (this.mFade)
    {
      this.mAlpha += 1f / this.mFadeTime * (float) this.mFadeDirection * iDeltaTime;
      if ((double) this.mAlpha <= 0.0)
      {
        this.mFade = false;
        this.mAlpha = 0.0f;
      }
      else if ((double) this.mAlpha >= 1.0)
      {
        this.mFade = false;
        this.mAlpha = 1f;
      }
    }
    iObject.Alpha = this.mAlpha;
    iObject.Transform = this.mTransformation;
    if (this.mRenderType != RenderType.GUI || (double) this.mAlpha <= 0.0)
      return;
    this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public bool IsDisposed => this.mIsDisposed;

  public void Dispose()
  {
    if (this.mIsDisposed)
      return;
    if (this.mBasicEffect != null)
      this.mBasicEffect.Dispose();
    this.mVertexBuffer.Dispose();
    this.mVertexDeclaration.Dispose();
    this.mIsDisposed = true;
  }

  protected class RenderData : IRenderableGUIObject
  {
    protected Effect mEffect;
    protected Matrix mTransform;
    protected float mAlpha;
    protected VertexBuffer mVertexBuffer;
    protected VertexDeclaration mVertexDeclaration;
    protected IndexBuffer mIndexBuffer;
    protected int mZIndex;

    public Effect Effect
    {
      get => this.mEffect;
      set => this.mEffect = value;
    }

    public VertexBuffer Vertices
    {
      get => this.mVertexBuffer;
      set => this.mVertexBuffer = value;
    }

    public VertexDeclaration VertexDeclaration
    {
      get => this.mVertexDeclaration;
      set => this.mVertexDeclaration = value;
    }

    public float Alpha
    {
      get => this.mAlpha;
      set => this.mAlpha = value;
    }

    public Matrix Transform
    {
      get => this.mTransform;
      set => this.mTransform = value;
    }

    public int ZIndex
    {
      get => this.mZIndex;
      set => this.mZIndex = value;
    }

    public void Draw(float iDeltaTime)
    {
      GUIBasicEffect mEffect = this.mEffect as GUIBasicEffect;
      mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
      mEffect.Transform = this.mTransform;
      mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      Vector4 one = Vector4.One with { W = this.mAlpha };
      mEffect.Color = one;
      mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
      mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
      mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      mEffect.Begin();
      int count = mEffect.CurrentTechnique.Passes.Count;
      for (int index = 0; index < count; ++index)
      {
        EffectPass pass = mEffect.CurrentTechnique.Passes[index];
        pass.Begin();
        mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
        pass.End();
      }
      mEffect.End();
    }
  }
}
