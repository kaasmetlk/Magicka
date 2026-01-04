// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.TitleRenderer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Threading;

#nullable disable
namespace Magicka.GameLogic;

public class TitleRenderer
{
  private TitleRenderData[] mRenderData;
  private GUIBasicEffect mEffect;
  private float mFadeInTime;
  private float mFadeOutTime;
  private float mFadeInTimeDivisor;
  private float mFadeOutTimeDivisor;
  private float mTitleDisplayTime;
  private float mTimer;
  private int mBufferDepth = 3;
  private MagickaFont mTitleFont;
  private MagickaFont mSubtitleFont;
  private TextAlign mTextAlignment = TextAlign.Center;
  private bool mFadeIn;
  private bool mStarted;
  private bool mStopped;

  public TitleRenderer()
    : this(MagickaFont.Stonecross36, MagickaFont.Stonecross28)
  {
  }

  public TitleRenderer(TextAlign iTextAlignment)
    : this(MagickaFont.Stonecross36, MagickaFont.Stonecross28, 3, iTextAlignment)
  {
  }

  public TitleRenderer(MagickaFont iTitleFont, MagickaFont iSubtitleFont)
    : this(iTitleFont, iSubtitleFont, 3)
  {
  }

  public TitleRenderer(MagickaFont iTitleFont, MagickaFont iSubtitleFont, int iBufferDepth)
    : this(iTitleFont, iSubtitleFont, iBufferDepth, TextAlign.Center)
  {
  }

  public TitleRenderer(
    MagickaFont iTitleFont,
    MagickaFont iSubtitleFont,
    int iBufferDepth,
    TextAlign iTextAlignment)
  {
    this.mTitleFont = iTitleFont;
    this.mSubtitleFont = iSubtitleFont;
    this.mTextAlignment = iTextAlignment;
    this.mBufferDepth = iBufferDepth;
    lock (Game.Instance.GraphicsDevice)
      this.mEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, (EffectPool) null);
    this.Initialize();
  }

  public void Clear()
  {
    if (this.mRenderData == null)
      return;
    for (int index = 0; index < this.mRenderData.Length; ++index)
      this.mRenderData[index] = (TitleRenderData) null;
    GC.Collect();
    Thread.Sleep(0);
  }

  public void Initialize()
  {
    this.mRenderData = new TitleRenderData[this.mBufferDepth];
    for (int index = 0; index < this.mBufferDepth; ++index)
      this.mRenderData[index] = new TitleRenderData(this.mEffect, this.mTitleFont, this.mSubtitleFont, this.mTextAlignment);
    this.mTimer = 0.0f;
    this.mStarted = false;
  }

  public void SetTitles(
    string iTitle,
    string iSubtitle,
    float iDisplayTime,
    float iFadeIn,
    float iFadeOut)
  {
    this.mFadeInTime = Math.Max(iFadeIn, 0.01f);
    this.mFadeOutTime = Math.Max(iFadeOut, 0.01f);
    this.mFadeInTimeDivisor = 1f / this.mFadeInTime;
    this.mFadeOutTimeDivisor = 1f / this.mFadeOutTime;
    this.mTitleDisplayTime = iDisplayTime;
    for (int index = 0; index < this.mBufferDepth; ++index)
      this.mRenderData[index].SetText(iTitle, iSubtitle, this.mRenderData[index].TitleFont, this.mRenderData[index].SubtitleFont);
    this.mFadeIn = true;
  }

  public void Start()
  {
    this.mFadeIn = true;
    this.mStarted = true;
    this.mStopped = false;
    this.mTimer = 0.0f;
  }

  public void Stop()
  {
    this.mFadeIn = true;
    this.mStarted = false;
    this.mStopped = true;
    this.mTimer = 0.0f;
  }

  public void ResetTimer()
  {
    this.mFadeIn = true;
    this.mTimer = 0.0f;
  }

  public TitleRenderData Update(int depth, float iDeltaTime)
  {
    if (this.mStopped)
      return (TitleRenderData) null;
    if (!this.mStarted)
      return (TitleRenderData) null;
    if (this.mFadeIn)
    {
      this.mTimer += iDeltaTime * this.mFadeInTimeDivisor;
      if ((double) this.mTimer > (double) this.mTitleDisplayTime + (double) this.mFadeInTime)
      {
        this.mFadeIn = false;
        this.mTimer = 1f;
      }
    }
    else
    {
      this.mTimer -= iDeltaTime * this.mFadeOutTimeDivisor;
      if ((double) this.mTimer <= 0.0)
        this.mStopped = true;
    }
    TitleRenderData titleRenderData = this.mRenderData[depth < 0 ? 0 : (depth > this.mRenderData.Length - 1 ? this.mRenderData.Length - 1 : depth)];
    titleRenderData.Alpha = Math.Min(this.mTimer, 1f);
    return titleRenderData;
  }
}
