// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.RemoveItemFromAvatars
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class RemoveItemFromAvatars(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mItemName;
  private int mItemID;

  protected override void Execute()
  {
    Player[] players = Game.Instance.Players;
    for (int index1 = 0; index1 < players.Length; ++index1)
    {
      if (players[index1].Playing)
      {
        Avatar avatar = players[index1].Avatar;
        if (avatar != null)
        {
          for (int index2 = 0; index2 < avatar.Equipment.Length; ++index2)
          {
            if (avatar.Equipment[index2].Item.Type == this.mItemID)
              avatar.Template.Equipment[index2].CopyToInstance(avatar.Equipment[index2]);
          }
        }
      }
    }
  }

  public override void QuickExecute() => this.Execute();

  public string Item
  {
    get => this.mItemName;
    set
    {
      this.mItemName = value;
      this.mItemID = this.mItemName.GetHashCodeCustom();
    }
  }
}
