// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.ConditionalUnlockAchievement
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.GameLogic.Entities;
using Magicka.Gamers;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class ConditionalUnlockAchievement(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mAchievement;
  private string mVictim;
  private int mID;

  protected override void Execute()
  {
    bool flag = false;
    if (this.mAchievement.Equals("thisismagicka", StringComparison.InvariantCultureIgnoreCase))
    {
      if (Entity.GetByID(this.mID) is NonPlayerCharacter byId1 && byId1.LastAttacker is Avatar lastAttacker1 && lastAttacker1.Player.Playing && !(lastAttacker1.Player.Gamer is NetworkGamer))
        flag = true;
    }
    else if (this.mAchievement.Equals("therecanbeonlyone", StringComparison.InvariantCultureIgnoreCase))
    {
      if (Entity.GetByID(this.mID) is NonPlayerCharacter byId2 && byId2.LastAttacker is Avatar lastAttacker2 && lastAttacker2.Player.Playing && !(lastAttacker2.Player.Gamer is NetworkGamer))
        flag = true;
    }
    else
    {
      if (!this.mAchievement.Equals("drivenmad", StringComparison.InvariantCultureIgnoreCase) && !this.mAchievement.Equals("breezedthrough", StringComparison.InvariantCultureIgnoreCase) && !this.mAchievement.Equals("handlingthefrustration", StringComparison.InvariantCultureIgnoreCase))
        throw new Exception("Not a implemented conditional Achievement");
      if (!this.mScene.PlayState.DiedInLevel)
        flag = true;
    }
    if (!flag)
      return;
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

  public string Victim
  {
    get => this.mVictim;
    set
    {
      this.mVictim = value;
      this.mID = this.mVictim.GetHashCodeCustom();
    }
  }
}
