// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.Present
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class Present(GameScene iScene) : Condition(iScene)
{
  private string mArea;
  private int mAreaID;
  private string mType;
  private int mTypeID;
  private int mNr;
  private bool mIgnoreInvisibility = true;
  private CompareMethod mCompareMethod;

  protected override bool InternalMet(Character iSender)
  {
    TriggerArea triggerArea = this.Scene.GetTriggerArea(this.mAreaID);
    int count = triggerArea.GetCount(this.mTypeID);
    if (!this.mIgnoreInvisibility)
    {
      foreach (Character presentCharacter in triggerArea.PresentCharacters)
      {
        if (presentCharacter.Type == this.mTypeID && presentCharacter.IsInvisibile)
          --count;
      }
    }
    switch (this.mCompareMethod)
    {
      case CompareMethod.LESS:
        if (count < this.mNr)
          return true;
        break;
      case CompareMethod.EQUAL:
        if (count == this.mNr)
          return true;
        break;
      case CompareMethod.GREATER:
        if (count > this.mNr)
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

  public string Type
  {
    get => this.mType;
    set
    {
      this.mType = value;
      this.mTypeID = this.mType.GetHashCodeCustom();
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
