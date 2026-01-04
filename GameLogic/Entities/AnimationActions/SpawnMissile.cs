// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.SpawnMissile
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class SpawnMissile : AnimationAction
{
  private int mWeapon;
  private Vector3? mVelocity;
  private bool mItemAligned;

  public SpawnMissile(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mVelocity = new Vector3?();
    this.mWeapon = iInput.ReadInt32();
    Vector3 vector3 = iInput.ReadVector3();
    if ((double) vector3.LengthSquared() > 9.9999999747524271E-07)
      this.mVelocity = new Vector3?(vector3);
    this.mItemAligned = iInput.ReadBoolean();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    Item obj = iOwner.Equipment[this.mWeapon].Item;
    if (!iFirstExecution)
      return;
    MissileEntity iMissile = (MissileEntity) null;
    obj.ExecuteRanged(ref iMissile, this.mVelocity, this.mItemAligned);
  }

  public override bool UsesBones => true;

  public override void Kill(Character iOwner) => base.Kill(iOwner);
}
