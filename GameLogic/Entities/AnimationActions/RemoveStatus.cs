// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.RemoveStatus
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using System;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class RemoveStatus : AnimationAction
{
  private StatusEffects mStatus;

  public RemoveStatus(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mStatus = (StatusEffects) Enum.Parse(typeof (StatusEffects), iInput.ReadString(), true);
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    iOwner.StopStatusEffects(this.mStatus);
  }

  public override bool UsesBones => false;
}
