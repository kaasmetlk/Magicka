// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.AddItemToAvatars
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using System;
using XNAnimation;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class AddItemToAvatars(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private Magicka.GameLogic.Entities.Items.Item mItem;
  private string mItemName;
  private int mItemID;
  private string mBone;

  public override void Initialize()
  {
    base.Initialize();
    try
    {
      this.mItem = this.GameScene.PlayState.Content.Load<Magicka.GameLogic.Entities.Items.Item>("data/items/wizard/" + this.mItemName);
    }
    catch (Exception ex1)
    {
      try
      {
        this.mItem = this.GameScene.PlayState.Content.Load<Magicka.GameLogic.Entities.Items.Item>("data/items/npc/" + this.mItemName);
      }
      catch (Exception ex2)
      {
      }
    }
  }

  protected override void Execute()
  {
    if (this.mItem == null || string.IsNullOrEmpty(this.mBone))
      return;
    Player[] players = Magicka.Game.Instance.Players;
    for (int index1 = 0; index1 < players.Length; ++index1)
    {
      if (players[index1].Playing)
      {
        Avatar avatar = players[index1].Avatar;
        if (avatar != null)
        {
          SkinnedModelBone iBone = (SkinnedModelBone) null;
          for (int index2 = 0; index2 < avatar.Model.SkeletonBones.Count; ++index2)
          {
            if (avatar.Model.SkeletonBones[index2].Name.Equals(this.mBone, StringComparison.OrdinalIgnoreCase))
            {
              iBone = avatar.Model.SkeletonBones[index2];
              break;
            }
          }
          if (iBone != null)
          {
            for (int index3 = 0; index3 < avatar.Equipment.Length; ++index3)
            {
              if (avatar.Equipment[index3].AttachIndex < 0)
              {
                avatar.Equipment[index3].Set(this.mItem, iBone, new Vector3?());
                break;
              }
            }
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

  public string Bone
  {
    get => this.mBone;
    set => this.mBone = value;
  }
}
