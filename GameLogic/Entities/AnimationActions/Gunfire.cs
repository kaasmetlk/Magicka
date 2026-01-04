// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Gunfire
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class Gunfire : AnimationAction
{
  private int mWeapon;
  private float mAccuracy;

  public Gunfire(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mWeapon = iInput.ReadInt32();
    this.mAccuracy = iInput.ReadSingle();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    Item obj = iOwner.Equipment[this.mWeapon].Item;
    if (!iFirstExecution)
      return;
    obj.ClearHitlist();
    obj.ExecuteGun(this.mAccuracy);
  }

  public override bool UsesBones => true;

  public override void Kill(Character iOwner)
  {
    base.Kill(iOwner);
    iOwner.Equipment[this.mWeapon].Item.StopGunfire();
  }
}
