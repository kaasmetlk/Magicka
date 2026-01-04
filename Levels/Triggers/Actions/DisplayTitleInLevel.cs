// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.DisplayTitleInLevel
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Localization;
using PolygonHead;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class DisplayTitleInLevel(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mTitle;
  private int mTitleHash;
  private string mSubtitle;
  private string mTextAlignmentValue;
  private TextAlign mTextAlignment;
  private int mSubtitleHash;
  private float mTitleDisplayTime = -1f;
  private float mFadeOut = 1f;
  private float mFadeIn = 1f;
  private bool mNoTranslation;

  public bool NoTranslation
  {
    get => this.mNoTranslation;
    set => this.mNoTranslation = value;
  }

  public string Title
  {
    get => this.mTitle;
    set
    {
      this.mTitle = value;
      if (this.mTitle != null)
        this.mTitle = this.mTitle.Trim();
      if (string.IsNullOrEmpty(this.mTitle))
      {
        this.mTitle = "";
        this.mTitleHash = 0;
      }
      else
        this.mTitleHash = this.mTitle.GetHashCodeCustom();
    }
  }

  public string SubTitle
  {
    get => this.mSubtitle;
    set
    {
      this.mSubtitle = value;
      if (this.mSubtitle != null)
        this.mSubtitle = this.mSubtitle.Trim();
      if (string.IsNullOrEmpty(this.mSubtitle))
      {
        this.mSubtitle = "";
        this.mSubtitleHash = 0;
      }
      else
        this.mSubtitleHash = this.mSubtitle.GetHashCodeCustom();
    }
  }

  public float DisplayTime
  {
    get => this.mTitleDisplayTime;
    set => this.mTitleDisplayTime = value;
  }

  public float FadeIn
  {
    get => this.mFadeIn;
    set => this.mFadeIn = value;
  }

  public float FadeOut
  {
    get => this.mFadeOut;
    set => this.mFadeOut = value;
  }

  public string TextAlignment
  {
    get => this.mTextAlignmentValue;
    set
    {
      this.mTextAlignmentValue = value.Trim().ToUpper();
      if (string.Compare(this.mTextAlignmentValue, "L") == 0 || string.Compare(this.mTextAlignmentValue, "LEFT") == 0)
      {
        this.mTextAlignmentValue = "L";
        this.mTextAlignment = TextAlign.Left;
      }
      else if (string.Compare(this.mTextAlignmentValue, "R") == 0 || string.Compare(this.mTextAlignmentValue, "RIGHT") == 0)
      {
        this.mTextAlignmentValue = "R";
        this.mTextAlignment = TextAlign.Right;
      }
      else if (string.Compare(this.mTextAlignmentValue, "C") == 0 || string.Compare(this.mTextAlignmentValue, "CENTER") == 0)
      {
        this.mTextAlignmentValue = "C";
        this.mTextAlignment = TextAlign.Center;
      }
      else
      {
        this.mTextAlignmentValue = "C";
        this.mTextAlignment = TextAlign.Center;
      }
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    if (!this.mNoTranslation && this.mTitleHash != 0)
    {
      this.mTitle = LanguageManager.Instance.GetString(this.mTitleHash).ToUpper();
      if (string.IsNullOrEmpty(this.mTitle))
        this.mTitle = "";
      else if (this.mTitle.Length >= 10 && string.Compare(this.mTitle.Substring(0, 10), "NOT FOUND:") == 0)
        this.mTitle = "";
    }
    else
      this.mTitle = this.mTitle.ToUpper();
    if (!this.mNoTranslation && this.mSubtitleHash != 0)
    {
      this.mSubtitle = LanguageManager.Instance.GetString(this.mSubtitleHash).ToUpper();
      if (string.IsNullOrEmpty(this.mSubtitle))
        this.mSubtitle = "";
      else if (this.mSubtitle.Length >= 10 && string.Compare(this.mSubtitle.Substring(0, 10), "NOT FOUND:") == 0)
        this.mSubtitle = "";
    }
    else if (this.mSubtitle != null)
      this.mSubtitle = this.mSubtitle.ToUpper();
    if (!string.IsNullOrEmpty(this.mTextAlignmentValue))
      return;
    this.mTextAlignmentValue = "C";
    this.mTextAlignment = TextAlign.Center;
  }

  public override void QuickExecute()
  {
  }

  protected override void Execute()
  {
    if ((this.mNoTranslation || this.mTitleHash == 0 && this.mSubtitleHash == 0) && (string.IsNullOrEmpty(this.mTitle) || string.IsNullOrEmpty(this.mSubtitle)))
      return;
    if ((double) this.mTitleDisplayTime < 0.0)
      this.mTitleDisplayTime = 2f;
    this.GameScene.Level.DisplayTitle(this.mTitle, this.mSubtitle, this.mTitleDisplayTime, this.mFadeIn, this.mFadeOut, this.mTextAlignment);
  }
}
