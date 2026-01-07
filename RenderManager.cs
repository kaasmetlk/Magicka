// Decompiled with JetBrains decompiler
// Type: PolygonHead.RenderManager
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using PolygonHead.ParticleEffects;
using System;
using System.Collections.Generic;

#nullable disable
namespace PolygonHead;

public sealed class RenderManager
{
  public const int MAXRENDERABLEOBJECTS = 512 /*0x0200*/;
  public const int MAXRENDERABLEGUIOBJECTS = 128 /*0x80*/;
  public const int MAXRENDERABLEGUITEXTOBJECTS = 512 /*0x0200*/;
  public const float DEFUALT_BLOOM_THRESHOLD = 0.8f;
  public const float DEFUALT_BLOOM_MULTIPLYER = 1f;
  public const float DEFUALT_BLUR_SIGMA = 2.5f;
  private const float mDefaultSize = 0.00138888892f;
  private Color mColor;
  private List<VertexDeclaration> mVertexDeclarations = new List<VertexDeclaration>();
  private RestoreDepthEffect mDepthRestoreEffect;
  private BasicPostProcessingEffect mRestoreEffect;
  private FilterEffect mFilterEffect;
  private BloomEffect mBloomEffect;
  private DeferredShadingEffect mDeferredShadingEffect;
  private DirectionalLightEffect mDirectionalLightEffect;
  private ClearEffect mClearEffect;
  private SkyMapEffect mSkyMapEffect;
  private Texture2D mSkyMapTexture;
  private Vector3 mSkyMapColor = new Vector3(1f);
  private RenderTarget2D mHalfBuffer;
  private RenderTarget2D mQuarterBuffer;
  private RenderTarget2D mEighthBuffer;
  private RenderTarget2D mSixteenthBufferA;
  private RenderTarget2D mSixteenthBufferB;
  private RenderTarget2D mDepthBuffer;
  private RenderTarget2D mNormalBuffer;
  private RenderTarget2D mMainBuffer;
  private RenderTarget2D mSecondaryBuffer;
  private RenderTarget2D mGUIBuffer;
  private DepthStencilBuffer mGUIDepthStencil;
  private Texture2D mLatestDepthMap;
  private bool mRecreateBuffers;
  private float mBloomThreshold = 0.8f;
  private float mBloomMultiplier = 1f;
  private float mBlurSigma = 2.5f;
  private TransitionEffect[] mTransitions;
  private TransitionEffect mCurrentTransition;
  private float mTime;
  private float mTimeModifier;
  private float mGUIScale;
  private Dictionary<int, Effect> mRegisteredEffects = new Dictionary<int, Effect>();
  private Point mSize;
  private DummyEffect mGlobalDummy;
  private DummyEffect mLocalDummy;
  private ResolveTexture2D mScreenShot;
  private bool mGetScreenShot;
  private Action mGetScreenShotComplete;
  private float mTextureLODBias = -0.5f;
  private static RenderManager mSingelton = (RenderManager) null;
  private static volatile object mSingeltonLock = new object();
  private GraphicsDevice mDevice;
  private Vector3[] mCornerBuffer = new Vector3[8];

  public event PolygonHead.TransitionEnd TransitionEnd;

  public static RenderManager Instance
  {
    get
    {
      if (RenderManager.mSingelton == null)
      {
        lock (RenderManager.mSingeltonLock)
        {
          if (RenderManager.mSingelton == null)
            RenderManager.mSingelton = new RenderManager();
        }
      }
      return RenderManager.mSingelton;
    }
  }

  public event Action<GraphicsDevice, Point> ResulutionChanged;

  private RenderManager()
  {
  }

  public void Initialize(GraphicsDevice iGraphicsDevice)
  {
    PresentationParameters presentationParameters = iGraphicsDevice.PresentationParameters;
    this.mSize.X = presentationParameters.BackBufferWidth;
    this.mSize.Y = presentationParameters.BackBufferHeight;
    this.mTimeModifier = 1f;
    this.mColor = Color.Black;
    this.mDevice = iGraphicsDevice;
    this.mRecreateBuffers = true;
    EffectPool effectPool = new EffectPool();
    this.mGlobalDummy = new DummyEffect(this.mDevice, effectPool);
    this.RegisterEffect((Effect) new RenderDeferredEffect(this.mDevice, effectPool));
    this.RegisterEffect((Effect) new RenderDeferredLiquidEffect(this.mDevice, effectPool));
    this.RegisterEffect((Effect) new LavaEffect(this.mDevice, effectPool));
    this.RegisterEffect((Effect) new SkinnedModelDeferredEffect(this.mDevice, effectPool));
    this.mRestoreEffect = new BasicPostProcessingEffect(this.mDevice, effectPool);
    this.mFilterEffect = new FilterEffect(this.mDevice, effectPool);
    this.mDepthRestoreEffect = new RestoreDepthEffect(this.mDevice, effectPool);
    this.mDeferredShadingEffect = new DeferredShadingEffect(this.mDevice, effectPool);
    this.mBloomEffect = new BloomEffect(this.mDevice, effectPool);
    this.mDirectionalLightEffect = new DirectionalLightEffect(this.mDevice, effectPool);
    this.mClearEffect = new ClearEffect(this.mDevice, effectPool);
    this.mSkyMapEffect = new SkyMapEffect(this.mDevice, effectPool);
    this.mTransitions = new TransitionEffect[3];
    this.mTransitions[1] = (TransitionEffect) new FadeTransitionEffect(this.mDevice);
    this.mTransitions[2] = (TransitionEffect) new CrossFadeTransitionEffect(this.mDevice);
    iGraphicsDevice.DeviceReset += new EventHandler(this.OnGraphicsDeviceReset);
  }

  private void OnGraphicsDeviceReset(object sender, EventArgs e)
  {
    this.mGlobalDummy = new DummyEffect(this.mDevice, this.mGlobalDummy.EffectPool);
    if (this.mLocalDummy == null)
      return;
    this.mLocalDummy = new DummyEffect(this.mDevice, this.mLocalDummy.EffectPool);
  }

  public void RenderScene(
    Scene currentScene,
    DataChannel iDataChannel,
    ref GameTime iGameTime,
    Scene iPersistentScene)
  {
    this.RenderScene(currentScene, iDataChannel, (float) iGameTime.ElapsedGameTime.TotalSeconds, iPersistentScene);
  }

  public void RenderScene(
    Scene currentScene,
    DataChannel iDataChannel,
    float iDeltaTime,
    Scene iPersistentScene)
  {
    float iDeltaTime1 = iDeltaTime * this.mTimeModifier;
    if (this.mDevice.PresentationParameters.BackBufferWidth != this.mSize.X || this.mDevice.PresentationParameters.BackBufferHeight != this.mSize.Y)
    {
      this.mSize.X = this.mDevice.PresentationParameters.BackBufferWidth;
      this.mSize.Y = this.mDevice.PresentationParameters.BackBufferHeight;
      this.mRecreateBuffers = true;
    }
    if (this.mRecreateBuffers)
      this.CreateBackBuffers();
    Point screenSize = this.ScreenSize;
    this.mGUIScale = (float) screenSize.Y * (1f / 720f);
    this.mTime = (float) (((double) this.mTime + (double) iDeltaTime1) % 1.0);
    Camera.CameraState cameraState = currentScene.Camera.mStateBuffer[(int) iDataChannel];
    Matrix result;
    Matrix.Invert(ref cameraState.ViewProjectionMatrix, out result);
    double farPlaneDistance = (double) currentScene.Camera.FarPlaneDistance;
    BoundingFrustum viewFrustum = currentScene.Camera.GetViewFrustum(iDataChannel);
    this.mGlobalDummy.View = cameraState.ViewMatrix;
    this.mGlobalDummy.Projection = cameraState.ProjectionMatrix;
    this.mGlobalDummy.ViewProjection = cameraState.ViewProjectionMatrix;
    this.mGlobalDummy.InverseViewProjection = result;
    this.mGlobalDummy.Time = this.mTime;
    this.mGlobalDummy.CameraPosition = cameraState.Position;
    this.mGlobalDummy.EyeOfTheBeholder = cameraState.EyeOfTheBeholder;
    if (this.mLocalDummy != null)
    {
      this.mLocalDummy.View = cameraState.ViewMatrix;
      this.mLocalDummy.Projection = cameraState.ProjectionMatrix;
      this.mLocalDummy.ViewProjection = cameraState.ViewProjectionMatrix;
      this.mLocalDummy.InverseViewProjection = result;
      this.mLocalDummy.Time = this.mTime;
      this.mLocalDummy.CameraPosition = cameraState.Position;
      this.mLocalDummy.EyeOfTheBeholder = cameraState.EyeOfTheBeholder;
    }
    DepthStencilBuffer depthStencilBuffer = this.mDevice.DepthStencilBuffer;
    float mTextureLodBias = this.mTextureLODBias;
    this.mDevice.SamplerStates[0].MipMapLevelOfDetailBias = mTextureLodBias;
    this.mDevice.SamplerStates[1].MipMapLevelOfDetailBias = mTextureLodBias;
    this.mDevice.SamplerStates[2].MipMapLevelOfDetailBias = mTextureLodBias;
    this.mDevice.SamplerStates[3].MipMapLevelOfDetailBias = mTextureLodBias;
    this.mDevice.SamplerStates[4].MipMapLevelOfDetailBias = mTextureLodBias;
    this.mDevice.SamplerStates[5].MipMapLevelOfDetailBias = mTextureLodBias;
    this.mDevice.SamplerStates[6].MipMapLevelOfDetailBias = mTextureLodBias;
    this.mDevice.SamplerStates[7].MipMapLevelOfDetailBias = mTextureLodBias;
    currentScene.UpdatePreRenderRenderers(iDataChannel, iDeltaTime1, ref cameraState.ViewProjectionMatrix, ref cameraState.Position, ref cameraState.Direction);
    this.mDevice.DepthStencilBuffer = depthStencilBuffer;
    this.mDevice.SetRenderTarget(0, this.mDepthBuffer);
    this.PostProcess(screenSize.X, screenSize.Y, (PostProcessingEffect) this.mClearEffect);
    currentScene.DrawDepth(iDataChannel, iDeltaTime1, viewFrustum, ref cameraState.ViewMatrix, ref cameraState.ProjectionMatrix, ref cameraState.ViewProjectionMatrix);
    this.mDevice.SetRenderTarget(0, this.mMainBuffer);
    this.mDevice.SetRenderTarget(1, this.mNormalBuffer);
    this.mLatestDepthMap = this.mDepthBuffer.GetTexture();
    this.mDepthRestoreEffect.SourceTexture0 = this.mLatestDepthMap;
    this.mDepthRestoreEffect.Color0 = new Vector4(this.mColor.ToVector3(), 0.0f);
    this.mDepthRestoreEffect.Color1 = new Vector4(0.5f, 0.75f, 0.5f, 0.5f);
    this.PostProcess(screenSize.X, screenSize.Y, (PostProcessingEffect) this.mDepthRestoreEffect);
    this.mDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
    currentScene.Draw(iDataChannel, iDeltaTime1, viewFrustum);
    currentScene.DrawProjection(iDataChannel, iDeltaTime1, viewFrustum, this.mLatestDepthMap);
    currentScene.UpdateLights(iDataChannel, iDeltaTime1, ref cameraState.Position, ref cameraState.Direction);
    this.mDevice.SetRenderTarget(0, (RenderTarget2D) null);
    this.mDevice.SetRenderTarget(1, (RenderTarget2D) null);
    currentScene.DrawLightShadows(iDataChannel, iDeltaTime1, viewFrustum);
    this.mDevice.DepthStencilBuffer = depthStencilBuffer;
    this.mDevice.SetRenderTarget(0, this.mSecondaryBuffer);
    this.mLatestDepthMap = this.mDepthBuffer.GetTexture();
    this.mDevice.Clear(ClearOptions.Target, new Color(), 1f, 0);
    this.mDevice.RenderState.AlphaTestEnable = false;
    this.mDevice.RenderState.AlphaBlendEnable = true;
    this.mDevice.RenderState.DestinationBlend = Blend.InverseSourceColor;
    this.mDevice.RenderState.SourceBlend = Blend.One;
    currentScene.DrawLights(iDataChannel, iDeltaTime1, ref cameraState.Position, ref cameraState.Direction, viewFrustum, ref cameraState.ViewProjectionMatrix, ref result, this.mNormalBuffer.GetTexture(), this.mDepthBuffer.GetTexture());
    this.mDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    this.mDevice.RenderState.DepthBufferEnable = false;
    this.mDeferredShadingEffect.SourceTexture0 = this.mMainBuffer.GetTexture();
    this.mDeferredShadingEffect.SourceTexture1 = this.mNormalBuffer.GetTexture();
    this.mDeferredShadingEffect.SourceTexture2 = this.mDepthBuffer.GetTexture();
    this.mDevice.SetRenderTarget(0, this.mMainBuffer);
    this.mDeferredShadingEffect.SourceTexture4 = this.mSecondaryBuffer.GetTexture();
    this.PostProcess(screenSize.X, screenSize.Y, (PostProcessingEffect) this.mDeferredShadingEffect);
    if (this.mSkyMapTexture != null && !this.mSkyMapTexture.IsDisposed)
    {
      this.mSkyMapEffect.SourceTexture0 = this.mSkyMapTexture;
      this.mSkyMapEffect.SourceDimensions = new Vector2()
      {
        X = (float) this.mSkyMapTexture.Width,
        Y = (float) this.mSkyMapTexture.Height
      };
      this.mSkyMapEffect.Color = this.mSkyMapColor;
      this.PostProcess(screenSize.X, screenSize.Y, (PostProcessingEffect) this.mSkyMapEffect);
    }
    currentScene.DrawAdditive(iDataChannel, iDeltaTime1, viewFrustum);
    ParticleSystem.Instance.Draw(iDataChannel, ref cameraState.ViewMatrix, ref cameraState.ProjectionMatrix, this.mDepthBuffer.GetTexture());
    this.mDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
    this.mDevice.SetRenderTarget(0, this.mSecondaryBuffer);
    this.mRestoreEffect.SourceTexture0 = this.mMainBuffer.GetTexture();
    this.PostProcess(screenSize.X, screenSize.Y, (PostProcessingEffect) this.mRestoreEffect);
    currentScene.DrawPost(iDataChannel, iDeltaTime1, ref new Vector2()
    {
      X = 1f / (float) screenSize.X,
      Y = 1f / (float) screenSize.Y
    }, ref cameraState.ViewMatrix, ref cameraState.ProjectionMatrix, this.mMainBuffer.GetTexture(), this.mDepthBuffer.GetTexture(), this.mNormalBuffer.GetTexture());
    this.mDevice.SetRenderTarget(0, this.mGUIBuffer);
    this.mDevice.DepthStencilBuffer = this.mGUIDepthStencil;
    this.mFilterEffect.SourceTexture0 = this.mSecondaryBuffer.GetTexture();
    this.mDevice.Clear(ClearOptions.DepthBuffer, new Color(), 1f, 0);
    this.PostProcess(screenSize.X, screenSize.Y, (PostProcessingEffect) this.mFilterEffect);
    currentScene.DrawGui(iDataChannel, iDeltaTime1);
    iPersistentScene?.DrawGui(iDataChannel, iDeltaTime1);
    TransitionEffect currentTransition = this.mCurrentTransition;
    if (currentTransition != null)
    {
      bool finished = currentTransition.Finished;
      currentTransition.Update(iDeltaTime1);
      this.mDevice.SetRenderTarget(0, (RenderTarget2D) null);
      currentTransition.SourceTexture0 = this.mGUIBuffer.GetTexture();
      this.mDevice.SetRenderTarget(0, this.mGUIBuffer);
      this.PostProcess(screenSize.X, screenSize.Y, (PostProcessingEffect) currentTransition);
      if (!finished & currentTransition.Finished)
        this.OnTransitionEnd(currentTransition);
    }
    this.mDevice.DepthStencilBuffer = depthStencilBuffer;
    this.Bloom();
    if (!this.mGetScreenShot)
      return;
    this.mDevice.ResolveBackBuffer(this.mScreenShot);
    this.mRestoreEffect.SourceTexture0 = (Texture2D) this.mScreenShot;
    this.PostProcess(screenSize.X, screenSize.Y, (PostProcessingEffect) this.mRestoreEffect);
    this.OnGetScreenshot();
  }

  public int RegisterEffect(Effect iEffect)
  {
    int hashCode = iEffect.GetType().GetHashCode();
    this.mRegisteredEffects[hashCode] = iEffect;
    return hashCode;
  }

  public void BeginTransition(Transitions iTransition, Color iColor, float iTransitionTime)
  {
    if (iTransition == Transitions.None)
    {
      this.mCurrentTransition = (TransitionEffect) null;
    }
    else
    {
      TransitionEffect mTransition = this.mTransitions[(int) iTransition];
      mTransition.Color = iColor;
      this.BeginTransition(mTransition, iTransitionTime);
    }
  }

  public void BeginTransition(TransitionEffect iTransitionEffect, float iTransitionTime)
  {
    iTransitionEffect.Start(TransitionEffect.TransitionDirections.Out, iTransitionTime);
    this.mCurrentTransition = iTransitionEffect;
  }

  public void EndTransition(Transitions iTransition, Color iColor, float iTransitionTime)
  {
    if (iTransition == Transitions.None)
    {
      this.mCurrentTransition = (TransitionEffect) null;
    }
    else
    {
      TransitionEffect mTransition = this.mTransitions[(int) iTransition];
      mTransition.Color = iColor;
      this.EndTransition(mTransition, iTransitionTime);
    }
  }

  public void EndTransition(TransitionEffect iTransitionEffect, float iTransitionTime)
  {
    iTransitionEffect.Start(TransitionEffect.TransitionDirections.In, iTransitionTime);
    this.mCurrentTransition = iTransitionEffect;
  }

  private void OnTransitionEnd(TransitionEffect iDeadTransition)
  {
    if (iDeadTransition.TransitionDirection == TransitionEffect.TransitionDirections.In)
      this.mCurrentTransition = (TransitionEffect) null;
    if (this.TransitionEnd == null)
      return;
    this.TransitionEnd(iDeadTransition);
  }

  public float TimeModifier
  {
    get => this.mTimeModifier;
    set => this.mTimeModifier = value;
  }

  public void CreateTransformPixelsToScreen(out Matrix oTransform)
  {
    this.CreateTransformPixelsToScreen((float) this.ScreenSize.X, (float) this.ScreenSize.Y, out oTransform);
  }

  public void CreateTransformPixelsToScreen(float iWidth, float iHeight, out Matrix oTransform)
  {
    oTransform = new Matrix();
    oTransform.M41 = (float) (-1.0 - 1.0 / (double) iWidth);
    oTransform.M42 = (float) (1.0 + 1.0 / (double) iHeight);
    oTransform.M11 = 2f / iWidth;
    oTransform.M22 = -2f / iHeight;
    oTransform.M44 = 1f;
  }

  public Texture2D DepthMap => this.mLatestDepthMap;

  private void Bloom()
  {
    this.mDevice.SetRenderTarget(0, this.mSixteenthBufferA);
    this.mBloomEffect.SourceTexture0 = this.mGUIBuffer.GetTexture();
    this.mBloomEffect.Threshold = this.mBloomThreshold;
    this.mBloomEffect.SourceDimensions = new Vector2()
    {
      X = (float) this.mGUIBuffer.Width,
      Y = (float) this.mGUIBuffer.Height
    };
    this.mBloomEffect.SetTechnique(BloomEffect.Technique.DownSample);
    this.PostProcess(this.mSixteenthBufferA.Width, this.mSixteenthBufferA.Height, (PostProcessingEffect) this.mBloomEffect);
    this.mDevice.SetRenderTarget(0, this.mSixteenthBufferB);
    this.mBloomEffect.SourceTexture0 = this.mSixteenthBufferA.GetTexture();
    this.mBloomEffect.Threshold = this.mBloomThreshold;
    this.mBloomEffect.Sigma = this.mBlurSigma;
    this.mBloomEffect.SetTechnique(BloomEffect.Technique.HorizontalBlur);
    this.PostProcess(this.mSixteenthBufferB.Width, this.mSixteenthBufferB.Height, (PostProcessingEffect) this.mBloomEffect);
    this.mDevice.SetRenderTarget(0, this.mSixteenthBufferA);
    this.mBloomEffect.SourceTexture0 = this.mSixteenthBufferB.GetTexture();
    this.mBloomEffect.Threshold = 0.0f;
    this.mBloomEffect.SetTechnique(BloomEffect.Technique.VerticalBlur);
    this.PostProcess(this.mSixteenthBufferA.Width, this.mSixteenthBufferA.Height, (PostProcessingEffect) this.mBloomEffect);
    this.mDevice.SetRenderTarget(0, (RenderTarget2D) null);
    this.mBloomEffect.SourceTexture0 = this.mGUIBuffer.GetTexture();
    this.mBloomEffect.SourceTexture1 = this.mSixteenthBufferA.GetTexture();
    this.mBloomEffect.Multiplier = this.mBloomMultiplier;
    this.mBloomEffect.SetTechnique(BloomEffect.Technique.Combine);
    this.mDevice.Viewport = new Viewport()
    {
      Width = this.mMainBuffer.Width,
      Height = this.mMainBuffer.Height
    };
    this.PostProcess(this.mMainBuffer.Width, this.mMainBuffer.Height, (PostProcessingEffect) this.mBloomEffect);
  }

  internal void PostProcess(int iTargetWidth, int iTargetHeight, PostProcessingEffect effect)
  {
    effect.DestinationDimensions = new Vector2()
    {
      X = (float) iTargetWidth,
      Y = (float) iTargetHeight
    };
    this.mDevice.VertexDeclaration = effect.VertexDeclaration;
    this.mDevice.Vertices[0].SetSource(effect.VertexBuffer, 0, effect.VertexDeclaration.GetVertexStrideSize(0));
    effect.Begin();
    effect.CurrentTechnique.Passes[0].Begin();
    this.mDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
    effect.CurrentTechnique.Passes[0].End();
    effect.End();
  }

  public bool SupportsVertexTexturing(SurfaceFormat iSurfaceFormat)
  {
    return this.mDevice.CreationParameters.Adapter.CheckDeviceFormat(this.mDevice.GraphicsDeviceCapabilities.DeviceType, this.mDevice.DisplayMode.Format, TextureUsage.None, QueryUsages.VertexTexture, ResourceType.Texture2D, iSurfaceFormat);
  }

  public Texture2D GetScreenShot(Action iCallback)
  {
    lock (this.mScreenShot)
    {
      this.mGetScreenShot = true;
      this.mGetScreenShotComplete += iCallback;
      return (Texture2D) this.mScreenShot;
    }
  }

  private void OnGetScreenshot()
  {
    lock (this.mScreenShot)
    {
      if (this.mGetScreenShotComplete != null)
        this.mGetScreenShotComplete();
      this.mGetScreenShotComplete = (Action) null;
      this.mGetScreenShot = false;
    }
  }

  private void CreateBackBuffers()
  {
    Point screenSize = this.ScreenSize;
    lock (this.mDevice)
    {
      this.mRecreateBuffers = false;
      if (this.mDepthBuffer != null)
        this.mDepthBuffer.Dispose();
      if (this.mNormalBuffer != null)
        this.mNormalBuffer.Dispose();
      if (this.mMainBuffer != null)
        this.mMainBuffer.Dispose();
      if (this.mSecondaryBuffer != null)
        this.mSecondaryBuffer.Dispose();
      if (this.mGUIBuffer != null)
        this.mGUIBuffer.Dispose();
      if (this.mGUIDepthStencil != null)
        this.mGUIDepthStencil.Dispose();
      if (this.mHalfBuffer != null)
        this.mHalfBuffer.Dispose();
      if (this.mQuarterBuffer != null)
        this.mQuarterBuffer.Dispose();
      if (this.mEighthBuffer != null)
        this.mEighthBuffer.Dispose();
      if (this.mSixteenthBufferA != null)
        this.mSixteenthBufferA.Dispose();
      if (this.mSixteenthBufferB != null)
        this.mSixteenthBufferB.Dispose();
      if (this.mScreenShot != null)
        this.mScreenShot.Dispose();
      this.mMainBuffer = new RenderTarget2D(this.mDevice, screenSize.X, screenSize.Y, 1, SurfaceFormat.Color, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents);
      this.mSecondaryBuffer = new RenderTarget2D(this.mDevice, screenSize.X, screenSize.Y, 1, SurfaceFormat.Color, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents);
      int qualityLevels;
      if (this.mDevice.CreationParameters.Adapter.CheckDeviceMultiSampleType(DeviceType.Hardware, SurfaceFormat.Color, false, MultiSampleType.FourSamples, out qualityLevels))
      {
        this.mGUIBuffer = new RenderTarget2D(this.mDevice, screenSize.X, screenSize.Y, 1, SurfaceFormat.Color, MultiSampleType.FourSamples, qualityLevels - 1, RenderTargetUsage.DiscardContents);
        this.mGUIDepthStencil = new DepthStencilBuffer(this.mDevice, screenSize.X, screenSize.Y, DepthFormat.Depth24Stencil8, MultiSampleType.FourSamples, qualityLevels - 1);
      }
      else
      {
        this.mGUIBuffer = new RenderTarget2D(this.mDevice, screenSize.X, screenSize.Y, 1, SurfaceFormat.Color, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents);
        this.mGUIDepthStencil = new DepthStencilBuffer(this.mDevice, screenSize.X, screenSize.Y, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);
      }
      this.mDepthBuffer = new RenderTarget2D(this.mDevice, screenSize.X, screenSize.Y, 1, SurfaceFormat.Single, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents);
      this.mNormalBuffer = new RenderTarget2D(this.mDevice, screenSize.X, screenSize.Y, 1, SurfaceFormat.Color, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents);
      this.mSixteenthBufferA = new RenderTarget2D(this.mDevice, screenSize.X / 16 /*0x10*/, screenSize.Y / 16 /*0x10*/, 1, SurfaceFormat.Color);
      this.mSixteenthBufferB = new RenderTarget2D(this.mDevice, screenSize.X / 16 /*0x10*/, screenSize.Y / 16 /*0x10*/, 1, SurfaceFormat.Color);
      this.mScreenShot = new ResolveTexture2D(this.mDevice, screenSize.X, screenSize.Y, 1, SurfaceFormat.Color);
    }
    if (this.ResulutionChanged == null)
      return;
    this.ResulutionChanged(this.mDevice, screenSize);
  }

  public float GUIScale => this.mGUIScale;

  public Color BackgroundColor
  {
    get => this.mColor;
    set => this.mColor = value;
  }

  public Point ScreenSize => this.mSize;

  public Vector2 CenterScreen
  {
    get => new Vector2((float) this.ScreenSize.X * 0.5f, (float) this.ScreenSize.Y * 0.5f);
  }

  public DummyEffect GlobalDummyEffect => this.mGlobalDummy;

  public DummyEffect LocalDummyEffect
  {
    get => this.mLocalDummy;
    set => this.mLocalDummy = value;
  }

  public Effect GetEffect(int iHash)
  {
    Effect effect;
    return this.mRegisteredEffects.TryGetValue(iHash, out effect) ? effect : (Effect) null;
  }

  public Fog Fog
  {
    get => this.mDeferredShadingEffect.Fog;
    set => this.mDeferredShadingEffect.Fog = value;
  }

  public float Brightness
  {
    get => this.mFilterEffect.Brightness;
    set => this.mFilterEffect.Brightness = value;
  }

  public float Contrast
  {
    get => this.mFilterEffect.Contrast;
    set => this.mFilterEffect.Contrast = value;
  }

  public float Saturation
  {
    get => this.mFilterEffect.Saturation;
    set => this.mFilterEffect.Saturation = value;
  }

  public GraphicsDevice GraphicsDevice => this.mDevice;

  public TransitionEffect GetTransitionEffect(Transitions transitions)
  {
    return this.mTransitions[(int) transitions];
  }

  public float BloomThreshold
  {
    get => this.mBloomThreshold;
    set => this.mBloomThreshold = value;
  }

  public float BloomMultiplier
  {
    get => this.mBloomMultiplier;
    set => this.mBloomMultiplier = value;
  }

  public float BlurSigma
  {
    get => this.mBlurSigma;
    set => this.mBlurSigma = value;
  }

  public Texture2D SkyMap
  {
    get => this.mSkyMapTexture;
    set => this.mSkyMapTexture = value;
  }

  public Vector3 SkyMapColor
  {
    get => this.mSkyMapColor;
    set => this.mSkyMapColor = value;
  }

  public float TextureLODBias
  {
    get => this.mTextureLODBias;
    set => this.mTextureLODBias = value;
  }

  public bool IsTransitionActive => this.mCurrentTransition != null;

  public VertexDeclaration CreateVertexDeclaration(VertexElement[] iVertexElements)
  {
    lock (this.mVertexDeclarations)
    {
      for (int index = 0; index < this.mVertexDeclarations.Count; ++index)
      {
        if (this.CompareVertexElementArray(this.mVertexDeclarations[index].GetVertexElements(), iVertexElements))
          return this.mVertexDeclarations[index];
      }
      VertexDeclaration vertexDeclaration;
      lock (this.mDevice)
        vertexDeclaration = new VertexDeclaration(this.mDevice, iVertexElements);
      this.mVertexDeclarations.Add(vertexDeclaration);
      return vertexDeclaration;
    }
  }

  private bool CompareVertexElementArray(VertexElement[] iA, VertexElement[] iB)
  {
    if (iA.Length != iB.Length)
      return false;
    for (int index = 0; index < iA.Length; ++index)
    {
      if (iA[index] != iB[index])
        return false;
    }
    return true;
  }
}
