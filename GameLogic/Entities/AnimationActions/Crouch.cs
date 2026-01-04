// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Crouch
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class Crouch : AnimationAction
{
  private float mRadius;
  private float mLength;

  public Crouch(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mRadius = iInput.ReadSingle();
    this.mLength = iInput.ReadSingle();
    if ((double) this.mLength <= 0.0)
      this.mLength = 0.01f;
    if ((double) this.mRadius > 0.0)
      return;
    this.mRadius = 0.01f;
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    Capsule primitiveNewWorld = iOwner.Body.CollisionSkin.GetPrimitiveNewWorld(0) as Capsule;
    if ((double) primitiveNewWorld.Length == (double) this.mLength && (double) primitiveNewWorld.Radius == (double) this.mRadius)
      return;
    iOwner.SetCapsuleForm(this.mLength, this.mRadius);
  }

  public override void Kill(Character iOwner)
  {
    base.Kill(iOwner);
    iOwner.ResetCapsuleForm();
  }

  public override bool UsesBones => false;
}
