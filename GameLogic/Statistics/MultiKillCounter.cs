// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Statistics.MultiKillCounter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Statistics;

public class MultiKillCounter(int iCapacity) : SortedList<double, KillCount>(iCapacity)
{
  public void Add(double iTimeStamp) => this.Check(iTimeStamp, 1, 120f);

  public void Add(double iTimeStamp, float iTTL) => this.Check(iTimeStamp, 1, iTTL);

  public void Add(double iTimeStamp, int iCount) => this.Check(iTimeStamp, iCount, 120f);

  public void Add(double iTimeStamp, int iCount, float iTTL)
  {
    this.Check(iTimeStamp, iCount, iTTL);
  }

  internal void Check(double iTimeStamp, int iCount, float iTTL)
  {
    KillCount killCount;
    if (this.TryGetValue(iTimeStamp, out killCount))
    {
      killCount.Count += iCount;
      killCount.TTL = iTTL;
    }
    else
      killCount = new KillCount(iCount, iTTL);
    this[iTimeStamp] = killCount;
  }

  public void Update(Player iOwner, float iDeltaTime)
  {
    for (int index = 0; index < this.Count; ++index)
    {
      KillCount killCount = this.Values[index];
      killCount.TTL -= iDeltaTime;
      if ((double) killCount.TTL <= 0.0)
      {
        this.RemoveAt(index--);
      }
      else
      {
        this[this.Keys[index]] = killCount;
        if (killCount.Count >= 20 && iOwner.Avatar != null)
          AchievementsManager.Instance.AwardAchievement(iOwner.Avatar.PlayState, "mumumumultikill");
      }
    }
  }
}
