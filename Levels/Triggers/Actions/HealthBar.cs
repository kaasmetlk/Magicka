// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.HealthBar
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.UI;
using Magicka.Localization;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class HealthBar(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private int mDisplayNameHash;
  private string mDisplayName;
  private bool mShowDisplayName;
  private float mNormalizedHealth = 1f;
  private float mPower = 1.1f;
  private float mAlpha;
  private float mTime;
  private float mDisplayHealth;
  private float mCount;
  private float mInitialTimerDelay;
  private GenericHealthBarTypes mType;
  private GenericHealthBarGraphics mGraphicsType;
  private bool mIsScaled;
  private bool mIsColoredRed;
  private bool mHasAnimatedSprite;
  private float mAnimationSpriteOffsetY = 16f;
  private float mFadeTime;
  private string mOnEndTrigger;
  private int mOnEndTriggerID;

  public override void Initialize()
  {
    base.Initialize();
    if (this.mDisplayNameHash == 0)
      return;
    this.mDisplayName = LanguageManager.Instance.GetString(this.mDisplayNameHash);
  }

  protected override void Execute()
  {
    GenericHealthBar genericHealthBar = this.GameScene.PlayState.GenericHealthBar;
    genericHealthBar.Power = this.mPower;
    genericHealthBar.Alpha = this.mAlpha;
    genericHealthBar.Type = this.mType;
    genericHealthBar.NormalizedHealth = this.mNormalizedHealth;
    genericHealthBar.DisplayHealth = this.mDisplayHealth;
    genericHealthBar.InitialTimerDelay = this.mInitialTimerDelay;
    genericHealthBar.IsColoredRed = this.mIsColoredRed;
    genericHealthBar.IsScaled = this.mIsScaled;
    genericHealthBar.ShowDisplayName = this.mShowDisplayName;
    genericHealthBar.DisplayName = this.mDisplayName;
    if (this.mType == GenericHealthBarTypes.TimerDecreasing || this.mType == GenericHealthBarTypes.TimerIncreasing)
      genericHealthBar.SetupTimer(this.mTime);
    else
      genericHealthBar.SetupCounter(this.mCount);
    if (this.mDisplayNameHash != 0)
      genericHealthBar.DisplayName = this.mDisplayName;
    genericHealthBar.GraphicsType = this.mGraphicsType;
    genericHealthBar.HasAnimatedSprite = this.mHasAnimatedSprite;
    genericHealthBar.AnimationSpriteOffsetY = this.mAnimationSpriteOffsetY;
    genericHealthBar.FadeTime = this.mFadeTime;
    genericHealthBar.OnEndTriggerID = this.mOnEndTriggerID;
  }

  public override void QuickExecute() => this.Execute();

  public string DisplayName
  {
    get => this.mDisplayName;
    set
    {
      this.mDisplayName = value;
      this.mDisplayNameHash = this.mDisplayName.GetHashCodeCustom();
    }
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

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }

  public GenericHealthBarTypes Type
  {
    get => this.mType;
    set => this.mType = value;
  }

  public float Power
  {
    get => this.mPower;
    set => this.mPower = value;
  }

  public float Alpha
  {
    get => this.mAlpha;
    set => this.mAlpha = value;
  }

  public float Count
  {
    get => this.mCount;
    set => this.mCount = value;
  }

  public GenericHealthBarGraphics GraphicsType
  {
    get => this.mGraphicsType;
    set => this.mGraphicsType = value;
  }

  public float InitialTimerDelay
  {
    get => this.mInitialTimerDelay;
    set => this.mInitialTimerDelay = value;
  }

  public bool IsScaled
  {
    get => this.mIsScaled;
    set => this.mIsScaled = value;
  }

  public bool IsColoredRed
  {
    get => this.mIsColoredRed;
    set => this.mIsColoredRed = value;
  }

  public bool ShowDisplayName
  {
    get => this.mShowDisplayName;
    set => this.mShowDisplayName = value;
  }

  public bool HasAnimatedSprite
  {
    get => this.mHasAnimatedSprite;
    set => this.mHasAnimatedSprite = value;
  }

  public float AnimationOffsetPositionY
  {
    get => this.mAnimationSpriteOffsetY;
    set => this.mAnimationSpriteOffsetY = value;
  }

  public float FadeTime
  {
    get => this.mFadeTime;
    set => this.mFadeTime = value;
  }

  public string OnEndTrigger
  {
    get => this.mOnEndTrigger;
    set
    {
      this.mOnEndTrigger = value;
      this.mOnEndTriggerID = this.mOnEndTrigger.GetHashCodeCustom();
    }
  }
}
