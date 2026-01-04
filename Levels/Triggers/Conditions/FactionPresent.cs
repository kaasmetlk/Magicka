// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.FactionPresent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class FactionPresent(GameScene iScene) : Condition(iScene)
{
  private string mArea;
  private int mAreaID;
  private Factions mFactions;
  private int mNr;
  private CompareMethod mOperator;
  private bool mIgnoreInvisibility = true;

  protected override bool InternalMet(Character iSender)
  {
    TriggerArea triggerArea = this.Scene.GetTriggerArea(this.mAreaID);
    int factionCount = triggerArea.GetFactionCount(this.mFactions);
    if (!this.mIgnoreInvisibility)
    {
      foreach (Character presentCharacter in triggerArea.PresentCharacters)
      {
        if ((presentCharacter.Faction & this.mFactions) != Factions.NONE && presentCharacter.IsInvisibile)
          --factionCount;
      }
    }
    switch (this.mOperator)
    {
      case CompareMethod.LESS:
        if (factionCount < this.mNr)
          return true;
        break;
      case CompareMethod.EQUAL:
        if (factionCount == this.mNr)
          return true;
        break;
      case CompareMethod.GREATER:
        if (factionCount > this.mNr)
          return true;
        break;
    }
    return false;
  }

  public bool IncludeInvisible
  {
    get => this.mIgnoreInvisibility;
    set => this.mIgnoreInvisibility = value;
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

  public Factions Factions
  {
    get => this.mFactions;
    set => this.mFactions = value;
  }

  public int Nr
  {
    get => this.mNr;
    set => this.mNr = value;
  }

  public CompareMethod CompareMethod
  {
    get => this.mOperator;
    set => this.mOperator = value;
  }
}
