// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.InteractorIs
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class InteractorIs(GameScene iScene) : Condition(iScene)
{
  private string mIDStr;
  private int mID;

  protected override bool InternalMet(Character iSender)
  {
    return iSender != null && iSender.UniqueID == this.mID;
  }

  public string ID
  {
    get => this.mIDStr;
    set
    {
      this.mIDStr = value;
      this.mID = this.mIDStr.GetHashCodeCustom();
    }
  }
}
