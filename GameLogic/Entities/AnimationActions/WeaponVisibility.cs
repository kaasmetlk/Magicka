// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.WeaponVisibility
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class WeaponVisibility : AnimationAction
{
  private int mWeapon;
  private bool mVisible;

  public WeaponVisibility(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mWeapon = iInput.ReadInt32();
    this.mVisible = iInput.ReadBoolean();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    iOwner.Equipment[this.mWeapon].Item.Visible = this.mVisible;
  }

  public override bool UsesBones => false;
}
