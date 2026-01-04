// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.TitleRenderData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.GameLogic;

public sealed class TitleRenderData : IRenderableGUIObject
{
  private GUIBasicEffect mGUIBasicEffect;
  private string mTitle;
  private string mSubtitle;
  private Text mTitleText;
  private Text mSubtitleText;
  private BitmapFont mTitleFont;
  private BitmapFont mSubtitleFont;
  private bool mIsDirty;
  private float mTitleHeight;
  public float Alpha;

  public BitmapFont TitleFont => this.mTitleFont;

  public BitmapFont SubtitleFont => this.mSubtitleFont;

  public bool DrawShadows
  {
    set
    {
      if (this.mTitleText != null)
        this.mTitleText.DrawShadows = value;
      if (this.mSubtitleText == null)
        return;
      this.mSubtitleText.DrawShadows = value;
    }
  }

  public float ShadowAlpha
  {
    set
    {
      if (this.mTitleText != null)
        this.mTitleText.ShadowAlpha = value;
      if (this.mSubtitleText == null)
        return;
      this.mSubtitleText.ShadowAlpha = value;
    }
  }

  public Vector2 TitleShadowOffset
  {
    set
    {
      if (this.mTitleText == null)
        return;
      this.mTitleText.ShadowsOffset = value;
    }
  }

  public Vector2 SubtitleShadowOffset
  {
    set
    {
      if (this.mSubtitleText == null)
        return;
      this.mSubtitleText.ShadowsOffset = value;
    }
  }

  public Vector2 ShadowOffset
  {
    set => this.TitleShadowOffset = this.SubtitleShadowOffset = value;
  }

  public TitleRenderData(
    GUIBasicEffect iGUIBasicEffect,
    MagickaFont iTitleFont,
    MagickaFont iSubtitleFont)
    : this(iGUIBasicEffect, iTitleFont, iSubtitleFont, TextAlign.Center)
  {
  }

  public TitleRenderData(
    GUIBasicEffect iGUIBasicEffect,
    MagickaFont iTitleFont,
    MagickaFont iSubtitleFont,
    TextAlign iTextAlignment)
  {
    this.mGUIBasicEffect = iGUIBasicEffect;
    this.mTitleFont = FontManager.Instance.GetFont(iTitleFont);
    this.mSubtitleFont = FontManager.Instance.GetFont(iSubtitleFont);
    this.mTitleText = new Text(100, this.mTitleFont, iTextAlignment, false);
    this.mTitleText.DrawShadows = true;
    this.mTitleText.ShadowsOffset = new Vector2(2f, 2f);
    this.mTitleText.ShadowAlpha = 1f;
    this.mSubtitleText = new Text(100, this.mSubtitleFont, iTextAlignment, false);
    this.mSubtitleText.DrawShadows = true;
    this.mSubtitleText.ShadowsOffset = new Vector2(2f, 2f);
    this.mSubtitleText.ShadowAlpha = 1f;
  }

  public void SetText(
    string iTitle,
    string iSubtitle,
    BitmapFont iTitleFont,
    BitmapFont iSubtitleFont)
  {
    if (iTitleFont != null)
    {
      this.mTitleFont = iTitleFont;
      this.mTitleText.Font = iTitleFont;
    }
    if (iSubtitleFont != null)
    {
      this.mSubtitleFont = iSubtitleFont;
      this.mSubtitleText.Font = iSubtitleFont;
    }
    this.mTitleHeight = this.mTitleFont.MeasureText(iTitle, true).Y;
    this.mTitle = iTitle;
    this.mSubtitle = iSubtitle;
    this.mIsDirty = true;
  }

  public void Draw(float iDeltaTime)
  {
    if (this.mIsDirty)
    {
      this.mTitleText.SetText(this.mTitle);
      this.mSubtitleText.SetText(this.mSubtitle);
      this.mIsDirty = false;
    }
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
    this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mGUIBasicEffect.TextureEnabled = true;
    this.mGUIBasicEffect.Begin();
    this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
    if (!string.IsNullOrEmpty(this.mTitle))
      this.mTitleText.Draw(this.mGUIBasicEffect, (float) ((double) screenSize.X * 0.5 + 0.5), (float) ((double) screenSize.Y * 0.30000001192092896 + 0.5));
    if (!string.IsNullOrEmpty(this.mSubtitle))
      this.mSubtitleText.Draw(this.mGUIBasicEffect, (float) ((double) screenSize.X * 0.5 + 0.5), (float) ((double) screenSize.Y * 0.30000001192092896 + 0.5) + this.mTitleHeight);
    this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
    this.mGUIBasicEffect.End();
  }

  public int ZIndex => 205;
}
