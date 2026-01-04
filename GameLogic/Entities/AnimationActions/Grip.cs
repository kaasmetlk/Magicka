// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.Grip
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities.CharacterStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class Grip : AnimationAction
{
  private float mRadius;
  private Grip.GripType mType;
  private int mBreakFreeTolerance;
  private bool mUseGripAttach;
  private BindJoint mGripAttach;
  private BindJoint mTargetAttach;
  private bool mFinishOnGrip;

  public Grip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    Matrix rotationY = Matrix.CreateRotationY(3.14159274f);
    this.mType = (Grip.GripType) iInput.ReadByte();
    this.mRadius = iInput.ReadSingle();
    this.mBreakFreeTolerance = (int) iInput.ReadSingle();
    if (this.mBreakFreeTolerance == 0)
      this.mBreakFreeTolerance = 10;
    string str1 = iInput.ReadString();
    if (!string.IsNullOrEmpty(str1))
    {
      this.mUseGripAttach = true;
      for (int index = 0; index < iSkeleton.Count; ++index)
      {
        SkinnedModelBone skinnedModelBone = iSkeleton[index];
        if (skinnedModelBone.Name.Equals(str1, StringComparison.OrdinalIgnoreCase))
        {
          this.mGripAttach.mIndex = (int) skinnedModelBone.Index;
          this.mGripAttach.mBindPose = skinnedModelBone.InverseBindPoseTransform;
          Matrix.Invert(ref this.mGripAttach.mBindPose, out this.mGripAttach.mBindPose);
          Matrix.Multiply(ref rotationY, ref this.mGripAttach.mBindPose, out this.mGripAttach.mBindPose);
          break;
        }
      }
    }
    string str2 = iInput.ReadString();
    if (!string.IsNullOrEmpty(str2))
    {
      for (int index = 0; index < iSkeleton.Count; ++index)
      {
        SkinnedModelBone skinnedModelBone = iSkeleton[index];
        if (skinnedModelBone.Name.Equals(str2, StringComparison.OrdinalIgnoreCase))
        {
          this.mTargetAttach.mIndex = (int) skinnedModelBone.Index;
          this.mTargetAttach.mBindPose = skinnedModelBone.InverseBindPoseTransform;
          Matrix.Invert(ref this.mTargetAttach.mBindPose, out this.mGripAttach.mBindPose);
          Matrix.Multiply(ref rotationY, ref this.mTargetAttach.mBindPose, out this.mTargetAttach.mBindPose);
          break;
        }
      }
    }
    else
      this.mTargetAttach.mIndex = -1;
    this.mFinishOnGrip = iInput.ReadBoolean();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!(iOwner is NonPlayerCharacter nonPlayerCharacter) || nonPlayerCharacter.IsGripping || !(nonPlayerCharacter.AI.CurrentTarget is Character currentTarget) || currentTarget.IsGripped || currentTarget.Dead || currentTarget.Drowning || currentTarget.IsSolidSelfShielded)
      return;
    int breakFreeTolerance = this.mBreakFreeTolerance;
    float num = this.mRadius;
    Vector3 result1;
    if (this.mUseGripAttach)
    {
      result1 = this.mGripAttach.mBindPose.Translation;
      Vector3.Transform(ref result1, ref iOwner.AnimationController.SkinnedBoneTransforms[this.mGripAttach.mIndex], out result1);
    }
    else
    {
      num = iOwner.Radius;
      result1 = nonPlayerCharacter.Position;
    }
    Vector3 position = currentTarget.Position;
    Vector3 vector3_1 = result1;
    Vector3 vector3_2 = position;
    vector3_1.Y = vector3_2.Y = 0.0f;
    float result2;
    Vector3.DistanceSquared(ref vector3_1, ref vector3_2, out result2);
    if ((double) result2 >= ((double) currentTarget.Radius + (double) num) * ((double) currentTarget.Radius + (double) num))
      return;
    Segment iSeg;
    iSeg.Origin = nonPlayerCharacter.Position;
    Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
    Vector3 vector3_3;
    if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out vector3_3, out Vector3 _, iSeg))
      return;
    List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
    bool flag = true;
    for (int index = 0; index < shields.Count & flag; ++index)
    {
      if (shields[index].SegmentIntersect(out vector3_3, iSeg, this.mRadius))
      {
        flag = false;
        break;
      }
    }
    if (!flag)
      return;
    switch (this.mType)
    {
      case Grip.GripType.Pickup:
        nonPlayerCharacter.GripCharacter(currentTarget, this.mType, this.mGripAttach.mIndex, breakFreeTolerance);
        break;
      case Grip.GripType.Ride:
        nonPlayerCharacter.GripCharacter(currentTarget, this.mType, this.mTargetAttach.mIndex, breakFreeTolerance);
        break;
      default:
        nonPlayerCharacter.GripCharacter(currentTarget, this.mType, this.mTargetAttach.mIndex, breakFreeTolerance);
        break;
    }
    if (!this.mFinishOnGrip)
      return;
    nonPlayerCharacter.ChangeState((BaseState) GrippingState.Instance);
  }

  public override bool UsesBones => true;

  public enum GripType
  {
    Pickup,
    Ride,
    Hold,
  }
}
