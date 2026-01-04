// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.DamageGrip
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using System;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class DamageGrip : AnimationAction
{
  private bool mDamageToOwner;
  private DamageCollection5 mDamages;

  public unsafe DamageGrip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mDamageToOwner = iInput.ReadBoolean();
    fixed (Damage* damagePtr = &this.mDamages.A)
    {
      int num = iInput.ReadInt32();
      if (num > 5)
        throw new Exception($"To many damages! Maximum allowed is {(object) 5}.");
      for (int index = 0; index < num; ++index)
      {
        damagePtr[index].AttackProperty = (AttackProperties) iInput.ReadInt32();
        damagePtr[index].Element = (Elements) iInput.ReadInt32();
        damagePtr[index].Amount = iInput.ReadSingle();
        damagePtr[index].Magnitude = iInput.ReadSingle();
      }
    }
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (this.mDamageToOwner)
    {
      int num1 = (int) iOwner.Damage(this.mDamages, (Entity) iOwner, iOwner.PlayState.PlayTime, iOwner.Position);
    }
    else
    {
      if (!iOwner.IsGripping)
        return;
      int num2 = (int) iOwner.GrippedCharacter.Damage(this.mDamages, (Entity) iOwner, iOwner.PlayState.PlayTime, iOwner.Position);
    }
  }

  public override bool UsesBones => false;
}
