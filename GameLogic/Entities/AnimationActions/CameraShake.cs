// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.CameraShake
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class CameraShake : AnimationAction
{
  private float mDuration;
  private float mMagnitude;

  public CameraShake(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    iInput.ReadString();
    this.mDuration = iInput.ReadSingle();
    this.mMagnitude = iInput.ReadSingle();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    iOwner.PlayState.Camera.CameraShake(iOwner.Position, this.mMagnitude, this.mDuration);
  }

  public override bool UsesBones => false;
}
