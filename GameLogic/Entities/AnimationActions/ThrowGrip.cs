// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.ThrowGrip
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class ThrowGrip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : AnimationAction(iInput, iSkeleton)
{
  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!(iFirstExecution & iOwner.IsGripping))
      return;
    Vector3 oVelocity;
    if (iOwner is NonPlayerCharacter nonPlayerCharacter)
    {
      if (!(nonPlayerCharacter.AI.BusyAbility is Magicka.GameLogic.Entities.Abilities.ThrowGrip busyAbility))
        return;
      busyAbility.CalcThrow(nonPlayerCharacter.AI, out oVelocity);
    }
    else
      oVelocity = iOwner.Direction with { Y = 0.1f };
    iOwner.GrippedCharacter.CharacterBody.AddImpulseVelocity(ref oVelocity);
    iOwner.ReleaseAttachedCharacter();
  }

  public override bool UsesBones => true;
}
