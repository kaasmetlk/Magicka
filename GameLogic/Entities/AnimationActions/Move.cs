// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Move
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class Move : AnimationAction
{
  private Vector3 mVelocity;

  public Move(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mVelocity = iInput.ReadVector3();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    float speed = iOwner.AnimationController.Speed;
    Matrix orientation = iOwner.CharacterBody.Orientation;
    Vector3 result;
    Vector3.Transform(ref this.mVelocity, ref orientation, out result);
    Vector3.Multiply(ref result, speed, out result);
    iOwner.CharacterBody.AdditionalForce = result;
  }

  public override bool UsesBones => true;
}
