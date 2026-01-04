// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.UnlockAchievement
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class UnlockAchievement(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mAchievement;

  protected override void Execute()
  {
    AchievementsManager.Instance.AwardAchievement(this.GameScene.PlayState, this.mAchievement);
  }

  public override void QuickExecute()
  {
  }

  public string Achievement
  {
    get => this.mAchievement;
    set => this.mAchievement = value;
  }
}
