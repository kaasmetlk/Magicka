// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Tongue
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class Tongue : AnimationAction
{
  private float mMaxLength;

  public Tongue(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mMaxLength = iInput.ReadSingle();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    FrogTongue instance = FrogTongue.GetInstance(iOwner.PlayState);
    Matrix attachOrientation = iOwner.GetMouthAttachOrientation();
    instance.Initialize(iOwner, attachOrientation.Forward, this.mMaxLength * this.mMaxLength);
    iOwner.PlayState.EntityManager.AddEntity((Entity) instance);
  }

  public override bool UsesBones => true;
}
