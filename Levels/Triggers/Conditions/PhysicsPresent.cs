// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.PhysicsPresent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class PhysicsPresent(GameScene iScene) : Condition(iScene)
{
  private string mArea = "any";
  private int mAreaID = Condition.ANYID;
  private int mNr;
  private CompareMethod mCompareMethod;
  private bool mOnlyDamagable;

  protected override bool InternalMet(Character iSender)
  {
    int num = 0;
    TriggerArea triggerArea = this.Scene.GetTriggerArea(this.mAreaID);
    for (int iIndex = 0; iIndex < triggerArea.PresentEntities.Count; ++iIndex)
    {
      if (this.mOnlyDamagable)
      {
        if (triggerArea.PresentEntities[iIndex] is DamageablePhysicsEntity)
          ++num;
      }
      else if (triggerArea.PresentEntities[iIndex] is PhysicsEntity)
        ++num;
    }
    switch (this.mCompareMethod)
    {
      case CompareMethod.LESS:
        if (num < this.mNr)
          return true;
        break;
      case CompareMethod.EQUAL:
        if (num == this.mNr)
          return true;
        break;
      case CompareMethod.GREATER:
        if (num > this.mNr)
          return true;
        break;
    }
    return false;
  }

  public bool OnlyDamagable
  {
    get => this.mOnlyDamagable;
    set => this.mOnlyDamagable = value;
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaID = this.mArea.GetHashCodeCustom();
    }
  }

  public int Nr
  {
    get => this.mNr;
    set => this.mNr = value;
  }

  public CompareMethod CompareMethod
  {
    get => this.mCompareMethod;
    set => this.mCompareMethod = value;
  }
}
