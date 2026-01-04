// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationClipAction
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.AnimationActions;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class AnimationClipAction
{
  private AnimationClip mClip;
  private AnimationAction[] mActions;
  private float mAnimationSpeed;
  private float mBlendTime;
  private bool mLoopAnimation;

  public AnimationClipAction(
    Animations iAnimation,
    ContentReader iInput,
    AnimationClipDictionary iAnimations,
    SkinnedModelBoneCollection iSkeleton)
  {
    string key = iInput.ReadString();
    float num = iInput.ReadSingle();
    this.mBlendTime = iInput.ReadSingle();
    bool flag = iInput.ReadBoolean();
    this.mClip = iAnimations[key];
    this.mLoopAnimation = flag;
    this.mAnimationSpeed = num;
    int length = iInput.ReadInt32();
    this.mActions = new AnimationAction[length];
    for (int index = 0; index < length; ++index)
      this.mActions[index] = AnimationAction.Read(iAnimation, iInput, iSkeleton);
  }

  public AnimationAction[] Actions => this.mActions;

  public AnimationClip Clip => this.mClip;

  public float AnimationSpeed => this.mAnimationSpeed;

  public float BlendTime => this.mBlendTime;

  public bool LoopAnimation => this.mLoopAnimation;
}
