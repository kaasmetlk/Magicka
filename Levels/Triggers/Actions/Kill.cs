// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Kill
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class Kill(Trigger iTrigger, GameScene iScene, XmlNode iNode) : Action(iTrigger, iScene)
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  protected string mID;
  protected int mIDHash;
  protected string mType;
  protected Factions mFactions;
  protected int mTypeHash;
  protected string mArea;
  protected int mAreaHash;
  protected bool mIsSpecific;

  protected override void Execute()
  {
    if (this.mIsSpecific)
    {
      Entity byId = Entity.GetByID(this.mIDHash);
      if (byId == null)
        return;
      if (byId is Character)
        (byId as Character).CannotDieWithoutExplicitKill = false;
      byId.Kill();
    }
    else
    {
      TriggerArea triggerArea = this.GameScene.GetTriggerArea(this.mAreaHash);
      for (int iIndex = 0; iIndex < triggerArea.PresentCharacters.Count; ++iIndex)
      {
        Character presentCharacter = triggerArea.PresentCharacters[iIndex];
        if (presentCharacter != null && (this.mTypeHash == Kill.ANYID || presentCharacter.Type == this.mTypeHash || (presentCharacter.GetOriginalFaction & this.mFactions) != Factions.NONE))
        {
          presentCharacter.CannotDieWithoutExplicitKill = false;
          presentCharacter.Kill();
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

  public string Type
  {
    get => this.mType;
    set
    {
      this.mType = value;
      this.mTypeHash = this.mType.GetHashCodeCustom();
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
}
