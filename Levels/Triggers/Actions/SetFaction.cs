// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SetFaction
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SetFaction(Trigger iTrigger, GameScene iScene, XmlNode iNode) : Magicka.Levels.Triggers.Actions.Action(iTrigger, iScene)
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  public static readonly string PLAYER1 = "player1";
  public static readonly string PLAYER2 = "player2";
  public static readonly string PLAYER3 = "player3";
  public static readonly string PLAYER4 = "player4";
  protected string mID;
  protected int mIDHash;
  protected OperationAction mAction;
  protected Factions mFaction;
  protected string mType;
  protected int mTypeHash;
  protected string mArea;
  protected int mAreaHash;
  protected bool mIsSpecific;

  public override void Initialize() => base.Initialize();

  protected override void Execute()
  {
    if (this.mIsSpecific)
    {
      Character character = (Character) null;
      int index = -1;
      if (this.mID.Equals(SetFaction.PLAYER1, StringComparison.OrdinalIgnoreCase))
        index = 0;
      else if (this.mID.Equals(SetFaction.PLAYER2, StringComparison.OrdinalIgnoreCase))
        index = 1;
      else if (this.mID.Equals(SetFaction.PLAYER3, StringComparison.OrdinalIgnoreCase))
        index = 2;
      else if (this.mID.Equals(SetFaction.PLAYER4, StringComparison.OrdinalIgnoreCase))
        index = 3;
      if (index != -1)
      {
        if (Game.Instance.Players[index].Playing && Game.Instance.Players[index].Avatar != null)
          character = (Character) Game.Instance.Players[index].Avatar;
      }
      else
        character = Entity.GetByID(this.mIDHash) as Character;
      if (character == null)
        return;
      switch (this.mAction)
      {
        case OperationAction.Add:
          character.Faction |= this.mFaction;
          break;
        case OperationAction.Remove:
          character.Faction &= ~this.mFaction;
          break;
        case OperationAction.Set:
          character.Faction = this.mFaction;
          break;
      }
    }
    else
    {
      TriggerArea triggerArea = this.GameScene.GetTriggerArea(this.mAreaHash);
      for (int iIndex = 0; iIndex < triggerArea.PresentCharacters.Count; ++iIndex)
      {
        Character presentCharacter = triggerArea.PresentCharacters[iIndex];
        if (presentCharacter != null && (this.mTypeHash == SetFaction.ANYID || presentCharacter.Type == this.mTypeHash))
        {
          switch (this.mAction)
          {
            case OperationAction.Add:
              presentCharacter.Faction |= this.mFaction;
              continue;
            case OperationAction.Remove:
              presentCharacter.Faction &= ~this.mFaction;
              continue;
            case OperationAction.Set:
              presentCharacter.Faction = this.mFaction;
              continue;
            default:
              continue;
          }
        }
      }
    }
  }

  public override void QuickExecute() => this.Execute();

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

  public Factions Faction
  {
    get => this.mFaction;
    set => this.mFaction = value;
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

  public OperationAction Action
  {
    get => this.mAction;
    set => this.mAction = value;
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
