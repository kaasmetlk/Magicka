// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.AnimationAction
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Content;
using System;
using System.Reflection;
using XNAnimation;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public abstract class AnimationAction
{
  protected float mStartTime;
  protected float mEndTime;
  protected Animations mAnimation;

  protected AnimationAction(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
  {
    this.mStartTime = iInput.ReadSingle();
    this.mEndTime = iInput.ReadSingle();
  }

  protected static Type GetType(string name)
  {
    Type[] types = Assembly.GetExecutingAssembly().GetTypes();
    for (int index = 0; index < types.Length; ++index)
    {
      if (types[index].BaseType == typeof (AnimationAction) && types[index].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        return types[index];
    }
    return (Type) null;
  }

  public static AnimationAction Read(
    Animations iAnimation,
    ContentReader iInput,
    SkinnedModelBoneCollection iSkeleton)
  {
    string name = iInput.ReadString();
    AnimationAction animationAction = (AnimationAction) ((AnimationAction.GetType(name) ?? throw new Exception($"Invalid AnimationAction \"{name}\"!")).GetConstructor(new Type[2]
    {
      typeof (ContentReader),
      typeof (SkinnedModelBoneCollection)
    }) ?? throw new Exception($"AnimationAction \"{name}\" + does not define a valid constructor!")).Invoke(new object[2]
    {
      (object) iInput,
      (object) iSkeleton
    });
    animationAction.mAnimation = iAnimation;
    return animationAction;
  }

  public void Execute(
    Character iOwner,
    AnimationController iController,
    ref bool iHasExecuted,
    ref bool iIsDead)
  {
    float num = iController.Time / iController.AnimationClip.Duration;
    if ((double) num >= (double) this.mStartTime && (!iHasExecuted || (double) num <= (double) this.mEndTime))
    {
      this.InternalExecute(iOwner, !iHasExecuted);
      iHasExecuted = true;
      iIsDead = false;
    }
    else
    {
      if (!((double) num >= (double) this.mEndTime & iHasExecuted & !iIsDead))
        return;
      this.Kill(iOwner);
      iIsDead = true;
    }
  }

  protected abstract void InternalExecute(Character iOwner, bool iFirstExecution);

  public virtual void Kill(Character iOwner)
  {
  }

  public abstract bool UsesBones { get; }

  public Animations Animation => this.mAnimation;
}
