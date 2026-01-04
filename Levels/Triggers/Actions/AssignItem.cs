// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.AssignItem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using Magicka.Storage;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class AssignItem(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  public static readonly int ALLID = "all".GetHashCodeCustom();
  private static readonly int DEFAULT_WEAPON = "weapon_default".GetHashCodeCustom();
  private static readonly int DEFAULT_STAFF = "staff_default".GetHashCodeCustom();
  private string mItem;
  private int mItemID;
  private int mPlayer;
  protected string mArea;
  protected int mAreaHash = AssignItem.ALLID;
  protected Animations mAnimation;
  private bool mQuickExecute;

  public override void Initialize()
  {
    base.Initialize();
    if (this.mItemID == AssignItem.DEFAULT_STAFF || this.mItemID == AssignItem.DEFAULT_WEAPON)
      return;
    this.GameScene.PlayState.Content.Load<Item>("Data/Items/Wizard/" + this.mItem);
  }

  protected override void Execute()
  {
    if (this.mAreaHash == AssignItem.ALLID && this.mPlayer == 0)
      this.executeAny();
    else
      this.executeSpecified();
  }

  public void executeAny()
  {
    Magicka.GameLogic.Player[] players = Game.Instance.Players;
    if (this.mItemID == AssignItem.DEFAULT_STAFF)
    {
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && players[index].Avatar != null && !players[index].Avatar.Dead)
        {
          Avatar avatar = players[index].Avatar;
          string iString = avatar.Template.Equipment[1].Item.Name;
          PlayerSaveData playerSaveData;
          if (this.mScene.PlayState.Info.Players.TryGetValue(avatar.Player.GamerTag, out playerSaveData) && !string.IsNullOrEmpty(playerSaveData.Staff))
            iString = playerSaveData.Staff;
          Item.GetCachedWeapon(iString.GetHashCodeCustom(), avatar.Equipment[1].Item);
          avatar.Player.Staff = iString;
          if (this.mAnimation != Animations.None && !this.mQuickExecute)
            avatar.Attack(this.mAnimation, true);
        }
      }
    }
    else if (this.mItemID == AssignItem.DEFAULT_WEAPON)
    {
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && players[index].Avatar != null && !players[index].Avatar.Dead)
        {
          Avatar avatar = players[index].Avatar;
          string iString = avatar.Template.Equipment[0].Item.Name;
          PlayerSaveData playerSaveData;
          if (this.mScene.PlayState.Info.Players.TryGetValue(avatar.Player.GamerTag, out playerSaveData) && !string.IsNullOrEmpty(playerSaveData.Weapon))
            iString = playerSaveData.Weapon;
          Item.GetCachedWeapon(iString.GetHashCodeCustom(), avatar.Equipment[0].Item);
          avatar.Player.Weapon = iString;
          if (this.mAnimation != Animations.None && !this.mQuickExecute)
            avatar.Attack(this.mAnimation, true);
        }
      }
    }
    else
    {
      WeaponClass weaponClass = Item.GetCachedWeapon(this.mItemID).WeaponClass;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && players[index].Avatar != null && !players[index].Avatar.Dead)
        {
          Avatar avatar = players[index].Avatar;
          if (weaponClass == WeaponClass.Staff)
          {
            Item.GetCachedWeapon(this.mItemID, avatar.Equipment[1].Item);
            avatar.Player.Staff = avatar.Equipment[1].Item.Name;
          }
          else
          {
            Item.GetCachedWeapon(this.mItemID, avatar.Equipment[0].Item);
            avatar.Player.Weapon = avatar.Equipment[0].Item.Name;
          }
          if (this.mAnimation != Animations.None && !this.mQuickExecute)
            avatar.Attack(this.mAnimation, true);
        }
      }
    }
  }

  public void executeSpecified()
  {
    avatar = (Avatar) null;
    if (this.mPlayer == 0)
    {
      TriggerArea triggerArea = this.GameScene.GetTriggerArea(this.mAreaHash);
      int iIndex = 0;
      while (iIndex < triggerArea.PresentCharacters.Count && !(triggerArea.PresentCharacters[iIndex] is Avatar avatar))
        ++iIndex;
    }
    else
      avatar = Game.Instance.Players[this.mPlayer - 1].Avatar;
    if (avatar == null)
      return;
    WeaponClass weaponClass1;
    if (this.mItemID == AssignItem.DEFAULT_STAFF)
    {
      if (avatar.Dead)
        return;
      Avatar avatar = avatar;
      string iString = avatar.Template.Equipment[1].Item.Name;
      PlayerSaveData playerSaveData;
      if (this.mScene.PlayState.Info.Players.TryGetValue(avatar.Player.GamerTag, out playerSaveData) && !string.IsNullOrEmpty(playerSaveData.Staff))
        iString = playerSaveData.Staff;
      weaponClass1 = Item.GetCachedWeapon(iString.GetHashCodeCustom()).WeaponClass;
      Item.GetCachedWeapon(iString.GetHashCodeCustom(), avatar.Equipment[1].Item);
      avatar.Player.Staff = iString;
      if (this.mAnimation == Animations.None || this.mQuickExecute)
        return;
      avatar.Attack(this.mAnimation, true);
    }
    else if (this.mItemID == AssignItem.DEFAULT_WEAPON)
    {
      if (avatar.Dead)
        return;
      Avatar avatar = avatar;
      string iString = avatar.Template.Equipment[0].Item.Name;
      PlayerSaveData playerSaveData;
      if (this.mScene.PlayState.Info.Players.TryGetValue(avatar.Player.GamerTag, out playerSaveData) && !string.IsNullOrEmpty(playerSaveData.Weapon))
        iString = playerSaveData.Weapon;
      weaponClass1 = Item.GetCachedWeapon(iString.GetHashCodeCustom()).WeaponClass;
      Item.GetCachedWeapon(iString.GetHashCodeCustom(), avatar.Equipment[0].Item);
      avatar.Player.Weapon = iString;
      if (this.mAnimation == Animations.None || this.mQuickExecute)
        return;
      avatar.Attack(this.mAnimation, true);
    }
    else
    {
      WeaponClass weaponClass2 = Item.GetCachedWeapon(this.mItemID).WeaponClass;
      if (avatar.Dead)
        return;
      Avatar avatar = avatar;
      if (weaponClass2 == WeaponClass.Staff)
      {
        Item.GetCachedWeapon(this.mItemID, avatar.Equipment[1].Item);
        avatar.Player.Staff = avatar.Equipment[1].Item.Name;
      }
      else
      {
        Item.GetCachedWeapon(this.mItemID, avatar.Equipment[0].Item);
        avatar.Player.Weapon = avatar.Equipment[0].Item.Name;
      }
      if (this.mAnimation == Animations.None || this.mQuickExecute)
        return;
      avatar.Attack(this.mAnimation, true);
    }
  }

  public override void QuickExecute()
  {
    this.mQuickExecute = true;
    this.Execute();
    this.mQuickExecute = false;
  }

  public string ID
  {
    get => this.mItem;
    set
    {
      this.mItem = value;
      this.mItemID = value.GetHashCodeCustom();
    }
  }

  public int Player
  {
    get => this.mPlayer;
    set => this.mPlayer = value;
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

  public Animations Animation
  {
    get => this.mAnimation;
    set => this.mAnimation = value;
  }
}
