// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.SpecialAbility
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class SpecialAbility : AnimationAction
{
  private int mWeapon;
  private Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility mAbility;

  public SpecialAbility(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mWeapon = iInput.ReadInt32();
    if (this.mWeapon >= 0)
      return;
    this.mAbility = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility.Read(iInput);
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    if (this.mWeapon < 0)
      this.mAbility.Execute((ISpellCaster) iOwner, iOwner.PlayState);
    else
      iOwner.Equipment[this.mWeapon].Item.ExecuteSpecialAbility();
  }

  public override bool UsesBones => true;

  public override void Kill(Character iOwner) => base.Kill(iOwner);
}
