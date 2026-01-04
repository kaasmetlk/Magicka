// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.DetachItem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class DetachItem : AnimationAction
{
  private int mItem;
  private Vector3 mVelocity;

  public DetachItem(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mItem = iInput.ReadInt32();
    this.mVelocity = iInput.ReadVector3();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution || iOwner.Equipment[this.mItem].Item == null)
      return;
    Matrix orientation = iOwner.Body.Orientation;
    Vector3 result;
    Vector3.Transform(ref this.mVelocity, ref orientation, out result);
    iOwner.Equipment[this.mItem].Release(iOwner.PlayState);
    iOwner.Equipment[this.mItem].Item.AnimationDetach();
    iOwner.Equipment[this.mItem].Item.Body.Velocity = result;
  }

  public override bool UsesBones => false;
}
