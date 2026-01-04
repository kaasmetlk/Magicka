// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Block
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Abilities;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class Block : AnimationAction
{
  private int mWeapon;

  public Block(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mWeapon = iInput.ReadInt32();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    iOwner.BlockItem = this.mWeapon;
    if (!(iOwner is NonPlayerCharacter nonPlayerCharacter) || !(nonPlayerCharacter.AI.BusyAbility is Magicka.GameLogic.Entities.Abilities.Block))
      return;
    nonPlayerCharacter.AI.BusyAbility = (Ability) null;
  }

  public override void Kill(Character iOwner)
  {
    base.Kill(iOwner);
    iOwner.BlockItem = -1;
  }

  public override bool UsesBones => false;
}
