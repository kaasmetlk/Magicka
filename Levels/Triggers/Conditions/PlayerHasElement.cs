// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.PlayerHasElement
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class PlayerHasElement(GameScene iScene) : Condition(iScene)
{
  private Elements mElement;

  protected override bool InternalMet(Character iSender)
  {
    Player[] players = Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      Avatar avatar = players[index].Avatar;
      if (players[index].Playing && avatar != null)
      {
        for (int iIndex = 0; iIndex < avatar.SpellQueue.Count; ++iIndex)
        {
          if ((avatar.SpellQueue[iIndex].Element & this.mElement) != Elements.None)
            return true;
        }
      }
    }
    return false;
  }

  public Elements Element
  {
    get => this.mElement;
    set => this.mElement = value;
  }
}
