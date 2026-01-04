// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.CharacterHealth
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class CharacterHealth(GameScene iScene) : Condition(iScene)
{
  private string mName;
  private int mID;
  private CompareMethod mCompareMethod;
  private float mHealth;

  protected override bool InternalMet(Character iSender)
  {
    float num = !(Entity.GetByID(this.mID) is Character byId) || byId.Dead ? 0.0f : byId.HitPoints / byId.MaxHitPoints;
    switch (this.mCompareMethod)
    {
      case CompareMethod.LESS:
        return (double) this.mHealth > (double) num;
      case CompareMethod.EQUAL:
        return (double) Math.Abs(this.mHealth - num) < 9.9999997473787516E-05;
      case CompareMethod.GREATER:
        return (double) this.mHealth < (double) num;
      default:
        return false;
    }
  }

  public string ID
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

  public float Health
  {
    get => this.mHealth;
    set
    {
      this.mHealth = value;
      if ((double) this.mHealth > 1.0 || (double) this.mHealth < 0.0)
        throw new Exception("Health must be between 0.0 and 1.0!");
    }
  }
}
