// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.GenericHealthBar
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

public class GenericHealthBar
{
  public const int HEALTHBARSIDESIZE = 96 /*0x60*/;
  public const float COUNTER_MIN = 0.0f;
  public const float COUNTER_MAX = 1f;
  private GUIBasicEffect mEffect;
  private GUIBasicEffect mDisplayNameEffect;
  private VertexBuffer mVertexBuffer;
  private VertexDeclaration mVertexDeclaration;
  private VertexPositionTexture[] mVertices = new VertexPositionTexture[20];
  private Scene mScene;
  private GenericHealthBar.RenderData[] mRenderData;
  private Vector2[] mBarTextureOffsets;
  private bool mActive;
  private bool mRemove;
  private float mAlpha;
  private float mPower;
  private float mNormalizedHealth;
  private float mDisplayHealth;
  private Texture2D mTexture;
  private Texture3D mAnimatedTexture;
  private GenericHealthBarTypes mType;
  private GenericHealthBarGraphics mGraphicsType;
  private GenericHealthBarPosition mBarPosition;
  private float mStartTime;
  private float mEndTime;
  private float mTTL;
  private float mInitialTimerDelay;
  private bool mTimeTicking;
  private bool mIsDone;
  private float mCounterStart;
  private float mCounterEnd;
  private float mCounterCurrent;
  private float mAnimationTimer;
  private float mDepthDivisor;
  private float mDepth;
  private float mHealthBarWidth = 0.8f;
  private string mDisplayName = "";
  private bool mShowDisplayName;
  private bool mIsPaused = true;
  private bool mIsScaled;
  private bool mIsColoredRed;
  private bool mHasAnimatedSprite;
  private float mAnimationSpriteOffsetY;
  private float mFadeTime;
  private float mCurrentFadeTime = 0.01f;
  private int mOnEndTriggerID;
  private bool mOnEndTriggerd;
  private float mDynamiteBarOffset = 40f;
  private static Random sRandom = new Random();
  private float mTest;

  public GenericHealthBar(Scene iScene)
  {
    this.mScene = iScene;
    this.mOnEndTriggerd = true;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      this.mEffect = new GUIBasicEffect(graphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
      this.mTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
      this.mEffect.Texture = (Texture) this.mTexture;
      this.mAnimatedTexture = Magicka.Game.Instance.Content.Load<Texture3D>("UI/HUD/counteranimation");
      this.mDisplayNameEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    }
    this.mDepth = (float) this.mAnimatedTexture.Depth;
    this.mDepthDivisor = 1f / this.mDepth;
    this.mDepth = this.mDepthDivisor * 0.5f;
    this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
    this.mEffect.TextureEnabled = true;
    this.mRenderData = new GenericHealthBar.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      GenericHealthBar.RenderData renderData = new GenericHealthBar.RenderData();
      this.mRenderData[index] = renderData;
      renderData.SetupDisplayName(MagickaFont.Maiandra14);
      renderData.mEffect = this.mEffect;
      renderData.SetText(this.mDisplayName);
      renderData.mDisplayNameEffect = this.mDisplayNameEffect;
    }
    this.mVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * this.mVertices.Length, BufferUsage.WriteOnly);
    this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    this.mBarTextureOffsets = new Vector2[3];
    this.CreateVertices(this.mHealthBarWidth, (float) this.mTexture.Width, (float) this.mTexture.Height);
    this.CreateAnimatedVertices();
    this.Reset();
  }

  public Scene Scene
  {
    get => this.mScene;
    set => this.mScene = value;
  }

  public void Reset()
  {
    this.mAlpha = 0.0f;
    this.mPower = 1f;
    this.mDisplayHealth = 0.0f;
    this.mNormalizedHealth = 1f;
    this.mActive = false;
    this.mRemove = false;
    this.mTimeTicking = false;
    this.mIsDone = false;
    this.mEndTime = 0.0f;
    this.mTTL = 0.0f;
    this.mInitialTimerDelay = 0.0f;
    if (this.mGraphicsType != GenericHealthBarGraphics.Dynamite)
      return;
    for (int index = 0; index < this.mRenderData.Length; ++index)
    {
      this.mRenderData[index].mHealthBarPosition -= this.mDynamiteBarOffset;
      this.mRenderData[index].mIsTicking = this.mTimeTicking;
      this.mRenderData[index].mAlpha = this.mAlpha;
      this.mRenderData[index].mNormalizedHealth = this.mNormalizedHealth;
      this.mRenderData[index].mIsDone = this.mIsDone;
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (!this.mActive)
      return;
    if (this.mIsDone)
      this.OnEndTrigger();
    if ((double) this.mInitialTimerDelay > 0.0 && !this.mIsPaused)
    {
      this.mInitialTimerDelay -= iDeltaTime;
      if ((double) this.mInitialTimerDelay <= 0.0)
        this.TimeTicking = true;
    }
    if ((double) this.mInitialTimerDelay <= 0.0 && (double) this.mCurrentFadeTime >= (double) this.mFadeTime)
    {
      this.mTest += iDeltaTime;
      if ((double) this.mTest >= 4.0)
      {
        this.mTest = 0.0f;
        float num = (float) GenericHealthBar.sRandom.Next(15);
        if (this.mType == GenericHealthBarTypes.Counter)
          this.mCounterCurrent += num;
        if (this.mType == GenericHealthBarTypes.HP)
          this.mCounterCurrent -= num;
      }
    }
    if (this.mType == GenericHealthBarTypes.TimerDecreasing)
    {
      if (this.mTimeTicking && !this.mIsPaused)
      {
        this.mTTL -= iDeltaTime;
        if ((double) this.mTTL <= (double) this.mEndTime)
          this.IsDone = true;
      }
      this.mDisplayHealth = Math.Max(this.mTTL / this.mStartTime, 0.0f);
    }
    else if (this.mType == GenericHealthBarTypes.TimerIncreasing)
    {
      if (this.mTimeTicking && !this.mIsPaused)
      {
        this.mTTL += iDeltaTime;
        if ((double) this.mTTL >= (double) this.mEndTime)
          this.IsDone = true;
      }
      this.mDisplayHealth = Math.Min(this.mTTL / this.mEndTime, 1f);
    }
    else if (this.mType == GenericHealthBarTypes.Counter)
    {
      this.mNormalizedHealth = Math.Min(this.mCounterCurrent / this.mCounterEnd, 1f);
      if ((double) this.mCounterCurrent >= (double) this.mCounterEnd)
        this.IsDone = true;
    }
    else
    {
      this.mNormalizedHealth = Math.Max(this.mCounterCurrent / this.mCounterStart, 0.0f);
      if ((double) this.mCounterCurrent <= (double) this.mCounterEnd)
        this.IsDone = true;
    }
    GenericHealthBar.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mCurrentFadeTime += iDeltaTime;
    this.mAlpha = !this.mRemove ? Math.Min(this.mCurrentFadeTime / this.mFadeTime, 1f) : Math.Max((float) (1.0 - (double) this.mCurrentFadeTime / (double) this.mFadeTime), 0.0f);
    if (((double) this.mAlpha > 0.99000000953674316 || this.mRemove) && this.mType == GenericHealthBarTypes.Counter | this.mType == GenericHealthBarTypes.HP)
      this.mDisplayHealth += (float) (((double) this.mNormalizedHealth - (double) this.mDisplayHealth) * 10.0) * iDeltaTime;
    iObject.mAlpha = this.mAlpha;
    iObject.mNormalizedHealth = this.mDisplayHealth;
    iObject.mHealthColor = new Vector3(this.mPower, 0.0f, 0.0f);
    if (this.mShowDisplayName)
      iObject.mAlphaDisplayName = this.mAlpha;
    if (this.mHasAnimatedSprite)
    {
      if (!this.mIsPaused)
      {
        this.mAnimationTimer += iDeltaTime;
        if ((double) this.mAnimationTimer > (double) this.mDepthDivisor / 2.25)
        {
          this.mDepth += this.mDepthDivisor;
          this.mAnimationTimer -= this.mDepthDivisor / 2.25f;
        }
      }
      iObject.Saturate = !this.mActive;
      iObject.AnimationSpriteDepth = this.mDepth;
      iObject.AnimationSpritePosition.X = (float) ((double) (RenderManager.Instance.ScreenSize.X / 2) - (double) this.mHealthBarWidth * (double) RenderManager.Instance.ScreenSize.X / 2.0 + (double) this.mDisplayHealth * ((double) this.mHealthBarWidth * (double) RenderManager.Instance.ScreenSize.X - 40.0 * (double) this.mHealthBarWidth) + 40.0 * (double) this.mHealthBarWidth + 32.0);
      iObject.AnimationSpritePosition.Y = this.mAnimationSpriteOffsetY;
    }
    this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public void SetWidth(float iHealthbarWidth)
  {
    this.CreateVertices(iHealthbarWidth, (float) this.mTexture.Width, (float) this.mTexture.Height);
  }

  protected void CreateVertices(float iHealthbarWidth, float iTextureWidth, float iTextureHeight)
  {
    int width = GlobalSettings.Instance.Resolution.Width;
    int num1 = width / 2;
    int num2 = (int) ((double) iHealthbarWidth * (double) width);
    int num3 = num2 / 2;
    this.mVertices[0].Position.X = (float) (num1 - num3);
    this.mVertices[0].Position.Y = 32f;
    this.mVertices[0].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[0].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[1].Position.X = (float) (num1 - num3);
    this.mVertices[1].Position.Y = 8f;
    this.mVertices[1].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[1].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[2].Position.X = (float) (num1 - num3 + 96 /*0x60*/);
    this.mVertices[2].Position.Y = 32f;
    this.mVertices[2].TextureCoordinate.X = 96f / iTextureWidth;
    this.mVertices[2].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[3].Position.X = (float) (num1 - num3 + 96 /*0x60*/);
    this.mVertices[3].Position.Y = 8f;
    this.mVertices[3].TextureCoordinate.X = 96f / iTextureWidth;
    this.mVertices[3].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[4].Position.X = (float) (num1 + num3 - 96 /*0x60*/);
    this.mVertices[4].Position.Y = 32f;
    this.mVertices[4].TextureCoordinate.X = 160f / iTextureWidth;
    this.mVertices[4].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[5].Position.X = (float) (num1 + num3 - 96 /*0x60*/);
    this.mVertices[5].Position.Y = 8f;
    this.mVertices[5].TextureCoordinate.X = 160f / iTextureWidth;
    this.mVertices[5].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[6].Position.X = (float) (num1 + num3);
    this.mVertices[6].Position.Y = 32f;
    this.mVertices[6].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[6].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[7].Position.X = (float) (num1 + num3);
    this.mVertices[7].Position.Y = 8f;
    this.mVertices[7].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[7].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[8].Position.X = 0.0f;
    this.mVertices[8].Position.Y = 32f;
    this.mVertices[8].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[8].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[9].Position.X = 0.0f;
    this.mVertices[9].Position.Y = 8f;
    this.mVertices[9].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[9].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[10].Position.X = (float) (num2 - 32 /*0x20*/);
    this.mVertices[10].Position.Y = 32f;
    this.mVertices[10].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[10].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[11].Position.X = (float) (num2 - 32 /*0x20*/);
    this.mVertices[11].Position.Y = 8f;
    this.mVertices[11].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[11].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[12].Position.X = (float) (num1 - num3);
    this.mVertices[12].Position.Y = 32f;
    this.mVertices[12].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[12].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[13].Position.X = (float) (num1 - num3);
    this.mVertices[13].Position.Y = 8f;
    this.mVertices[13].TextureCoordinate.X = 0.0f / iTextureWidth;
    this.mVertices[13].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[14].Position.X = (float) (num1 - num3 + 96 /*0x60*/);
    this.mVertices[14].Position.Y = 32f;
    this.mVertices[14].TextureCoordinate.X = 96f / iTextureWidth;
    this.mVertices[14].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[15].Position.X = (float) (num1 - num3 + 96 /*0x60*/);
    this.mVertices[15].Position.Y = 8f;
    this.mVertices[15].TextureCoordinate.X = 96f / iTextureWidth;
    this.mVertices[15].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[16 /*0x10*/].Position.X = (float) (num1 + num3 - 96 /*0x60*/);
    this.mVertices[16 /*0x10*/].Position.Y = 32f;
    this.mVertices[16 /*0x10*/].TextureCoordinate.X = 160f / iTextureWidth;
    this.mVertices[16 /*0x10*/].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[17].Position.X = (float) (num1 + num3 - 96 /*0x60*/);
    this.mVertices[17].Position.Y = 8f;
    this.mVertices[17].TextureCoordinate.X = 160f / iTextureWidth;
    this.mVertices[17].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertices[18].Position.X = (float) (num1 + num3);
    this.mVertices[18].Position.Y = 32f;
    this.mVertices[18].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[18].TextureCoordinate.Y = 24f / iTextureHeight;
    this.mVertices[19].Position.X = (float) (num1 + num3);
    this.mVertices[19].Position.Y = 8f;
    this.mVertices[19].TextureCoordinate.X = 256f / iTextureWidth;
    this.mVertices[19].TextureCoordinate.Y = 0.0f / iTextureHeight;
    this.mVertexBuffer.SetData<VertexPositionTexture>(this.mVertices);
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].mVertices = this.mVertexBuffer;
      this.mRenderData[index].mVertexDeclaration = this.mVertexDeclaration;
      this.mRenderData[index].mHealthBarPosition = (float) (num1 - num3 + 16 /*0x10*/);
    }
  }

  public void CreateAnimatedVertices()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    VertexPositionTexture[] data = new VertexPositionTexture[4];
    data[0].Position = new Vector3(-32f, 32f, 0.0f);
    data[0].TextureCoordinate = new Vector2(0.0f, 1f);
    data[1].Position = new Vector3(-32f, -32f, 0.0f);
    data[1].TextureCoordinate = new Vector2(0.0f, 0.0f);
    data[2].Position = new Vector3(32f, -32f, 0.0f);
    data[2].TextureCoordinate = new Vector2(1f, 0.0f);
    data[3].Position = new Vector3(32f, 32f, 0.0f);
    data[3].TextureCoordinate = new Vector2(1f, 1f);
    VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.SizeInBytes * 4, BufferUsage.WriteOnly);
    lock (graphicsDevice)
      vertexBuffer.SetData<VertexPositionTexture>(data);
    VertexDeclaration vertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
    GUIBasicEffect guiBasicEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].mAnimationSpriteEffect = guiBasicEffect;
      this.mRenderData[index].mAnimationSpriteVertexBuffer = vertexBuffer;
      this.mRenderData[index].mAnimationSpriteVertexDeclaration = vertexDeclaration;
      this.mRenderData[index].mAnimationSpriteTexture = this.mAnimatedTexture;
    }
  }

  public float Alpha
  {
    get => this.mAlpha;
    set => this.mAlpha = value;
  }

  public float Power
  {
    get => this.mPower;
    set => this.mPower = value;
  }

  public void SetNormalizedHealth(float iPercent) => this.mNormalizedHealth = iPercent;

  public void Remove()
  {
    this.mRemove = true;
    this.TimeTicking = false;
    this.mCurrentFadeTime = 0.0f;
    this.HasAnimatedSprite = false;
  }

  public void Activate()
  {
    this.mActive = true;
    if (this.mType == GenericHealthBarTypes.TimerDecreasing | this.mType == GenericHealthBarTypes.TimerIncreasing && (double) this.mInitialTimerDelay <= 0.0)
      this.TimeTicking = true;
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].mHealthBarBackgroundTextureOffset = this.mBarTextureOffsets[0];
      this.mRenderData[index].mHealthBarTextureOffset = this.mBarTextureOffsets[1];
      this.mRenderData[index].mHealthBarOverlayTextureOffset = this.mBarTextureOffsets[2];
      if (this.mGraphicsType == GenericHealthBarGraphics.Dynamite)
        this.mRenderData[index].mHealthBarPosition += this.mDynamiteBarOffset;
    }
    this.mIsPaused = false;
  }

  public GenericHealthBarTypes Type
  {
    get => this.mType;
    set => this.mType = value;
  }

  public float NormalizedHealth
  {
    get => this.mNormalizedHealth;
    set => this.mNormalizedHealth = value;
  }

  public float DisplayHealth
  {
    get => this.mDisplayHealth;
    set => this.mDisplayHealth = value;
  }

  public float TTL
  {
    get => this.mTTL;
    set => this.mTTL = value;
  }

  public float StartTime
  {
    get => this.mStartTime;
    set => this.mStartTime = value;
  }

  public float EndTime
  {
    get => this.mEndTime;
    set => this.mEndTime = value;
  }

  public void SetupTimer(float iTime)
  {
    if (this.mType == GenericHealthBarTypes.TimerDecreasing)
    {
      this.mStartTime = iTime;
      this.mTTL = iTime;
      this.mEndTime = 0.0f;
    }
    else
    {
      this.mStartTime = 0.0f;
      this.mTTL = 0.0f;
      this.mEndTime = iTime;
    }
  }

  public float CounterCurrent
  {
    get => this.mCounterCurrent;
    set => this.mCounterCurrent = value;
  }

  public float CounterStart
  {
    get => this.mCounterStart;
    set => this.mCounterStart = value;
  }

  public float CounterEnd
  {
    get => this.mCounterEnd;
    set => this.mCounterEnd = value;
  }

  public void SetupCounter(float iCounterCount)
  {
    if (this.mType == GenericHealthBarTypes.Counter)
    {
      this.mCounterStart = 0.0f;
      this.mCounterCurrent = 0.0f;
      this.mCounterEnd = iCounterCount;
    }
    else
    {
      this.mCounterStart = iCounterCount;
      this.mCounterCurrent = iCounterCount;
      this.mCounterEnd = 0.0f;
    }
  }

  public bool IsDone
  {
    get => this.mIsDone;
    set
    {
      this.mIsDone = value;
      this.mRenderData[0].mIsDone = this.mIsDone;
      this.mRenderData[1].mIsDone = this.mIsDone;
      this.mRenderData[2].mIsDone = this.mIsDone;
    }
  }

  public GenericHealthBarPosition BarPosition
  {
    get => this.mBarPosition;
    set => this.mBarPosition = value;
  }

  public float InitialTimerDelay
  {
    get => this.mInitialTimerDelay;
    set => this.mInitialTimerDelay = value;
  }

  public bool IsPaused
  {
    get => this.mIsPaused;
    set => this.mIsPaused = value;
  }

  public string DisplayName
  {
    get => this.mDisplayName;
    set
    {
      this.mDisplayName = value;
      this.mShowDisplayName = true;
      for (int index = 0; index < 3; ++index)
      {
        this.mRenderData[index].SetText(this.mDisplayName);
        this.mRenderData[index].mShowDisplayName = true;
      }
    }
  }

  public bool ShowDisplayName
  {
    get => this.mShowDisplayName;
    set
    {
      this.mShowDisplayName = value;
      for (int index = 0; index < 3; ++index)
        this.mRenderData[index].mShowDisplayName = this.mShowDisplayName;
    }
  }

  public bool IsScaled
  {
    get => this.mIsScaled;
    set
    {
      this.mIsScaled = value;
      this.mRenderData[0].mIsScaled = this.mIsScaled;
      this.mRenderData[1].mIsScaled = this.mIsScaled;
      this.mRenderData[2].mIsScaled = this.mIsScaled;
    }
  }

  public bool IsColoredRed
  {
    get => this.mIsColoredRed;
    set
    {
      this.mIsColoredRed = value;
      this.mRenderData[0].mIsColored = this.mIsColoredRed;
      this.mRenderData[1].mIsColored = this.mIsColoredRed;
      this.mRenderData[2].mIsColored = this.mIsColoredRed;
    }
  }

  public GenericHealthBarGraphics GraphicsType
  {
    get => this.mGraphicsType;
    set
    {
      this.mGraphicsType = value;
      if (this.mGraphicsType == GenericHealthBarGraphics.Dynamite)
      {
        this.mBarTextureOffsets[0].X = 72f / (float) this.mTexture.Width;
        this.mBarTextureOffsets[0].Y = 392f / (float) this.mTexture.Height;
        this.mBarTextureOffsets[1].X = 128f / (float) this.mTexture.Width;
        this.mBarTextureOffsets[1].Y = 360f / (float) this.mTexture.Height;
        this.mBarTextureOffsets[2].X = 0.0f / (float) this.mTexture.Width;
        this.mBarTextureOffsets[2].Y = 450f / (float) this.mTexture.Height;
      }
      else
      {
        this.mBarTextureOffsets[0].X = 0.0f / (float) this.mTexture.Width;
        this.mBarTextureOffsets[0].Y = 48f / (float) this.mTexture.Height;
        this.mBarTextureOffsets[1].X = 128f / (float) this.mTexture.Width;
        this.mBarTextureOffsets[1].Y = 424f / (float) this.mTexture.Height;
        this.mBarTextureOffsets[2].X = 0.0f / (float) this.mTexture.Width;
        this.mBarTextureOffsets[2].Y = 24f / (float) this.mTexture.Height;
      }
    }
  }

  public bool HasAnimatedSprite
  {
    get => this.mHasAnimatedSprite;
    set
    {
      this.mHasAnimatedSprite = value;
      this.mRenderData[0].mHasAnimatedSprite = this.mHasAnimatedSprite;
      this.mRenderData[1].mHasAnimatedSprite = this.mHasAnimatedSprite;
      this.mRenderData[2].mHasAnimatedSprite = this.mHasAnimatedSprite;
    }
  }

  public bool TimeTicking
  {
    get => this.mTimeTicking;
    set
    {
      this.mTimeTicking = value;
      this.mRenderData[0].mIsTicking = this.mTimeTicking;
      this.mRenderData[1].mIsTicking = this.mTimeTicking;
      this.mRenderData[2].mIsTicking = this.mTimeTicking;
    }
  }

  public float AnimationSpriteOffsetY
  {
    get => this.mAnimationSpriteOffsetY;
    set => this.mAnimationSpriteOffsetY = value;
  }

  public float FadeTime
  {
    get => this.mFadeTime;
    set => this.mFadeTime = value;
  }

  public int OnEndTriggerID
  {
    get => this.mOnEndTriggerID;
    set
    {
      this.mOnEndTriggerID = value;
      if (this.mOnEndTriggerID == 0)
        return;
      this.mOnEndTriggerd = false;
    }
  }

  public void OnEndTrigger()
  {
    if (this.mOnEndTriggerd)
      return;
    this.mOnEndTriggerd = true;
    (GameStateManager.Instance.CurrentState as PlayState).Level.CurrentScene.ExecuteTrigger(this.mOnEndTriggerID, (Magicka.GameLogic.Entities.Character) null, false);
  }

  public void UpdateResolution()
  {
    this.CreateVertices(this.mHealthBarWidth, (float) this.mTexture.Width, (float) this.mTexture.Height);
    this.CreateAnimatedVertices();
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].mHealthBarBackgroundTextureOffset = this.mBarTextureOffsets[0];
      this.mRenderData[index].mHealthBarTextureOffset = this.mBarTextureOffsets[1];
      this.mRenderData[index].mHealthBarOverlayTextureOffset = this.mBarTextureOffsets[2];
      if (this.mGraphicsType == GenericHealthBarGraphics.Dynamite)
        this.mRenderData[index].mHealthBarPosition += this.mDynamiteBarOffset;
    }
  }

  protected class RenderData : IRenderableGUIObject
  {
    public GUIBasicEffect mEffect;
    public VertexBuffer mVertices;
    public VertexDeclaration mVertexDeclaration;
    public float mAlpha;
    public float mNormalizedHealth;
    public Vector3 mHealthColor;
    public float mHealthBarPosition;
    public Vector2 mHealthBarTextureOffset;
    public Vector2 mHealthBarBackgroundTextureOffset;
    public Vector2 mHealthBarOverlayTextureOffset;
    public Vector2 mHealthBarScale = new Vector2(1f, 1f);
    private Vector2 mHealthBarStandardScale;
    public bool mIsScaled;
    public bool mIsColored;
    public GUIBasicEffect mAnimationSpriteEffect;
    public VertexBuffer mAnimationSpriteVertexBuffer;
    public VertexDeclaration mAnimationSpriteVertexDeclaration;
    public Texture3D mAnimationSpriteTexture;
    public Vector2 AnimationSpritePosition;
    public float AnimationSpriteDepth;
    public bool Saturate;
    private Matrix mTransform = Matrix.Identity;
    private static readonly Vector4 COLOR = new Vector4(1f, 1f, 1f, 1f);
    private static readonly Vector4 SATURATED_COLOR = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    public bool mHasAnimatedSprite;
    public bool mIsTicking;
    public bool mIsDone;
    public GUIBasicEffect mDisplayNameEffect;
    private string mDisplayName;
    private Text mDisplayNameText;
    public bool mShowDisplayName;
    public BitmapFont mFont;
    public bool mDisplayNameIsDirty;
    public float mDisplayNameHeight;
    public float mAlphaDisplayName;

    public void SetText(string iDisplayName)
    {
      switch (iDisplayName)
      {
        case "":
          break;
        case null:
          break;
        default:
          this.mDisplayNameText.Font = this.mFont;
          this.mDisplayNameHeight = this.mFont.MeasureText(iDisplayName, true).Y;
          this.mDisplayName = iDisplayName;
          this.mDisplayNameIsDirty = true;
          break;
      }
    }

    public void SetupDisplayName(MagickaFont iFont)
    {
      this.mFont = FontManager.Instance.GetFont(iFont);
      this.mDisplayNameText = new Text(100, this.mFont, TextAlign.Center, false);
      this.mDisplayNameText.DrawShadows = true;
      this.mDisplayNameText.ShadowsOffset = new Vector2(2f, 2f);
      this.mDisplayNameText.ShadowAlpha = 1f;
    }

    public void Draw(float iDeltaTime)
    {
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, VertexPositionTexture.SizeInBytes);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
      this.mEffect.Begin();
      EffectPassCollection passes = this.mEffect.CurrentTechnique.Passes;
      for (int index = 0; index < passes.Count; ++index)
      {
        passes[index].Begin();
        this.mEffect.Transform = Matrix.Identity;
        Vector4 vector4 = new Vector4();
        vector4.X = 1f;
        vector4.Y = 1f;
        vector4.Z = 1f;
        vector4.W = this.mAlpha;
        this.mEffect.Color = vector4;
        this.mEffect.TextureOffset = this.mHealthBarBackgroundTextureOffset;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 6);
        if (this.mIsScaled)
          this.mHealthBarScale.X = this.mNormalizedHealth;
        this.mEffect.Transform = new Matrix()
        {
          M11 = this.mNormalizedHealth,
          M41 = this.mHealthBarPosition,
          M22 = 1f,
          M33 = 1f,
          M44 = 1f
        };
        this.mEffect.TextureOffset = this.mHealthBarTextureOffset;
        this.mHealthBarStandardScale = this.mEffect.TextureScale;
        this.mEffect.TextureScale = this.mHealthBarScale * this.mHealthBarStandardScale;
        if (this.mIsColored)
        {
          vector4.X = this.mHealthColor.X;
          vector4.Y = this.mHealthColor.Y;
          vector4.Z = this.mHealthColor.Z;
        }
        vector4.W = this.mAlpha;
        this.mEffect.Color = vector4;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 8, 2);
        this.mEffect.Transform = Matrix.Identity;
        vector4.X = 1f;
        vector4.Y = 1f;
        vector4.Z = 1f;
        vector4.W = this.mAlpha;
        this.mEffect.TextureOffset = this.mHealthBarOverlayTextureOffset;
        this.mEffect.TextureScale = this.mHealthBarStandardScale;
        this.mEffect.Color = vector4;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 12, 6);
        passes[index].End();
      }
      this.mEffect.End();
      if (this.mShowDisplayName)
      {
        if (this.mDisplayNameIsDirty)
        {
          this.mDisplayNameText.SetText(this.mDisplayName);
          this.mDisplayNameIsDirty = false;
        }
        Point screenSize = RenderManager.Instance.ScreenSize;
        this.mDisplayNameEffect.Color = new Vector4(1f, 1f, 1f, this.mAlphaDisplayName);
        this.mDisplayNameEffect.SetScreenSize(screenSize.X, screenSize.Y);
        this.mDisplayNameEffect.TextureEnabled = true;
        this.mDisplayNameEffect.Begin();
        this.mDisplayNameEffect.CurrentTechnique.Passes[0].Begin();
        this.mDisplayNameText.Draw(this.mDisplayNameEffect, (float) ((double) screenSize.X * 0.5 + 0.5), 8f);
        this.mDisplayNameEffect.CurrentTechnique.Passes[0].End();
        this.mDisplayNameEffect.End();
      }
      if (!this.mHasAnimatedSprite || !this.mIsTicking || this.mIsDone)
        return;
      Point screenSize1 = RenderManager.Instance.ScreenSize;
      this.mTransform.M41 = this.AnimationSpritePosition.X;
      this.mTransform.M42 = this.AnimationSpritePosition.Y;
      this.mAnimationSpriteEffect.Transform = this.mTransform;
      this.mAnimationSpriteEffect.SetScreenSize(screenSize1.X, screenSize1.Y);
      this.mAnimationSpriteEffect.SetTechnique(GUIBasicEffect.Technique.Texture3D);
      this.mAnimationSpriteEffect.W = this.AnimationSpriteDepth;
      this.mAnimationSpriteEffect.Color = this.Saturate ? GenericHealthBar.RenderData.SATURATED_COLOR : GenericHealthBar.RenderData.COLOR;
      this.mAnimationSpriteEffect.VertexColorEnabled = false;
      this.mAnimationSpriteEffect.TextureEnabled = true;
      this.mAnimationSpriteEffect.Texture = (Texture) this.mAnimationSpriteTexture;
      this.mAnimationSpriteEffect.TextureOffset = new Vector2();
      this.mAnimationSpriteEffect.GraphicsDevice.Vertices[0].SetSource(this.mAnimationSpriteVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      this.mAnimationSpriteEffect.GraphicsDevice.VertexDeclaration = this.mAnimationSpriteVertexDeclaration;
      this.mAnimationSpriteEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
      this.mAnimationSpriteEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
      this.mAnimationSpriteEffect.Begin();
      this.mAnimationSpriteEffect.CurrentTechnique.Passes[0].Begin();
      this.mAnimationSpriteEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      this.mAnimationSpriteEffect.CurrentTechnique.Passes[0].End();
      this.mAnimationSpriteEffect.End();
      this.mAnimationSpriteEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
      this.mAnimationSpriteEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
    }

    public int ZIndex => 200;
  }
}
