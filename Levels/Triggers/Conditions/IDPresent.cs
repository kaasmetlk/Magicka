// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.IDPresent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class IDPresent(GameScene iScene) : Condition(iScene)
{
  private string mArea = "any";
  private int mAreaID = Condition.ANYID;
  private string mName;
  private int mID;

  protected override bool InternalMet(Character iSender)
  {
    TriggerArea triggerArea = this.Scene.GetTriggerArea(this.mAreaID);
    for (int iIndex = 0; iIndex < triggerArea.PresentEntities.Count; ++iIndex)
    {
      if (triggerArea.PresentEntities[iIndex].UniqueID == this.mID)
        return !triggerArea.PresentEntities[iIndex].Dead;
    }
    return false;
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

  public string ID
  {
    get => this.mName;
    set
    {
      this.mName = value;
      this.mID = this.mName.GetHashCodeCustom();
    }
  }
}
