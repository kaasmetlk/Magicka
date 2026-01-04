// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.SetItemAttach
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.ObjectModel;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

internal class SetItemAttach : AnimationAction
{
  private int mItem;
  private BindJoint mJoint;
  private static Matrix ROTATE_Y = Matrix.CreateRotationY(3.14159274f);

  public SetItemAttach(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mItem = iInput.ReadInt32();
    string str = iInput.ReadString();
    if (string.IsNullOrEmpty(str))
      return;
    foreach (SkinnedModelBone skinnedModelBone in (ReadOnlyCollection<SkinnedModelBone>) iSkeleton)
    {
      if (skinnedModelBone.Name.Equals(str, StringComparison.OrdinalIgnoreCase))
      {
        this.mJoint.mIndex = (int) skinnedModelBone.Index;
        this.mJoint.mBindPose = skinnedModelBone.InverseBindPoseTransform;
        Matrix.Invert(ref this.mJoint.mBindPose, out this.mJoint.mBindPose);
        Matrix.Multiply(ref SetItemAttach.ROTATE_Y, ref this.mJoint.mBindPose, out this.mJoint.mBindPose);
        break;
      }
    }
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    iOwner.Equipment[this.mItem].AttachIndex = this.mJoint.mIndex;
    iOwner.Equipment[this.mItem].BindBose = this.mJoint.mBindPose;
  }

  public override bool UsesBones => true;
}
