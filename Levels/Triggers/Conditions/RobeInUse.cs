// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.RobeInUse
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

internal class RobeInUse(GameScene iScene) : Condition(iScene)
{
  private string mName;
  private int mNameID;
  private int mPlayer;

  protected override bool InternalMet(Character iSender)
  {
    if (string.IsNullOrEmpty(this.mName))
      return false;
    Magicka.GameLogic.Player[] players = Game.Instance.Players;
    if (this.mPlayer == 0)
    {
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && players[index].Avatar != null && players[index].Avatar.Template.ID == this.mNameID)
          return true;
      }
    }
    else if (players[this.mPlayer - 1].Playing && players[this.mPlayer - 1].Avatar != null && players[this.mPlayer - 1].Avatar.Template.ID == this.mNameID)
      return true;
    return false;
  }

  public string Name
  {
    get => this.mName;
    set
    {
      this.mName = value;
      this.mNameID = this.mName.ToLowerInvariant().GetHashCodeCustom();
    }
  }

  public int Player
  {
    get => this.mPlayer;
    set => this.mPlayer = value;
  }
}
