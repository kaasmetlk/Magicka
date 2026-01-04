// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.ReleaseGrip
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class ReleaseGrip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : 
  AnimationAction(iInput, iSkeleton)
{
  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    DamageCollection5 iDamages = new DamageCollection5();
    if (iOwner.GrippedCharacter != null)
      iOwner.GrippedCharacter.SetCollisionDamage(ref iDamages);
    iOwner.ReleaseAttachedCharacter();
  }

  public override bool UsesBones => false;
}
