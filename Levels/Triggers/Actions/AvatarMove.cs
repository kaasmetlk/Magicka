// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.AvatarMove
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.AI;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class AvatarMove : Action
{
  private XmlNode mNode;
  private AIEvent[][] mEvents = new AIEvent[4][];

  public AvatarMove(Trigger iTrigger, GameScene iScene, XmlNode iNode)
    : base(iTrigger, iScene)
  {
    this.mNode = iNode;
  }

  public override void Initialize()
  {
    base.Initialize();
    for (int index = 0; index < 4; ++index)
      this.mEvents[index] = (AIEvent[]) null;
    LevelModel levelModel = this.GameScene.LevelModel;
    foreach (XmlNode childNode1 in this.mNode.ChildNodes)
    {
      if (!(childNode1 is XmlComment) && childNode1.Name.StartsWith("playerid", StringComparison.OrdinalIgnoreCase))
      {
        string iString = "";
        foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode1.Attributes)
        {
          if (attribute.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
            iString = attribute.Value.ToLowerInvariant();
        }
        List<AIEvent> aiEventList = new List<AIEvent>();
        foreach (XmlNode childNode2 in childNode1.ChildNodes)
        {
          if (!(childNode2 is XmlComment))
            aiEventList.Add(new AIEvent(levelModel, childNode2));
        }
        Entity byId = Entity.GetByID(iString.GetHashCodeCustom());
        if (byId != null)
        {
          for (int index = 0; index < Game.Instance.Players.Length; ++index)
          {
            if (Game.Instance.Players[index].Avatar == byId)
            {
              this.mEvents[index] = aiEventList.ToArray();
              break;
            }
          }
        }
      }
    }
    foreach (XmlNode childNode3 in this.mNode.ChildNodes)
    {
      if (!(childNode3 is XmlComment))
      {
        if (childNode3.Name.StartsWith("player", StringComparison.OrdinalIgnoreCase) && char.IsDigit(childNode3.Name[childNode3.Name.Length - 1]))
        {
          List<AIEvent> aiEventList = new List<AIEvent>();
          foreach (XmlNode childNode4 in childNode3.ChildNodes)
          {
            if (!(childNode4 is XmlComment))
              aiEventList.Add(new AIEvent(levelModel, childNode4));
          }
          int index = (int) childNode3.Name[childNode3.Name.Length - 1] - 49;
          int num = 0;
          while (true)
          {
            if (index > 3)
            {
              index = 0;
              ++num;
              if (num > 10)
                break;
            }
            if (this.mEvents[index] != null)
              ++index;
            else
              goto label_49;
          }
          throw new Exception($"Unable to map AvatarMove event {(object) aiEventList.ToArray()}!");
label_49:
          this.mEvents[index] = aiEventList.ToArray();
        }
        else if (!childNode3.Name.StartsWith("playerid", StringComparison.OrdinalIgnoreCase))
          throw new Exception($"Invalid node \"{childNode3.Name}\" in AvatarMove! Expected \"Player1\"-\"Player4\".");
      }
    }
  }

  protected override void Execute()
  {
    Player[] players = Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing && players[index].Avatar != null && this.mEvents[index] != null)
        players[index].Avatar.Events = this.mEvents[index];
    }
  }

  public override void QuickExecute()
  {
  }
}
