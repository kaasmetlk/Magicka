// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.HasAchievement
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class HasAchievement(GameScene iScene) : Condition(iScene)
{
  private string mName;

  protected override bool InternalMet(Character iSender)
  {
    return AchievementsManager.Instance.HasAchievement(this.mName);
  }

  public string Name
  {
    get => this.mName;
    set => this.mName = value;
  }
}
