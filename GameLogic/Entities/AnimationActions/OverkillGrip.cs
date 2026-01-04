// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.OverkillGrip
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Network;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class OverkillGrip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : 
  AnimationAction(iInput, iSkeleton)
{
  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (iOwner.GrippedCharacter == null || NetworkManager.Instance.State == NetworkState.Client)
      return;
    iOwner.GrippedCharacter.OverKill();
  }

  public override bool UsesBones => false;
}
