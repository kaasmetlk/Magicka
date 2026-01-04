// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.PlayEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class PlayEffect : AnimationAction
{
  private int mBoneIndex;
  private Matrix mBoneBindPose;
  private bool mAttach;
  private int mEffectHash;
  private Dictionary<int, VisualEffectReference> mMemory;

  public PlayEffect(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    string str = iInput.ReadString();
    for (int index = 0; index < iSkeleton.Count; ++index)
    {
      SkinnedModelBone skinnedModelBone = iSkeleton[index];
      if (skinnedModelBone.Name.Equals(str, StringComparison.InvariantCultureIgnoreCase))
      {
        this.mBoneBindPose = skinnedModelBone.InverseBindPoseTransform;
        this.mBoneBindPose *= Matrix.CreateRotationY(3.14159274f);
        Matrix.Invert(ref this.mBoneBindPose, out this.mBoneBindPose);
        this.mBoneIndex = index;
        break;
      }
    }
    this.mAttach = iInput.ReadBoolean();
    this.mEffectHash = iInput.ReadString().ToLowerInvariant().GetHashCodeCustom();
    if ((double) this.mStartTime < (double) this.mEndTime)
      this.mMemory = new Dictionary<int, VisualEffectReference>(8);
    double num = (double) iInput.ReadSingle();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    Matrix boneOrientation = iOwner.GetBoneOrientation(this.mBoneIndex, ref this.mBoneBindPose);
    Vector3 translation = boneOrientation.Translation;
    Vector3 forward = boneOrientation.Forward;
    forward.Normalize();
    VisualEffectReference oRef;
    if (this.mMemory != null && this.mMemory.TryGetValue((int) iOwner.Handle, out oRef))
    {
      if (EffectManager.Instance.UpdatePositionDirection(ref oRef, ref translation, ref forward))
        this.mMemory[(int) iOwner.Handle] = oRef;
      else
        this.mMemory.Remove((int) iOwner.Handle);
    }
    else
    {
      if (!iFirstExecution)
        return;
      EffectManager.Instance.StartEffect(this.mEffectHash, ref translation, ref forward, out oRef);
      if (this.mMemory == null)
        return;
      this.mMemory[(int) iOwner.Handle] = oRef;
    }
  }

  public override void Kill(Character iOwner)
  {
    base.Kill(iOwner);
    VisualEffectReference iRef;
    if (this.mMemory == null || !this.mMemory.TryGetValue((int) iOwner.Handle, out iRef))
      return;
    EffectManager.Instance.Stop(ref iRef);
    this.mMemory.Remove((int) iOwner.Handle);
  }

  public override bool UsesBones => true;
}
