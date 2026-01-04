// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.ReleaseGrip
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class ReleaseGrip(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  protected Factions mFactions;
  protected string mType;
  protected int mTypeHash;
  protected string mID;
  protected int mIDHash;
  protected string mArea;
  protected int mAreaHash;
  protected bool mIsSpecific;

  protected override void Execute()
  {
    if (this.mIsSpecific)
    {
      if (!(Entity.GetByID(this.mIDHash) is NonPlayerCharacter byId))
        return;
      byId.ReleaseAttachedCharacter();
      if (byId.Gripper == null)
        return;
      byId.Gripper.ReleaseAttachedCharacter();
    }
    else
    {
      TriggerArea triggerArea = this.GameScene.GetTriggerArea(this.mAreaHash);
      for (int iIndex = 0; iIndex < triggerArea.PresentCharacters.Count; ++iIndex)
      {
        if (triggerArea.PresentCharacters[iIndex] is NonPlayerCharacter presentCharacter && (this.mTypeHash == ReleaseGrip.ANYID || presentCharacter.Type == this.mTypeHash || (presentCharacter.GetOriginalFaction & this.mFactions) != Factions.NONE))
        {
          presentCharacter.ReleaseAttachedCharacter();
          if (presentCharacter.Gripper != null)
            presentCharacter.Gripper.ReleaseAttachedCharacter();
        }
      }
    }
  }

  public override void QuickExecute() => this.Execute();

  public Factions Factions
  {
    get => this.mFactions;
    set => this.mFactions = value;
  }

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      if (!string.IsNullOrEmpty(this.mID))
      {
        this.mIsSpecific = true;
        this.mIDHash = this.mID.GetHashCodeCustom();
      }
      else
      {
        this.mIsSpecific = false;
        this.mIDHash = 0;
      }
    }
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaHash = this.mArea.GetHashCodeCustom();
    }
  }

  public string Type
  {
    get => this.mType;
    set
    {
      this.mType = value;
      this.mTypeHash = this.mType.GetHashCodeCustom();
    }
  }
}
