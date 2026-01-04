// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.Timer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class Timer(GameScene iScene) : Condition(iScene)
{
  private string mName;
  private int mID;
  private CompareMethod mCompareMethod;
  private float mValue;

  protected override bool InternalMet(Character iSender)
  {
    float timerValue = this.Scene.Level.GetTimerValue(this.mID);
    switch (this.mCompareMethod)
    {
      case CompareMethod.LESS:
        return (double) this.mValue > (double) timerValue;
      case CompareMethod.EQUAL:
        return (double) this.mValue == (double) timerValue;
      case CompareMethod.GREATER:
        return (double) this.mValue < (double) timerValue;
      default:
        return false;
    }
  }

  public string Name
  {
    get => this.mName;
    set
    {
      this.mName = value;
      this.mID = this.mName.GetHashCodeCustom();
    }
  }

  public CompareMethod CompareMethod
  {
    get => this.mCompareMethod;
    set => this.mCompareMethod = value;
  }

  public float Value
  {
    get => this.mValue;
    set => this.mValue = value;
  }
}
