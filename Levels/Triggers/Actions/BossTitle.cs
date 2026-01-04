// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.BossTitle
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Bosses;
using Magicka.Localization;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class BossTitle(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mBossTitle;
  private int mBossTitleHash;
  private string mBossSubTitle;
  private int mBossSubTitleHash;
  private float mTitleDisplayTime = -1f;
  private float mFadeOut = 1f;
  private float mFadeIn = 1f;

  public override void Initialize()
  {
    base.Initialize();
    if (this.mBossTitleHash != 0)
      this.mBossTitle = LanguageManager.Instance.GetString(this.mBossTitleHash).ToUpper();
    if (this.mBossSubTitleHash == 0)
      return;
    this.mBossSubTitle = LanguageManager.Instance.GetString(this.mBossSubTitleHash).ToUpper();
  }

  protected override void Execute()
  {
    if (this.mBossTitleHash == 0 && this.mBossSubTitleHash == 0)
      return;
    if ((double) this.mTitleDisplayTime < 0.0)
      this.mTitleDisplayTime = 2f;
    BossFight.Instance.SetTitles(this.mBossTitleHash == 0 ? "" : this.mBossTitle, this.mBossSubTitleHash == 0 ? "" : this.mBossSubTitle, this.mTitleDisplayTime, this.mFadeIn, this.mFadeOut);
  }

  public override void QuickExecute()
  {
  }

  public string Title
  {
    get => this.mBossTitle;
    set
    {
      this.mBossTitle = value;
      this.mBossTitleHash = this.mBossTitle.GetHashCodeCustom();
    }
  }

  public string SubTitle
  {
    get => this.mBossSubTitle;
    set
    {
      this.mBossSubTitle = value;
      this.mBossSubTitleHash = this.mBossSubTitle.GetHashCodeCustom();
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
}
